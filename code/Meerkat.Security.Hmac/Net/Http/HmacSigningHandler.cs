using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Common.Logging;

using Meerkat.Security;

namespace Meerkat.Net.Http
{
    /// <summary>
    /// Handler reponsible for HMAC signing a message if we have a <see cref="HmacAuthentication.ClientIdHeader"/>
    /// </summary>
    public class HmacSigningHandler : DelegatingHandler
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ISecretRepository secretRepository;
        private readonly IMessageRepresentationBuilder representationBuilder;
        private readonly ISignatureCalculator signatureCalculator;

        /// <summary>
        /// Create a new instance of the <see cref="HmacSigningHandler"/> class.
        /// </summary>
        /// <param name="secretRepository"></param>
        /// <param name="representationBuilder"></param>
        /// <param name="signatureCalculator"></param>
        public HmacSigningHandler(ISecretRepository secretRepository,
                              IMessageRepresentationBuilder representationBuilder,
                              ISignatureCalculator signatureCalculator)
        {
            this.secretRepository = secretRepository;
            this.representationBuilder = representationBuilder;
            this.signatureCalculator = signatureCalculator;
        }

        /// <summary>
        /// Sends the message after adding the HMAC signature if a HMAC client id header is present.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Only try and sign if we have a user
            var userName = request.Headers.GetValues<string>(HmacAuthentication.ClientIdHeader).FirstOrDefault();
            if (string.IsNullOrEmpty(userName))
            {
                return base.SendAsync(request, cancellationToken);
            }

            // Need the date present in UTC for the signature calculation
            request.Headers.Date = new DateTimeOffset(DateTime.Now, DateTime.Now - DateTime.UtcNow);

            // Get the canonical representation.
            var representation = representationBuilder.BuildRequestRepresentation(request);

            // Now try and sign it
            var secret = secretRepository.ClientSecret(userName);
            if (secret == null)
            {
                Logger.WarnFormat("No secret for client id {0}: {1}", userName, request.RequestUri);
            }
            else
            {
                var signature = signatureCalculator.Signature(secret, representation);

                // Ok, valid signature so add the authentication header
                var header = new AuthenticationHeaderValue(HmacAuthentication.AuthenticationScheme, signature);

                request.Headers.Authorization = header;
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}