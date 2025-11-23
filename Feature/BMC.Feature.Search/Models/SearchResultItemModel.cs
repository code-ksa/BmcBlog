using System;

namespace BMC.Feature.Search.Models
{
    public class SearchResultItemModel
    {
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Url { get; set; }
        public DateTime PublishDate { get; set; }
        public string CategoryName { get; set; }

        public SearchResultItemModel()
        {
            Title = string.Empty;
            Summary = string.Empty;
            Url = string.Empty;
            CategoryName = string.Empty;
            PublishDate = DateTime.MinValue;
        }
    }
}