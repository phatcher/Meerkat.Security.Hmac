using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

using Meerkat.Caching;
using Meerkat.Logging;
using Meerkat.Net.Http;

namespace Meerkat.Security.Authentication.Hmac
{
    /// <summary>
    /// Validates a HMAC signature if present.
    /// </summary>
    public class HmacSignatureValidator : ISignatureValidator
    {
        private static readonly ILog Logger = LogProvider.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const string CacheRegion = "hmac";

        private readonly ISignatureCalculator signatureCalculator;
        private readonly IMessageRepresentationBuilder representationBuilder;
        private readonly ISecretRepository secretRepository;
        private readonly ICache objectCache;

        /// <summary>
        /// Creates a new instance of the <see cref="HmacSignatureValidator"/> class.
        /// </summary>
        /// <param name="signatureCalculator"></param>
        /// <param name="representationBuilder"></param>
        /// <param name="secretRepository"></param>
        /// <param name="objectCache"></param>
        /// <param name="validityPeriod"></param>
        /// <param name="clockDrift"></param>
        public HmacSignatureValidator(ISignatureCalculator signatureCalculator, 
            IMessageRepresentationBuilder representationBuilder,
            ISecretRepository secretRepository,
            ICache objectCache,
            int validityPeriod,            
            int clockDrift)
        {
            this.secretRepository = secretRepository;
            this.representationBuilder = representationBuilder;
            this.signatureCalculator = signatureCalculator;
            this.objectCache = objectCache;
            ValidityPeriod = validityPeriod;
            ClockDrift = clockDrift;
        }

        /// <summary>
        /// Get the allowable clock drift between server and client in minutes.
        /// </summary>
        public int ClockDrift { get; }

        /// <summary>
        /// Gets the validity period of a signature in minutes, used to avoid replay attacks.
        /// </summary>
        public int ValidityPeriod { get; }

        /// <summary>
        /// Validates whether we are using HMAC and if so is the signature valid.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <remarks>
        /// We log all failures a Info, except for replay attacks which are logged as Warn as they may be a symptom of an attack.
        /// </remarks>
        public async Task<bool> IsValid(HttpRequestMessage request)
        {
            // TODO: Generalize so we can using any HMAC scheme.
            if (request.Headers.Authorization == null || request.Headers.Authorization.Scheme != HmacAuthentication.AuthenticationScheme)
            {
                // No authorization or not our authorization schema, so no 
                Logger.InfoFormat("Not our authorization schema: {0}", request.RequestUri);
                return false;
            }

            var isDateValid = request.IsMessageDateValid(ClockDrift);
            if (!isDateValid)
            {
                // Date is not present or valid
                Logger.InfoFormat("Invalid date: {0}", request.RequestUri);
                return false;
            }

            var userName = request.Headers.GetValues<string>(HmacAuthentication.ClientIdHeader).FirstOrDefault();
            if (string.IsNullOrEmpty(userName))
            {
                // No user name
                Logger.InfoFormat("No client id: {0}", request.RequestUri);
                return false;
            }

            var secret = secretRepository.ClientSecret(userName);
            if (secret == null)
            {
                // Can't find a secret for the user, so no
                Logger.InfoFormat("No secret for client id {0}: {1}", userName, request.RequestUri);
                return false;
            }

            if (!await request.Content.IsMd5Valid())
            {
                // MD5 is invalid, so no
                Logger.InfoFormat("Invalid MD5 hash: {0}", request.RequestUri);
                return false;
            }

            // Construct the representation
            var representation = representationBuilder.BuildRequestRepresentation(request);
            if (representation == null)
            {
                // Something broken in the representation, so no
                Logger.InfoFormat("Invalid canonical representation: {0}", request.RequestUri);
                return false;
            }

            // Compute the signature
            // TODO: Pass the encryption algorithm used e.g. SHA256
            var signature = signatureCalculator.Signature(secret, representation);

            // Have we seen it before
            if (objectCache.Contains(signature))
            {
                // Already seen, so no to avoid replay attack
                Logger.WarnFormat("Request replayed {0}: {1}", signature, request.RequestUri);
                return false;
            }

            // Validate the signature
            var result = request.Headers.Authorization.Parameter == signature;
            if (!result)
            {
                // Signatures differ, so no
                Logger.InfoFormat("Signatures differ {0}: {1}", signature, request.RequestUri);
            }
            else 
            {
                // Store valid signatures to avoid replay attack
                objectCache.Set(signature, userName, DateTimeOffset.UtcNow.AddMinutes(ValidityPeriod), CacheRegion);
            }

            // Return the signature validation.
            return result;
        }
    }
}