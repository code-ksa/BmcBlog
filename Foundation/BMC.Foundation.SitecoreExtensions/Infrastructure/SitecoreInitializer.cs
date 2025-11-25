using Sitecore.Data;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;

namespace BMC.Foundation.SitecoreExtensions.Infrastructure
{
    /// <summary>
    /// Sitecore initializer for BMC Blog setup
    /// Orchestrates the creation of layouts, renderings, templates, and presentation setup
    /// </summary>
    public class SitecoreInitializer
    {
        private static bool _isInitialized = false;
        private static readonly object _lock = new object();

        /// <summary>
        /// Main initialization method - sets up everything for BMC Blog
        /// </summary>
        public static InitializationResult Initialize(Database database = null)
        {
            lock (_lock)
            {
                var result = new InitializationResult();

                try
                {
                    Sitecore.Diagnostics.Log.Info("========================================", typeof(SitecoreInitializer));
                    Sitecore.Diagnostics.Log.Info("BMC Blog - Sitecore Initialization Started", typeof(SitecoreInitializer));
                    Sitecore.Diagnostics.Log.Info("========================================", typeof(SitecoreInitializer));

                    // Get master database
                    if (database == null)
                    {
                        database = Sitecore.Configuration.Factory.GetDatabase("master");
                    }

                    if (database == null)
                    {
                        result.Success = false;
                        result.ErrorMessage = "Master database not found";
                        Sitecore.Diagnostics.Log.Error("SitecoreInitializer: Master database not found", typeof(SitecoreInitializer));
                        return result;
                    }

                    // Step 1: Create Layout
                    Sitecore.Diagnostics.Log.Info("Step 1: Creating Layout items", typeof(SitecoreInitializer));
                    if (!CreateLayoutItems(database, result))
                    {
                        return result; // Stop if layout creation fails
                    }

                    // Step 2: Create Renderings
                    Sitecore.Diagnostics.Log.Info("Step 2: Creating Rendering items", typeof(SitecoreInitializer));
                    if (!CreateRenderingItems(database, result))
                    {
                        return result; // Stop if rendering creation fails
                    }

                    // Step 3: Create Templates
                    Sitecore.Diagnostics.Log.Info("Step 3: Creating Template items", typeof(SitecoreInitializer));
                    if (!CreateTemplateItems(database, result))
                    {
                        Sitecore.Diagnostics.Log.Warn("SitecoreInitializer: Template creation had issues, but continuing", typeof(SitecoreInitializer));
                        // Continue even if templates fail - they're optional
                    }

                    // Step 4: Assign Layout to Templates
                    Sitecore.Diagnostics.Log.Info("Step 4: Assigning Layout to Templates", typeof(SitecoreInitializer));
                    AssignLayoutToTemplates(database, result);

                    // Step 5: Apply Layout to Existing Pages
                    Sitecore.Diagnostics.Log.Info("Step 5: Applying Layout to existing pages", typeof(SitecoreInitializer));
                    ApplyLayoutToPages(database, result);

                    // Mark as initialized
                    _isInitialized = true;
                    result.Success = true;

                    Sitecore.Diagnostics.Log.Info("========================================", typeof(SitecoreInitializer));
                    Sitecore.Diagnostics.Log.Info("BMC Blog - Initialization Completed Successfully", typeof(SitecoreInitializer));
                    Sitecore.Diagnostics.Log.Info($"Summary: {result.GetSummary()}", typeof(SitecoreInitializer));
                    Sitecore.Diagnostics.Log.Info("========================================", typeof(SitecoreInitializer));

                    return result;
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.ErrorMessage = ex.Message;
                    Sitecore.Diagnostics.Log.Error("SitecoreInitializer: Initialization failed", ex, typeof(SitecoreInitializer));
                    return result;
                }
            }
        }

        /// <summary>
        /// Step 1: Create Layout items
        /// </summary>
        private static bool CreateLayoutItems(Database database, InitializationResult result)
        {
            try
            {
                var layout = Helpers.LayoutItemManager.CreateOrUpdateBlogLayout(database);
                
                if (layout == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "Failed to create Blog Layout";
                    Sitecore.Diagnostics.Log.Error("SitecoreInitializer: Failed to create Blog Layout", typeof(SitecoreInitializer));
                    return false;
                }

                result.LayoutCreated = true;
                result.LayoutId = layout.ID.ToString();
                Sitecore.Diagnostics.Log.Info($"? Blog Layout created: {layout.ID}", typeof(SitecoreInitializer));
                
                return true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"Error creating layout: {ex.Message}";
                Sitecore.Diagnostics.Log.Error("SitecoreInitializer: Error in CreateLayoutItems", ex, typeof(SitecoreInitializer));
                return false;
            }
        }

        /// <summary>
        /// Step 2: Create Rendering items
        /// </summary>
        private static bool CreateRenderingItems(Database database, InitializationResult result)
        {
            try
            {
                var renderings = Helpers.RenderingItemManager.CreateAllDefaultRenderings(database);
                
                if (renderings == null || renderings.Count == 0)
                {
                    result.Success = false;
                    result.ErrorMessage = "Failed to create Renderings";
                    Sitecore.Diagnostics.Log.Error("SitecoreInitializer: Failed to create Renderings", typeof(SitecoreInitializer));
                    return false;
                }

                result.RenderingsCreated = renderings.Count;
                Sitecore.Diagnostics.Log.Info($"? {renderings.Count} Renderings created", typeof(SitecoreInitializer));
                
                return true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"Error creating renderings: {ex.Message}";
                Sitecore.Diagnostics.Log.Error("SitecoreInitializer: Error in CreateRenderingItems", ex, typeof(SitecoreInitializer));
                return false;
            }
        }

