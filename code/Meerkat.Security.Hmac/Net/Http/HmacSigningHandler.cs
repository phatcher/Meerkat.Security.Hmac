using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Meerkat.Logging;
using Meerkat.Security.Authentication;
using Meerkat.Security.Authentication.Hmac;

namespace Meerkat.Net.Http
{
    /// <summary>
    /// Handler responsible for HMAC signing a message if we have a <see cref="HmacAuthentication.ClientIdHeader"/>
    /// </summary>
    public class HmacSigningHandler : DelegatingHandler
    {
        private static readonly ILog Logger = LogProvider.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ISecretRepository secretRepository;
        private readonly IMessageRepresentationBuilder representationBuilder;
        private readonly ISignatureCalculator signatureCalculator;
        private readonly string scheme;

        /// <summary>
        /// Create a new instance of the <see cref="HmacSigningHandler"/> class.
        /// </summary>
        /// <param name="secretRepository"></param>
        /// <param name="representationBuilder"></param>
        /// <param name="signatureCalculator"></param>
        /// <param name="scheme"></param>
        public HmacSigningHandler(ISecretRepository secretRepository, IMessageRepresentationBuilder representationBuilder, ISignatureCalculator signatureCalculator, string scheme = "SHA256")
        {
            this.secretRepository = secretRepository;
            this.representationBuilder = representationBuilder;
            this.signatureCalculator = signatureCalculator;
            this.scheme = scheme;
        }

        /// <summary>
        /// Sends the message after adding the HMAC signature if a HMAC client id header is present.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Only try and sign if we have a client id
            var clientId = request.Headers.GetValues<string>(HmacAuthentication.ClientIdHeader).FirstOrDefault();
            if (string.IsNullOrEmpty(clientId))
            {
                return base.SendAsync(request, cancellationToken);
            }

            // Can we sign it
            var secret = secretRepository.ClientSecret(clientId);
            if (secret == null)
            {
                Logger.WarnFormat("No secret for client id {0}: {1}", clientId, request.RequestUri);
            }
            else
            {
                // Need the date present in UTC for the signature calculation
                request.Headers.Date = DateTimeOffset.UtcNow;

                // Get the canonical representation.
                var representation = representationBuilder.BuildRequestRepresentation(request);

                // Compute the signature
                var signature = signatureCalculator.Signature(secret, representation, scheme);

                if (string.IsNullOrEmpty(signature))
                {
                    Logger.WarnFormat("Invalid signature for client id {0}: {1}", clientId, request.RequestUri);
                }
                else
                {
                    // Valid signature so add the authentication header
                    Logger.InfoFormat("HMAC signed for client id {0}: {1}", clientId, request.RequestUri);
                    var header = new AuthenticationHeaderValue(HmacAuthentication.AuthenticationSchemePrefix + scheme, signature);
                    request.Headers.Authorization = header;
                }
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}