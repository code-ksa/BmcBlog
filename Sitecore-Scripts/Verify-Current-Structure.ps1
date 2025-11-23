<#
.SYNOPSIS
    Verifies the current BMC Blog structure in Sitecore

.DESCRIPTION
    This script checks the current state of BMC Blog in Sitecore and reports:
    - Existing templates and their IDs
    - Existing content items
    - Current configuration
    - Missing items or conflicts

    Use this before running other scripts to understand the current state.

.NOTES
    Author: BMC Blog Team
    Date: 2025-11-23
    Version: 1.0

.EXAMPLE
    # Run in Sitecore PowerShell ISE
    .\Verify-Current-Structure.ps1
#>

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "BMC Blog Structure Verification" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

$database = Get-Database "master"
$report = @()

# دالة للتحقق من وجود عنصر
function Check-Item {
    param(
        [string]$Path,
        [string]$Description,
        [string]$Type = "Item"
    )

    $item = Get-Item -Path $Path -Database $database -ErrorAction SilentlyContinue

    if ($item) {
        Write-Host "✓ $Description" -ForegroundColor Green
        Write-Host "  └─ Path: $Path" -ForegroundColor Gray
        Write-Host "  └─ ID: $($item.ID)" -ForegroundColor Gray
        if ($item.TemplateName) {
            Write-Host "  └─ Template: $($item.TemplateName)" -ForegroundColor Gray
        }
        return @{
            Status = "موجود - Exists"
            Path = $Path
            ID = $item.ID.ToString()
            Template = $item.TemplateName
            Description = $Description
        }
    }
    else {
        Write-Host "✗ $Description" -ForegroundColor Red
        Write-Host "  └─ Path: $Path (غير موجود - Not Found)" -ForegroundColor Gray
        return @{
            Status = "مفقود - Missing"
            Path = $Path
            ID = "N/A"
            Template = "N/A"
            Description = $Description
        }
    }
}

# ====================================
# 1. التحقق من Templates
# ====================================
Write-Host ""
Write-Host "1. التحقق من Templates..." -ForegroundColor Cyan
Write-Host ""

$templateChecks = @(
    @{ Path = "/sitecore/templates/Project/BMC"; Desc = "مجلد Project/BMC Templates" },
    @{ Path = "/sitecore/templates/Project/BMC/Site"; Desc = "Site Template" },
    @{ Path = "/sitecore/templates/Feature/BMC"; Desc = "مجلد Feature/BMC Templates" },
    @{ Path = "/sitecore/templates/Feature/BMC/Blog"; Desc = "مجلد Blog Templates" },
    @{ Path = "/sitecore/templates/Feature/BMC/Blog/Blog Root"; Desc = "Blog Root Template" },
    @{ Path = "/sitecore/templates/Feature/BMC/Blog/Blog Post"; Desc = "Blog Post Template" },
    @{ Path = "/sitecore/templates/Feature/BMC/Blog/Category"; Desc = "Category Template" },
    @{ Path = "/sitecore/templates/Feature/BMC/Blog/Author"; Desc = "Author Template" },
    @{ Path = "/sitecore/templates/Feature/BMC/Blog/Blog Listing"; Desc = "Blog Listing Template" }
)

foreach ($check in $templateChecks) {
    $result = Check-Item -Path $check.Path -Description $check.Desc -Type "Template"
    $report += $result
    Write-Host ""
}

# ====================================
# 2. التحقق من Content Structure
# ====================================
Write-Host ""
Write-Host "2. التحقق من بنية المحتوى..." -ForegroundColor Cyan
Write-Host ""

# التحقق من المسار القديم
Write-Host "التحقق من المسار القديم (SA/Blog):" -ForegroundColor Yellow
$oldPath = Check-Item -Path "/sitecore/content/BMC/SA/Blog" -Description "المسار القديم - Old Path (SA/Blog)"
$report += $oldPath
Write-Host ""

