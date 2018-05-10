using Microsoft.AspNetCore.Authentication;

namespace Meerkat.Security.Authentication
{
    public class HmacAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "HMAC";

        public string Scheme => DefaultScheme;
    }
}