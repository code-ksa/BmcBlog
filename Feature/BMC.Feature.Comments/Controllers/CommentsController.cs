using System.Linq;
using System.Web.Mvc;
using BMC.Feature.Comments.Models;
using BMC.Feature.Comments.Repositories;
using Sitecore.Data;

namespace BMC.Feature.Comments.Controllers
{
    public class CommentsController : Controller
    {
        private readonly CommentRepository _repository;

        public CommentsController()
        {
            _repository = new CommentRepository();
        }

        [HttpGet]
        public ActionResult List(string postId)
        {
            if (string.IsNullOrEmpty(postId))
            {
                var contextItem = Sitecore.Context.Item;
                if (contextItem != null)
                {
                    postId = contextItem.ID.ToString();
                }
            }

            var viewModel = new CommentListViewModel();

            if (!string.IsNullOrEmpty(postId) && ID.TryParse(postId, out ID blogPostId))
            {
                var comments = _repository.GetCommentsByPost(blogPostId);
                viewModel.Comments = comments.Select(item => MapItemToModel(item)).ToList();
                viewModel.TotalComments = comments.Count;
            }

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Add(string postId)
        {
            var model = new CommentModel();
            
            if (!string.IsNullOrEmpty(postId) && ID.TryParse(postId, out ID blogPostId))
            {
                model.BlogPostId = blogPostId;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(CommentModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!Sitecore.Context.User.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "You must be logged in to post comments.";
                return View(model);
            }

            var comment = _repository.AddComment(model);

            if (comment != null)
            {
                TempData["SuccessMessage"] = "Your comment has been submitted and is awaiting approval.";
                return RedirectToAction("List", new { postId = model.BlogPostId.ToString() });
            }

            TempData["ErrorMessage"] = "Unable to submit comment. Please try again.";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reply(CommentModel model, string parentCommentId)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("List", new { postId = model.BlogPostId.ToString() });
            }

            if (!Sitecore.Context.User.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "You must be logged in to reply to comments.";
                return RedirectToAction("List", new { postId = model.BlogPostId.ToString() });
            }

            if (!string.IsNullOrEmpty(parentCommentId) && ID.TryParse(parentCommentId, out ID parentId))
            {
                var database = Sitecore.Context.Database ?? Sitecore.Data.Database.GetDatabase("master");
                model.ParentComment = database.GetItem(parentId);
            }

            var comment = _repository.AddComment(model);

            if (comment != null)
            {
                TempData["SuccessMessage"] = "Your reply has been submitted and is awaiting approval.";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to submit reply. Please try again.";
            }

            return RedirectToAction("List", new { postId = model.BlogPostId.ToString() });
        }

        private CommentModel MapItemToModel(Sitecore.Data.Items.Item item)
        {
            if (item == null)
                return null;

            var model = new CommentModel
            {
                CommentId = item.ID,
                AuthorName = item.Fields["Author Name"]?.Value ?? string.Empty,
                AuthorEmail = item.Fields["Author Email"]?.Value ?? string.Empty,
                Content = item.Fields["Content"]?.Value ?? string.Empty
            };

            var postDateField = item.Fields["Post Date"];
            if (postDateField != null && !string.IsNullOrEmpty(postDateField.Value))
            {
                System.DateTime.TryParse(postDateField.Value, out System.DateTime parsedDate);
                model.PostDate = parsedDate;
            }

            var isApprovedField = item.Fields["Is Approved"];
            if (isApprovedField != null && !string.IsNullOrEmpty(isApprovedField.Value))
            {
                model.IsApproved = isApprovedField.Value == "1";
            }

            var parentCommentField = item.Fields["Parent Comment"];
            if (parentCommentField != null && !string.IsNullOrEmpty(parentCommentField.Value))
            {
                model.ParentComment = item.Database.GetItem(parentCommentField.Value);
            }

            return model;
        }
    }
}