namespace Meerkat.Security
{
    public class HmacAuthentication
    {
        public const string AuthenticationSchemePrefix = "MSEC-HMAC-";
        public const string AuthenticationScheme = AuthenticationSchemePrefix + "SHA256";
        public const string ClientIdHeader = "x-msec-clientid";
        public const string CustomHeaders = "x-msec-headers";
    }
}