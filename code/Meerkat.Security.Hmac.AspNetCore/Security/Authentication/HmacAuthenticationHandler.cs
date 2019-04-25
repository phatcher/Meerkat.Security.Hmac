using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using Meerkat.Security.Authentication.Hmac;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.WebApiCompatShim;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Meerkat.Security.Authentication
{
    public class HmacAuthenticationHandler : AuthenticationHandler<HmacAuthenticationOptions>
    {
        private readonly IHmacAuthenticator authenticator;

        public HmacAuthenticationHandler(IHmacAuthenticator authenticator, IOptionsMonitor<HmacAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) :
            base(options, logger, encoder, clock)
        {
            this.authenticator = authenticator;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Do we have authorization
            if (!AuthenticationHeaderValue.TryParse(Request.Headers["Authorization"], out var authorization))
            {
                // No Authorization header, so ignore
                return AuthenticateResult.NoResult();
            }

            if (authorization.Scheme != Options.Scheme)
            {
                Logger.LogDebug("Not HMAC authenticated");
                // Not our scheme, so ignore
                return AuthenticateResult.NoResult();
            }

            if (string.IsNullOrEmpty(authorization.Parameter))
            {
                // Authentication was attempted but failed. Set ErrorResult to indicate an error.
                Logger.LogWarning("Missing credentials");
                return AuthenticateResult.Fail("Missing credentials");
            }

            var httpRequest = new HttpRequestMessageFeature(Context);
            var identity = await authenticator.Authenticate(httpRequest.HttpRequestMessage);
            if (identity == null)
            {
                // Authentication was attempted but failed. Set ErrorResult to indicate an error.
                Logger.LogWarning("Invalid signature");
                return AuthenticateResult.Fail("Invalid signature");
            }

            // Ok, wrap the identity in a principal and say we're ok.
            var principal = new ClaimsPrincipal(identity);

            var ticket = new AuthenticationTicket(principal, Options.Scheme);

            return AuthenticateResult.Success(ticket);
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers["WWW-Authenticate"] = $"HMAC {Options.Scheme}";
            await base.HandleChallengeAsync(properties);
        }
    }
}