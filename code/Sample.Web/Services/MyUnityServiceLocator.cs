using System;
using System.Collections.Generic;
using CommonServiceLocator;

using Unity;
using Unity.Lifetime;
using Unity.ServiceLocation;

namespace Sample.Web.Services
{
    /// <summary>
    /// An implementation of <see cref="IServiceLocator"/> that wraps a Unity container.
    /// <remarks>
    /// Differs from the standard Microsoft implementation in that we put ourselves into the container
    /// using <see cref="ContainerControlledLifetimeManager" />, the original used a lifetime manager
    /// that held a weak reference which can lead to the ServiceLocator disappearing from the container!
    /// </remarks>
    /// </summary>
    public class MyUnityServiceLocator : ServiceLocatorImplBase, IDisposable
    {
        private IUnityContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityServiceLocator"/> class for a container.
        /// </summary>
        /// <param name="container">The <see cref="IUnityContainer"/> to wrap with the <see cref="IServiceLocator"/>
        /// interface implementation.</param>
        public MyUnityServiceLocator(IUnityContainer container)
        {
            this.container = container;
            container.RegisterInstance<IServiceLocator>(this, new ContainerControlledLifetimeManager());
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly",
            Justification = "Object is not finalizable, no reason to call SuppressFinalize")]
        public void Dispose()
        {
            if (container == null)
            {
                return;
            }

            // take a copy of the container and set the variable to null before calling Dispose()
            // This prevents an infinite loop where container.Dispose() calls this Dispose() function and container is always != null
            var tempContainer = container;
            container = null;
            tempContainer.Dispose();
        }

        /// <summary>
        /// When implemented by inheriting classes, this method will do the actual work of resolving the requested service instance.
        /// </summary>
        /// <param name="serviceType">Type of instance requested.</param><param name="key">Name of registered service you want. May be null.</param>
        /// <returns>
        /// The requested service instance.
        /// </returns>
        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (container == null)
            {
                throw new ObjectDisposedException("container");
            }

            return container.Resolve(serviceType, key);
        }

        /// <summary>
        /// When implemented by inheriting classes, this method will do the actual work of resolving all the requested service instances.
        /// </summary>
        /// <param name="serviceType">Type of service requested.</param>
        /// <returns>
        /// Sequence of service instance objects.
        /// </returns>
        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            if (container == null)
            {
                throw new ObjectDisposedException("container");
            }

            return container.ResolveAll(serviceType);
        }
    }
}