using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Layouts;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BMC.Foundation.SitecoreExtensions.Helpers
{
    /// <summary>
    /// Helper class for managing Sitecore Presentation Details
    /// </summary>
    public class PresentationManager
    {
        private static readonly ID DefaultDeviceId = new ID("{FE5D7FDF-89C0-4D99-9AA3-B5FBD009C9F3}");
        private static readonly ID LayoutFieldId = new ID("{F1A1FE9E-A60C-4DDB-A3A0-BB5B29FE732E}");

        /// <summary>
        /// Assigns a layout to a Sitecore item
        /// </summary>
        public static bool AssignLayoutToItem(Item targetItem, Item layoutItem)
        {
            if (targetItem == null || layoutItem == null)
                return false;

            try
            {
                using (new SecurityDisabler())
                {
                    bool success = BMC.Foundation.SitecoreExtensions.Extensions.ItemExtensions.SetLayout(targetItem, layoutItem);

                    if (success)
                    {
                        Sitecore.Diagnostics.Log.Info($"PresentationManager: Layout '{layoutItem.Name}' assigned to item '{targetItem.Name}'", typeof(PresentationManager));
                    }
                    else
                    {
                        Sitecore.Diagnostics.Log.Error($"PresentationManager: Failed to assign layout to item '{targetItem.Name}'", typeof(PresentationManager));
                    }

                    return success;
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error($"PresentationManager: Error assigning layout: {ex.Message}", ex, typeof(PresentationManager));
                return false;
            }
        }

        /// <summary>
        /// Adds a rendering to a specific placeholder on an item
        /// </summary>
        public static bool AddRenderingToItem(Item targetItem, Item renderingItem, string placeholderKey)
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
                        Sitecore.Diagnostics.Log.Info($"PresentationManager: Rendering '{renderingItem.Name}' added to '{targetItem.Name}' in placeholder '{placeholderKey}'", typeof(PresentationManager));
                    }
                    else
                    {
                        Sitecore.Diagnostics.Log.Error($"PresentationManager: Failed to add rendering to item '{targetItem.Name}'", typeof(PresentationManager));
                    }

                    return success;
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error($"PresentationManager: Error adding rendering: {ex.Message}", ex, typeof(PresentationManager));
                return false;
            }
        }

        /// <summary>
        /// Adds multiple renderings to an item at once
        /// </summary>
        public static int AddMultipleRenderings(Item targetItem, Dictionary<Item, string> renderingsWithPlaceholders)
        {
            if (targetItem == null || renderingsWithPlaceholders == null || renderingsWithPlaceholders.Count == 0)
                return 0;

            int successCount = 0;

            Sitecore.Diagnostics.Log.Info($"PresentationManager: Adding {renderingsWithPlaceholders.Count} renderings to '{targetItem.Name}'", typeof(PresentationManager));

            using (new SecurityDisabler())
            {
                foreach (var kvp in renderingsWithPlaceholders)
                {
                    if (AddRenderingToItem(targetItem, kvp.Key, kvp.Value))
                    {
                        successCount++;
                    }
                }
            }

            Sitecore.Diagnostics.Log.Info($"PresentationManager: Successfully added {successCount} of {renderingsWithPlaceholders.Count} renderings", typeof(PresentationManager));

            return successCount;
        }

        /// <summary>
        /// Adds standard renderings (Header, Breadcrumb, Footer) to an item
        /// </summary>
        public static bool AddStandardRenderings(Item targetItem, Database database)
        {
            if (targetItem == null || database == null)
                return false;

            try
            {
                var renderingsToAdd = new Dictionary<Item, string>();

                // Get standard renderings
                var header = database.GetItem("/sitecore/layout/Renderings/BMC/Header");
                var breadcrumb = database.GetItem("/sitecore/layout/Renderings/BMC/Breadcrumb");
                var footer = database.GetItem("/sitecore/layout/Renderings/BMC/Footer");

                if (header != null)
                    renderingsToAdd.Add(header, "header");

                if (breadcrumb != null)
                    renderingsToAdd.Add(breadcrumb, "breadcrumb");

                if (footer != null)
                    renderingsToAdd.Add(footer, "footer");

                if (renderingsToAdd.Count == 0)
                {
                    Sitecore.Diagnostics.Log.Warn("PresentationManager: No standard renderings found", typeof(PresentationManager));
                    return false;
                }

                int added = AddMultipleRenderings(targetItem, renderingsToAdd);
                return added > 0;
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error($"PresentationManager: Error adding standard renderings: {ex.Message}", ex, typeof(PresentationManager));
                return false;
            }
        }

        /// <summary>
        /// Clears all presentation details from an item
        /// </summary>
        public static bool ClearPresentationDetails(Item targetItem)
        {
            if (targetItem == null)
                return false;

            try
            {
                using (new SecurityDisabler())
                {
                    targetItem.Editing.BeginEdit();
                    try
                    {
                        targetItem.Fields[LayoutFieldId].Reset();
                        targetItem.Editing.EndEdit();

                        Sitecore.Diagnostics.Log.Info($"PresentationManager: Presentation details cleared for item '{targetItem.Name}'", typeof(PresentationManager));
                        return true;
                    }
                    catch
                    {
                        targetItem.Editing.CancelEdit();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error($"PresentationManager: Error clearing presentation: {ex.Message}", ex, typeof(PresentationManager));
                return false;
            }
        }

        /// <summary>
        /// Gets the current presentation details for an item
        /// </summary>
        public static PresentationDetails GetPresentationDetails(Item targetItem)
        {
            if (targetItem == null)
                return null;

            try
            {
                var details = new PresentationDetails();

                // Check if item has layout
                details.HasLayout = BMC.Foundation.SitecoreExtensions.Extensions.ItemExtensions.HasLayout(targetItem);

                if (details.HasLayout)
                {
                    // Get layout
                    details.Layout = BMC.Foundation.SitecoreExtensions.Extensions.ItemExtensions.GetLayout(targetItem);

                    // Get renderings
                    var renderings = BMC.Foundation.SitecoreExtensions.Extensions.ItemExtensions.GetRenderings(targetItem);
                    details.RenderingsCount = renderings.Count;
                    details.Renderings = renderings;
                }

                return details;
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error($"PresentationManager: Error getting presentation details: {ex.Message}", ex, typeof(PresentationManager));
                return null;
            }
        }

        /// <summary>
        /// Copies presentation details from one item to another
        /// </summary>
        public static bool CopyPresentation(Item sourceItem, Item targetItem)
        {
            if (sourceItem == null || targetItem == null)
                return false;

            try
            {
                var renderingsField = sourceItem.Fields[LayoutFieldId];
                if (renderingsField == null || string.IsNullOrEmpty(renderingsField.Value))
                {
                    Sitecore.Diagnostics.Log.Warn($"PresentationManager: Source item '{sourceItem.Name}' has no presentation details", typeof(PresentationManager));
                    return false;
                }

                using (new SecurityDisabler())
                {
                    targetItem.Editing.BeginEdit();
                    try
                    {
                        targetItem.Fields[LayoutFieldId].Value = renderingsField.Value;
                        targetItem.Editing.EndEdit();

                        Sitecore.Diagnostics.Log.Info($"PresentationManager: Presentation copied from '{sourceItem.Name}' to '{targetItem.Name}'", typeof(PresentationManager));
                        return true;
                    }
                    catch
                    {
                        targetItem.Editing.CancelEdit();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error($"PresentationManager: Error copying presentation: {ex.Message}", ex, typeof(PresentationManager));
                return false;
            }
        }

        /// <summary>
        /// Applies layout and standard renderings to an item
        /// </summary>
        public static bool ApplyStandardPresentation(Item targetItem, Item layoutItem, Database database)
        {
            if (targetItem == null || layoutItem == null || database == null)
                return false;

            try
            {
                Sitecore.Diagnostics.Log.Info($"PresentationManager: Applying standard presentation to '{targetItem.Name}'", typeof(PresentationManager));

                // Assign layout
                if (!AssignLayoutToItem(targetItem, layoutItem))
                {
                    Sitecore.Diagnostics.Log.Error($"PresentationManager: Failed to assign layout to '{targetItem.Name}'", typeof(PresentationManager));
                    return false;
                }

                // Add standard renderings
                if (!AddStandardRenderings(targetItem, database))
                {
                    Sitecore.Diagnostics.Log.Warn($"PresentationManager: Failed to add standard renderings to '{targetItem.Name}'", typeof(PresentationManager));
                    return false;
                }

                Sitecore.Diagnostics.Log.Info($"PresentationManager: Standard presentation applied successfully to '{targetItem.Name}'", typeof(PresentationManager));
                return true;
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error($"PresentationManager: Error applying standard presentation: {ex.Message}", ex, typeof(PresentationManager));
                return false;
            }
        }

        /// <summary>
        /// Applies presentation to multiple items
        /// </summary>
        public static int ApplyPresentationToMultipleItems(List<Item> items, Item layoutItem, Database database)
        {
            if (items == null || items.Count == 0 || layoutItem == null || database == null)
                return 0;

            int successCount = 0;

            Sitecore.Diagnostics.Log.Info($"PresentationManager: Applying presentation to {items.Count} items", typeof(PresentationManager));

            foreach (var item in items)
            {
                if (ApplyStandardPresentation(item, layoutItem, database))
                {
                    successCount++;
                }
            }

            Sitecore.Diagnostics.Log.Info($"PresentationManager: Presentation applied to {successCount} of {items.Count} items", typeof(PresentationManager));

            return successCount;
        }
    }

    /// <summary>
    /// Class to hold presentation details information
    /// </summary>
    public class PresentationDetails
    {
        public bool HasLayout { get; set; }
        public Item Layout { get; set; }
        public int RenderingsCount { get; set; }
        public List<RenderingDefinition> Renderings { get; set; }

        public PresentationDetails()
        {
            Renderings = new List<RenderingDefinition>();
        }
    }
}
