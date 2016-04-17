using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;

using Common.Logging;

using Meerkat.Net.Http;

namespace Meerkat.Security
{
    /// <summary>
    /// Builds a canonical representation of the request messsage.
    /// </summary>
    public class MessageRepresentationBuilder : IMessageRepresentationBuilder
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly List<Func<HttpRequestMessage, string>> headers;

        /// <summary>
        /// Construct a new instance of the <see cref="MessageRepresentationBuilder"/> class.
        /// </summary>
        public MessageRepresentationBuilder()
        {
            // Function order matches required order in representation
            headers = new List<Func<HttpRequestMessage, string>>
            {
                // NB Needed as the server-side request uri has been escaped e.g. /odata/%24metadata
                // Also different .NET libraries encode differently - see http://stackoverflow.com/questions/575440/url-encoding-using-c-sharp/21771206#21771206
                m => WebUtility.UrlDecode(m.RequestUri.AbsolutePath.ToLower()),
                m => m.Method.Method,
                m => m.Content.Md5Base64(),
                m => m.Headers.MessageDate(),
                m => m.Headers.GetFirstOrDefaultValue<string>(HmacAuthentication.ClientIdHeader),
                m => m.Headers.CustomHeadersRepresentation(HmacAuthentication.CustomHeaders),
            };
        }

        /// <summary>
        /// Builds message representation as follows:
        /// HTTPMethod\n +
        /// Request URI\n +
        /// Content-MD5\n +  
        /// Timestamp\n +
        /// Username\n +
        /// Custom Headers (x-msec-headers values)
        /// </summary>
        /// <returns></returns>
        public string BuildRequestRepresentation(HttpRequestMessage requestMessage)
        {
            var values = new List<string>();
            foreach (var gm in headers)
            {
                var value = gm(requestMessage);
                if (value == null)
                {
                    // Fail on first null
                    return null;
                }
                values.Add(value);
            }

            var result = string.Join("\n", values);

            Logger.DebugFormat("Representation: {0}", result);

            return result;
        }
    }
}