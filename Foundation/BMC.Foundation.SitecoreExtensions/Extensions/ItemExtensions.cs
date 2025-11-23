using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Fields;

namespace BMC.Foundation.SitecoreExtensions.Extensions
{
    public static class ItemExtensions
    {
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
    }
}
