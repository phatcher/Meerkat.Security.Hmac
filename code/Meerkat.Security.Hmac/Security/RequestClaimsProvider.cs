using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;

using Meerkat.Net.Http;

namespace Meerkat.Security
{
    /// <copydoc cref="IRequestClaimsProvider" />
    public class RequestClaimsProvider : IRequestClaimsProvider
    {
        private readonly string nameClaimType;

        /// <summary>
        /// Creates a new instance of the <see cref="RequestClaimsProvider"/> class.
        /// </summary>
        /// <param name="nameClaimType"></param>
        public RequestClaimsProvider(string nameClaimType)
        {
            this.nameClaimType = nameClaimType;
        }

        /// <copydoc cref="IRequestClaimsProvider.Claims" />
        /// <remarks>
        /// Default implementation can only process the <see cref="P:HmacAuthentication.ClientIdHeader"/> header that is 
        /// part of the HMAC signature, any other available claims are domain specific.
        /// </remarks>
        public IList<Claim> Claims(HttpRequestMessage request)
        {
            var clientId = request.Headers.GetFirstOrDefaultValue<string>(HmacAuthentication.ClientIdHeader);
            var claims = new List<Claim>
            {
                // Make the clientId header the name, simple hook if no other claims are provided
                new Claim(nameClaimType, clientId)
            };

            return claims;
        }
    }
}