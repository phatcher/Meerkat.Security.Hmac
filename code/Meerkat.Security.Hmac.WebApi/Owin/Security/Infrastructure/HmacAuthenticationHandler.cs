using System.Threading.Tasks;

using Meerkat.Security.Authentication;
using Meerkat.Security.Authentication.Hmac;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;

namespace Meerkat.Owin.Security.Infrastructure
{
    /// <summary>
    /// Performs HMAC authentication as Owin middleware.
    /// </summary>
    public class HmacAuthenticationHandler : AuthenticationHandler<HmacAuthenticationOptions>
    {
        private readonly ILogger logger;
        private readonly IHmacAuthenticator authenticator;
        private string reasonPhrase;

        /// <summary>
        /// Create a new instance of the <see cref="HmacAuthenticationHandler"/> class.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="authenticator"></param>
        public HmacAuthenticationHandler(ILogger logger, IHmacAuthenticator authenticator)
        {
            this.logger = logger;
            this.authenticator = authenticator;
        }

        /// <copydoc cref="AuthenticationHandler.AuthenticateCoreAsync" />
        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            var request = Context.Request;
            var authorization = request.Headers.Authentication();

            reasonPhrase = "Unauthorized";

            if (authorization == null)
            {
                // No authentication, so ignore
                return null;
            }

            if (authorization.Scheme != HmacAuthentication.AuthenticationScheme)
            {
                logger.WriteVerbose("Not HMAC authenticated");
                // Not our scheme, so ignore
                return null;
            }

            if (string.IsNullOrEmpty(authorization.Parameter))
            {
                // Authentication was attempted but failed. Set ErrorResult to indicate an error.
                logger.WriteWarning("Missing credentials");
                reasonPhrase = "Missing credentials";
                return null;
            }

            var httpRequest = request.ToHttpRequestMessage();
            var identity = await authenticator.Authenticate(httpRequest);
            if (identity == null)
            {
                // Authentication was attempted but failed. Set ErrorResult to indicate an error.
                logger.WriteWarning("Invalid signature");
                reasonPhrase = "Invalid signature";
                return null;
            }

            var ticket = new AuthenticationTicket(identity, null);

            return ticket;
        }

        /// <copydoc cref="AuthenticationHandler.ApplyResponseChallengeAsync" />
        protected override Task ApplyResponseChallengeAsync()
        {
            if (Response.StatusCode != 401)
            {
                return Task.FromResult(0);
            }

            // Should play nice with other authentication schemes
            Response.Headers.Append("WWW-Authenticate", HmacAuthentication.AuthenticationScheme);
            Response.ReasonPhrase = reasonPhrase;

            return Task.FromResult(0);
        }
    }
}