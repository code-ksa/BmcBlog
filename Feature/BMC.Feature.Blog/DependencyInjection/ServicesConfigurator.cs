using BMC.Feature.Blog.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

namespace BMC.Feature.Blog.DependencyInjection
{
    /// <summary>
    /// Configures dependency injection for the Blog feature
    /// </summary>
    public class ServicesConfigurator : IServicesConfigurator
    {
        /// <summary>
        /// Configures services for dependency injection
        /// </summary>
        public void Configure(IServiceCollection serviceCollection)
        {
            // Register repositories
            serviceCollection.AddTransient<BlogRepository>();

            // Register services if any are added in the future
            // serviceCollection.AddTransient<IBlogService, BlogService>();

            // Register other dependencies as needed
        }
    }
}
