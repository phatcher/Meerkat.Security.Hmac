using System.Web.Http.Dependencies;

using Meerkat.Security.Authentication.Hmac;

using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security.Infrastructure;

using Owin;

namespace Meerkat.Owin.Security.Infrastructure
{
    /// <summary>
    /// Creates a <see cref="HmacAuthenticationHandler"/> using <see cref="IDependencyResolver"/>.
    /// </summary>
    public class HmacAuthenticationMiddleware : AuthenticationMiddleware<HmacAuthenticationOptions>
    {
        private readonly ILogger logger;
        private readonly IDependencyResolver resolver;

        /// <summary>
        /// Create a new instance of the <see cref="HmacAuthenticationMiddleware"/> class.
        /// </summary>
        /// <param name="next"></param>
        /// <param name="options"></param>
        /// <param name="app"></param>
        /// <param name="resolver"></param>
        public HmacAuthenticationMiddleware(OwinMiddleware next, IAppBuilder app, IDependencyResolver resolver, HmacAuthenticationOptions options) : base(next, options)
        {
            logger = app.CreateLogger<HmacAuthenticationHandler>();
            this.resolver = resolver;
        }

        /// <copydoc cref="AuthenticationMiddleware{TOptions}.CreateHandler" />
        protected override AuthenticationHandler<HmacAuthenticationOptions> CreateHandler()
        {
            var authenticator = GetService<IHmacAuthenticator>();
            return new HmacAuthenticationHandler(logger, authenticator);
        }

        private T GetService<T>()
            where T : class
        {
            // NB Presumes we have an appropriate scope.
            // Would like to use context.GetDependencyScope but MS didn't pass the context to CreateHandler and it uses internal methods so can't re-implement.
            // If implementing using an EF backed store for client secrets, use PerResolve for this specific repository, doesn't need much state and avoids connection dispose issues.
            return resolver.GetService<T>();
        }
    }
}
