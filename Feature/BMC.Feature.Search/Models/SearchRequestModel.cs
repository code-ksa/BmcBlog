namespace BMC.Feature.Search.Models
{
    public class SearchRequestModel
    {
        public string Query { get; set; }
        public string Category { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public SearchRequestModel()
        {
            PageNumber = 1;
            PageSize = 10;
            Query = string.Empty;
            Category = string.Empty;
        }
    }
}