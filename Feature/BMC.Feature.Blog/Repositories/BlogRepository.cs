using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Fields; // ضروري للتعامل مع حقول Multilist و Reference

namespace BMC.Feature.Blog.Repositories
{
    public class BlogRepository
    {
        private readonly Database _database;

        public BlogRepository()
        {
            _database = Sitecore.Context.Database ?? Database.GetDatabase("master");
        }

        // =============================================================
        // 1. BASIC POST RETRIEVAL (Existing & Enhanced)
        // =============================================================

        public List<Item> GetAllPosts()
        {
            var blogRoot = GetBlogRoot();
            if (blogRoot == null)
                return new List<Item>();

            // تم التحديث لاستخدام Sitecore Query لأداء أفضل قليلاً
            // أو البقاء على GetDescendants كما في كودك (وهو جيد للمواقع الصغيرة)
            var posts = blogRoot.Axes.GetDescendants()
                .Where(i => i.TemplateName == "Blog Post")
                .OrderByDescending(i => i.Fields["Publish Date"]?.Value ?? i.Statistics.Created.ToString("yyyyMMddTHHmmss"))
                .ToList();

            return posts;
        }

        public List<Item> GetRecentPosts(int count)
        {
            return GetAllPosts().Take(count).ToList();
        }

        public Item GetPost(ID postId)
        {
            if (ID.IsNullOrEmpty(postId)) return null;
            return _database.GetItem(postId);
        }

        // =============================================================
        // 2. CATEGORIES & TAGS (New Features)
        // =============================================================

        public List<Item> GetAllCategories()
        {
            var blogRoot = GetBlogRoot();
            if (blogRoot == null) return new List<Item>();

            // البحث عن مجلد Categories أو العناصر التي نوعها Blog Category
            return blogRoot.Axes.GetDescendants()
                .Where(i => i.TemplateName == "Blog Category")
                .ToList();
        }

        public List<Item> GetAllTags()
        {
            var blogRoot = GetBlogRoot();
            if (blogRoot == null) return new List<Item>();

            return blogRoot.Axes.GetDescendants()
                .Where(i => i.TemplateName == "Tag")
                .ToList();
        }

        public List<Item> GetPostsByCategory(ID categoryId)
        {
            if (ID.IsNullOrEmpty(categoryId)) return new List<Item>();

            // تم تحسين المنطق للتعامل مع الحالات التي قد تكون فارغة
            return GetAllPosts()
                .Where(p => p.Fields["Category"] != null && p.Fields["Category"].Value.Contains(categoryId.ToString()))
                .ToList();
        }

        public List<Item> GetPostsByTag(ID tagId)
        {
            if (ID.IsNullOrEmpty(tagId)) return new List<Item>();

            return GetAllPosts().Where(post =>
            {
                MultilistField tagsField = post.Fields["Tags"];
                if (tagsField == null) return false;
                return tagsField.TargetIDs.Contains(tagId);
            }).ToList();
        }

        // =============================================================
        // 3. AUTHORS & ARCHIVE (New Features)
        // =============================================================

        public List<Item> GetPostsByAuthor(ID authorId, int maxCount = 100)
        {
            if (ID.IsNullOrEmpty(authorId)) return new List<Item>();

            return GetAllPosts()
                .Where(p => p.Fields["Author"] != null && p.Fields["Author"].Value == authorId.ToString())
                .Take(maxCount)
                .ToList();
        }

        public List<Item> GetPostsByArchive(int? year, int? month)
        {
            var posts = GetAllPosts();

            if (year.HasValue)
            {
                posts = posts.Where(p =>
                {
                    var date = Sitecore.DateUtil.IsoDateToDateTime(p["Publish Date"]);
                    return date != DateTime.MinValue && date.Year == year.Value;
                }).ToList();
            }

            if (month.HasValue)
            {
                posts = posts.Where(p =>
                {
                    var date = Sitecore.DateUtil.IsoDateToDateTime(p["Publish Date"]);
                    return date != DateTime.MinValue && date.Month == month.Value;
                }).ToList();
            }

            return posts;
        }

        // =============================================================
        // 4. WIDGET LOGIC (Popular, Trending, Related)
        // =============================================================

        public List<Item> GetPopularPosts(int count)
        {
            // في البيئة الحقيقية نستخدم Analytics
            // هنا سنحاكيها بجلب أقدم المقالات (أو التي لها View Count أعلى إذا كنا نسجله)
            return GetAllPosts()
                .OrderByDescending(p =>
                {
                    int.TryParse(p["View Count"], out int views);
                    return views;
                })
                .Take(count)
                .ToList();
        }

        public List<Item> GetTrendingPosts(int count)
        {
            // جلب المقالات المعلمة بـ IsTrending
            return GetAllPosts()
                .Where(p => p["IsTrending"] == "1")
                .Take(count)
                .ToList();
        }

        public List<Item> GetRelatedPosts(Item currentPost, int count)
        {
            if (currentPost == null) return new List<Item>();

            // الاستراتيجية: جلب مقالات من نفس التصنيف، باستثناء المقال الحالي
            var currentCategory = currentPost["Category"];

            var related = GetAllPosts()
                .Where(p => p.ID != currentPost.ID) // استثناء المقال الحالي
                .Where(p => !string.IsNullOrEmpty(currentCategory) && p["Category"] == currentCategory)
                .Take(count)
                .ToList();

            // إذا لم نجد ما يكفي، نملأ الباقي بآخر المقالات
            if (related.Count < count)
            {
                var remaining = count - related.Count;
                var recent = GetRecentPosts(count + 1) // +1 لأننا سنحذف المقال الحالي
                             .Where(p => p.ID != currentPost.ID && !related.Any(r => r.ID == p.ID))
                             .Take(remaining);
                related.AddRange(recent);
            }

            return related;
        }

        // =============================================================
        // 5. HELPER METHODS
        // =============================================================

        public Item GetBlogRoot()
        {
            var contextItem = Sitecore.Context.Item;
            if (contextItem == null) return null;

            // محاولة الصعود للأعلى للعثور على Blog Root
            var current = contextItem;
            while (current != null)
            {
                // التحقق باستخدام Template Name أو ID (الأسم أسهل للقراءة)
                if (current.TemplateName == "Blog Root" || current.Name.Equals("Home", StringComparison.OrdinalIgnoreCase))
                {
                    // تأكدنا في السكربتات أن مجلد المدونة قد يكون تحت Home/Blog
                    var blogFolder = current.Axes.GetChild("Blog");
                    if (blogFolder != null) return blogFolder; // إذا كنا في Home
                    if (current.TemplateName == "Blog Root") return current; // إذا كنا بالفعل في الروت
                }
                current = current.Parent;
            }

            // Fallback: Hardcoded path if traversal fails (Backup plan)
            return _database.GetItem("/sitecore/content/BMC/BmcBlog/Home/Blog");
        }
    }
}