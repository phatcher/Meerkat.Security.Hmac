using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;

namespace Meerkat.Security
{
    /// <summary>
    /// Provide <see cref="Claim"/>s that are supported by the request
    /// </summary>
    public interface IRequestClaimsProvider
    {
        /// <summary>
        /// Provide claims from the message
        /// </summary>
        /// <param name="request">Request to process</param>
        /// <returns>Claims contained in the request</returns>
        IList<Claim> Claims(HttpRequestMessage request);
    }
}