using System;

using Meerkat.Security.Authentication.Hmac;

using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security.Infrastructure;

using Owin;

namespace Meerkat.Owin.Security.Infrastructure
{
    /// <summary>
    /// Creates <see cref="HmacAuthenticationHandler"/> using <see cref="IServiceProvider"/>.
    /// </summary>
    public class HmacAuthenticationMiddleware : AuthenticationMiddleware<HmacAuthenticationOptions>
    {
        private readonly ILogger logger;
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Create a new instance of the <see cref="HmacAuthenticationMiddleware"/> class.
        /// </summary>
        /// <param name="next"></param>
        /// <param name="options"></param>
        /// <param name="app"></param>
        /// <param name="serviceProvider"></param>
        public HmacAuthenticationMiddleware(OwinMiddleware next, IAppBuilder app, IServiceProvider serviceProvider, HmacAuthenticationOptions options) : base(next, options)
        {
            logger = app.CreateLogger<HmacAuthenticationHandler>();
            this.serviceProvider = serviceProvider;
        }

        /// <copydoc cref="AuthenticationMiddleware{TOptions}.CreateHandler" />
        protected override AuthenticationHandler<HmacAuthenticationOptions> CreateHandler()
        {
            var authenticator = (IHmacAuthenticator) serviceProvider.GetService(typeof(IHmacAuthenticator));
            return new HmacAuthenticationHandler(logger, authenticator);
        }
    }
}