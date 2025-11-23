using System;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Pipelines.Response.RenderRendering;

namespace BMC.Feature.Blog.Pipelines
{
    /// <summary>
    /// Handles caching for blog renderings
    /// </summary>
    public class CacheBlogRendering : RenderRenderingProcessor
    {
        private const string CacheKeyPrefix = "BMC.Blog.Rendering_";

        public override void Process(RenderRenderingArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));

            if (args.Rendering == null || !IsCachingEnabled())
                return;

            // Check if this is a blog-related rendering
            if (!IsBlogRendering(args))
                return;

            var cacheKey = GetCacheKey(args);
            var cache = GetCache();

            if (cache == null)
                return;

            // Try to get cached content
            var cachedContent = cache.GetHtml(cacheKey);
            if (!string.IsNullOrEmpty(cachedContent))
            {
                args.Writer.Write(cachedContent);
                args.AbortPipeline();
                Log.Debug($"Blog rendering served from cache: {cacheKey}", this);
                return;
            }

            // Cache will be populated by subsequent processors
            Log.Debug($"Blog rendering not found in cache: {cacheKey}", this);
        }

        private bool IsCachingEnabled()
        {
            var setting = Settings.GetBoolSetting("BMC.Feature.Blog.EnableCaching", true);
            return setting;
        }

        private bool IsBlogRendering(RenderRenderingArgs args)
        {
            // Check if we're in a blog context
            if (Context.Items["IsBlogContext"] as bool? == true ||
                Context.Items["IsBlogPost"] as bool? == true)
            {
                return true;
            }

            // Check rendering datasource
            if (args.Rendering.Item != null)
            {
                var templateName = args.Rendering.Item.TemplateName;
                if (templateName == "Blog Post" ||
                    templateName == "Blog Root" ||
                    templateName == "Category")
                {
                    return true;
                }
            }

            return false;
        }

        private string GetCacheKey(RenderRenderingArgs args)
        {
            var renderingId = args.Rendering.RenderingItem?.ID.ToString() ?? "unknown";
            var itemId = args.Rendering.Item?.ID.ToString() ?? "noitem";
            var language = Context.Language?.Name ?? "en";

            return $"{CacheKeyPrefix}{renderingId}_{itemId}_{language}";
        }

        private Sitecore.Caching.Cache GetCache()
        {
            return Sitecore.Caching.CacheManager.FindCacheByName("BMC.Blog.Cache");
        }
    }
}
