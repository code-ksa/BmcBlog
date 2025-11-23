using Sitecore.Sites;
using Sitecore.Web;

namespace BMC.Foundation.Multisite.Providers
{
    public class SiteResolver
    {
        public SiteContext GetCurrentSite()
        {
            return Sitecore.Context.Site;
        }

        public SiteContext GetSiteByName(string siteName)
        {
            if (string.IsNullOrEmpty(siteName))
                return null;

            var site = SiteContextFactory.GetSiteContext(siteName);
            return site;
        }

        public SiteContext ResolveSiteFromHostname(string hostname)
        {
            if (string.IsNullOrEmpty(hostname))
                return null;

            // Normalize hostname
            hostname = hostname.ToLower().Trim();

            // Handle blog subdomain
            if (hostname.Contains("blog.sa.bmc.local") || hostname.Contains("blog"))
            {
                return GetSiteByName("blog");
            }

            // Handle main site
            if (hostname.Contains("sa.bmc.local"))
            {
                return GetSiteByName("website");
            }

            // Fallback to current site
            return GetCurrentSite();
        }
    }
}