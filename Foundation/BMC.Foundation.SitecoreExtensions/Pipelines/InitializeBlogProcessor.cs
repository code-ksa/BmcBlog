using Sitecore.Pipelines.HttpRequest;
using System;
using System.Web;

namespace BMC.Foundation.SitecoreExtensions.Pipelines
{
    /// <summary>
    /// Pipeline processor to initialize BMC Blog setup
    /// Runs once when triggered by ?initBlog=true query string
    /// </summary>
    public class InitializeBlogProcessor : HttpRequestProcessor
    {
        private const string InitQueryStringKey = "initBlog";
        private const string InitFlagItemPath = "/sitecore/system/Settings/BMC/InitializationFlag";
        private const string InitFlagFieldName = "IsInitialized";

        public override void Process(HttpRequestArgs args)
        {
            // Check if initialization is requested
            if (!ShouldInitialize())
            {
                return;
            }

            try
            {
                var context = HttpContext.Current;
                if (context == null)
                    return;

                Sitecore.Diagnostics.Log.Info("InitializeBlogProcessor: Initialization requested via query string", this);

                // Check if already initialized
                if (IsAlreadyInitialized())
                {
                    Sitecore.Diagnostics.Log.Info("InitializeBlogProcessor: Already initialized, skipping", this);
                    context.Response.Write("<h1>BMC Blog Setup</h1><p>Already initialized. Remove the initialization flag to run again.</p>");
                    context.Response.End();
                    return;
                }

                // Run initialization
                var result = Infrastructure.SitecoreInitializer.Initialize();

                // Save initialization flag
                if (result.Success)
                {
                    SaveInitializationFlag();
                }

                // Show results
                ShowResults(result);
                context.Response.End();
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("InitializeBlogProcessor: Error during initialization", ex, this);
                
                var context = HttpContext.Current;
                if (context != null)
                {
                    context.Response.Write("<h1>BMC Blog Setup - Error</h1>");
                    context.Response.Write($"<p style='color: red;'>Error: {ex.Message}</p>");
                    context.Response.Write($"<pre>{ex.StackTrace}</pre>");
                    context.Response.End();
                }
            }
        }

