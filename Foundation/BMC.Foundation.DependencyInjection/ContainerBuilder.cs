using System;
using Microsoft.Extensions.DependencyInjection;
using BMC.Foundation.DependencyInjection.Infrastructure;
using BMC.Foundation.DependencyInjection.Extensions;

namespace BMC.Foundation.DependencyInjection
{
    /// <summary>
    /// Builds and manages the dependency injection container
    /// </summary>
    public static class ContainerBuilder
    {
        private static IServiceProvider _serviceProvider;
        private static readonly object _lock = new object();

        /// <summary>
        /// Gets the configured service provider instance
        /// </summary>
        public static IServiceProvider ServiceProvider
        {
            get
            {
                if (_serviceProvider == null)
                {
                    lock (_lock)
                    {
                        if (_serviceProvider == null)
                        {
                            BuildContainer();
                        }
                    }
                }
                return _serviceProvider;
            }
        }

        /// <summary>
        /// Builds the dependency injection container with all registered services
        /// </summary>
        /// <returns>The configured service provider</returns>
        public static IServiceProvider BuildContainer()
        {
            lock (_lock)
            {
                var services = new ServiceCollection();

                // Configure all services
                ServiceConfigurator.ConfigureServices(services);

                // Add BMC services using extension methods
                services.AddBmcServices();

                // Build the service provider
                _serviceProvider = services.BuildServiceProvider();

                return _serviceProvider;
            }
        }

        /// <summary>
        /// Resolves a service of type T from the container
        /// </summary>
        /// <typeparam name="T">The type of service to resolve</typeparam>
        /// <returns>The resolved service instance</returns>
        public static T GetService<T>()
        {
            if (ServiceProvider == null)
            {
                throw new InvalidOperationException("Service provider has not been initialized. Call BuildContainer() first.");
            }

            return ServiceProvider.GetService<T>();
        }

        /// <summary>
        /// Resolves a required service of type T from the container
        /// </summary>
        /// <typeparam name="T">The type of service to resolve</typeparam>
        /// <returns>The resolved service instance</returns>
        /// <exception cref="InvalidOperationException">Thrown when the service cannot be resolved</exception>
        public static T GetRequiredService<T>()
        {
            if (ServiceProvider == null)
            {
                throw new InvalidOperationException("Service provider has not been initialized. Call BuildContainer() first.");
            }

            return ServiceProvider.GetRequiredService<T>();
        }

        /// <summary>
        /// Resets the container (useful for testing)
        /// </summary>
        public static void Reset()
        {
            lock (_lock)
            {
                if (_serviceProvider is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                _serviceProvider = null;
            }
        }
    }
}
