using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Resources.Media;

namespace BMC.Foundation.SitecoreExtensions.Helpers
{
    public static class LinkHelper
    {
        public static string GetItemUrl(Item item)
        {
            if (item == null)
                return string.Empty;

            var urlOptions = LinkManager.GetDefaultUrlOptions();
            urlOptions.AlwaysIncludeServerUrl = false;
            urlOptions.LowercaseUrls = true;

            return LinkManager.GetItemUrl(item, urlOptions);
        }

        public static string GetMediaUrl(MediaItem mediaItem)
        {
            if (mediaItem == null)
                return string.Empty;

            var mediaOptions = new MediaUrlOptions
            {
                AlwaysIncludeServerUrl = false,
                LowercaseUrls = true
            };

            return MediaManager.GetMediaUrl(mediaItem, mediaOptions);
        }

        public static bool IsInternalLink(Item item)
        {
            if (item == null)
                return false;

            return item.Paths.IsContentItem && 
                   !string.IsNullOrEmpty(item.Name) && 
                   item.Database != null;
        }
    }
}
