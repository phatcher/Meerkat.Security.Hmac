using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using Meerkat.Owin.Security.Infrastructure;
using Meerkat.Security.Web.Http;

using Microsoft.Owin;
using Microsoft.Practices.Unity.Mvc;

using Owin;

[assembly: OwinStartup(typeof(Sample.Web.Startup))]

namespace Sample.Web
{
    public class Startup
    {
        ~Startup()
        {
            // Needed for use with OwinTestServer
            UseOwinHmac = true;
        }

        public static bool UseOwinHmac { get; set; }

        public static bool IgnoreMvc { get; set; }

        public static HttpConfiguration Config { get; private set; }

        public static void Reset()
        {
            UnityConfig.Container = null;
            Config = null;
        }

        public void Configuration(IAppBuilder app)
        {
            var container = UnityConfig.GetConfiguredContainer();

            // MVC configuration
            // NB Have to flip sense as run before static
            if (!IgnoreMvc)
            {
                FilterProviders.Providers.Remove(FilterProviders.Providers.OfType<FilterAttributeFilterProvider>().First());
                FilterProviders.Providers.Add(new UnityFilterAttributeFilterProvider(container));

                DependencyResolver.SetResolver(new UnityDependencyResolver(container));

                AreaRegistration.RegisterAllAreas();
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                BundleConfig.RegisterBundles(BundleTable.Bundles);
            }

            // WebAPI configuration
            Config = new HttpConfiguration();
            WebApiConfig.Register(Config);

            // Owin config
            if (UseOwinHmac)
            {
                app.UseHmacAuthentication(UnityConfig.ServiceLocator);
            }
            else
            {
                Config.Filters.Add(new HmacAuthenticationAttribute());

            }
            app.UseWebApi(Config);
        }
    }
}