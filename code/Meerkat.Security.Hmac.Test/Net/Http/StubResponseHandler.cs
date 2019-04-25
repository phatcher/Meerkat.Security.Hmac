using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Meerkat.Hmac.Test.Net.Http
{
    public class StubResponseHandler : DelegatingHandler
    {
        public StubResponseHandler()
        {
            Response = new HttpResponseMessage(HttpStatusCode.OK);
        }

        public HttpResponseMessage Response { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(Response);
        }
    }
}