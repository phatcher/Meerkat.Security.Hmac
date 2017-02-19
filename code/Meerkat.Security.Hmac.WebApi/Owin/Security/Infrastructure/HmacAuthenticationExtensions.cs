using System;
using System.Web.Http.Dependencies;
using Owin;

namespace Meerkat.Owin.Security.Infrastructure
{
    public static class HmacAuthenticationExtensions
    {
        /// <summary>
        /// Adds HMAC authentication to the Owin pipeline
        /// </summary>
        /// <param name="app"></param>
        /// <param name="resolver"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IAppBuilder UseHmacAuthentication(this IAppBuilder app, IDependencyResolver resolver, HmacAuthenticationOptions options = null)
        {
            if (options == null)
            {
                options = new HmacAuthenticationOptions();
            }

            app.Use<HmacAuthenticationMiddleware>(app, resolver, options);

            return app;
        }

        /// <summary>
        /// Adds HMAC authentication to the Owin pipeline
        /// </summary>
        /// <param name="app"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        [Obsolete("Use version with IDepdedencyResolver so we get scoped dependencies")]
        public static IAppBuilder UseHmacAuthentication(this IAppBuilder app, IServiceProvider serviceProvider, HmacAuthenticationOptions options = null)
        {
            if (options == null)
            {
                options = new HmacAuthenticationOptions();
            }

            app.Use<HmacAuthenticationServiceProviderMiddleware>(app, serviceProvider, options);

            return app;
        }
    }
}
