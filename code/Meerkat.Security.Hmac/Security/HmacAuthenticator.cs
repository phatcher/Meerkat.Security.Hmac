using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

using Meerkat.Net.Http;

namespace Meerkat.Security
{
    /// <summary>
    /// Implementation of <see cref="IHmacAuthenticator"/>
    /// </summary>
    public class HmacAuthenticator : IHmacAuthenticator
    {
        private readonly ISignatureValidator validator;
        private readonly string nameClaimType;
        private readonly string roleNameType;

        /// <summary>
        /// Creates a new instance of the <see cref="HmacAuthenticator"/> class.
        /// </summary>
        /// <param name="validator">Validator to use</param>
        /// <param name="nameClaimType">Name of the name claim type</param>
        /// <param name="roleNameType">Name of the role claim type</param>
        public HmacAuthenticator(ISignatureValidator validator, string nameClaimType, string roleNameType)
        {
            this.validator = validator;
            this.nameClaimType = nameClaimType;
            this.roleNameType = roleNameType;
        }

        /// <copydoc cref="IHmacAuthenticator.Authenticate" />
        public async Task<IPrincipal> Authenticate(HttpRequestMessage request)
        {
            var authenticated = await validator.IsValid(request);
            if (authenticated == false)
            {
                // Not authenticated, so no principal.
                return null;
            }

            var clientId = request.Headers.GetFirstOrDefaultValue<string>(HmacAuthentication.ClientIdHeader);
            var claims = new List<Claim>
            {
                // NB: We won't add further claims here, this the responsiblilty of the ClaimsTransformer
                new Claim(nameClaimType, clientId)
            };

            var identity = new ClaimsIdentity(claims, HmacAuthentication.AuthenticationScheme, nameClaimType, roleNameType);
            var principal = new ClaimsPrincipal(identity);

            return principal;
        }
    }
}