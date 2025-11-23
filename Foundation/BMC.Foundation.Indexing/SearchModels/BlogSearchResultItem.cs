using System;
using System.Collections.Generic;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;

namespace BMC.Foundation.Indexing.SearchModels
{
    public class BlogSearchResultItem : SearchResultItem
    {
        [IndexField("title")]
        public string Title { get; set; }

        [IndexField("content")]
        public string Content { get; set; }

        [IndexField("summary")]
        public string Summary { get; set; }

        [IndexField("category")]
        public string Category { get; set; }

        [IndexField("author")]
        public string Author { get; set; }

        [IndexField("publishdate")]
        public DateTime PublishDate { get; set; }

        [IndexField("tags")]
        public List<string> Tags { get; set; }

        [IndexField("viewcount")]
        public int ViewCount { get; set; }

        [IndexField("engagementscore")]
        public int EngagementScore { get; set; }

        [IndexField("shortdescription")]
        public string ShortDescription { get; set; }

        [IndexField("featuredimage")]
        public string FeaturedImage { get; set; }

        public BlogSearchResultItem()
        {
            Tags = new List<string>();
        }
    }
}