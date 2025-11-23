using Sitecore.Data;

namespace BMC.Foundation.SitecoreExtensions.Constants
{
    public static class Templates
    {
        public static class BlogPost
        {
            public static readonly ID TemplateId = new ID("{A1B2C3D4-E5F6-7890-ABCD-EF1234567890}");
            
            public static class Fields
            {
                public static readonly ID Title = new ID("{B1C2D3E4-F5A6-7890-BCDE-F12345678901}");
                public static readonly ID Content = new ID("{C2D3E4F5-A6B7-8901-CDEF-123456789012}");
                public static readonly ID ShortDescription = new ID("{D3E4F5A6-B7C8-9012-DEF1-234567890123}");
                public static readonly ID FeaturedImage = new ID("{E4F5A6B7-C8D9-0123-EF12-345678901234}");
                public static readonly ID PublishDate = new ID("{F5A6B7C8-D9E0-1234-F123-456789012345}");
                public static readonly ID Author = new ID("{A6B7C8D9-E0F1-2345-1234-567890123456}");
            }
        }

        public static class BlogListing
        {
            public static readonly ID TemplateId = new ID("{B7C8D9E0-F1A2-3456-2345-678901234567}");
            
            public static class Fields
            {
                public static readonly ID PageTitle = new ID("{C8D9E0F1-A2B3-4567-3456-789012345678}");
            }
        }

        public static class Page
        {
            public static readonly ID TemplateId = new ID("{D9E0F1A2-B3C4-5678-4567-890123456789}");
        }

        public static class Site
        {
            public static readonly ID TemplateId = new ID("{F2FD4169-6FF9-4A5B-826C-63A2F091E91E}");

            public static class Fields
            {
                public static readonly ID SiteMediaLibrary = new ID("{B1A2C3D4-E5F6-7890-ABCD-EF1234567890}");
            }
        }

        public static class BlogRoot
        {
            public static readonly ID TemplateId = new ID("{E1F2A3B4-C5D6-7890-ABCD-EF1234567890}");
        }

        public static class Category
        {
            public static readonly ID TemplateId = new ID("{C1D2E3F4-A5B6-7890-ABCD-EF1234567890}");

            public static class Fields
            {
                public static readonly ID CategoryName = new ID("{D2E3F4A5-B6C7-8901-BCDE-F12345678901}");
                public static readonly ID CategoryDescription = new ID("{E3F4A5B6-C7D8-9012-CDEF-123456789012}");
            }
        }

        public static class Author
        {
            public static readonly ID TemplateId = new ID("{F4A5B6C7-D8E9-0123-DEF1-234567890123}");

            public static class Fields
            {
                public static readonly ID AuthorName = new ID("{A5B6C7D8-E9F0-1234-EF12-345678901234}");
                public static readonly ID Biography = new ID("{B6C7D8E9-F0A1-2345-F123-456789012345}");
                public static readonly ID ProfileImage = new ID("{C7D8E9F0-A1B2-3456-1234-567890123456}");
            }
        }
    }
}
