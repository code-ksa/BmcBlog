<#
.SYNOPSIS
    Updates Template IDs in the BMC Blog project constants file

.DESCRIPTION
    This script reads actual Template IDs from Sitecore and generates
    the correct C# constants file for use in the BMC Blog project.

.NOTES
    Author: BMC Blog Team
    Date: 2025-11-23
    Version: 1.0

.EXAMPLE
    # Run in Sitecore PowerShell ISE
    .\Update-Template-IDs.ps1
#>

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "BMC Blog Template IDs Extractor" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# مسارات القوالب في Sitecore
$templatePaths = @{
    "Site" = "/sitecore/templates/Project/BMC/Site"
    "BlogPost" = "/sitecore/templates/Feature/BMC/Blog/Blog Post"
    "BlogRoot" = "/sitecore/templates/Feature/BMC/Blog/Blog Root"
    "Category" = "/sitecore/templates/Feature/BMC/Blog/Category"
    "Author" = "/sitecore/templates/Feature/BMC/Blog/Author"
    "BlogListing" = "/sitecore/templates/Feature/BMC/Blog/Blog Listing"
    "Page" = "/sitecore/templates/Foundation/Page"
}

# التحقق من القوالب واستخراج IDs
Write-Host "استخراج Template IDs من Sitecore..." -ForegroundColor Yellow
Write-Host ""

$templateIds = @{}
$database = Get-Database "master"

foreach ($templateName in $templatePaths.Keys) {
    $templatePath = $templatePaths[$templateName]
    $template = Get-Item -Path $templatePath -Database $database -ErrorAction SilentlyContinue

    if ($template) {
        $templateIds[$templateName] = $template.ID.ToString()
        Write-Host "✓ $templateName : $($template.ID)" -ForegroundColor Green
    }
    else {
        Write-Host "✗ $templateName : القالب غير موجود في $templatePath" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "إنشاء ملف C# Constants" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# إنشاء محتوى ملف C#
$csharpContent = @"
using Sitecore.Data;

namespace BMC.Foundation.SitecoreExtensions.Constants
{
    /// <summary>
    /// Sitecore template IDs for BMC Blog
    /// Auto-generated on $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
    /// </summary>
    public static class Templates
    {
        public static class BlogPost
        {
            public static readonly ID TemplateId = new ID("$($templateIds['BlogPost'])");

            public static class Fields
            {
                public static readonly ID Title = new ID("{B1C2D3E4-F5A6-7890-BCDE-F12345678901}");
                public static readonly ID Content = new ID("{C2D3E4F5-A6B7-8901-CDEF-123456789012}");
                public static readonly ID ShortDescription = new ID("{D3E4F5A6-B7C8-9012-DEF1-234567890123}");
                public static readonly ID FeaturedImage = new ID("{E4F5A6B7-C8D9-0123-EF12-345678901234}");
                public static readonly ID PublishDate = new ID("{F5A6B7C8-D9E0-1234-F123-456789012345}");
                public static readonly ID Author = new ID("{A6B7C8D9-E0F1-2345-1234-567890123456}");
                public static readonly ID Category = new ID("{B7C8D9E0-F1A2-3456-2345-678901234567}");
                public static readonly ID Tags = new ID("{C8D9E0F1-A2B3-4567-3456-789012345678}");
                public static readonly ID ViewCount = new ID("{D9E0F1A2-B3C4-5678-4567-890123456789}");
            }
        }

        public static class BlogListing
        {
            public static readonly ID TemplateId = new ID("$($templateIds['BlogListing'])");

            public static class Fields
            {
                public static readonly ID PageTitle = new ID("{C8D9E0F1-A2B3-4567-3456-789012345678}");
                public static readonly ID ItemsPerPage = new ID("{D9E0F1A2-B3C4-5678-4567-890123456789}");
            }
        }

        public static class Page
        {
            public static readonly ID TemplateId = new ID("$($templateIds['Page'])");

            public static class Fields
            {
                public static readonly ID Title = new ID("{E0F1A2B3-C4D5-6789-5678-901234567890}");
                public static readonly ID MetaDescription = new ID("{F1A2B3C4-D5E6-7890-6789-012345678901}");
            }
        }

        public static class Site
        {
            public static readonly ID TemplateId = new ID("$($templateIds['Site'])");

            public static class Fields
            {
                public static readonly ID SiteMediaLibrary = new ID("{B1A2C3D4-E5F6-7890-ABCD-EF1234567890}");
                public static readonly ID HostName = new ID("{C2D3E4F5-A6B7-8901-BCDE-F12345678901}");
            }
        }

        public static class BlogRoot
        {
            public static readonly ID TemplateId = new ID("$($templateIds['BlogRoot'])");

            public static class Fields
            {
                public static readonly ID Title = new ID("{D3E4F5A6-B7C8-9012-DEF1-234567890123}");
            }
        }

        public static class Category
        {
            public static readonly ID TemplateId = new ID("$($templateIds['Category'])");

            public static class Fields
            {
                public static readonly ID CategoryName = new ID("{D2E3F4A5-B6C7-8901-BCDE-F12345678901}");
                public static readonly ID CategoryDescription = new ID("{E3F4A5B6-C7D8-9012-CDEF-123456789012}");
            }
        }

        public static class Author
        {
            public static readonly ID TemplateId = new ID("$($templateIds['Author'])");

            public static class Fields
            {
                public static readonly ID AuthorName = new ID("{A5B6C7D8-E9F0-1234-EF12-345678901234}");
                public static readonly ID Biography = new ID("{B6C7D8E9-F0A1-2345-F123-456789012345}");
                public static readonly ID ProfileImage = new ID("{C7D8E9F0-A1B2-3456-1234-567890123456}");
                public static readonly ID Email = new ID("{D8E9F0A1-B2C3-4567-2345-678901234567}");
            }
        }
    }
}
"@

Write-Host "محتوى ملف C# Templates.cs:" -ForegroundColor Green
Write-Host $csharpContent
Write-Host ""

# حفظ إلى ملف مؤقت
$outputPath = "$SitecoreDataFolder\temp\Templates.cs"
try {
    $csharpContent | Out-File -FilePath $outputPath -Encoding UTF8
    Write-Host "✓ تم حفظ الملف في: $outputPath" -ForegroundColor Green
}
catch {
    Write-Host "✗ فشل حفظ الملف: $_" -ForegroundColor Red
}

Write-Host ""
Write-Host "=====================================" -ForegroundColor Yellow
Write-Host "ملاحظات مهمة:" -ForegroundColor Yellow
Write-Host "=====================================" -ForegroundColor Yellow
Write-Host "1. قم بنسخ المحتوى أعلاه إلى:" -ForegroundColor Gray
Write-Host "   Foundation/BMC.Foundation.SitecoreExtensions/Constants/Templates.cs" -ForegroundColor White
Write-Host ""
Write-Host "2. تحقق من Field IDs واستبدلها بالقيم الصحيحة من Sitecore" -ForegroundColor Gray
Write-Host ""
Write-Host "3. إعادة بناء المشروع بعد التحديث" -ForegroundColor Gray
Write-Host ""
