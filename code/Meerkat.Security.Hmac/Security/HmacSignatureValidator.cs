using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

using Common.Logging;
using Meerkat.Caching;
using Meerkat.Net.Http;

namespace Meerkat.Security
{
    /// <summary>
    /// Validates a HMAC signature if present.
    /// </summary>
    public class HmacSignatureValidator : ISignatureValidator
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const string CacheRegion = "hmac";

        private readonly ISignatureCalculator signatureCalculator;
        private readonly IMessageRepresentationBuilder representationBuilder;
        private readonly ISecretRepository secretRepository;
        private readonly ICache objectCache;

        /// <summary>
        /// Creates a new instance of the <see cref="HmacSignatureCalculator"/> class.
        /// </summary>
        /// <param name="signatureCalculator"></param>
        /// <param name="representationBuilder"></param>
        /// <param name="secretRepository"></param>
        /// <param name="objectCache"></param>
        /// <param name="validityPeriod"></param>
        public HmacSignatureValidator(ISignatureCalculator signatureCalculator, 
            IMessageRepresentationBuilder representationBuilder,
            ISecretRepository secretRepository,
            ICache objectCache,
            int validityPeriod)
        {
            this.secretRepository = secretRepository;
            this.representationBuilder = representationBuilder;
            this.signatureCalculator = signatureCalculator;
            this.objectCache = objectCache;
            ValidityPeriod = validityPeriod;
        }

        /// <summary>
        /// Gets the validity period of a signature in minutes, used to allow for clock-drift between client and server
        /// and also to avoid replay attacks
        /// </summary>
        public int ValidityPeriod { get; }

        /// <summary>
        /// Validates whether we are using HMAC and if so is the signature valid.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<bool> IsValid(HttpRequestMessage request)
        {
            // TODO: Generalize so we can using any HMAC scheme.
            if (request.Headers.Authorization == null || request.Headers.Authorization.Scheme != HmacAuthentication.AuthenticationScheme)
            {
                // No authorization or not our authorization schema, so no 
                Logger.DebugFormat("Not our authorization schema: {0}", request.RequestUri);
                return false;
            }

            var isDateValid = request.IsMessageDateValid(ValidityPeriod);
            if (!isDateValid)
            {
                // Date is not present or valid
                Logger.DebugFormat("Invalid date: {0}", request.RequestUri);
                return false;
            }

            var userName = request.Headers.GetFirstOrDefaultValue<string>(HmacAuthentication.ClientIdHeader);
            if (string.IsNullOrEmpty(userName))
            {
                // No user name
                Logger.DebugFormat("No client id: {0}", request.RequestUri);
                return false;
            }

            var secret = secretRepository.ClientSecret(userName);
            if (secret == null)
            {
                // Can't find a secret for the user, so no
                Logger.DebugFormat("No secret for client id {0}: {1}", userName, request.RequestUri);
                return false;
            }

            if (!await request.Content.IsMd5Valid())
            {
                // Invalid MD5 is correct, so no
                Logger.DebugFormat("Invalid MD5 hash: {0}", request.RequestUri);
                return false;
            }

            // Construct the representation
            var representation = representationBuilder.BuildRequestRepresentation(request);
            if (representation == null)
            {
                // Something broken in the representation, so no
                Logger.DebugFormat("Invalid canonical representation: {0}", request.RequestUri);
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
            if (result)
            {
                // Store valid signatures to avoid replay attack
                objectCache.Set(signature, userName, DateTimeOffset.UtcNow.AddMinutes(ValidityPeriod), CacheRegion);
            }

            // Return the signature validation.
            return result;
        }
    }
}