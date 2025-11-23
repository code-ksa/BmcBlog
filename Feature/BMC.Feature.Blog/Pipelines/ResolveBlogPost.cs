using Sitecore.Diagnostics;
using Sitecore.Pipelines.HttpRequest;

namespace BMC.Feature.Blog.Pipelines
{
    /// <summary>
    /// Resolves blog post items in the HTTP request pipeline
    /// </summary>
    public class ResolveBlogPost : HttpRequestProcessor
    {
        public override void Process(HttpRequestArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));

            if (Sitecore.Context.Item == null || Sitecore.Context.Database == null)
                return;

            var contextItem = Sitecore.Context.Item;

            // Check if the current item is a blog post
            if (contextItem.TemplateName == "Blog Post")
            {
                Log.Debug($"Blog Post resolved: {contextItem.Paths.FullPath}", this);

                // Set custom context data for blog posts if needed
                Sitecore.Context.Items["IsBlogPost"] = true;
                Sitecore.Context.Items["BlogPostItem"] = contextItem;
            }
            // Check if we're in a blog section
            else if (IsBlogContext(contextItem))
            {
                Sitecore.Context.Items["IsBlogContext"] = true;
                Log.Debug($"Blog context detected: {contextItem.Paths.FullPath}", this);
            }
        }

        private bool IsBlogContext(Sitecore.Data.Items.Item item)
        {
            if (item == null)
                return false;

            // Check if current item or any ancestor is a Blog Root
            var currentItem = item;
            while (currentItem != null)
            {
                if (currentItem.TemplateName == "Blog Root")
                    return true;

                currentItem = currentItem.Parent;
            }

            return false;
        }
    }
}
