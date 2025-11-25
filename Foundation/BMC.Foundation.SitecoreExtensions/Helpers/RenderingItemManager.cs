using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;

namespace BMC.Foundation.SitecoreExtensions.Helpers
{
    /// <summary>
    /// Helper class for managing Sitecore Rendering items
    /// </summary>
    public class RenderingItemManager
    {
        // Template IDs
        private static readonly ID ControllerRenderingTemplateId = new ID("{2A3E91A0-7987-44B5-AB34-35C2D9DE83B9}"); // Controller Rendering
        private static readonly ID ViewRenderingTemplateId = new ID("{99F8905D-4A87-4EB8-9F8B-A9BEBFB3ADD6}"); // View Rendering
        private static readonly ID FolderTemplateId = new ID("{A87A00B1-E6DB-45AB-8B54-636FEC3B5523}"); // Common/Folder

        // Rendering paths
        private const string RenderingsFolderPath = "/sitecore/layout/Renderings";
        private const string BmcRenderingsFolderPath = "/sitecore/layout/Renderings/BMC";

        /// <summary>
        /// Rendering configuration class
        /// </summary>
        public class RenderingConfig
        {
            public string Name { get; set; }
            public string Controller { get; set; }
            public string Action { get; set; }
            public string Placeholder { get; set; }

            public RenderingConfig(string name, string controller, string action, string placeholder = "")
            {
                Name = name;
                Controller = controller;
                Action = action;
                Placeholder = placeholder;
            }
        }

        /// <summary>
        /// Gets the default rendering configurations for BMC Blog
        /// </summary>
        public static List<RenderingConfig> GetDefaultRenderings()
        {
            return new List<RenderingConfig>
            {
                new RenderingConfig("Header", "BmcNavigation", "Header", "header"),
                new RenderingConfig("Footer", "BmcNavigation", "Footer", "footer"),
                new RenderingConfig("Breadcrumb", "BmcNavigation", "Breadcrumb", "breadcrumb"),
                new RenderingConfig("Newsletter Subscribe", "BmcNewsletter", "Subscribe", "newsletter")
            };
        }

        /// <summary>
        /// Creates or updates a Controller Rendering item
        /// </summary>
        public static Item CreateOrUpdateControllerRendering(Database database, string name, string controller, string action)
        {
            if (database == null || string.IsNullOrEmpty(name))
                return null;

            try
            {
                // Ensure BMC folder exists
                Item bmcFolder = EnsureBmcRenderingsFolder(database);
                if (bmcFolder == null)
                {
                    Sitecore.Diagnostics.Log.Error("RenderingItemManager: Could not create or find BMC renderings folder", typeof(RenderingItemManager));
                    return null;
                }

                // Check if rendering already exists
                Item renderingItem = bmcFolder.Children[name];

                using (new SecurityDisabler())
                {
                    if (renderingItem != null)
                    {
                        // Update existing rendering
                        Sitecore.Diagnostics.Log.Info($"RenderingItemManager: Updating existing rendering: {name}", typeof(RenderingItemManager));
                        
                        renderingItem.Editing.BeginEdit();
                        try
                        {
                            renderingItem["Controller"] = controller;
                            renderingItem["Controller Action"] = action;
                            renderingItem.Editing.EndEdit();
                            
                            Sitecore.Diagnostics.Log.Info($"RenderingItemManager: Rendering '{name}' updated successfully", typeof(RenderingItemManager));
                        }
                        catch
                        {
                            renderingItem.Editing.CancelEdit();
                            throw;
                        }
                    }
                    else
                    {
                        // Create new rendering
                        Sitecore.Diagnostics.Log.Info($"RenderingItemManager: Creating new rendering: {name}", typeof(RenderingItemManager));
                        
                        renderingItem = bmcFolder.Add(name, new TemplateID(ControllerRenderingTemplateId));
                        
                        if (renderingItem != null)
                        {
                            renderingItem.Editing.BeginEdit();
                            try
                            {
                                renderingItem["Controller"] = controller;
                                renderingItem["Controller Action"] = action;
                                renderingItem.Editing.EndEdit();
                                
                                Sitecore.Diagnostics.Log.Info($"RenderingItemManager: Rendering '{name}' created successfully - ID: {renderingItem.ID}", typeof(RenderingItemManager));
                            }
                            catch
                            {
                                renderingItem.Editing.CancelEdit();
                                throw;
                            }
                        }
                    }
                }

                return renderingItem;
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error($"RenderingItemManager: Error creating/updating rendering '{name}': {ex.Message}", ex, typeof(RenderingItemManager));
                return null;
            }
        }

