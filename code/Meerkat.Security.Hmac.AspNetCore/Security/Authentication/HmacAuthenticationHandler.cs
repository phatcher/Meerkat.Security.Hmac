using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using Meerkat.Security.Authentication.Hmac;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.WebApiCompatShim;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Meerkat.Security.Authentication
{
    public class HmacAuthenticationHandler : AuthenticationHandler<HmacAuthenticationOptions>
    {
        private readonly IHmacAuthenticator authenticator;
        private string reasonPhrase;

        public HmacAuthenticationHandler(IHmacAuthenticator authenticator, IOptionsMonitor<HmacAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) :
            base(options, logger, encoder, clock)
        {
            this.authenticator = authenticator;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Do we have authorization
            if (!Request.Headers.TryGetValue(HeaderNames.Authorization, out var header))
            {
                return AuthenticateResult.NoResult();
                //return AuthenticateResult.Fail("Cannot read authorization header.");
            }

            reasonPhrase = "Unauthorized";

            if (!AuthenticationHeaderValue.TryParse(Request.Headers["Authorization"], out AuthenticationHeaderValue authorization))
            {
                //Invalid Authorization header
                return AuthenticateResult.NoResult();
            }

            if (authorization.Scheme != HmacAuthentication.AuthenticationScheme)
            {
                Logger.LogDebug("Not HMAC authenticated");
                // Not our scheme, so ignore
                return AuthenticateResult.NoResult();
            }

            if (string.IsNullOrEmpty(authorization.Parameter))
            {
                // Authentication was attempted but failed. Set ErrorResult to indicate an error.
                Logger.LogWarning("Missing credentials");
                reasonPhrase = "Missing credentials";
                return AuthenticateResult.NoResult();
            }

            var httpRequest = new HttpRequestMessageFeature(Context);
            var identity = await authenticator.Authenticate(httpRequest.HttpRequestMessage);
            if (identity == null)
            {
                // Authentication was attempted but failed. Set ErrorResult to indicate an error.
                Logger.LogWarning("Invalid signature");
                reasonPhrase = "Invalid signature";
                return AuthenticateResult.NoResult();
            }

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