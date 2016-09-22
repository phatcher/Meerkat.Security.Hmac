using System.Net.Http;
using System.Security.Principal;
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
        /// <returns>If using HMAC and the signature validates an authenticated principal, otherwise null.</returns>
        Task<IPrincipal> Authenticate(HttpRequestMessage request);
    }
}