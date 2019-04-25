using Meerkat.Security.Authentication.Hmac;

using Microsoft.AspNetCore.Authentication;

namespace Meerkat.Security.Authentication
{
    public class HmacAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = HmacAuthentication.AuthenticationScheme;

        public HmacAuthenticationOptions()
        {
            Scheme = DefaultScheme;
        }

        public string Scheme { get; set; }
    }
}