// src/Foundation/SitecoreExtensions/code/Extensions/ItemExtensions.cs
using Sitecore.Collections;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Sites;
using System.Collections.Generic;
using System.Linq;

namespace YourNamespace.Extensions
{
    public static class ItemExtensions
    {
        // تحقق من وجود Layout
        public static bool HasLayout(this Item item)
        {
            Assert.ArgumentNotNull(item, "item");

            return !string.IsNullOrEmpty(item.LayoutField);
        }

        // الحصول على Layout
        public static LayoutItem GetLayout(this Item item)
        {
            Assert.ArgumentNotNull(item, "item");

            return item.Database?.Layouts.Get(item.LayoutField);
        }

        // تعيين Layout
        public static void SetLayout(this Item item, string layoutName)
        {
            Assert.ArgumentNotNull(item, "item");
            Assert.ArgumentNotNullOrEmpty(layoutName, "layoutName");

            using (new EditContext(item))
            {
                item.Layout_field.SetValue(layoutName);
            }
        }

        // إضافة Rendering
        public static void AddRendering(this Item item, string renderingItemId, string placeholderKey)
        {
            Assert.ArgumentNotNull(item, "item");
            Assert.ArgumentNotNullOrEmpty(renderingItemId, "renderingItemId");
            Assert.ArgumentNotNullOrEmpty(placeholderKey, "placeholderKey");

            var placeholder = item.Descendants().FirstOrDefault(i => i.TemplateID == TemplateIDs.PlaceholderSettings && i["Key"] == placeholderKey);
            if (placeholder != null)
            {
                using (new EditContext(placeholder))
                {
                    var renderingItem = item.Database.GetItem(renderingItemId);
                    if (renderingItem != null)
                    {
                        placeholder.LayoutField.SetValue($"{{ renderings }}|{renderingItemId}");
                    }
                }
            }
        }

        // حذف Rendering
        public static void RemoveRendering(this Item item, string renderingItemId, string placeholderKey)
        {
            Assert.ArgumentNotNull(item, "item");
            Assert.ArgumentNotNullOrEmpty(renderingItemId, "renderingItemId");
            Assert.ArgumentNotNullOrEmpty(placeholderKey, "placeholderKey");

            var placeholder = item.Descendants().FirstOrDefault(i => i.TemplateID == TemplateIDs.PlaceholderSettings && i["Key"] == placeholderKey);
            if (placeholder != null)
            {
                using (new EditContext(placeholder))
                {
                    var renderings = placeholder.Renderings.ToList();
                    var renderingToRemove = renderings.FirstOrDefault(r => r.RenderingItem.ID.ToString() == renderingItemId);
                    if (renderingToRemove != null)
                    {
                        renderings.Remove(renderingToRemove);
                        placeholder.LayoutField.SetValue(string.Join("|", renderings.Select(r => r.RenderingItem.ID)));
                    }
                }
            }
        }

        // الحصول على كل Renderings
        public static IEnumerable<RenderingItem> GetRenderings(this Item item, string placeholderKey)
        {
            Assert.ArgumentNotNull(item, "item");
            Assert.ArgumentNotNullOrEmpty(placeholderKey, "placeholderKey");

            var placeholder = item.Descendants().FirstOrDefault(i => i.TemplateID == TemplateIDs.PlaceholderSettings && i["Key"] == placeholderKey);
            return placeholder?.Renderings ?? Enumerable.Empty<RenderingItem>();
        }

        // تحقق من وجود أي Presentation
        public static bool HasPresentation(this Item item)
        {
            Assert.ArgumentNotNull(item, "item");

            return item.Templates.Any(t => t.BaseTemplates.Any(bt => bt.ID == TemplateIDs.StandardTemplate));
        }
    }
}

// scripts/Test-SitecoreSetup.ps1
<#
.SYNOPSIS
    This script checks the Sitecore setup for missing components such as Layouts, Renderings, Templates, and physical files (Views, DLLs).
.DESCRIPTION
    The script connects to the Sitecore instance and verifies the existence of essential components required for the proper functioning of the site.
    It generates a clear report on the status of each item checked.
#>

param (
    [string]$sitecoreUrl = "http://localhost",
    [string]$username = "admin",
    [string]$password = "admin"
)

# تسجيل الدخول إلى Sitecore
function Login-Sitecore {
    param (
        [string]$url,
        [string]$user,
        [string]$pass
    )

    # هنا يمكنك إضافة كود تسجيل الدخول إلى Sitecore باستخدام Sitecore PowerShell Extensions
}

# التحقق من وجود Layout item
function Test-Layout {
    # هنا يمكنك إضافة كود التحقق من وجود Layout item في Sitecore
}

# التحقق من وجود Rendering items
function Test-Rendering {
    # هنا يمكنك إضافة كود التحقق من وجود Rendering items في Sitecore
}

# التحقق من وجود Templates
function Test-Template {
    # هنا يمكنك إضافة كود التحقق من وجود Templates في Sitecore
}

# التحقق من ربط Layout بالصفحات
function Test-LayoutAssignment {
    # هنا يمكنك إضافة كود التحقق من ربط Layout بالصفحات في Sitecore
}

# التحقق من وجود الملفات الفعلية (Views, DLLs)
function Test-PhysicalFiles {
    # هنا يمكنك إضافة كود التحقق من وجود الملفات الفعلية (Views, DLLs) في Sitecore
}

# تقرير الحالة النهائية
function Generate-Report {
    # هنا يمكنك إضافة كود لتوليد تقرير نهائي عن حالة كل شيء تم التحقّق منه
}

# التحقق من كل شيء
Login-Sitecore -url $sitecoreUrl -user $username -pass $password
Test-Layout
Test-Rendering
Test-Template
Test-LayoutAssignment
Test-PhysicalFiles
Generate-Report