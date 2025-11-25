using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;

namespace BMC.Foundation.SitecoreExtensions.Helpers
{
    /// <summary>
    /// Helper class for managing Sitecore Template items
    /// </summary>
    public class TemplateManager
    {
        // Template IDs
        private static readonly ID TemplateFolderTemplateId = new ID("{0437FEE2-44C9-46A6-ABE9-28858D9FEE8C}"); // Template Folder
        private static readonly ID TemplateTemplateId = new ID("{AB86861A-6030-46C5-B394-E8F99E8B87DB}"); // Template
        private static readonly ID TemplateSectionTemplateId = new ID("{E269FBB5-3750-427A-9149-7AA950B49301}"); // Template Section
        private static readonly ID TemplateFieldTemplateId = new ID("{455A3E98-A627-4B40-8035-E683A0331AC7}"); // Template Field
        private static readonly ID FolderTemplateId = new ID("{A87A00B1-E6DB-45AB-8B54-636FEC3B5523}"); // Common/Folder

        // Template paths
        private const string TemplatesFolderPath = "/sitecore/templates";
        private const string BmcTemplatesFolderPath = "/sitecore/templates/BMC";
        private const string UserDefinedFolderPath = "/sitecore/templates/User Defined";

        /// <summary>
        /// Creates a template folder
        /// </summary>
        public static Item CreateTemplateFolder(Database database, string folderName, string parentPath = BmcTemplatesFolderPath)
        {
            if (database == null || string.IsNullOrEmpty(folderName))
                return null;

            try
            {
                // Get parent folder
                Item parentFolder = database.GetItem(parentPath);
                if (parentFolder == null)
                {
                    // Create BMC folder if it doesn't exist
                    if (parentPath == BmcTemplatesFolderPath)
                    {
                        parentFolder = EnsureBmcTemplatesFolder(database);
                    }

                    if (parentFolder == null)
                    {
                        Sitecore.Diagnostics.Log.Error($"TemplateManager: Parent folder not found: {parentPath}", typeof(TemplateManager));
                        return null;
                    }
                }

                // Check if folder already exists
                Item folder = parentFolder.Children[folderName];
                if (folder != null)
                {
                    Sitecore.Diagnostics.Log.Info($"TemplateManager: Template folder '{folderName}' already exists", typeof(TemplateManager));
                    return folder;
                }

                // Create folder
                using (new SecurityDisabler())
                {
                    Sitecore.Diagnostics.Log.Info($"TemplateManager: Creating template folder '{folderName}'", typeof(TemplateManager));
                    folder = parentFolder.Add(folderName, new TemplateID(TemplateFolderTemplateId));

                    if (folder != null)
                    {
                        Sitecore.Diagnostics.Log.Info($"TemplateManager: Template folder created - ID: {folder.ID}", typeof(TemplateManager));
                    }
                }

                return folder;
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error($"TemplateManager: Error creating template folder: {ex.Message}", ex, typeof(TemplateManager));
                return null;
            }
        }

        /// <summary>
        /// Creates a template item
        /// </summary>
        public static Item CreateTemplate(Database database, string templateName, string parentPath = BmcTemplatesFolderPath, string baseTemplateIds = null)
        {
            if (database == null || string.IsNullOrEmpty(templateName))
                return null;

            try
            {
                // Ensure parent folder exists
                Item parentFolder = database.GetItem(parentPath);
                if (parentFolder == null)
                {
                    parentFolder = EnsureBmcTemplatesFolder(database);
                }

                if (parentFolder == null)
                {
                    Sitecore.Diagnostics.Log.Error($"TemplateManager: Parent folder not found: {parentPath}", typeof(TemplateManager));
                    return null;
                }

                // Check if template already exists
                Item template = parentFolder.Children[templateName];
                if (template != null)
                {
                    Sitecore.Diagnostics.Log.Info($"TemplateManager: Template '{templateName}' already exists", typeof(TemplateManager));
                    return template;
                }

                // Create template
                using (new SecurityDisabler())
                {
                    Sitecore.Diagnostics.Log.Info($"TemplateManager: Creating template '{templateName}'", typeof(TemplateManager));
                    template = parentFolder.Add(templateName, new TemplateID(TemplateTemplateId));

                    if (template != null)
                    {
                        // Set base templates if provided
                        if (!string.IsNullOrEmpty(baseTemplateIds))
                        {
                            template.Editing.BeginEdit();
                            try
                            {
                                template["__Base template"] = baseTemplateIds;
                                template.Editing.EndEdit();
                            }
                            catch
                            {
                                template.Editing.CancelEdit();
                                throw;
                            }
                        }

                        Sitecore.Diagnostics.Log.Info($"TemplateManager: Template created - ID: {template.ID}", typeof(TemplateManager));
                    }
                }

                return template;
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error($"TemplateManager: Error creating template: {ex.Message}", ex, typeof(TemplateManager));
                return null;
            }
        }

