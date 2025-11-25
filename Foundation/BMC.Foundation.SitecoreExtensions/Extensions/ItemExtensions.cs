using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Fields;
using Sitecore.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BMC.Foundation.SitecoreExtensions.Extensions
{
    public static class ItemExtensions
    {
        // Default Device ID constant
        private static readonly ID DefaultDeviceId = new ID("{FE5D7FDF-89C0-4D99-9AA3-B5FBD009C9F3}");
        private static readonly ID LayoutFieldId = new ID("{F1A1FE9E-A60C-4DDB-A3A0-BB5B29FE732E}");

        public static string GetFieldValue(this Item item, string fieldName)
        {
            if (item == null || string.IsNullOrEmpty(fieldName))
                return string.Empty;

            var field = item.Fields[fieldName];
            return field != null ? field.Value : string.Empty;
        }

        public static bool HasField(this Item item, string fieldName)
        {
            if (item == null || string.IsNullOrEmpty(fieldName))
                return false;

            return item.Fields[fieldName] != null;
        }

        public static bool IsOfTemplate(this Item item, ID templateId)
        {
            if (item == null || templateId == (ID)null)
                return false;

            return item.TemplateID == templateId || item.IsDerived(templateId);
        }

        private static bool IsDerived(this Item item, ID templateId)
        {
            if (item == null)
                return false;

            var template = item.Database.GetItem(item.TemplateID);
            if (template == null)
                return false;

            return template.ID == templateId || template.IsDerivedFrom(templateId);
        }

        private static bool IsDerivedFrom(this Item template, ID templateId)
        {
            if (template == null)
                return false;

            var baseTemplatesField = template.Fields["__Base template"];
            if (baseTemplatesField == null || string.IsNullOrEmpty(baseTemplatesField.Value))
                return false;

            var baseTemplateIds = baseTemplatesField.Value.Split('|');
            foreach (var baseTemplateId in baseTemplateIds)
            {
                if (string.IsNullOrEmpty(baseTemplateId))
                    continue;

                if (ID.TryParse(baseTemplateId, out ID parsedId))
                {
                    if (parsedId == templateId)
                        return true;

                    var baseTemplate = template.Database.GetItem(parsedId);
                    if (baseTemplate != null && baseTemplate.IsDerivedFrom(templateId))
                        return true;
                }
            }

            return false;
        }

        #region Presentation Extension Methods

        /// <summary>
        /// Checks if the item has a layout assigned
        /// </summary>
        public static bool HasLayout(this Item item)
        {
            if (item == null)
                return false;

            try
            {
                var renderingsField = item.Fields[LayoutFieldId];
                if (renderingsField == null || string.IsNullOrEmpty(renderingsField.Value))
                    return false;

                var layoutField = new LayoutField(renderingsField);
                var layoutDefinition = LayoutDefinition.Parse(layoutField.Value);
                
                var defaultDevice = item.Database.GetItem(DefaultDeviceId);
                if (defaultDevice == null)
                    return false;

                var deviceDefinition = layoutDefinition.GetDevice(defaultDevice.ID.ToString());
                return deviceDefinition != null && !string.IsNullOrEmpty(deviceDefinition.Layout);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the layout item assigned to this item
        /// </summary>
        public static Item GetLayout(this Item item)
        {
            if (item == null || !item.HasLayout())
                return null;

            try
            {
                var renderingsField = item.Fields[LayoutFieldId];
                var layoutField = new LayoutField(renderingsField);
                var layoutDefinition = LayoutDefinition.Parse(layoutField.Value);
                
                var defaultDevice = item.Database.GetItem(DefaultDeviceId);
                if (defaultDevice == null)
                    return null;

                var deviceDefinition = layoutDefinition.GetDevice(defaultDevice.ID.ToString());
                if (deviceDefinition == null || string.IsNullOrEmpty(deviceDefinition.Layout))
                    return null;

                return item.Database.GetItem(deviceDefinition.Layout);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Sets the layout for this item
        /// </summary>
        public static bool SetLayout(this Item item, Item layoutItem)
        {
            if (item == null || layoutItem == null)
                return false;

            try
            {
                item.Editing.BeginEdit();
                
                var renderingsField = item.Fields[LayoutFieldId];
                var layoutField = new LayoutField(renderingsField);
                
                string layoutXml = layoutField.Value;
                if (string.IsNullOrEmpty(layoutXml))
                {
                    layoutXml = "<r />";
                }
                
                var layoutDefinition = LayoutDefinition.Parse(layoutXml);
                
                var defaultDevice = item.Database.GetItem(DefaultDeviceId);
                if (defaultDevice == null)
                {
                    item.Editing.CancelEdit();
                    return false;
                }

                var deviceDefinition = layoutDefinition.GetDevice(defaultDevice.ID.ToString());
                if (deviceDefinition == null)
                {
                    deviceDefinition = new DeviceDefinition
                    {
                        ID = defaultDevice.ID.ToString(),
                        Layout = layoutItem.ID.ToString()
                    };
                    
                    layoutDefinition.Devices.Add(deviceDefinition);
                }
                else
                {
                    deviceDefinition.Layout = layoutItem.ID.ToString();
                }

                layoutField.Value = layoutDefinition.ToXml();
                
                item.Editing.EndEdit();
                return true;
            }
            catch
            {
                if (item.Editing.IsEditing)
                    item.Editing.CancelEdit();
                return false;
            }
        }

        /// <summary>
        /// Adds a rendering to the specified placeholder
        /// </summary>
        public static bool AddRendering(this Item item, Item renderingItem, string placeholderKey)
        {
            if (item == null || renderingItem == null || string.IsNullOrEmpty(placeholderKey))
                return false;

            try
            {
                item.Editing.BeginEdit();
                
                var renderingsField = item.Fields[LayoutFieldId];
                var layoutField = new LayoutField(renderingsField);
                
                string layoutXml = layoutField.Value;
                if (string.IsNullOrEmpty(layoutXml))
                {
                    layoutXml = "<r />";
                }
                
                var layoutDefinition = LayoutDefinition.Parse(layoutXml);
                
                var defaultDevice = item.Database.GetItem(DefaultDeviceId);
                if (defaultDevice == null)
                {
                    item.Editing.CancelEdit();
                    return false;
                }

                var deviceDefinition = layoutDefinition.GetDevice(defaultDevice.ID.ToString());
                if (deviceDefinition == null)
                {
                    item.Editing.CancelEdit();
                    return false;
                }

                var renderingDefinition = new RenderingDefinition
                {
                    ItemID = renderingItem.ID.ToString(),
                    Placeholder = placeholderKey,
                    UniqueId = Guid.NewGuid().ToString()
                };

                deviceDefinition.AddRendering(renderingDefinition);
                layoutField.Value = layoutDefinition.ToXml();
                
                item.Editing.EndEdit();
                return true;
            }
            catch
            {
                if (item.Editing.IsEditing)
                    item.Editing.CancelEdit();
                return false;
            }
        }

        /// <summary>
        /// Removes a rendering from the item
        /// </summary>
        public static bool RemoveRendering(this Item item, string renderingId)
        {
            if (item == null || string.IsNullOrEmpty(renderingId))
                return false;

            try
            {
                item.Editing.BeginEdit();
                
                var renderingsField = item.Fields[LayoutFieldId];
                var layoutField = new LayoutField(renderingsField);
                var layoutDefinition = LayoutDefinition.Parse(layoutField.Value);
                
                var defaultDevice = item.Database.GetItem(DefaultDeviceId);
                if (defaultDevice == null)
                {
                    item.Editing.CancelEdit();
                    return false;
                }

                var deviceDefinition = layoutDefinition.GetDevice(defaultDevice.ID.ToString());
                if (deviceDefinition == null)
                {
                    item.Editing.CancelEdit();
                    return false;
                }

                var renderingsToRemove = deviceDefinition.Renderings
                    .Cast<RenderingDefinition>()
                    .Where(r => r.ItemID == renderingId || r.UniqueId == renderingId)
                    .ToList();

                foreach (var rendering in renderingsToRemove)
                {
                    deviceDefinition.Renderings.Remove(rendering);
                }

                layoutField.Value = layoutDefinition.ToXml();
                
                item.Editing.EndEdit();
                return renderingsToRemove.Any();
            }
            catch
            {
                if (item.Editing.IsEditing)
                    item.Editing.CancelEdit();
                return false;
            }
        }

        /// <summary>
        /// Gets all renderings assigned to this item
        /// </summary>
        public static List<RenderingDefinition> GetRenderings(this Item item)
        {
            var renderings = new List<RenderingDefinition>();
            
            if (item == null)
                return renderings;

            try
            {
                var renderingsField = item.Fields[LayoutFieldId];
                if (renderingsField == null || string.IsNullOrEmpty(renderingsField.Value))
                    return renderings;

                var layoutField = new LayoutField(renderingsField);
                var layoutDefinition = LayoutDefinition.Parse(layoutField.Value);
                
                var defaultDevice = item.Database.GetItem(DefaultDeviceId);
                if (defaultDevice == null)
                    return renderings;

                var deviceDefinition = layoutDefinition.GetDevice(defaultDevice.ID.ToString());
                if (deviceDefinition == null)
                    return renderings;

                renderings = deviceDefinition.Renderings.Cast<RenderingDefinition>().ToList();
            }
            catch
            {
                // Return empty list on error
            }

            return renderings;
        }

        /// <summary>
        /// Checks if the item has any presentation details (layout or renderings)
        /// </summary>
        public static bool HasPresentation(this Item item)
        {
            if (item == null)
                return false;

            try
            {
                var renderingsField = item.Fields[LayoutFieldId];
                if (renderingsField == null || string.IsNullOrEmpty(renderingsField.Value))
                    return false;

                return item.HasLayout() || item.GetRenderings().Any();
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
