using Microsoft.Extensions.DependencyInjection;

namespace BMC.Foundation.DependencyInjection.Extensions
{
    /// <summary>
    /// Extension methods for IServiceCollection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Foundation layer services to the service collection
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection for method chaining</returns>
        public static IServiceCollection AddFoundationServices(this IServiceCollection services)
        {
            if (services == null)
                return new ServiceCollection();

            // Register caching services
            // services.AddSingleton<ICacheService, CacheService>();

            // Register Sitecore extension services
            // services.AddScoped<ISitecoreService, SitecoreService>();

            // Register indexing services
            // services.AddScoped<IIndexingService, IndexingService>();

            // Register multisite services
            // services.AddSingleton<IMultisiteService, MultisiteService>();

            return services;
        }

        /// <summary>
        /// Adds Feature layer services to the service collection
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection for method chaining</returns>
        public static IServiceCollection AddFeatureServices(this IServiceCollection services)
        {
            if (services == null)
                return new ServiceCollection();

            // Register blog services
            // services.AddScoped<IBlogService, BlogService>();

            // Register navigation services
            // services.AddScoped<INavigationService, NavigationService>();

            // Register search services
            // services.AddScoped<ISearchService, SearchService>();

            // Register newsletter services
            // services.AddScoped<INewsletterService, NewsletterService>();

            // Register identity services
            // services.AddScoped<IIdentityService, IdentityService>();

            // Register comments services
            // services.AddScoped<ICommentService, CommentService>();

            return services;
        }

        /// <summary>
        /// Adds all BMC services (Foundation + Feature) to the service collection
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection for method chaining</returns>
        public static IServiceCollection AddBmcServices(this IServiceCollection services)
        {
            services.AddFoundationServices();
            services.AddFeatureServices();

            return services;
        }
    }
}
