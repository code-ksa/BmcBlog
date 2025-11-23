using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace BMC.Feature.Blog.Repositories
{
    public class BlogRepository
    {
        private readonly Database _database;

        public BlogRepository()
        {
            _database = Sitecore.Context.Database ?? Database.GetDatabase("master");
        }

        public List<Item> GetAllPosts()
        {
            var blogRoot = GetBlogRoot();
            if (blogRoot == null)
                return new List<Item>();

            var posts = blogRoot.Axes.GetDescendants()
                .Where(i => i.TemplateName == "Blog Post")
                .OrderByDescending(i => i.Fields["Publish Date"]?.Value)
                .ToList();

            return posts;
        }

        public List<Item> GetPostsByCategory(ID categoryId)
        {
            if (categoryId == (ID)null)
                return new List<Item>();

            var allPosts = GetAllPosts();
            var categoryPosts = allPosts
                .Where(p => p.Fields["Category"]?.Value == categoryId.ToString())
                .ToList();

            return categoryPosts;
        }

        public Item GetPost(ID postId)
        {
            if (postId == (ID)null)
                return null;

            return _database.GetItem(postId);
        }

        public List<Item> GetRecentPosts(int count)
        {
            var allPosts = GetAllPosts();
            return allPosts.Take(count).ToList();
        }

        private Item GetBlogRoot()
        {
            var contextItem = Sitecore.Context.Item;
            if (contextItem == null)
                return null;

            while (contextItem != null && contextItem.TemplateName != "Blog Root")
            {
                contextItem = contextItem.Parent;
            }

            return contextItem;
        }
    }
}