using Sitecore.Data.Items;
using Sitecore.Data.Fields;

namespace BMC.Feature.Blog.Models
{
    public class PodcastEpisodeModel
    {
        public string Title { get; set; }
        public string Summary { get; set; }
        public string AudioUrl { get; set; }
        public string Duration { get; set; }
        public string ImageUrl { get; set; }

        public PodcastEpisodeModel(Item item)
        {
            if (item == null) return;

            Title = item["EpisodeTitle"];
            Summary = item["EpisodeSummary"];
            Duration = item["Duration"];

            // جلب رابط الصوت
            var audioField = (FileField)item.Fields["AudioFile"];
            if (audioField?.MediaItem != null)
            {
                AudioUrl = Sitecore.Resources.Media.MediaManager.GetMediaUrl(audioField.MediaItem);
            }

            // جلب الصورة
            var imgField = (ImageField)item.Fields["CoverImage"];
            if (imgField?.MediaItem != null)
            {
                ImageUrl = Sitecore.Resources.Media.MediaManager.GetMediaUrl(imgField.MediaItem);
            }
        }
    }
}