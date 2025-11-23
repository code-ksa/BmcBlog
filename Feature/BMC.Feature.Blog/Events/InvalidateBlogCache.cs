using System;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Events;

namespace BMC.Feature.Blog.Events
{
    /// <summary>
    /// Handles cache invalidation for blog-related items
    /// </summary>
    public class InvalidateBlogCache
    {
        /// <summary>
        /// Handles the item saved event
        /// </summary>
        public void OnItemSaved(object sender, EventArgs args)
        {
            try
            {
                var item = Event.ExtractParameter(args, 0) as Item;
                if (item == null)
                    return;

                if (IsBlogRelatedItem(item))
                {
                    InvalidateCache();
                    Log.Info($"Blog cache invalidated due to item saved: {item.Paths.FullPath}", this);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in InvalidateBlogCache.OnItemSaved", ex, this);
            }
        }

        /// <summary>
        /// Handles the item deleted event
        /// </summary>
        public void OnItemDeleted(object sender, EventArgs args)
        {
            try
            {
                var item = Event.ExtractParameter(args, 0) as Item;
                if (item == null)
                    return;

                if (IsBlogRelatedItem(item))
                {
                    InvalidateCache();
                    Log.Info($"Blog cache invalidated due to item deleted: {item.Paths.FullPath}", this);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in InvalidateBlogCache.OnItemDeleted", ex, this);
            }
        }

        /// <summary>
        /// Determines if an item is blog-related
        /// </summary>
        private bool IsBlogRelatedItem(Item item)
        {
            if (item == null)
                return false;

            // Check if item is a blog-related template
            var templateName = item.TemplateName;
            if (templateName == "Blog Post" ||
                templateName == "Blog Root" ||
                templateName == "Category" ||
                templateName == "Author" ||
                templateName == "Tag")
            {
                return true;
            }

            // Check if item is under blog content path
            var path = item.Paths.FullPath.ToLowerInvariant();
            if (path.Contains("/sitecore/content/bmc/bmcblog"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Invalidates all blog-related caches
        /// </summary>
        private void InvalidateCache()
        {
            try
            {
                // Clear the main blog cache
                var blogCache = Sitecore.Caching.CacheManager.FindCacheByName("BMC.Blog.Cache");
                if (blogCache != null)
                {
                    blogCache.Clear();
                    Log.Debug("BMC.Blog.Cache cleared", this);
                }

                // Clear the categories cache
                var categoriesCache = Sitecore.Caching.CacheManager.FindCacheByName("BMC.Blog.Categories.Cache");
                if (categoriesCache != null)
                {
                    categoriesCache.Clear();
                    Log.Debug("BMC.Blog.Categories.Cache cleared", this);
                }

                // Clear HTML cache for the blog site
                var htmlCache = Sitecore.Caching.CacheManager.GetHtmlCache(Context.Site);
                if (htmlCache != null)
                {
                    htmlCache.Clear();
                    Log.Debug("HTML cache cleared for blog site", this);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error clearing blog cache", ex, this);
            }
        }
    }
}
