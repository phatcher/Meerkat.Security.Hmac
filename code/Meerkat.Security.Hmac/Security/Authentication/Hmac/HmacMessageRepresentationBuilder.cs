using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Meerkat.Net.Http;

namespace Meerkat.Security.Authentication.Hmac
{
    /// <summary>
    /// Builds a canonical representation of the request messsage.
    /// <para>
    /// Builds message representation as follows:
    /// HTTPMethod\n +
    /// Request URI\n +
    /// Content-MD5\n +  
    /// Timestamp\n +
    /// ClientId\n +
    /// Custom Headers (x-msec-headers values)
    /// </para>
    /// </summary>
    public class HmacMessageRepresentationBuilder : MessageRepresentationBuilder
    {
        /// <summary>
        /// Construct a new instance of the <see cref="HmacMessageRepresentationBuilder"/> class.
        /// </summary>
        public HmacMessageRepresentationBuilder()
        {
            // Additional headers neeed by HMAC
            Headers.AddRange(new List<Func<HttpRequestMessage, string>>
            {
                m => m.Headers.GetValues<string>(HmacAuthentication.ClientIdHeader).FirstOrDefault(),
                m => m.Headers.CustomHeadersRepresentation(HmacAuthentication.CustomHeaders),
            });
        }
    }
}