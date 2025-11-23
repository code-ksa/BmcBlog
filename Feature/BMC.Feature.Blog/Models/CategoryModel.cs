using Sitecore.Data.Items;
using Sitecore.Resources.Media;

namespace BMC.Feature.Blog.Models
{
    public class CategoryModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public string HeroImageUrl { get; set; }

        public CategoryModel(Item item)
        {
            if (item == null)
                return;

            Name = item.Fields["Name"]?.Value ?? item.Name;
            Description = item.Fields["Description"]?.Value ?? string.Empty;

            var iconField = item.Fields["Icon"];
            if (iconField != null && !string.IsNullOrEmpty(iconField.Value))
            {
                var iconItem = item.Database.GetItem(iconField.Value);
                if (iconItem != null)
                {
                    var mediaUrlOptions = new MediaUrlOptions { AlwaysIncludeServerUrl = false };
                    IconUrl = MediaManager.GetMediaUrl(iconItem, mediaUrlOptions);
                }
            }

            var heroImageField = item.Fields["Hero Image"];
            if (heroImageField != null && !string.IsNullOrEmpty(heroImageField.Value))
            {
                var heroImageItem = item.Database.GetItem(heroImageField.Value);
                if (heroImageItem != null)
                {
                    var mediaUrlOptions = new MediaUrlOptions { AlwaysIncludeServerUrl = false };
                    HeroImageUrl = MediaManager.GetMediaUrl(heroImageItem, mediaUrlOptions);
                }
            }
        }
    }
}