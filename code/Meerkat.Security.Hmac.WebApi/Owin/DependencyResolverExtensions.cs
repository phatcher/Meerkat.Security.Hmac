using System.Web.Http.Dependencies;

namespace Meerkat.Owin
{
    internal static class DependencyResolverExtensions
    {
        public static TService GetService<TService>(this IDependencyScope scope) where TService : class
        {
            return scope.GetService(typeof(TService)) as TService;
        }
    }
}