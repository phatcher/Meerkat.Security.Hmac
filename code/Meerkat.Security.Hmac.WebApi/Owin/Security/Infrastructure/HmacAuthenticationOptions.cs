using Meerkat.Security.Authentication.Hmac;

using Microsoft.Owin.Security;

namespace Meerkat.Owin.Security.Infrastructure
{
    public class HmacAuthenticationOptions : AuthenticationOptions
    {
        public HmacAuthenticationOptions() : base(HmacAuthentication.AuthenticationScheme)
        {
            // TODO: Should we be active or passive?
            AuthenticationMode = AuthenticationMode.Active;
        }
    }
}