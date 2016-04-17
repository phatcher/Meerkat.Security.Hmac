using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Meerkat.Net.Http
{
    /// <summary>
    /// Handler to assign the MD5 hash value if content is present
    /// </summary>
    public class ResponseContentMd5Handler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode || response.Content == null)
            {
                return response;
            }

            await response.Content.AssignMd5Hash();

            return response;
        }
    }
}