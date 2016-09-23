using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

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
            var principal = await authenticator.Authenticate(request);
            if (principal == null)
            {
                // Authentication was attempted but failed. Set ErrorResult to indicate an error.
                context.ErrorResult = new AuthenticationFailureResult("Invalid signature", request);
                return;
            }

            context.Principal = principal;
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            Challenge(context);

            return Task.FromResult(0);
        }

        private void Challenge(HttpAuthenticationChallengeContext context)
        {
            context.ChallengeWith(HmacAuthentication.AuthenticationScheme);
        }

        private T GetService<T>(HttpAuthenticationContext context)
        {
            return (T)context.ActionContext.ControllerContext.Configuration.DependencyResolver.GetService(typeof(T));
        }
    }
}
