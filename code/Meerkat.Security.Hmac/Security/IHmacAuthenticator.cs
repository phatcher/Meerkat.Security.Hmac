using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Meerkat.Security
{
    /// <summary>
    /// Authenticator for a HMAC encoded message.
    /// </summary>
    public interface IHmacAuthenticator
    {
        /// <summary>
        /// Attempts to authenticate a request using HMAC.
        /// </summary>
        /// <param name="request">Request to authenticate</param>
        /// <returns>If using HMAC and the signature validates an authenticated identity, otherwise null.</returns>
        Task<ClaimsIdentity> Authenticate(HttpRequestMessage request);
    }
}