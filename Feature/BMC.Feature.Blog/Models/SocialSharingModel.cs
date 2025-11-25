using Sitecore.Data.Items;
using Sitecore.Links;
using System.Web;

namespace BMC.Feature.Blog.Models
{
    public class SocialSharingModel
    {
        public bool EnableFacebook { get; set; }
        public bool EnableTwitter { get; set; }
        public bool EnableLinkedIn { get; set; }
        public string PageUrl { get; set; }
        public string PageTitle { get; set; }

        public SocialSharingModel(Item item)
        {
            if (item == null) return;

            // قراءة الإعدادات من الـ Item نفسه أو من إعدادات الموقع
            EnableFacebook = item["EnableFacebook"] == "1";
            EnableTwitter = item["EnableTwitter"] == "1";
            EnableLinkedIn = item["EnableLinkedIn"] == "1";

            var options = LinkManager.GetDefaultUrlOptions();
            options.AlwaysIncludeServerUrl = true; // مهم للمشاركة

            PageUrl = LinkManager.GetItemUrl(item, options);
            PageTitle = item.DisplayName;
        }

        // دوال مساعدة لتوليد الروابط
        public string FacebookUrl => $"https://www.facebook.com/sharer/sharer.php?u={HttpUtility.UrlEncode(PageUrl)}";
        public string TwitterUrl => $"https://twitter.com/intent/tweet?text={HttpUtility.UrlEncode(PageTitle)}&url={HttpUtility.UrlEncode(PageUrl)}";
        public string LinkedInUrl => $"https://www.linkedin.com/sharing/share-offsite/?url={HttpUtility.UrlEncode(PageUrl)}";
    }
}