        /// <summary>
        /// Checks if initialization should run
        /// </summary>
        private bool ShouldInitialize()
        {
            var context = HttpContext.Current;
            if (context?.Request == null)
                return false;

            var queryValue = context.Request.QueryString[InitQueryStringKey];
            return !string.IsNullOrEmpty(queryValue) && 
                   queryValue.Equals("true", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks if initialization has already been run
        /// </summary>
        private bool IsAlreadyInitialized()
        {
            try
            {
                var database = Sitecore.Configuration.Factory.GetDatabase("master");
                if (database == null)
                    return false;

                var flagItem = database.GetItem(InitFlagItemPath);
                if (flagItem == null)
                    return false;

                var isInitialized = flagItem[InitFlagFieldName];
                return isInitialized == "1" || isInitialized.Equals("true", StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("InitializeBlogProcessor: Error checking initialization flag", ex, this);
                return false;
            }
        }

        /// <summary>
        /// Saves the initialization flag
        /// </summary>
        private void SaveInitializationFlag()
        {
            try
            {
                var database = Sitecore.Configuration.Factory.GetDatabase("master");
                if (database == null)
                    return;

                // Create settings folder structure if it doesn't exist
                var settingsFolder = database.GetItem("/sitecore/system/Settings");
                if (settingsFolder == null)
                {
                    Sitecore.Diagnostics.Log.Warn("InitializeBlogProcessor: Settings folder not found", this);
                    return;
                }

                // Get or create BMC folder
                var bmcFolder = settingsFolder.Children["BMC"];
                if (bmcFolder == null)
                {
                    using (new Sitecore.SecurityModel.SecurityDisabler())
                    {
                        bmcFolder = settingsFolder.Add("BMC", new Sitecore.Data.TemplateID(new Sitecore.Data.ID("{A87A00B1-E6DB-45AB-8B54-636FEC3B5523}"))); // Folder template
                    }
                }

                if (bmcFolder == null)
                    return;

                // Get or create flag item
                var flagItem = bmcFolder.Children["InitializationFlag"];
                if (flagItem == null)
                {
                    using (new Sitecore.SecurityModel.SecurityDisabler())
                    {
                        // Create using a simple item template
                        flagItem = bmcFolder.Add("InitializationFlag", new Sitecore.Data.TemplateID(Sitecore.TemplateIDs.Folder));
                    }
                }

                if (flagItem != null)
                {
                    using (new Sitecore.SecurityModel.SecurityDisabler())
                    {
                        flagItem.Editing.BeginEdit();
                        try
                        {
                            flagItem["IsInitialized"] = "1";
                            flagItem["InitializationDate"] = Sitecore.DateUtil.IsoNow;
                            flagItem.Editing.EndEdit();
                            
                            Sitecore.Diagnostics.Log.Info("InitializeBlogProcessor: Initialization flag saved", this);
                        }
                        catch
                        {
                            flagItem.Editing.CancelEdit();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("InitializeBlogProcessor: Error saving initialization flag", ex, this);
            }
        }

        /// <summary>
        /// Shows the initialization results
        /// </summary>
        private void ShowResults(Infrastructure.InitializationResult result)
        {
            var context = HttpContext.Current;
            if (context == null)
                return;

            var response = context.Response;

            response.Write("<html><head><title>BMC Blog Setup</title>");
            response.Write("<style>");
            response.Write("body { font-family: Arial, sans-serif; margin: 40px; }");
            response.Write("h1 { color: #0066cc; }");
            response.Write(".success { color: green; font-weight: bold; }");
            response.Write(".error { color: red; font-weight: bold; }");
            response.Write(".info { background: #f0f0f0; padding: 15px; border-left: 4px solid #0066cc; margin: 20px 0; }");
            response.Write("table { border-collapse: collapse; width: 100%; margin: 20px 0; }");
            response.Write("td, th { border: 1px solid #ddd; padding: 8px; text-align: left; }");
            response.Write("th { background-color: #0066cc; color: white; }");
            response.Write("</style></head><body>");

            response.Write("<h1>BMC Blog - Sitecore Setup Results</h1>");

            if (result.Success)
            {
                response.Write("<p class='success'>✓ Initialization completed successfully!</p>");
                
                response.Write("<div class='info'>");
                response.Write("<h2>Summary</h2>");
                response.Write("<table>");
                response.Write("<tr><th>Item</th><th>Status</th></tr>");
                response.Write($"<tr><td>Layout Created</td><td>{(result.LayoutCreated ? "✓ Yes" : "✗ No")}</td></tr>");
                response.Write($"<tr><td>Layout ID</td><td>{result.LayoutId ?? "N/A"}</td></tr>");
                response.Write($"<tr><td>Renderings Created</td><td>{result.RenderingsCreated}</td></tr>");
                response.Write($"<tr><td>Templates Created</td><td>{result.TemplatesCreated}</td></tr>");
                response.Write($"<tr><td>Templates with Layout</td><td>{result.TemplatesWithLayout}</td></tr>");
                response.Write($"<tr><td>Pages with Layout</td><td>{result.PagesWithLayout}</td></tr>");
                response.Write("</table>");
                response.Write("</div>");

                response.Write("<h2>Next Steps</h2>");
                response.Write("<ol>");
                response.Write("<li>Open Content Editor and verify the items</li>");
                response.Write("<li>Check <code>/sitecore/layout/Layouts/BMC/</code></li>");
                response.Write("<li>Check <code>/sitecore/layout/Renderings/BMC/</code></li>");
                response.Write("<li>Check <code>/sitecore/templates/BMC/</code></li>");
                response.Write("<li>Publish all items to web database</li>");
                response.Write("<li>Test your site: <a href='/home'>http://abdo.sc/home</a></li>");
                response.Write("</ol>");
            }
            else
            {
                response.Write("<p class='error'>✗ Initialization failed</p>");
                response.Write($"<p><strong>Error:</strong> {result.ErrorMessage}</p>");
            }

            response.Write("<hr/>");
            response.Write("<p><small>To run initialization again, delete the flag item at: <code>/sitecore/system/Settings/BMC/InitializationFlag</code></small></p>");
            response.Write("</body></html>");
        }
    }
}
