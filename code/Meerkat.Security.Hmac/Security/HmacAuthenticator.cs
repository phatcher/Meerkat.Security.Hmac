using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Meerkat.Security
{
    /// <summary>
    /// Validates a HMAC encoded message, creating a <see cref="ClaimsIdentity"/> with claims provided by a <see cref="IRequestClaimsProvider"/>.
    /// </summary>
    public class HmacAuthenticator : IHmacAuthenticator
    {
        private readonly ISignatureValidator validator;
        private readonly IRequestClaimsProvider claimsProvider;
        private readonly string nameClaimType;
        private readonly string roleNameType;

        /// <summary>
        /// Creates a new instance of the <see cref="HmacAuthenticator"/> class.
        /// </summary>
        /// <param name="validator">Validator to use</param>
        /// <param name="claimsProvider">Claims provider to process the request</param>
        /// <param name="nameClaimType">Name of the name claim type</param>
        /// <param name="roleNameType">Name of the role claim type</param>
        public HmacAuthenticator(ISignatureValidator validator, IRequestClaimsProvider claimsProvider, string nameClaimType, string roleNameType)
        {
            this.validator = validator;
            this.claimsProvider = claimsProvider;
            this.nameClaimType = nameClaimType;
            this.roleNameType = roleNameType;
        }

        /// <copydoc cref="IHmacAuthenticator.Authenticate" />
        /// <remarks>Enrichs the authenticated principal with claims provided by the <see cref="IRequestClaimsProvider"/></remarks>
        public async Task<ClaimsIdentity> Authenticate(HttpRequestMessage request)
        {
            var authenticated = await validator.IsValid(request);
            if (authenticated == false)
            {
                // Not authenticated, so no principal.
                return null;
            }

            // Get all the claims we can derive from the request e.g. the HmacAuthentication.ClientIdHeader
            var claims = new List<Claim>(claimsProvider.Claims(request));

            var identity = new ClaimsIdentity(claims, HmacAuthentication.AuthenticationScheme, nameClaimType, roleNameType);

            return identity;
        }
    }
}