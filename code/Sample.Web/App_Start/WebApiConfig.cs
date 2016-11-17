using System.Linq;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;
using System.Web.Http.Tracing;

using Microsoft.Practices.Unity.WebApi;

using Sample.Web.Http;

namespace Sample.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Tracing and error handling
            //var writer = config.EnableSystemDiagnosticsTracing();
            //writer.IsVerbose = true;
            //writer.MinimumLevel = TraceLevel.Debug;
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            // Wire up dependency resolution for WebAPI
            var container = UnityConfig.GetConfiguredContainer();
            config.DependencyResolver = new UnityHierarchicalDependencyResolver(container);

            // Web API configuration and services

            // Use Unity filter provider to get attribute injection
            var providers = config.Services.GetFilterProviders().ToList();
            var defaultProvider = providers.Single(x => x is ActionDescriptorFilterProvider);
            config.Services.Remove(typeof(IFilterProvider), defaultProvider);
            config.Services.Add(typeof(IFilterProvider), new UnityFilterProvider(container));

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                     name: "WithActionApi",
                     routeTemplate: "api/{controller}/{action}/{id}",
                     defaults: new { id = System.Web.Http.RouteParameter.Optional }
                     );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}