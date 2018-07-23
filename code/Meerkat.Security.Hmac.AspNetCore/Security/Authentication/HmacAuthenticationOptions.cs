using Microsoft.AspNetCore.Authentication;

namespace Meerkat.Security.Authentication
{
    public class HmacAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "MSEC-HMAC-SHA256";

        public string Scheme => DefaultScheme;
    }
}