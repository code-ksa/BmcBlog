using Sitecore.Data.Items;
using Sitecore.Resources.Media;

namespace BMC.Feature.Hero.Models
{
    public class HeroSlideModel
    {
        public string BackgroundImageUrl { get; set; }
        public string TitleEN { get; set; }
        public string TitleAR { get; set; }
        public string DescriptionEN { get; set; }
        public string DescriptionAR { get; set; }
        public string CTALinkUrl { get; set; }
        public string CTALinkText { get; set; }

        public HeroSlideModel(Item item)
        {
            if (item == null)
                return;

            TitleEN = item.Fields["Title EN"]?.Value ?? string.Empty;
            TitleAR = item.Fields["Title AR"]?.Value ?? string.Empty;
            DescriptionEN = item.Fields["Description EN"]?.Value ?? string.Empty;
            DescriptionAR = item.Fields["Description AR"]?.Value ?? string.Empty;
            CTALinkText = item.Fields["CTA Link Text"]?.Value ?? string.Empty;

            var backgroundImageField = item.Fields["Background Image"];
            if (backgroundImageField != null && !string.IsNullOrEmpty(backgroundImageField.Value))
            {
                var mediaItem = item.Database.GetItem(backgroundImageField.Value);
                if (mediaItem != null)
                {
                    var mediaUrlOptions = new MediaUrlOptions { AlwaysIncludeServerUrl = false };
                    BackgroundImageUrl = MediaManager.GetMediaUrl(mediaItem, mediaUrlOptions);
                }
            }

            var ctaLinkField = item.Fields["CTA Link"];
            if (ctaLinkField != null && !string.IsNullOrEmpty(ctaLinkField.Value))
            {
                var linkField = (Sitecore.Data.Fields.LinkField)item.Fields["CTA Link"];
                if (linkField != null && linkField.TargetItem != null)
                {
                    CTALinkUrl = Sitecore.Links.LinkManager.GetItemUrl(linkField.TargetItem);
                }
                else if (linkField != null && !string.IsNullOrEmpty(linkField.Url))
                {
                    CTALinkUrl = linkField.Url;
                }
            }
        }
    }
}