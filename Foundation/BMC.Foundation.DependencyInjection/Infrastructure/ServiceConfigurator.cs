using Microsoft.Extensions.DependencyInjection;

namespace BMC.Foundation.DependencyInjection.Infrastructure
{
    /// <summary>
    /// Configures services for dependency injection container
    /// </summary>
    public static class ServiceConfigurator
    {
        /// <summary>
        /// Configures all services from Foundation and Feature layers
        /// </summary>
        /// <param name="services">The service collection to configure</param>
        /// <returns>Configured service collection</returns>
        public static IServiceCollection ConfigureServices(IServiceCollection services)
        {
            if (services == null)
                return new ServiceCollection();

            // Register Foundation layer services
            RegisterFoundationServices(services);

            // Register Feature layer services
            RegisterFeatureServices(services);

            return services;
        }

        /// <summary>
        /// Registers services from Foundation layer projects
        /// </summary>
        /// <param name="services">The service collection</param>
        private static void RegisterFoundationServices(IServiceCollection services)
        {
            // Register foundation services here
            // Example: services.AddSingleton<IFoundationService, FoundationService>();
        }

        /// <summary>
        /// Registers services from Feature layer projects
        /// </summary>
        /// <param name="services">The service collection</param>
        private static void RegisterFeatureServices(IServiceCollection services)
        {
            // Register feature services here
            // Example: services.AddScoped<IFeatureService, FeatureService>();
        }
    }
}