        /// <summary>
        /// Step 3: Create Template items
        /// </summary>
        private static bool CreateTemplateItems(Database database, InitializationResult result)
        {
            try
            {
                // Create Blog Post template
                var blogPostTemplate = Helpers.TemplateManager.CreateBlogPostTemplate(database);
                if (blogPostTemplate != null)
                {
                    result.TemplatesCreated++;
                    Sitecore.Diagnostics.Log.Info($"? Blog Post template created: {blogPostTemplate.ID}", typeof(SitecoreInitializer));
                }

                // Create Blog Home template
                var blogHomeTemplate = Helpers.TemplateManager.CreateBlogHomeTemplate(database);
                if (blogHomeTemplate != null)
                {
                    result.TemplatesCreated++;
                    Sitecore.Diagnostics.Log.Info($"? Blog Home template created: {blogHomeTemplate.ID}", typeof(SitecoreInitializer));
                }

                return result.TemplatesCreated > 0;
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("SitecoreInitializer: Error in CreateTemplateItems", ex, typeof(SitecoreInitializer));
                return false;
            }
        }

        /// <summary>
        /// Step 4: Assign Layout to Templates
        /// </summary>
        private static void AssignLayoutToTemplates(Database database, InitializationResult result)
        {
            try
            {
                var layout = Helpers.LayoutItemManager.GetBlogLayout(database);
                if (layout == null)
                {
                    Sitecore.Diagnostics.Log.Warn("SitecoreInitializer: Blog Layout not found, skipping template assignment", typeof(SitecoreInitializer));
                    return;
                }

                // Get templates
                var blogPostTemplate = database.GetItem("/sitecore/templates/BMC/Blog Post");
                var blogHomeTemplate = database.GetItem("/sitecore/templates/BMC/Blog Home");

                // Assign to Blog Post template
                if (blogPostTemplate != null)
                {
                    if (Helpers.LayoutItemManager.AssignLayoutToTemplate(blogPostTemplate, layout))
                    {
                        result.TemplatesWithLayout++;
                        Sitecore.Diagnostics.Log.Info($"? Layout assigned to Blog Post template", typeof(SitecoreInitializer));
                    }
                }

                // Assign to Blog Home template
                if (blogHomeTemplate != null)
                {
                    if (Helpers.LayoutItemManager.AssignLayoutToTemplate(blogHomeTemplate, layout))
                    {
                        result.TemplatesWithLayout++;
                        Sitecore.Diagnostics.Log.Info($"? Layout assigned to Blog Home template", typeof(SitecoreInitializer));
                    }
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("SitecoreInitializer: Error in AssignLayoutToTemplates", ex, typeof(SitecoreInitializer));
            }
        }

        /// <summary>
        /// Step 5: Apply Layout to Existing Pages
        /// </summary>
        private static void ApplyLayoutToPages(Database database, InitializationResult result)
        {
            try
            {
                var layout = Helpers.LayoutItemManager.GetBlogLayout(database);
                if (layout == null)
                {
                    Sitecore.Diagnostics.Log.Warn("SitecoreInitializer: Blog Layout not found, skipping page setup", typeof(SitecoreInitializer));
                    return;
                }

                // Get Home page
                var homePage = database.GetItem("/sitecore/content/BMC/BmcBlog/Home");
                if (homePage != null)
                {
                    // Apply standard presentation (layout + renderings)
                    if (Helpers.PresentationManager.ApplyStandardPresentation(homePage, layout, database))
                    {
                        result.PagesWithLayout++;
                        Sitecore.Diagnostics.Log.Info($"? Presentation applied to Home page", typeof(SitecoreInitializer));
                    }
                }

                // Get Blog page
                var blogPage = database.GetItem("/sitecore/content/BMC/BmcBlog/Home/Blog");
                if (blogPage != null)
                {
                    if (Helpers.PresentationManager.ApplyStandardPresentation(blogPage, layout, database))
                    {
                        result.PagesWithLayout++;
                        Sitecore.Diagnostics.Log.Info($"? Presentation applied to Blog page", typeof(SitecoreInitializer));
                    }
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("SitecoreInitializer: Error in ApplyLayoutToPages", ex, typeof(SitecoreInitializer));
            }
        }

        /// <summary>
        /// Checks if initialization has already been run
        /// </summary>
        public static bool IsInitialized()
        {
            return _isInitialized;
        }

        /// <summary>
        /// Resets the initialization flag (for testing purposes)
        /// </summary>
        public static void ResetInitializationFlag()
        {
            lock (_lock)
            {
                _isInitialized = false;
                Sitecore.Diagnostics.Log.Info("SitecoreInitializer: Initialization flag reset", typeof(SitecoreInitializer));
            }
        }
    }

    /// <summary>
    /// Result class for initialization
    /// </summary>
    public class InitializationResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        
        public bool LayoutCreated { get; set; }
        public string LayoutId { get; set; }
        
        public int RenderingsCreated { get; set; }
        public int TemplatesCreated { get; set; }
        public int TemplatesWithLayout { get; set; }
        public int PagesWithLayout { get; set; }

        public InitializationResult()
        {
            Success = false;
        }

        public string GetSummary()
        {
            if (!Success)
            {
                return $"Failed: {ErrorMessage}";
            }

            return $"Layout: {(LayoutCreated ? "?" : "?")}, " +
                   $"Renderings: {RenderingsCreated}, " +
                   $"Templates: {TemplatesCreated}, " +
                   $"Templates with Layout: {TemplatesWithLayout}, " +
                   $"Pages with Layout: {PagesWithLayout}";
        }
    }
}
