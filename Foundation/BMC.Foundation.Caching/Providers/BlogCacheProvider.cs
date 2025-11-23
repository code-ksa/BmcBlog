using Sitecore.Data;
using System;

namespace BMC.Foundation.Caching.Providers
{
    public class BlogCacheProvider
    {
        private readonly SitecoreCacheManager _cache;
        private const string CacheName = "BMC.Blog.Cache";

        public BlogCacheProvider()
        {
            _cache = new SitecoreCacheManager();
        }

        public void CacheBlogPost(ID itemId, object data)
        {
            if (itemId == (ID)null || data == null)
                return;

            var key = CacheKeys.GetBlogPostKey(itemId);
            _cache.Set(key, data, CacheName);
        }

        public object GetCachedBlogPost(ID itemId)
        {
            if (itemId == (ID)null)
                return null;

            var key = CacheKeys.GetBlogPostKey(itemId);
            return _cache.Get(key, CacheName);
        }

        public void CacheCategory(ID itemId, object data)
        {
            if (itemId == (ID)null || data == null)
                return;

            var key = CacheKeys.GetCategoryKey(itemId);
            _cache.Set(key, data, CacheName);
        }

        public object GetCachedCategory(ID itemId)
        {
            if (itemId == (ID)null)
                return null;

            var key = CacheKeys.GetCategoryKey(itemId);
            return _cache.Get(key, CacheName);
        }

        public void InvalidateBlogCache()
        {
            _cache.Clear(CacheName);
        }
    }
}