        /// <summary>
        /// Adds a section to a template
        /// </summary>
        public static Item AddSection(Item template, string sectionName, int sortOrder = 100)
        {
            if (template == null || string.IsNullOrEmpty(sectionName))
                return null;

            try
            {
                // Check if section already exists
                Item section = template.Children[sectionName];
                if (section != null)
                {
                    Sitecore.Diagnostics.Log.Info($"TemplateManager: Section '{sectionName}' already exists in template '{template.Name}'", typeof(TemplateManager));
                    return section;
                }

                // Create section
                using (new SecurityDisabler())
                {
                    Sitecore.Diagnostics.Log.Info($"TemplateManager: Adding section '{sectionName}' to template '{template.Name}'", typeof(TemplateManager));
                    section = template.Add(sectionName, new TemplateID(TemplateSectionTemplateId));

                    if (section != null)
                    {
                        section.Editing.BeginEdit();
                        try
                        {
                            section.Appearance.Sortorder = sortOrder;
                            section.Editing.EndEdit();
                        }
                        catch
                        {
                            section.Editing.CancelEdit();
                            throw;
                        }

                        Sitecore.Diagnostics.Log.Info($"TemplateManager: Section created - ID: {section.ID}", typeof(TemplateManager));
                    }
                }

                return section;
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error($"TemplateManager: Error adding section: {ex.Message}", ex, typeof(TemplateManager));
                return null;
            }
        }

        /// <summary>
        /// Adds a field to a template section
        /// </summary>
        public static Item AddField(Item section, string fieldName, string fieldType, int sortOrder = 100)
        {
            if (section == null || string.IsNullOrEmpty(fieldName))
                return null;

            try
            {
                // Check if field already exists
                Item field = section.Children[fieldName];
                if (field != null)
                {
                    Sitecore.Diagnostics.Log.Info($"TemplateManager: Field '{fieldName}' already exists in section '{section.Name}'", typeof(TemplateManager));
                    return field;
                }

                // Create field
                using (new SecurityDisabler())
                {
                    Sitecore.Diagnostics.Log.Info($"TemplateManager: Adding field '{fieldName}' to section '{section.Name}'", typeof(TemplateManager));
                    field = section.Add(fieldName, new TemplateID(TemplateFieldTemplateId));

                    if (field != null)
                    {
                        field.Editing.BeginEdit();
                        try
                        {
                            field["Type"] = fieldType;
                            field.Appearance.Sortorder = sortOrder;
                            field.Editing.EndEdit();
                        }
                        catch
                        {
                            field.Editing.CancelEdit();
                            throw;
                        }

                        Sitecore.Diagnostics.Log.Info($"TemplateManager: Field created - ID: {field.ID}", typeof(TemplateManager));
                    }
                }

                return field;
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error($"TemplateManager: Error adding field: {ex.Message}", ex, typeof(TemplateManager));
                return null;
            }
        }

        /// <summary>
        /// Creates or gets standard values for a template
        /// </summary>
        public static Item CreateStandardValues(Item template)
        {
            if (template == null)
                return null;

            try
            {
                // Check if standard values already exist
                Item standardValues = template.Children["__Standard Values"];
                if (standardValues != null)
                {
                    Sitecore.Diagnostics.Log.Info($"TemplateManager: Standard values already exist for template '{template.Name}'", typeof(TemplateManager));
                    return standardValues;
                }

                // Create standard values
                using (new SecurityDisabler())
                {
                    Sitecore.Diagnostics.Log.Info($"TemplateManager: Creating standard values for template '{template.Name}'", typeof(TemplateManager));

                    TemplateItem templateItem = new TemplateItem(template);
                    standardValues = templateItem.CreateStandardValues();

                    if (standardValues != null)
                    {
                        Sitecore.Diagnostics.Log.Info($"TemplateManager: Standard values created - ID: {standardValues.ID}", typeof(TemplateManager));
                    }
                }

                return standardValues;
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error($"TemplateManager: Error creating standard values: {ex.Message}", ex, typeof(TemplateManager));
                return null;
            }
        }

