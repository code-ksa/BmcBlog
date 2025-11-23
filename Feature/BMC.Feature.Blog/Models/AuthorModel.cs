using Sitecore.Data.Items;
using Sitecore.Resources.Media;

namespace BMC.Feature.Blog.Models
{
    public class AuthorModel
    {
        public string Name { get; set; }
        public string Bio { get; set; }
        public string PhotoUrl { get; set; }
        public string Email { get; set; }

        public AuthorModel(Item item)
        {
            if (item == null)
                return;

            Name = item.Fields["Name"]?.Value ?? item.Name;
            Bio = item.Fields["Bio"]?.Value ?? string.Empty;
            Email = item.Fields["Email"]?.Value ?? string.Empty;

            var photoField = item.Fields["Photo"];
            if (photoField != null && !string.IsNullOrEmpty(photoField.Value))
            {
                var photoItem = item.Database.GetItem(photoField.Value);
                if (photoItem != null)
                {
                    var mediaUrlOptions = new MediaUrlOptions { AlwaysIncludeServerUrl = false };
                    PhotoUrl = MediaManager.GetMediaUrl(photoItem, mediaUrlOptions);
                }
            }
        }
    }
}