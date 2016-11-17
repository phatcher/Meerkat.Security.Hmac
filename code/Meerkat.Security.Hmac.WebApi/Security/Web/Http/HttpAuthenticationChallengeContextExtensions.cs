using System;
using System.Net.Http.Headers;
using System.Web.Http.Filters;

namespace Meerkat.Security.Web.Http
{
    public static class HttpAuthenticationChallengeContextExtensions
    {
        public static void ChallengeWith(this HttpAuthenticationChallengeContext context, string scheme)
        {
            context.ChallengeWith(new AuthenticationHeaderValue(scheme));
        }

        public static void ChallengeWith(this HttpAuthenticationChallengeContext context, AuthenticationHeaderValue challenge)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Result = new AddChallengeOnUnauthorizedResult(challenge, context.Result);
        }
    }
}
