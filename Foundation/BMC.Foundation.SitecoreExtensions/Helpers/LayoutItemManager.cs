using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;
using System;

namespace BMC.Foundation.SitecoreExtensions.Helpers
{
    /// <summary>
    /// Helper class for managing Sitecore Layout items
    /// </summary>
    public class LayoutItemManager
    {
        // Template IDs
        private static readonly ID LayoutTemplateId = new ID("{3A45A723-64EE-4919-9D41-02FD40FD1466}"); // System/Layout/Layout
        private static readonly ID FolderTemplateId = new ID("{A87A00B1-E6DB-45AB-8B54-636FEC3B5523}"); // Common/Folder

        // Layout paths
        private const string LayoutsFolderPath = "/sitecore/layout/Layouts";
        private const string BmcLayoutFolderPath = "/sitecore/layout/Layouts/BMC";
        private const string BlogLayoutPath = "/sitecore/layout/Layouts/BMC/Blog Layout";

        /// <summary>
        /// Creates or updates the Blog Layout item in Sitecore
        /// </summary>
        /// <param name="database">The Sitecore database (master)</param>
        /// <param name="physicalPath">Physical path to the layout file (~/Views/Layouts/BlogLayout.cshtml)</param>
        /// <returns>The created or updated layout item</returns>
        public static Item CreateOrUpdateBlogLayout(Database database, string physicalPath = "~/Views/Layouts/BlogLayout.cshtml")
        {
            if (database == null)
                throw new ArgumentNullException(nameof(database));

            try
            {
                // Ensure BMC folder exists
                Item bmcFolder = EnsureBmcFolder(database);
                if (bmcFolder == null)
                {
                    Sitecore.Diagnostics.Log.Error("LayoutItemManager: Could not create or find BMC folder", typeof(LayoutItemManager));
                    return null;
                }

                // Check if layout already exists
                Item layoutItem = database.GetItem(BlogLayoutPath);

                using (new SecurityDisabler())
                {
                    if (layoutItem != null)
                    {
                        // Update existing layout
                        Sitecore.Diagnostics.Log.Info($"LayoutItemManager: Updating existing Blog Layout - ID: {layoutItem.ID}", typeof(LayoutItemManager));
                        
                        layoutItem.Editing.BeginEdit();
                        try
                        {
                            layoutItem["Path"] = physicalPath;
                            layoutItem.Editing.EndEdit();
                            
                            Sitecore.Diagnostics.Log.Info("LayoutItemManager: Blog Layout updated successfully", typeof(LayoutItemManager));
                        }
                        catch
                        {
                            layoutItem.Editing.CancelEdit();
                            throw;
                        }
                    }
                    else
                    {
                        // Create new layout
                        Sitecore.Diagnostics.Log.Info("LayoutItemManager: Creating new Blog Layout item", typeof(LayoutItemManager));
                        
                        layoutItem = bmcFolder.Add("Blog Layout", new TemplateID(LayoutTemplateId));
                        
                        if (layoutItem != null)
                        {
                            layoutItem.Editing.BeginEdit();
                            try
                            {
                                layoutItem["Path"] = physicalPath;
                                layoutItem.Editing.EndEdit();
                                
                                Sitecore.Diagnostics.Log.Info($"LayoutItemManager: Blog Layout created successfully - ID: {layoutItem.ID}", typeof(LayoutItemManager));
                            }
                            catch
                            {
                                layoutItem.Editing.CancelEdit();
                                throw;
                            }
                        }
                    }
                }

                return layoutItem;
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error($"LayoutItemManager: Error creating/updating Blog Layout: {ex.Message}", ex, typeof(LayoutItemManager));
                return null;
            }
        }

        /// <summary>
        /// Ensures the BMC folder exists under Layouts
        /// </summary>
        private static Item EnsureBmcFolder(Database database)
        {
            if (database == null)
                return null;

            // Check if BMC folder exists
            Item bmcFolder = database.GetItem(BmcLayoutFolderPath);
            if (bmcFolder != null)
                return bmcFolder;

            // Get Layouts folder
            Item layoutsFolder = database.GetItem(LayoutsFolderPath);
            if (layoutsFolder == null)
            {
                Sitecore.Diagnostics.Log.Error("LayoutItemManager: Layouts folder not found", typeof(LayoutItemManager));
                return null;
            }

            // Create BMC folder
            using (new SecurityDisabler())
            {
                Sitecore.Diagnostics.Log.Info("LayoutItemManager: Creating BMC folder", typeof(LayoutItemManager));
                bmcFolder = layoutsFolder.Add("BMC", new TemplateID(FolderTemplateId));
                
                if (bmcFolder != null)
                {
                    Sitecore.Diagnostics.Log.Info($"LayoutItemManager: BMC folder created - ID: {bmcFolder.ID}", typeof(LayoutItemManager));
                }
            }

            return bmcFolder;
        }

