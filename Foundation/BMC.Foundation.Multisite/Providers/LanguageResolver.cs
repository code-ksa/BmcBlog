using Sitecore.Globalization;
using Sitecore.Web;
using System.Collections.Generic;
using System.Linq;

namespace BMC.Foundation.Multisite.Providers
{
    public class LanguageResolver
    {
        private const string LanguageCookieName = "sc_lang";

        /// <summary>
        /// Gets the current language from Sitecore context
        /// </summary>
        public Language GetCurrentLanguage()
        {
            if (Sitecore.Context.Language != null)
                return Sitecore.Context.Language;

            return Language.Parse("en");
        }

        /// <summary>
        /// Sets the language for the current context
        /// </summary>
        public void SetLanguage(string languageCode)
        {
            if (string.IsNullOrEmpty(languageCode))
                return;

            var language = Language.Parse(languageCode);
            Sitecore.Context.Language = language;

            // Store in cookie using Sitecore's WebUtil
            if (WebUtil.GetCookieValue(LanguageCookieName) != languageCode)
            {
                WebUtil.SetCookieValue(LanguageCookieName, languageCode);
            }
        }

        /// <summary>
        /// Gets the language from cookie
        /// </summary>
        private Language GetLanguageFromCookie()
        {
            var languageCode = WebUtil.GetCookieValue(LanguageCookieName);

            if (string.IsNullOrEmpty(languageCode))
                return null;

            try
            {
                return Language.Parse(languageCode);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets all available languages (English and Arabic)
        /// </summary>
        public List<Language> GetAvailableLanguages()
        {
            var languages = new List<Language>
            {
                Language.Parse("en"),
                Language.Parse("ar")
            };

            return languages;
        }
    }
}