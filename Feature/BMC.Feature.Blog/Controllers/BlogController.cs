using System.Linq;
using System.Web.Mvc;
using BMC.Feature.Blog.Models;
using BMC.Feature.Blog.Repositories;

namespace BMC.Feature.Blog.Controllers
{
    public class BlogController : Controller
    {
        private readonly BlogRepository _repository;

        public BlogController()
        {
            _repository = new BlogRepository();
        }

        public ActionResult Index()
        {
            var contextItem = Sitecore.Context.Item;
            if (contextItem == null)
                return Content("No context item found");

            var posts = _repository.GetRecentPosts(10);
            var postModels = posts.Select(p => new BlogPostModel(p)).ToList();

            return View(postModels);
        }

        public ActionResult Category(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
                return RedirectToAction("Index");

            var contextItem = Sitecore.Context.Item;
            if (contextItem == null)
                return Content("No context item found");

            var categoryItem = contextItem.Axes.GetDescendants()
                .FirstOrDefault(i => i.Name.Equals(categoryName, System.StringComparison.OrdinalIgnoreCase));

            if (categoryItem == null)
                return RedirectToAction("Index");

            var posts = _repository.GetPostsByCategory(categoryItem.ID);
            var postModels = posts.Select(p => new BlogPostModel(p)).ToList();

            ViewBag.Category = new CategoryModel(categoryItem);

            return View("Index", postModels);
        }
    }
}