        /// <summary>
        /// Assigns a layout to a template's standard values
        /// </summary>
        /// <param name="templateItem">The template item</param>
        /// <param name="layoutItem">The layout item to assign</param>
        /// <returns>True if successful</returns>
        public static bool AssignLayoutToTemplate(Item templateItem, Item layoutItem)
        {
            if (templateItem == null || layoutItem == null)
                return false;

            try
            {
                // Get or create standard values
                Item standardValues = GetOrCreateStandardValues(templateItem);
                if (standardValues == null)
                {
                    Sitecore.Diagnostics.Log.Error($"LayoutItemManager: Could not get/create standard values for template: {templateItem.Name}", typeof(LayoutItemManager));
                    return false;
                }

                // Set layout using ItemExtensions
                using (new SecurityDisabler())
                {
                    bool success = BMC.Foundation.SitecoreExtensions.Extensions.ItemExtensions.SetLayout(standardValues, layoutItem);
                    
                    if (success)
                    {
                        Sitecore.Diagnostics.Log.Info($"LayoutItemManager: Layout assigned to template '{templateItem.Name}' standard values", typeof(LayoutItemManager));
                    }
                    else
                    {
                        Sitecore.Diagnostics.Log.Error($"LayoutItemManager: Failed to assign layout to template '{templateItem.Name}'", typeof(LayoutItemManager));
                    }
                    
                    return success;
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error($"LayoutItemManager: Error assigning layout to template: {ex.Message}", ex, typeof(LayoutItemManager));
                return false;
            }
        }

        /// <summary>
        /// Gets or creates standard values for a template
        /// </summary>
        private static Item GetOrCreateStandardValues(Item templateItem)
        {
            if (templateItem == null)
                return null;

            // Check if standard values already exist
            Item standardValues = templateItem.Children["__Standard Values"];
            if (standardValues != null)
                return standardValues;

            // Create standard values
            using (new SecurityDisabler())
            {
                Sitecore.Diagnostics.Log.Info($"LayoutItemManager: Creating standard values for template: {templateItem.Name}", typeof(LayoutItemManager));
                
                TemplateItem template = new TemplateItem(templateItem);
                standardValues = template.CreateStandardValues();
                
                if (standardValues != null)
                {
                    Sitecore.Diagnostics.Log.Info($"LayoutItemManager: Standard values created - ID: {standardValues.ID}", typeof(LayoutItemManager));
                }
            }

            return standardValues;
        }

        /// <summary>
        /// Checks if a layout item exists
        /// </summary>
        /// <param name="database">The Sitecore database</param>
        /// <param name="layoutPath">Path to the layout item (default: Blog Layout)</param>
        /// <returns>True if layout exists</returns>
        public static bool LayoutExists(Database database, string layoutPath = BlogLayoutPath)
        {
            if (database == null || string.IsNullOrEmpty(layoutPath))
                return false;

            Item layoutItem = database.GetItem(layoutPath);
            return layoutItem != null;
        }

        /// <summary>
        /// Gets the Blog Layout item
        /// </summary>
        /// <param name="database">The Sitecore database</param>
        /// <returns>The Blog Layout item or null</returns>
        public static Item GetBlogLayout(Database database)
        {
            if (database == null)
                return null;

            return database.GetItem(BlogLayoutPath);
        }

        /// <summary>
        /// Validates that a layout item has the correct path field set
        /// </summary>
        /// <param name="layoutItem">The layout item to validate</param>
        /// <param name="expectedPath">Expected physical path</param>
        /// <returns>True if valid</returns>
        public static bool ValidateLayoutPath(Item layoutItem, string expectedPath = "~/Views/Layouts/BlogLayout.cshtml")
        {
            if (layoutItem == null)
                return false;

            string actualPath = layoutItem["Path"];
            bool isValid = string.Equals(actualPath, expectedPath, StringComparison.OrdinalIgnoreCase);

            if (!isValid)
            {
                Sitecore.Diagnostics.Log.Warn($"LayoutItemManager: Layout path mismatch. Expected: '{expectedPath}', Actual: '{actualPath}'", typeof(LayoutItemManager));
            }

            return isValid;
        }
    }
}
