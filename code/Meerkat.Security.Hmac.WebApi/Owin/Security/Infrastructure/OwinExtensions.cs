using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

using Meerkat.Logging;

using Microsoft.Owin;

namespace Meerkat.Owin.Security.Infrastructure
{
    public static class OwinExtensions
    {
        private static readonly ILog Logger = LogProvider.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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

            if (string.IsNullOrEmpty(value?[0]))
            {
                return null;
            }

            AuthenticationHeaderValue auth;
            return AuthenticationHeaderValue.TryParse(value[0], out auth) ? auth : null;
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

            // Copy the headers, handle content headers we need
            foreach (var header in request.Headers)
            {
                switch (header.Key)
                {
                    case "Content-Length":
                        requestMessage.Content.Headers.ContentLength = long.Parse(header.Value[0]);
                        break;

                    case "Content-Type":
                        requestMessage.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(header.Value[0]);
                        break;

                    case "Content-MD5":
                        requestMessage.Content.Headers.ContentMD5 = Convert.FromBase64String(header.Value[0]);
                        break;

                    default:
                        try
                        {
                            requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);
                        }
                        catch (Exception)
                        {
                            Logger.WarnFormat("Error copying header: {0} - {1}", header.Key, header.Value);
                        }
                        break;
                }
            }

            // We're done.
            return requestMessage;
        }
    }
}