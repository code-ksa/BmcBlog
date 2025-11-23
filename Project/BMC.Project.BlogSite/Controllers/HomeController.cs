using System.Linq;
using System.Web.Mvc;
using BMC.Feature.Blog.Models;
using BMC.Feature.Blog.Repositories;

namespace BMC.Project.BlogSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly BlogRepository _blogRepository;

        public HomeController()
        {
            _blogRepository = new BlogRepository();
        }

        public ActionResult Index()
        {
            var contextItem = Sitecore.Context.Item;
            if (contextItem == null)
                return Content("No context item found");

            var latestPosts = _blogRepository.GetRecentPosts(6);
            var postModels = latestPosts.Select(p => new BlogPostModel(p)).ToList();

            ViewBag.SiteName = contextItem.Fields["Site Name"]?.Value ?? "BMC Blog";
            ViewBag.SiteDescription = contextItem.Fields["Site Description"]?.Value ?? "Welcome to BMC Blog";

            return View(postModels);
        }
    }
}