using System.Net.Http;
using System.Threading.Tasks;

namespace Meerkat.Security
{
    /// <summary>
    /// Validates a message signature
    /// </summary>
    public interface ISignatureValidator
    {
        /// <summary>
        /// Attempts to validate a request signature.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<bool> IsValid(HttpRequestMessage request);
    }
}