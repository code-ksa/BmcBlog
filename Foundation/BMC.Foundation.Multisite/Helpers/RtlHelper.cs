using Sitecore.Globalization;

namespace BMC.Foundation.Multisite.Helpers
{
    public static class RtlHelper
    {
        public static bool IsRtlLanguage(string languageCode)
        {
            if (string.IsNullOrEmpty(languageCode))
                return false;

            var normalizedCode = languageCode.ToLower().Trim();
            
            return normalizedCode == "ar" || 
                   normalizedCode == "ar-sa" || 
                   normalizedCode == "he" || 
                   normalizedCode == "he-il";
        }

        public static string GetDirection()
        {
            var currentLanguage = Sitecore.Context.Language;
            if (currentLanguage == null)
                return "ltr";

            return IsRtlLanguage(currentLanguage.Name) ? "rtl" : "ltr";
        }

        public static string GetTextAlign()
        {
            var currentLanguage = Sitecore.Context.Language;
            if (currentLanguage == null)
                return "left";

            return IsRtlLanguage(currentLanguage.Name) ? "right" : "left";
        }

        public static string GetDirection(string languageCode)
        {
            return IsRtlLanguage(languageCode) ? "rtl" : "ltr";
        }

        public static string GetTextAlign(string languageCode)
        {
            return IsRtlLanguage(languageCode) ? "right" : "left";
        }
    }
}