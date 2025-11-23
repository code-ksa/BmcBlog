using System.Web.Mvc;
using BMC.Feature.Search.Models;
using BMC.Feature.Search.Services;

namespace BMC.Feature.Search.Controllers
{
    public class SearchController : Controller
    {
        private readonly SearchService _searchService;

        public SearchController()
        {
            _searchService = new SearchService();
        }

        public ActionResult Index()
        {
            var model = new SearchRequestModel();
            return View(model);
        }

        [HttpGet]
        public ActionResult Results(SearchRequestModel model)
        {
            if (model == null)
            {
                model = new SearchRequestModel();
            }

            SearchResultModel results;

            if (!string.IsNullOrEmpty(model.Query) || !string.IsNullOrEmpty(model.Category))
            {
                results = _searchService.SearchAdvanced(model);
            }
            else
            {
                results = new SearchResultModel
                {
                    CurrentPage = model.PageNumber
                };
            }

            ViewBag.SearchRequest = model;

            return View(results);
        }

        [HttpPost]
        public ActionResult Results(string query, string category, int pageNumber = 1)
        {
            var model = new SearchRequestModel
            {
                Query = query,
                Category = category,
                PageNumber = pageNumber
            };

            return Results(model);
        }
    }
}