# التحقق من المسار الجديد
Write-Host "التحقق من المسار الجديد (BmcBlog):" -ForegroundColor Yellow
$contentChecks = @(
    @{ Path = "/sitecore/content/BMC"; Desc = "مجلد BMC الرئيسي" },
    @{ Path = "/sitecore/content/BMC/BmcBlog"; Desc = "موقع BmcBlog" },
    @{ Path = "/sitecore/content/BMC/BmcBlog/Home"; Desc = "صفحة Home" },
    @{ Path = "/sitecore/content/BMC/BmcBlog/Home/Blog"; Desc = "Blog Root" },
    @{ Path = "/sitecore/content/BMC/BmcBlog/Home/Blog/Posts"; Desc = "مجلد Posts" },
    @{ Path = "/sitecore/content/BMC/BmcBlog/Home/Blog/Categories"; Desc = "مجلد Categories" },
    @{ Path = "/sitecore/content/BMC/BmcBlog/Home/Blog/Authors"; Desc = "مجلد Authors" }
)

foreach ($check in $contentChecks) {
    $result = Check-Item -Path $check.Path -Description $check.Desc
    $report += $result
    Write-Host ""
}

# عد العناصر الموجودة
Write-Host "إحصائيات المحتوى:" -ForegroundColor Yellow

$categoriesPath = "/sitecore/content/BMC/BmcBlog/Home/Blog/Categories"
$categories = Get-ChildItem -Path $categoriesPath -Database $database -ErrorAction SilentlyContinue
if ($categories) {
    Write-Host "  ✓ عدد التصنيفات: $($categories.Count)" -ForegroundColor Green
}
else {
    Write-Host "  ✗ لا توجد تصنيفات" -ForegroundColor Red
}

$authorsPath = "/sitecore/content/BMC/BmcBlog/Home/Blog/Authors"
$authors = Get-ChildItem -Path $authorsPath -Database $database -ErrorAction SilentlyContinue
if ($authors) {
    Write-Host "  ✓ عدد الكتّاب: $($authors.Count)" -ForegroundColor Green
}
else {
    Write-Host "  ✗ لا يوجد كتّاب" -ForegroundColor Red
}

$postsPath = "/sitecore/content/BMC/BmcBlog/Home/Blog/Posts"
$posts = Get-ChildItem -Path $postsPath -Database $database -ErrorAction SilentlyContinue
if ($posts) {
    Write-Host "  ✓ عدد المقالات: $($posts.Count)" -ForegroundColor Green
}
else {
    Write-Host "  ✗ لا توجد مقالات" -ForegroundColor Red
}

Write-Host ""

# ====================================
# 3. التحقق من Media Library
# ====================================
Write-Host ""
Write-Host "3. التحقق من Media Library..." -ForegroundColor Cyan
Write-Host ""

$mediaChecks = @(
    @{ Path = "/sitecore/media library/Project"; Desc = "مجلد Project في Media Library" },
    @{ Path = "/sitecore/media library/Project/BMC"; Desc = "مجلد BMC في Media Library" },
    @{ Path = "/sitecore/media library/Project/BMC/BmcBlog"; Desc = "مجلد BmcBlog في Media Library" }
)

foreach ($check in $mediaChecks) {
    $result = Check-Item -Path $check.Path -Description $check.Desc
    $report += $result
    Write-Host ""
}

# ====================================
# 4. التحقق من التكامل
# ====================================
Write-Host ""
Write-Host "4. التحقق من التكامل..." -ForegroundColor Cyan
Write-Host ""

# التحقق من ربط Site بـ Media Library
$siteItem = Get-Item -Path "/sitecore/content/BMC/BmcBlog" -Database $database -ErrorAction SilentlyContinue
if ($siteItem) {
    $mediaLibraryField = $siteItem.Fields["Site Media Library"]
    if ($mediaLibraryField -and $mediaLibraryField.Value) {
        $mediaLibraryId = $mediaLibraryField.Value
        $mediaLibraryItem = Get-Item -Database $database -ID $mediaLibraryId -ErrorAction SilentlyContinue
        if ($mediaLibraryItem) {
            Write-Host "✓ Site مربوط بمكتبة الوسائط" -ForegroundColor Green
            Write-Host "  └─ Media Library: $($mediaLibraryItem.Paths.FullPath)" -ForegroundColor Gray
        }
        else {
            Write-Host "✗ معرف مكتبة الوسائط غير صحيح" -ForegroundColor Red
        }
    }
    else {
        Write-Host "⚠ Site غير مربوط بمكتبة الوسائط" -ForegroundColor Yellow
    }
}

Write-Host ""

