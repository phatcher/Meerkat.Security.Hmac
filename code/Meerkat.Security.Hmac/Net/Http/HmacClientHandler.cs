using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Meerkat.Security.Authentication.Hmac;

namespace Meerkat.Net.Http
{
    /// <summary>
    /// Handler responsible for adding the HMAC client id and any other domain specific headers.
    /// <para>
    /// One possible issue is that we can't easily scope this so the IHmacClient is resolved per user.
    /// </para>
    /// </summary>
    public class HmacClientHandler : DelegatingHandler
    {
        private readonly IHmacClient client;

        public HmacClientHandler(IHmacClient client)
        {
            this.client = client;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var headers = request.Headers;
            client.AddHmacClientHeaders(headers);

            return base.SendAsync(request, cancellationToken);
        }
    }
}