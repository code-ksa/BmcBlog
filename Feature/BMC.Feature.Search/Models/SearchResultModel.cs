using System.Collections.Generic;

namespace BMC.Feature.Search.Models
{
    public class SearchResultModel
    {
        public List<SearchResultItemModel> Items { get; set; }
        public int TotalResults { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public SearchResultModel()
        {
            Items = new List<SearchResultItemModel>();
            TotalResults = 0;
            CurrentPage = 1;
            TotalPages = 0;
        }

        public void CalculateTotalPages()
        {
            if (TotalResults > 0 && Items != null && Items.Count > 0)
            {
                var pageSize = Items.Count > 0 ? Items.Count : 10;
                TotalPages = (TotalResults + pageSize - 1) / pageSize;
            }
            else
            {
                TotalPages = 0;
            }
        }
    }
}