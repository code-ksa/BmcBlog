using System.Linq;
using System.Web.Mvc;
using BMC.Feature.Blog.Models;
using BMC.Feature.Blog.Repositories;

namespace BMC.Feature.Blog.Controllers
{
    public class BmcBlogController : Controller
    {
        private readonly BlogRepository _repository;

        public BmcBlogController(BlogRepository repository)
        {
            _repository = repository;
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

        // Widget Actions
        public ActionResult PopularPosts(int count = 5)
        {
            var posts = _repository.GetPopularPosts(count);
            var postModels = posts.Select(p => new BlogPostModel(p)).ToList();

            return PartialView("_PopularPosts", postModels);
        }

        public ActionResult RecentPosts(int count = 5)
        {
            var posts = _repository.GetRecentPosts(count);
            var postModels = posts.Select(p => new BlogPostModel(p)).ToList();

            return PartialView("_RecentPosts", postModels);
        }

        public ActionResult TagCloud()
        {
            var tags = _repository.GetAllTags();
            var tagModels = tags.Select(t => new TagModel(t)).ToList();

            return PartialView("_TagCloud", tagModels);
        }

        public ActionResult CategoriesList()
        {
            var categories = _repository.GetAllCategories();
            var categoryModels = categories.Select(c => new CategoryModel(c)).ToList();

            return PartialView("_CategoriesList", categoryModels);
        }
    }
}