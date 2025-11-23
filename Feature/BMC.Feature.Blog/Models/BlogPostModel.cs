using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data.Items;
using Sitecore.Resources.Media;

namespace BMC.Feature.Blog.Models
{
    public class BlogPostModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Summary { get; set; }
        public DateTime PublishDate { get; set; }
        public string FeaturedImageUrl { get; set; }
        public Item Author { get; set; }
        public Item Category { get; set; }
        public int ViewCount { get; set; }
        public List<Item> Tags { get; set; }

        public BlogPostModel(Item item)
        {
            if (item == null)
                return;

            Title = item.Fields["Title"]?.Value ?? string.Empty;
            Content = item.Fields["Content"]?.Value ?? string.Empty;
            Summary = item.Fields["Short Description"]?.Value ?? string.Empty;

            var publishDateField = item.Fields["Publish Date"];
            if (publishDateField != null && !string.IsNullOrEmpty(publishDateField.Value))
            {
                DateTime.TryParse(publishDateField.Value, out DateTime parsedDate);
                PublishDate = parsedDate;
            }

            var featuredImageField = item.Fields["Featured Image"];
            if (featuredImageField != null && !string.IsNullOrEmpty(featuredImageField.Value))
            {
                var mediaItem = item.Database.GetItem(featuredImageField.Value);
                if (mediaItem != null)
                {
                    var mediaUrlOptions = new MediaUrlOptions { AlwaysIncludeServerUrl = false };
                    FeaturedImageUrl = MediaManager.GetMediaUrl(mediaItem, mediaUrlOptions);
                }
            }

            var authorField = item.Fields["Author"];
            if (authorField != null && !string.IsNullOrEmpty(authorField.Value))
            {
                Author = item.Database.GetItem(authorField.Value);
            }

            var categoryField = item.Fields["Category"];
            if (categoryField != null && !string.IsNullOrEmpty(categoryField.Value))
            {
                Category = item.Database.GetItem(categoryField.Value);
            }

            var viewCountField = item.Fields["View Count"];
            if (viewCountField != null && !string.IsNullOrEmpty(viewCountField.Value))
            {
                int.TryParse(viewCountField.Value, out int count);
                ViewCount = count;
            }

            Tags = new List<Item>();
            var tagsField = item.Fields["Tags"];
            if (tagsField != null && !string.IsNullOrEmpty(tagsField.Value))
            {
                var tagIds = tagsField.Value.Split('|');
                foreach (var tagId in tagIds)
                {
                    if (string.IsNullOrEmpty(tagId))
                        continue;

                    var tagItem = item.Database.GetItem(tagId);
                    if (tagItem != null)
                    {
                        Tags.Add(tagItem);
                    }
                }
            }
        }
    }
}