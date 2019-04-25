using System.Net.Http.Headers;

namespace Meerkat.Security.Authentication.Hmac
{
    /// <summary>
    /// Holds the HMAC client id and provides custom headers prior to signing.
    /// </summary>
    public interface IHmacClient
    {
        /// <summary>
        /// Get or set the client id used for HMAC authentication
        /// </summary>
        /// <returns></returns>
        string ClientId { get; set;  }

        /// <summary>
        /// Add any custom headers required for the HMAC signature.
        /// </summary>
        /// <returns>The names of headers added</returns>
        string AddCustomHeaders(HttpRequestHeaders headers);
    }
}