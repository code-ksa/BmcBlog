using System;
using System.Collections.Generic;
using System.Linq;
using BMC.Feature.Comments.Models;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace BMC.Feature.Comments.Repositories
{
    public class CommentRepository
    {
        private readonly Database _database;
        private static readonly ID CommentTemplateId = new ID("{11223344-5566-7788-99AA-BBCCDDEEFF00}");

        public CommentRepository()
        {
            _database = Sitecore.Context.Database ?? Database.GetDatabase("master");
        }

        public List<Item> GetCommentsByPost(ID postId)
        {
            if (postId == (ID)null)
                return new List<Item>();

            try
            {
                var blogPost = _database.GetItem(postId);
                if (blogPost == null)
                    return new List<Item>();

                var commentsFolder = blogPost.Children["Comments"];
                if (commentsFolder == null)
                    return new List<Item>();

                var comments = commentsFolder.Children
                    .Where(c => c.TemplateName == "Comment" || c.Fields["Is Approved"]?.Value == "1")
                    .OrderByDescending(c => c.Fields["Post Date"]?.Value)
                    .ToList();

                return comments;
            }
            catch
            {
                return new List<Item>();
            }
        }

        public Item AddComment(CommentModel model)
        {
            if (model == null || model.BlogPostId == (ID)null)
                return null;

            try
            {
                var blogPost = _database.GetItem(model.BlogPostId);
                if (blogPost == null)
                    return null;

                using (new Sitecore.SecurityModel.SecurityDisabler())
                {
                    var commentsFolder = blogPost.Children["Comments"];
                    if (commentsFolder == null)
                    {
                        commentsFolder = blogPost.Add("Comments", new TemplateID(Sitecore.TemplateIDs.Folder));
                    }

                    if (commentsFolder == null)
                        return null;

                    var itemName = ItemUtil.ProposeValidItemName("Comment_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
                    var template = _database.GetTemplate(CommentTemplateId) ?? _database.GetTemplate(Sitecore.TemplateIDs.Folder);

                    var comment = commentsFolder.Add(itemName, template);

                    if (comment != null)
                    {
                        comment.Editing.BeginEdit();

                        if (comment.Fields["Author Name"] != null)
                            comment.Fields["Author Name"].Value = model.AuthorName;

                        if (comment.Fields["Author Email"] != null)
                            comment.Fields["Author Email"].Value = model.AuthorEmail;

                        if (comment.Fields["Content"] != null)
                            comment.Fields["Content"].Value = model.Content;

                        if (comment.Fields["Post Date"] != null)
                            comment.Fields["Post Date"].Value = Sitecore.DateUtil.ToIsoDate(model.PostDate);

                        if (comment.Fields["Is Approved"] != null)
                            comment.Fields["Is Approved"].Value = model.IsApproved ? "1" : "0";

                        if (model.ParentComment != null && comment.Fields["Parent Comment"] != null)
                            comment.Fields["Parent Comment"].Value = model.ParentComment.ID.ToString();

                        comment.Editing.EndEdit();
                    }

                    return comment;
                }
            }
            catch
            {
                return null;
            }
        }

        public bool DeleteComment(ID commentId)
        {
            if (commentId == (ID)null)
                return false;

            try
            {
                var comment = _database.GetItem(commentId);
                if (comment == null)
                    return false;

                using (new Sitecore.SecurityModel.SecurityDisabler())
                {
                    comment.Delete();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ApproveComment(ID commentId)
        {
            if (commentId == (ID)null)
                return false;

            try
            {
                var comment = _database.GetItem(commentId);
                if (comment == null)
                    return false;

                using (new Sitecore.SecurityModel.SecurityDisabler())
                {
                    comment.Editing.BeginEdit();

                    if (comment.Fields["Is Approved"] != null)
                        comment.Fields["Is Approved"].Value = "1";

                    comment.Editing.EndEdit();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}