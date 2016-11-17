using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Meerkat.Security.Web.Http
{
    public static class HttpResponseMessageExtensions
    {
        public static void AddChallenge(this HttpResponseMessage response, AuthenticationHeaderValue challenge)
        {
            // Only add one challenge per authentication scheme.
            if (response.StatusCode == HttpStatusCode.Unauthorized &&
                response.Headers.WwwAuthenticate.All(h => h.Scheme != challenge.Scheme))
            {
                response.Headers.WwwAuthenticate.Add(challenge);
            }
        }
    }
}
