using Meerkat.Security.Authentication;
using Meerkat.Security.Authentication.Hmac;

namespace Meerkat.Net.Http
{
    /// <summary>
    /// Handler responsible for HMAC signing a message if we have a <see cref="HmacAuthentication.ClientIdHeader"/>
    /// </summary>
    public class HmacSha256SigningHandler : HmacSigningHandler
    {
        /// <summary>
        /// Create a new instance of the <see cref="HmacSha256SigningHandler"/> class.
        /// </summary>
        /// <param name="secretRepository"></param>
        /// <param name="representationBuilder"></param>
        /// <param name="signatureCalculator"></param>
        public HmacSha256SigningHandler(ISecretRepository secretRepository, IMessageRepresentationBuilder representationBuilder, ISignatureCalculator signatureCalculator) : base(secretRepository, representationBuilder, signatureCalculator, "SHA256")
        {
        }
    }
}