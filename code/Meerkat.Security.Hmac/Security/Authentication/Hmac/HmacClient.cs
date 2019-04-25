using System.Net.Http.Headers;

namespace Meerkat.Security.Authentication.Hmac
{
    /// <summary>
    /// Simple HMAC client that adds a nonce custom header
    /// </summary>
    public class HmacClient : IHmacClient
    {
        private static int nonce;
        
        /// <copydoc cref="IHmacClient.ClientId" />
        public string ClientId { get; set; }

        /// <copydoc cref="IHmacClient.AddCustomHeaders" />
        public virtual string AddCustomHeaders(HttpRequestHeaders headers)
        {
            var customHeaders = HmacAuthentication.NonceHeader;
            headers.Add(HmacAuthentication.NonceHeader, nonce++.ToString());

            return customHeaders;
        }
    }
}