using System;
using System.Linq;
using BMC.Feature.Search.Models;
using BMC.Foundation.Indexing.SearchModels;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.Linq.Utilities;

namespace BMC.Feature.Search.Services
{
    public class SearchService
    {
        private const string IndexName = "bmc_blog_index";

        public SearchResultModel Search(string query)
        {
            if (string.IsNullOrEmpty(query))
                return new SearchResultModel();

            var request = new SearchRequestModel
            {
                Query = query
            };

            return SearchAdvanced(request);
        }

        public SearchResultModel SearchByCategory(string category)
        {
            if (string.IsNullOrEmpty(category))
                return new SearchResultModel();

            var request = new SearchRequestModel
            {
                Category = category
            };

            return SearchAdvanced(request);
        }

        public SearchResultModel SearchAdvanced(SearchRequestModel request)
        {
            if (request == null)
                return new SearchResultModel();

            var result = new SearchResultModel
            {
                CurrentPage = request.PageNumber
            };

            try
            {
                var index = ContentSearchManager.GetIndex(IndexName);
                if (index == null)
                    return result;

                using (var context = index.CreateSearchContext())
                {
                    var queryable = context.GetQueryable<BlogSearchResultItem>();

                    var predicate = PredicateBuilder.True<BlogSearchResultItem>();

                    if (!string.IsNullOrEmpty(request.Query))
                    {
                        var queryPredicate = PredicateBuilder.False<BlogSearchResultItem>();
                        queryPredicate = queryPredicate.Or(item => item.Title.Contains(request.Query));
                        queryPredicate = queryPredicate.Or(item => item.Content.Contains(request.Query));
                        queryPredicate = queryPredicate.Or(item => item.Summary.Contains(request.Query));
                        
                        predicate = predicate.And(queryPredicate);
                    }

                    if (!string.IsNullOrEmpty(request.Category))
                    {
                        predicate = predicate.And(item => item.Category == request.Category);
                    }

                    queryable = queryable.Where(predicate);

                    var totalResults = queryable.Count();
                    result.TotalResults = totalResults;

                    var searchResults = queryable
                        .OrderByDescending(item => item.PublishDate)
                        .Skip((request.PageNumber - 1) * request.PageSize)
                        .Take(request.PageSize)
                        .ToList();

                    foreach (var searchResult in searchResults)
                    {
                        var resultItem = new SearchResultItemModel
                        {
                            Title = searchResult.Title ?? string.Empty,
                            Summary = searchResult.ShortDescription ?? searchResult.Summary ?? string.Empty,
                            Url = searchResult.Url ?? string.Empty,
                            PublishDate = searchResult.PublishDate,
                            CategoryName = searchResult.Category ?? string.Empty
                        };

                        result.Items.Add(resultItem);
                    }

                    var pageSize = request.PageSize > 0 ? request.PageSize : 10;
                    result.TotalPages = (totalResults + pageSize - 1) / pageSize;
                }
            }
            catch (Exception)
            {
                // Log exception if needed
                result = new SearchResultModel
                {
                    CurrentPage = request.PageNumber
                };
            }

            return result;
        }
    }
}