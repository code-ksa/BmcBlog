using Sitecore.Data;

namespace BMC.Foundation.Caching
{
    public static class CacheKeys
    {
        public const string BlogPostPrefix = "blogpost_";
        public const string CategoryPrefix = "category_";
        public const string NavigationPrefix = "navigation_";
        public const string AuthorPrefix = "author_";
        public const string TagsPrefix = "tags_";

        public static string GetBlogPostKey(ID itemId)
        {
            if (itemId == (ID)null)
                return string.Empty;

            return BlogPostPrefix + itemId.ToString();
        }

        public static string GetCategoryKey(ID itemId)
        {
            if (itemId == (ID)null)
                return string.Empty;

            return CategoryPrefix + itemId.ToString();
        }

        public static string GetNavigationKey(ID itemId)
        {
            if (itemId == (ID)null)
                return string.Empty;

            return NavigationPrefix + itemId.ToString();
        }

        public static string GetAuthorKey(ID itemId)
        {
            if (itemId == (ID)null)
                return string.Empty;

            return AuthorPrefix + itemId.ToString();
        }

        public static string GetTagsKey(ID itemId)
        {
            if (itemId == (ID)null)
                return string.Empty;

            return TagsPrefix + itemId.ToString();
        }
    }
}