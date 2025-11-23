using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Maintenance;

namespace BMC.Foundation.Indexing.Providers
{
    public class CustomIndexConfiguration
    {
        public static ISearchIndex GetIndexConfiguration()
        {
            return GetIndexConfiguration("bmc_blog_index");
        }

        public static ISearchIndex GetIndexConfiguration(string indexName)
        {
            if (string.IsNullOrEmpty(indexName))
                return null;

            var index = ContentSearchManager.GetIndex(indexName);
            return index;
        }

        public static void RebuildIndex(string indexName)
        {
            if (string.IsNullOrEmpty(indexName))
                return;

            var index = ContentSearchManager.GetIndex(indexName);
            if (index != null)
            {
                IndexCustodian.FullRebuild(index, true);
            }
        }

        public static void RefreshIndex(string indexName)
        {
            if (string.IsNullOrEmpty(indexName))
                return;

            var index = ContentSearchManager.GetIndex(indexName);
            if (index != null)
            {
                var indexable = new SitecoreIndexableItem(Sitecore.Data.Database.GetDatabase("master").GetRootItem());
                index.Refresh(indexable);
            }
        }
    }
}