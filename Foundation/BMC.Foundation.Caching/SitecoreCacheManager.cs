using Sitecore.Caching;

namespace BMC.Foundation.Caching
{
    public class SitecoreCacheManager
    {
        private const long DefaultCacheSize = 10485760;

        public object Get(string key, string cacheName)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(cacheName))
                return null;

            var cache = GetCache(cacheName);
            if (cache == null)
                return null;

            return cache.GetValue(key);
        }

        public void Set(string key, object value, string cacheName)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(cacheName) || value == null)
                return;

            var cache = GetCache(cacheName);
            cache?.Add(key, value);
        }

        public void Remove(string key, string cacheName)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(cacheName))
                return;

            var cache = GetCache(cacheName);
            cache?.Remove(key);
        }

        public void Clear(string cacheName)
        {
            if (string.IsNullOrEmpty(cacheName))
                return;

            var cache = GetCache(cacheName);
            cache?.Clear();
        }

        private ICache GetCache(string cacheName)
        {
            return Sitecore.Caching.CacheManager.GetNamedInstance(cacheName, DefaultCacheSize, true);
        }
    }
}