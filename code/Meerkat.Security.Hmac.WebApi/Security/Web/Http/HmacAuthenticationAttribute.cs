using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using Meerkat.Security.Authentication;
using Meerkat.Security.Authentication.Hmac;

namespace Meerkat.Security.Web.Http
{
    /// <summary>
    /// Authenticate a HMAC encoded message.
    /// </summary>
    public class HmacAuthenticationAttribute : Attribute, IAuthenticationFilter
    {
        public bool AllowMultiple
        {
            get { return false; }
        }

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var request = context.Request;
            var authorization = request.Headers.Authorization;

            if (authorization == null || authorization.Scheme != HmacAuthentication.AuthenticationScheme)
            {
                // No authentication or not our scheme, so ignore
                return;
            }

            if (string.IsNullOrEmpty(authorization.Parameter))
            {
                // Authentication was attempted but failed. Set ErrorResult to indicate an error.
                context.ErrorResult = new AuthenticationFailureResult("Missing credentials", request);
                return;
            }

            var authenticator = GetService<IHmacAuthenticator>(context);
            var identity = await authenticator.Authenticate(request);
            if (identity == null)
            {
                // Authentication was attempted but failed. Set ErrorResult to indicate an error.
                context.ErrorResult = new AuthenticationFailureResult("Invalid signature", request);
                return;
            }

            context.Principal = new ClaimsPrincipal(identity);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            context.ChallengeWith(HmacAuthentication.AuthenticationScheme);

            return Task.FromResult(0);
        }

        private T GetService<T>(HttpAuthenticationContext context)
        {
            return (T)context.ActionContext.ControllerContext.Configuration.DependencyResolver.GetService(typeof(T));
        }
    }
}
