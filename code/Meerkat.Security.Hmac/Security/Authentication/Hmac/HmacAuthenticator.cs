using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

using Meerkat.Security.Authorization;

namespace Meerkat.Security.Authentication.Hmac
{
    /// <summary>
    /// Validates a HMAC encoded message, creating a <see cref="ClaimsIdentity"/> with claims provided by a <see cref="IRequestClaimsProvider"/>.
    /// </summary>
    public class HmacAuthenticator : IHmacAuthenticator
    {
        private readonly ISignatureValidator validator;
        private readonly IRequestClaimsProvider claimsProvider;
        private readonly string nameClaimType;
        private readonly string roleClaimType;

        /// <summary>
        /// Creates a new instance of the <see cref="HmacAuthenticator"/> class.
        /// </summary>
        /// <param name="validator">Validator to use</param>
        /// <param name="claimsProvider">Claims provider to process the request</param>
        /// <param name="nameClaimType">Name of the name claim type</param>
        /// <param name="roleClaimType">Name of the role claim type</param>
        public HmacAuthenticator(ISignatureValidator validator, IRequestClaimsProvider claimsProvider, string nameClaimType, string roleClaimType)
        {
            this.validator = validator;
            this.claimsProvider = claimsProvider;
            this.nameClaimType = nameClaimType;
            this.roleClaimType = roleClaimType;
        }

        /// <copydoc cref="IHmacAuthenticator.Authenticate" />
        /// <remarks>Enriches the authenticated principal with claims provided by the <see cref="IRequestClaimsProvider"/></remarks>
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

            var identity = new ClaimsIdentity(claims, HmacAuthentication.AuthenticationScheme, nameClaimType, roleClaimType);

            return identity;
        }
    }
}