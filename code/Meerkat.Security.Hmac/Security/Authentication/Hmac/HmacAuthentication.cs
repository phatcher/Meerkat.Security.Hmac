namespace Meerkat.Security.Authentication.Hmac
{
    public class HmacAuthentication
    {
        /// <summary>
        /// Prefix for all HMAC authentication schemes.
        /// </summary>
        public const string AuthenticationSchemePrefix = "MSEC-HMAC-";

        /// <summary>
        /// Default HMAC authentication scheme, SHA256.
        /// </summary>
        public const string AuthenticationScheme = AuthenticationSchemePrefix + "SHA256";

        /// <summary>
        /// Header holding the HMAC client id.
        /// </summary>
        public const string ClientIdHeader = "x-msec-clientid";

        /// <summary>
        /// Header holding the message headers which form the signature.
        /// </summary>
        public const string CustomHeaders = "x-msec-headers";

        /// <summary>
        /// Nonce header name, used to make the request unique
        /// </summary>
        public const string NonceHeader = "x-msec-nonce";
    }
}