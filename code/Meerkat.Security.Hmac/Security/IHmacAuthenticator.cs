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
        /// <param name="request"></param>
        /// <returns></returns>
        Task<IPrincipal> Authenticate(HttpRequestMessage request);
    }
}