        /// <summary>
        /// Assigns a layout to template standard values
        /// </summary>
        public static bool AssignLayoutToStandardValues(Item template, Item layoutItem)
        {
            if (template == null || layoutItem == null)
                return false;

            try
            {
                // Get or create standard values
                Item standardValues = CreateStandardValues(template);
                if (standardValues == null)
                {
                    Sitecore.Diagnostics.Log.Error($"TemplateManager: Could not create standard values for template '{template.Name}'", typeof(TemplateManager));
                    return false;
                }

                // Assign layout
                return LayoutItemManager.AssignLayoutToTemplate(template, layoutItem);
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error($"TemplateManager: Error assigning layout to standard values: {ex.Message}", ex, typeof(TemplateManager));
                return false;
            }
        }

        /// <summary>
        /// Creates the Blog Post template with all fields
        /// </summary>
        public static Item CreateBlogPostTemplate(Database database)
        {
            if (database == null)
                return null;

            try
            {
                Sitecore.Diagnostics.Log.Info("TemplateManager: Creating Blog Post template", typeof(TemplateManager));

                // Create template
                Item template = CreateTemplate(database, "Blog Post", BmcTemplatesFolderPath);
                if (template == null)
                    return null;

                // Add Content section
                Item contentSection = AddSection(template, "Content", 100);
                if (contentSection != null)
                {
                    AddField(contentSection, "Title", "Single-Line Text", 100);
                    AddField(contentSection, "Content", "Rich Text", 200);
                    AddField(contentSection, "Summary", "Multi-Line Text", 300);
                }

                // Add Metadata section
                Item metadataSection = AddSection(template, "Metadata", 200);
                if (metadataSection != null)
                {
                    AddField(metadataSection, "Author", "Single-Line Text", 100);
                    AddField(metadataSection, "PublishDate", "Datetime", 200);
                    AddField(metadataSection, "Category", "Droplink", 300);
                    AddField(metadataSection, "Tags", "Multilist", 400);
                }

                // Add Media section
                Item mediaSection = AddSection(template, "Media", 300);
                if (mediaSection != null)
                {
                    AddField(mediaSection, "FeaturedImage", "Image", 100);
                }

                Sitecore.Diagnostics.Log.Info("TemplateManager: Blog Post template created successfully", typeof(TemplateManager));

                return template;
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error($"TemplateManager: Error creating Blog Post template: {ex.Message}", ex, typeof(TemplateManager));
                return null;
            }
        }

        /// <summary>
        /// Creates the Blog Home template with all fields
        /// </summary>
        public static Item CreateBlogHomeTemplate(Database database)
        {
            if (database == null)
                return null;

            try
            {
                Sitecore.Diagnostics.Log.Info("TemplateManager: Creating Blog Home template", typeof(TemplateManager));

                // Create template
                Item template = CreateTemplate(database, "Blog Home", BmcTemplatesFolderPath);
                if (template == null)
                    return null;

                // Add Content section
                Item contentSection = AddSection(template, "Content", 100);
                if (contentSection != null)
                {
                    AddField(contentSection, "Title", "Single-Line Text", 100);
                    AddField(contentSection, "IntroText", "Rich Text", 200);
                }

                Sitecore.Diagnostics.Log.Info("TemplateManager: Blog Home template created successfully", typeof(TemplateManager));

                return template;
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error($"TemplateManager: Error creating Blog Home template: {ex.Message}", ex, typeof(TemplateManager));
                return null;
            }
        }

        /// <summary>
        /// Ensures the BMC templates folder exists
        /// </summary>
        private static Item EnsureBmcTemplatesFolder(Database database)
        {
            if (database == null)
                return null;

            // Check if BMC folder exists
            Item bmcFolder = database.GetItem(BmcTemplatesFolderPath);
            if (bmcFolder != null)
                return bmcFolder;

            // Get Templates folder
            Item templatesFolder = database.GetItem(TemplatesFolderPath);
            if (templatesFolder == null)
            {
                Sitecore.Diagnostics.Log.Error("TemplateManager: Templates folder not found", typeof(TemplateManager));
                return null;
            }

            // Create BMC folder
            using (new SecurityDisabler())
            {
                Sitecore.Diagnostics.Log.Info("TemplateManager: Creating BMC templates folder", typeof(TemplateManager));
                bmcFolder = templatesFolder.Add("BMC", new TemplateID(TemplateFolderTemplateId));

                if (bmcFolder != null)
                {
                    Sitecore.Diagnostics.Log.Info($"TemplateManager: BMC templates folder created - ID: {bmcFolder.ID}", typeof(TemplateManager));
                }
            }

            return bmcFolder;
        }
    }
}
