using Sitecore.Diagnostics;
using Sitecore.Pipelines;

namespace BMC.Foundation.DependencyInjection.Pipelines
{
    /// <summary>
    /// Pipeline processor to initialize dependency injection container on application start
    /// </summary>
    public class InitializeDependencyInjection
    {
        /// <summary>
        /// Processes the initialize pipeline to build the DI container
        /// </summary>
        /// <param name="args">Pipeline arguments</param>
        public virtual void Process(PipelineArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));

            try
            {
                Log.Info("BMC.Foundation.DependencyInjection: Initializing dependency injection container...", this);

                // Build the DI container
                ContainerBuilder.BuildContainer();

                Log.Info("BMC.Foundation.DependencyInjection: Dependency injection container initialized successfully.", this);
            }
            catch (System.Exception ex)
            {
                Log.Error("BMC.Foundation.DependencyInjection: Failed to initialize dependency injection container.", ex, this);
                throw;
            }
        }
    }
}