# ====================================
# 5. ملخص النتائج
# ====================================
Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "ملخص التحقق" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

$existingItems = ($report | Where-Object { $_.Status -like "*Exists*" }).Count
$missingItems = ($report | Where-Object { $_.Status -like "*Missing*" }).Count
$totalItems = $report.Count

Write-Host "إحصائيات العناصر:" -ForegroundColor Yellow
Write-Host "  ✓ عناصر موجودة: $existingItems من $totalItems" -ForegroundColor Green
Write-Host "  ✗ عناصر مفقودة: $missingItems من $totalItems" -ForegroundColor Red
Write-Host ""

# عرض العناصر المفقودة
if ($missingItems -gt 0) {
    Write-Host "العناصر المفقودة:" -ForegroundColor Red
    $report | Where-Object { $_.Status -like "*Missing*" } | ForEach-Object {
        Write-Host "  • $($_.Description)" -ForegroundColor Gray
        Write-Host "    Path: $($_.Path)" -ForegroundColor DarkGray
    }
    Write-Host ""
}

# ====================================
# 6. التوصيات
# ====================================
Write-Host ""
Write-Host "=====================================" -ForegroundColor Yellow
Write-Host "التوصيات" -ForegroundColor Yellow
Write-Host "=====================================" -ForegroundColor Yellow
Write-Host ""

# تحليل الوضع الحالي
$hasOldPath = (Get-Item -Path "/sitecore/content/BMC/SA/Blog" -Database $database -ErrorAction SilentlyContinue) -ne $null
$hasNewPath = (Get-Item -Path "/sitecore/content/BMC/BmcBlog" -Database $database -ErrorAction SilentlyContinue) -ne $null
$hasTemplates = (Get-Item -Path "/sitecore/templates/Feature/BMC/Blog" -Database $database -ErrorAction SilentlyContinue) -ne $null

if (-not $hasTemplates) {
    Write-Host "⚠ Templates غير موجودة" -ForegroundColor Yellow
    Write-Host "   → قم بتشغيل: Create-BMC-Blog-Templates.ps1" -ForegroundColor White
    Write-Host ""
}

if ($hasTemplates -and -not $hasNewPath) {
    Write-Host "⚠ Templates موجودة لكن المحتوى مفقود" -ForegroundColor Yellow
    Write-Host "   → قم بتشغيل: Create-BMC-Blog-Structure.ps1" -ForegroundColor White
    Write-Host ""
}

if ($hasOldPath -and $hasNewPath) {
    Write-Host "⚠ كلا المسارين موجودان (القديم والجديد)" -ForegroundColor Yellow
    Write-Host "   → قرر أي مسار تريد استخدامه" -ForegroundColor White
    Write-Host "   → احذف المسار القديم إذا لم تعد بحاجة إليه" -ForegroundColor White
    Write-Host ""
}

if ($hasTemplates -and $hasNewPath) {
    Write-Host "✓ البنية الأساسية موجودة" -ForegroundColor Green
    Write-Host "   → قم بتشغيل: Update-Template-IDs.ps1 لاستخراج IDs" -ForegroundColor White
    Write-Host "   → راجع المحتوى وأضف المزيد حسب الحاجة" -ForegroundColor White
    Write-Host ""
}

# ====================================
# 7. معلومات إضافية
# ====================================
Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "معلومات إضافية" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "للحصول على معلومات مفصلة عن عنصر معين:" -ForegroundColor Gray
Write-Host '  Get-Item -Path "/sitecore/content/BMC/BmcBlog" | Format-List' -ForegroundColor White
Write-Host ""

Write-Host "لعرض جميع الحقول:" -ForegroundColor Gray
Write-Host '  $item = Get-Item -Path "/sitecore/content/BMC/BmcBlog"' -ForegroundColor White
Write-Host '  $item.Fields | Format-Table Name, Value -AutoSize' -ForegroundColor White
Write-Host ""

Write-Host "لعرض العناصر الفرعية:" -ForegroundColor Gray
Write-Host '  Get-ChildItem -Path "/sitecore/content/BMC/BmcBlog" -Recurse' -ForegroundColor White
Write-Host ""

Write-Host "=====================================" -ForegroundColor Green
Write-Host "انتهى التحقق!" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green
Write-Host ""
