using Meerkat.Security;
using Microsoft.Owin.Security;

namespace Meerkat.Owin.Security.Infrastructure
{
    public class HmacAuthenticationOptions : AuthenticationOptions
    {
        public HmacAuthenticationOptions() : base(HmacAuthentication.AuthenticationScheme)
        {
            AuthenticationMode = AuthenticationMode.Active;
        }
    }
}