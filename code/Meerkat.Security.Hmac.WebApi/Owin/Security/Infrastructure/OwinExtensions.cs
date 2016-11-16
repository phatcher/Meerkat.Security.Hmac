using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

using Microsoft.Owin;

namespace Meerkat.Owin.Security.Infrastructure
{
    public static class OwinExtensions
    {
        /// <summary>
        /// Get the Authentication header.
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public static AuthenticationHeaderValue Authentication(this IHeaderDictionary header)
        {
            string[] value;
            if (!header.TryGetValue("Authorization", out value))
            {
                return null;
            }

            if (string.IsNullOrEmpty(value[0]))
            {
                return null;
            }

            // TODO: Error trap
            return AuthenticationHeaderValue.Parse(value[0]);
        }

        /// <summary>
        /// Construct a <see cref="HttpRequestMessage"/> from a <see cref="OwinRequest" />, also preserves the body of the original request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HttpRequestMessage ToHttpRequestMessage(this IOwinRequest request)
        {
            var body = new StreamReader(request.Body).ReadToEnd();
            byte[] requestData = Encoding.UTF8.GetBytes(body);

            // Preserve the existing request body
            request.Body = new MemoryStream(requestData);

            // And have another copy for the request message.
            var requestMessage = new HttpRequestMessage(new HttpMethod(request.Method), request.Uri)
            {
                Content = new StreamContent(new MemoryStream(requestData))
            };

            // Copy the headers
            request.Headers.ToList()
                   .ForEach(x => requestMessage.Headers.Add(x.Key, x.Value));

            // We're done.
            return requestMessage;
        }

        /// <summary>
        /// Construct a <see cref="HttpResponseMessage"/> from a <see cref="OwinRequest" />
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static HttpResponseMessage ToHttpResponseMessage(this IOwinResponse response)
        {
            var requestMessage = response.Context.Request.ToHttpRequestMessage();
            var responseMessage = requestMessage.CreateResponse((HttpStatusCode)response.StatusCode);

            if (response.Body != null)
            {
                responseMessage.Content = new StreamContent(response.Body);
            }

            foreach (var header in response.Headers)
            {

                if (!responseMessage.Headers.TryAddWithoutValidation(header.Key, header.Value))
                {
                    responseMessage.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            return responseMessage;
        }
    }
}