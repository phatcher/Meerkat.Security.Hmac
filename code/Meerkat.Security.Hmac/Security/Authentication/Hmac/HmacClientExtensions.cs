using System.Net.Http.Headers;

namespace Meerkat.Security.Authentication.Hmac
{
    public static class HmacClientExtensions
    {
        /// <summary>
        /// Adds the headers necessary to trigger HMAC signing if we don't already have an Authentication header.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="headers"></param>
        public static void AddHmacClientHeaders(this IHmacClient client, HttpRequestHeaders headers)
        {
            var auth = headers.Authorization;
            if (auth != null)
            {
                return;
            }

            // Only bother if we don't have an authentication header?
            var clientId = client.ClientId;
            if (!string.IsNullOrEmpty(clientId))
            {
                // Mark it so we know to sign the message
                headers.Add(HmacAuthentication.ClientIdHeader, clientId);

                // See if we have any custom headers
                var customHeaders = client.AddCustomHeaders(headers);
                if (!string.IsNullOrEmpty(customHeaders))
                {
                    // Ok, mark it so the signer knows to include them in the signature.
                    headers.Add(HmacAuthentication.CustomHeaders, customHeaders.Trim());
                }
            }
        }
    }
}