        /// <summary>
        /// Creates or updates a View Rendering item
        /// </summary>
        public static Item CreateOrUpdateViewRendering(Database database, string name, string viewPath)
        {
            if (database == null || string.IsNullOrEmpty(name))
                return null;

            try
            {
                // Ensure BMC folder exists
                Item bmcFolder = EnsureBmcRenderingsFolder(database);
                if (bmcFolder == null)
                {
                    Sitecore.Diagnostics.Log.Error("RenderingItemManager: Could not create or find BMC renderings folder", typeof(RenderingItemManager));
                    return null;
                }

                // Check if rendering already exists
                Item renderingItem = bmcFolder.Children[name];

                using (new SecurityDisabler())
                {
                    if (renderingItem != null)
                    {
                        // Update existing rendering
                        Sitecore.Diagnostics.Log.Info($"RenderingItemManager: Updating existing view rendering: {name}", typeof(RenderingItemManager));
                        
                        renderingItem.Editing.BeginEdit();
                        try
                        {
                            renderingItem["Path"] = viewPath;
                            renderingItem.Editing.EndEdit();
                            
                            Sitecore.Diagnostics.Log.Info($"RenderingItemManager: View rendering '{name}' updated successfully", typeof(RenderingItemManager));
                        }
                        catch
                        {
                            renderingItem.Editing.CancelEdit();
                            throw;
                        }
                    }
                    else
                    {
                        // Create new view rendering
                        Sitecore.Diagnostics.Log.Info($"RenderingItemManager: Creating new view rendering: {name}", typeof(RenderingItemManager));
                        
                        renderingItem = bmcFolder.Add(name, new TemplateID(ViewRenderingTemplateId));
                        
                        if (renderingItem != null)
                        {
                            renderingItem.Editing.BeginEdit();
                            try
                            {
                                renderingItem["Path"] = viewPath;
                                renderingItem.Editing.EndEdit();
                                
                                Sitecore.Diagnostics.Log.Info($"RenderingItemManager: View rendering '{name}' created successfully - ID: {renderingItem.ID}", typeof(RenderingItemManager));
                            }
                            catch
                            {
                                renderingItem.Editing.CancelEdit();
                                throw;
                            }
                        }
                    }
                }

                return renderingItem;
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error($"RenderingItemManager: Error creating/updating view rendering '{name}': {ex.Message}", ex, typeof(RenderingItemManager));
                return null;
            }
        }

        /// <summary>
        /// Creates all default BMC renderings
        /// </summary>
        public static List<Item> CreateAllDefaultRenderings(Database database)
        {
            var createdRenderings = new List<Item>();
            
            if (database == null)
                return createdRenderings;

            Sitecore.Diagnostics.Log.Info("RenderingItemManager: Creating all default renderings", typeof(RenderingItemManager));

            foreach (var config in GetDefaultRenderings())
            {
                var rendering = CreateOrUpdateControllerRendering(database, config.Name, config.Controller, config.Action);
                if (rendering != null)
                {
                    createdRenderings.Add(rendering);
                }
            }

            Sitecore.Diagnostics.Log.Info($"RenderingItemManager: Created/updated {createdRenderings.Count} renderings", typeof(RenderingItemManager));

            return createdRenderings;
        }

        /// <summary>
        /// Assigns a rendering to a placeholder on an item
        /// </summary>
        public static bool AssignRenderingToItem(Item targetItem, Item renderingItem, string placeholderKey)
        {
            if (targetItem == null || renderingItem == null || string.IsNullOrEmpty(placeholderKey))
                return false;

            try
            {
                using (new SecurityDisabler())
                {
                    bool success = BMC.Foundation.SitecoreExtensions.Extensions.ItemExtensions.AddRendering(
                        targetItem, 
                        renderingItem, 
                        placeholderKey
                    );

                    if (success)
                    {
                        Sitecore.Diagnostics.Log.Info($"RenderingItemManager: Rendering '{renderingItem.Name}' assigned to '{targetItem.Name}' in placeholder '{placeholderKey}'", typeof(RenderingItemManager));
                    }
                    else
                    {
                        Sitecore.Diagnostics.Log.Error($"RenderingItemManager: Failed to assign rendering '{renderingItem.Name}' to '{targetItem.Name}'", typeof(RenderingItemManager));
                    }

                    return success;
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error($"RenderingItemManager: Error assigning rendering: {ex.Message}", ex, typeof(RenderingItemManager));
                return false;
            }
        }

        /// <summary>
        /// Ensures the BMC renderings folder exists
        /// </summary>
        private static Item EnsureBmcRenderingsFolder(Database database)
        {
            if (database == null)
                return null;

            // Check if BMC folder exists
            Item bmcFolder = database.GetItem(BmcRenderingsFolderPath);
            if (bmcFolder != null)
                return bmcFolder;

            // Get Renderings folder
            Item renderingsFolder = database.GetItem(RenderingsFolderPath);
            if (renderingsFolder == null)
            {
                Sitecore.Diagnostics.Log.Error("RenderingItemManager: Renderings folder not found", typeof(RenderingItemManager));
                return null;
            }

            // Create BMC folder
            using (new SecurityDisabler())
            {
                Sitecore.Diagnostics.Log.Info("RenderingItemManager: Creating BMC renderings folder", typeof(RenderingItemManager));
                bmcFolder = renderingsFolder.Add("BMC", new TemplateID(FolderTemplateId));
                
                if (bmcFolder != null)
                {
                    Sitecore.Diagnostics.Log.Info($"RenderingItemManager: BMC renderings folder created - ID: {bmcFolder.ID}", typeof(RenderingItemManager));
                }
            }

            return bmcFolder;
        }

        /// <summary>
        /// Gets a rendering item by name
        /// </summary>
        public static Item GetRendering(Database database, string renderingName)
        {
            if (database == null || string.IsNullOrEmpty(renderingName))
                return null;

            string renderingPath = $"{BmcRenderingsFolderPath}/{renderingName}";
            return database.GetItem(renderingPath);
        }

        /// <summary>
        /// Checks if a rendering exists
        /// </summary>
        public static bool RenderingExists(Database database, string renderingName)
        {
            return GetRendering(database, renderingName) != null;
        }

        /// <summary>
        /// Gets all BMC renderings
        /// </summary>
        public static List<Item> GetAllBmcRenderings(Database database)
        {
            var renderings = new List<Item>();
            
            if (database == null)
                return renderings;

            Item bmcFolder = database.GetItem(BmcRenderingsFolderPath);
            if (bmcFolder != null)
            {
                foreach (Item child in bmcFolder.Children)
                {
                    renderings.Add(child);
                }
            }

            return renderings;
        }
    }
}
