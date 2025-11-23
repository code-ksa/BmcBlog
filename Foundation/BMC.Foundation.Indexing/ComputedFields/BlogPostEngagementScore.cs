using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data.Items;

namespace BMC.Foundation.Indexing.ComputedFields
{
    public class BlogPostEngagementScore : IComputedIndexField
    {
        public string FieldName { get; set; }
        public string ReturnType { get; set; }

        public object ComputeFieldValue(IIndexable indexable)
        {
            var item = indexable as SitecoreIndexableItem;
            if (item == null)
                return 0;

            var sitecoreItem = (Item)item;
            if (sitecoreItem == null)
                return 0;

            // Get view count from item field
            var viewCountField = sitecoreItem.Fields["View Count"];
            var viewCount = 0;
            if (viewCountField != null && !string.IsNullOrEmpty(viewCountField.Value))
            {
                int.TryParse(viewCountField.Value, out viewCount);
            }

            // Get comment count from item field
            var commentCountField = sitecoreItem.Fields["Comment Count"];
            var commentCount = 0;
            if (commentCountField != null && !string.IsNullOrEmpty(commentCountField.Value))
            {
                int.TryParse(commentCountField.Value, out commentCount);
            }

            // Calculate engagement score
            // Formula: (ViewCount * 1) + (CommentCount * 5)
            // Comments are weighted higher as they indicate more engagement
            var engagementScore = (viewCount * 1) + (commentCount * 5);

            return engagementScore;
        }
    }
}