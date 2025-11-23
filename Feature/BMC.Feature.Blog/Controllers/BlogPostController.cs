using System.Linq;
using System.Web.Mvc;
using BMC.Feature.Blog.Models;
using BMC.Feature.Blog.Repositories;

namespace BMC.Feature.Blog.Controllers
{
    public class BlogPostController : Controller
    {
        private readonly BlogRepository _repository;

        public BlogPostController()
        {
            _repository = new BlogRepository();
        }

        public ActionResult Detail()
        {
            var contextItem = Sitecore.Context.Item;
            if (contextItem == null)
                return Content("Blog post not found");

            IncrementViewCount(contextItem);

            var postModel = new BlogPostModel(contextItem);

            var relatedPosts = LoadRelatedPosts(postModel);
            ViewBag.RelatedPosts = relatedPosts;

            return View(postModel);
        }

        private void IncrementViewCount(Sitecore.Data.Items.Item item)
        {
            if (item == null)
                return;

            try
            {
                using (new Sitecore.SecurityModel.SecurityDisabler())
                {
                    item.Editing.BeginEdit();
                    
                    var viewCountField = item.Fields["View Count"];
                    if (viewCountField != null)
                    {
                        int currentCount = 0;
                        if (!string.IsNullOrEmpty(viewCountField.Value))
                        {
                            int.TryParse(viewCountField.Value, out currentCount);
                        }

                        item.Fields["View Count"].Value = (currentCount + 1).ToString();
                    }

                    item.Editing.EndEdit();
                }
            }
            catch
            {
                // Log error if needed
            }
        }

        private System.Collections.Generic.List<BlogPostModel> LoadRelatedPosts(BlogPostModel currentPost)
        {
            if (currentPost?.Category == null)
                return new System.Collections.Generic.List<BlogPostModel>();

            var relatedItems = _repository.GetPostsByCategory(currentPost.Category.ID);
            
            var relatedPosts = relatedItems
                .Where(i => i.ID != Sitecore.Context.Item.ID)
                .Take(3)
                .Select(i => new BlogPostModel(i))
                .ToList();

            return relatedPosts;
        }
    }
}