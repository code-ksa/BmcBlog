<#
.SYNOPSIS
    Creates or updates the BMC Blog structure in Sitecore

.DESCRIPTION
    This script creates the complete BMC Blog content structure in Sitecore including:
    - Site root item
    - Home page
    - Blog root with folders
    - Sample categories, authors, and posts
    - Media library structure

    If items already exist, they will be updated instead of creating duplicates.

.NOTES
    Author: BMC Blog Team
    Date: 2025-11-23
    Version: 1.0

.EXAMPLE
    # Run in Sitecore PowerShell ISE
    .\Create-BMC-Blog-Structure.ps1
#>

# التحقق العام - General Validation
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "BMC Blog Structure Creator" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# تعريف المسارات - Define Paths
$rootPath = "/sitecore/content/BMC"
$sitePath = "/sitecore/content/BMC/BmcBlog"
$homePath = "$sitePath/Home"
$blogPath = "$sitePath/Home/Blog"
$postsPath = "$blogPath/Posts"
$categoriesPath = "$blogPath/Categories"
$authorsPath = "$blogPath/Authors"
$settingsPath = "$sitePath/Settings"
$mediaLibraryPath = "/sitecore/media library/Project/BMC/BmcBlog"

# تعريف Template IDs
$siteTemplateId = "{F2FD4169-6FF9-4A5B-826C-63A2F091E91E}"
$folderTemplateId = "{A87A00B1-E6DB-45AB-8B54-636FEC3B5523}"
$blogRootTemplateId = "{E1F2A3B4-C5D6-7890-ABCD-EF1234567890}"
$blogPostTemplateId = "{A1B2C3D4-E5F6-7890-ABCD-EF1234567890}"
$categoryTemplateId = "{C1D2E3F4-A5B6-7890-ABCD-EF1234567890}"
$authorTemplateId = "{F4A5B6C7-D8E9-0123-DEF1-234567890123}"

# الحصول على قاعدة البيانات
$database = Get-Database "master"

# دالة مساعدة لإنشاء أو تحديث عنصر
function Create-OrUpdate-Item {
    param(
        [string]$Path,
        [string]$Name,
        [string]$TemplateId,
        [hashtable]$Fields = @{},
        [string]$Language = "en"
    )

    $parentPath = Split-Path $Path -Parent
    $itemName = Split-Path $Path -Leaf

    # التحقق من وجود العنصر
    $existingItem = Get-Item -Path $Path -Database $database -ErrorAction SilentlyContinue

    if ($existingItem) {
        Write-Host "✓ تحديث العنصر الموجود: $Path" -ForegroundColor Yellow
        $item = $existingItem
    }
    else {
        # التحقق من وجود العنصر الأب
        $parentItem = Get-Item -Path $parentPath -Database $database -ErrorAction SilentlyContinue

        if (-not $parentItem) {
            Write-Host "✗ العنصر الأب غير موجود: $parentPath" -ForegroundColor Red
            return $null
        }

        # إنشاء العنصر الجديد
        Write-Host "✓ إنشاء عنصر جديد: $Path" -ForegroundColor Green
        $item = New-Item -Path $parentPath -Name $itemName -ItemType $TemplateId -Database $database
    }

    # تحديث الحقول إذا تم تحديدها
    if ($Fields.Count -gt 0) {
        $item.Editing.BeginEdit()
        try {
            foreach ($fieldName in $Fields.Keys) {
                $fieldValue = $Fields[$fieldName]
                if ($item.Fields[$fieldName]) {
                    $item.Fields[$fieldName].Value = $fieldValue
                    Write-Host "  └─ تحديث حقل: $fieldName" -ForegroundColor Gray
                }
            }
            $item.Editing.EndEdit() | Out-Null
        }
        catch {
            $item.Editing.CancelEdit()
            Write-Host "  └─ خطأ في تحديث الحقول: $_" -ForegroundColor Red
        }
    }

    return $item
}

# دالة لإنشاء مجلد
function Create-Folder {
    param(
        [string]$Path,
        [string]$DisplayName = ""
    )

    $name = Split-Path $Path -Leaf
    $fields = @{}
    if ($DisplayName) {
        $fields["__Display name"] = $DisplayName
    }

    return Create-OrUpdate-Item -Path $Path -Name $name -TemplateId $folderTemplateId -Fields $fields
}

# ====================================
# 1. إنشاء بنية المحتوى الأساسية
# ====================================
Write-Host ""
Write-Host "1. إنشاء بنية المحتوى الأساسية..." -ForegroundColor Cyan

# التحقق من مجلد BMC الرئيسي
$bmcFolder = Get-Item -Path $rootPath -Database $database -ErrorAction SilentlyContinue
if (-not $bmcFolder) {
    Write-Host "✓ إنشاء مجلد BMC الرئيسي" -ForegroundColor Green
    $bmcFolder = Create-Folder -Path $rootPath -DisplayName "BMC"
}
else {
    Write-Host "✓ مجلد BMC موجود مسبقاً" -ForegroundColor Yellow
}

# إنشاء موقع BmcBlog
$siteItem = Create-OrUpdate-Item -Path $sitePath -Name "BmcBlog" -TemplateId $siteTemplateId -Fields @{
    "__Display name" = "BMC Blog"
    "Title" = "BMC Blog"
}

# إنشاء صفحة Home
$homeItem = Create-OrUpdate-Item -Path $homePath -Name "Home" -TemplateId $folderTemplateId -Fields @{
    "__Display name" = "Home"
    "Title" = "الصفحة الرئيسية - Home"
}

# ====================================
# 2. إنشاء بنية المدونة
# ====================================
Write-Host ""
Write-Host "2. إنشاء بنية المدونة..." -ForegroundColor Cyan

# إنشاء Blog Root
$blogItem = Create-OrUpdate-Item -Path $blogPath -Name "Blog" -TemplateId $blogRootTemplateId -Fields @{
    "__Display name" = "Blog"
    "Title" = "المدونة - Blog"
}

# إنشاء مجلدات المدونة
$postsFolder = Create-Folder -Path $postsPath -DisplayName "المقالات - Posts"
$categoriesFolder = Create-Folder -Path $categoriesPath -DisplayName "التصنيفات - Categories"
$authorsFolder = Create-Folder -Path $authorsPath -DisplayName "الكتّاب - Authors"

# ====================================
# 3. إنشاء التصنيفات النموذجية
# ====================================
Write-Host ""
Write-Host "3. إنشاء التصنيفات النموذجية..." -ForegroundColor Cyan

$sampleCategories = @(
    @{
        Name = "Technology"
        DisplayName = "التقنية - Technology"
        Description = "مقالات حول التقنية والبرمجة"
    },
    @{
        Name = "Business"
        DisplayName = "الأعمال - Business"
        Description = "مقالات حول إدارة الأعمال"
    },
    @{
        Name = "Development"
        DisplayName = "التطوير - Development"
        Description = "مقالات حول تطوير البرمجيات"
    },
    @{
        Name = "Cloud"
        DisplayName = "السحابة - Cloud"
        Description = "مقالات حول الحوسبة السحابية"
    }
)

foreach ($category in $sampleCategories) {
    $categoryPath = "$categoriesPath/$($category.Name)"
    Create-OrUpdate-Item -Path $categoryPath -Name $category.Name -TemplateId $categoryTemplateId -Fields @{
        "__Display name" = $category.DisplayName
        "Category Name" = $category.DisplayName
        "Category Description" = $category.Description
    }
}

# ====================================
# 4. إنشاء الكتّاب النموذجيين
# ====================================
Write-Host ""
Write-Host "4. إنشاء الكتّاب النموذجيين..." -ForegroundColor Cyan

$sampleAuthors = @(
    @{
        Name = "Ahmad-Khalil"
        DisplayName = "أحمد خليل"
        Biography = "خبير في تطوير البرمجيات والحلول السحابية"
    },
    @{
        Name = "Sara-Mohammed"
        DisplayName = "سارة محمد"
        Biography = "مختصة في أمن المعلومات والتقنيات الحديثة"
    },
    @{
        Name = "Mohammed-Ali"
        DisplayName = "محمد علي"
        Biography = "مهندس برمجيات متخصص في Sitecore"
    }
)

foreach ($author in $sampleAuthors) {
    $authorPath = "$authorsPath/$($author.Name)"
    Create-OrUpdate-Item -Path $authorPath -Name $author.Name -TemplateId $authorTemplateId -Fields @{
        "__Display name" = $author.DisplayName
        "Author Name" = $author.DisplayName
        "Biography" = $author.Biography
    }
}

# ====================================
# 5. إنشاء مقالات نموذجية
# ====================================
Write-Host ""
Write-Host "5. إنشاء مقالات نموذجية..." -ForegroundColor Cyan

$publishDate = Get-Date -Format "yyyyMMddTHHmmss"

$samplePosts = @(
    @{
        Name = "Welcome-to-BMC-Blog"
        Title = "مرحباً بكم في مدونة BMC"
        Summary = "مقالة ترحيبية في المدونة الجديدة"
        Content = "<p>نرحب بكم في مدونة BMC الجديدة. هنا ستجدون أحدث المقالات والأخبار التقنية.</p>"
        Category = "Technology"
    },
    @{
        Name = "Getting-Started-with-Sitecore"
        Title = "البدء مع Sitecore"
        Summary = "دليل شامل للمبتدئين في Sitecore"
        Content = "<p>تعلم كيفية البدء مع منصة Sitecore لإدارة المحتوى.</p>"
        Category = "Development"
    },
    @{
        Name = "Cloud-Computing-Best-Practices"
        Title = "أفضل ممارسات الحوسبة السحابية"
        Summary = "نصائح وإرشادات للعمل مع السحابة"
        Content = "<p>اكتشف أفضل الممارسات للاستفادة من الحوسبة السحابية في مشاريعك.</p>"
        Category = "Cloud"
    }
)

foreach ($post in $samplePosts) {
    $postPath = "$postsPath/$($post.Name)"

    # الحصول على معرف التصنيف
    $categoryItem = Get-Item -Path "$categoriesPath/$($post.Category)" -Database $database -ErrorAction SilentlyContinue
    $categoryId = if ($categoryItem) { $categoryItem.ID } else { "" }

    # الحصول على معرف كاتب عشوائي
    $authorItems = Get-ChildItem -Path $authorsPath -Database $database
    $authorId = if ($authorItems) { ($authorItems | Get-Random).ID } else { "" }

    Create-OrUpdate-Item -Path $postPath -Name $post.Name -TemplateId $blogPostTemplateId -Fields @{
        "__Display name" = $post.Title
        "Title" = $post.Title
        "Content" = $post.Content
        "Short Description" = $post.Summary
        "Publish Date" = $publishDate
        "Category" = $categoryId
        "Author" = $authorId
        "View Count" = "0"
    }
}

# ====================================
# 6. إنشاء مجلد Settings
# ====================================
Write-Host ""
Write-Host "6. إنشاء مجلد الإعدادات..." -ForegroundColor Cyan

$settingsFolder = Create-Folder -Path $settingsPath -DisplayName "Settings"

# ====================================
# 7. إنشاء بنية Media Library
# ====================================
Write-Host ""
Write-Host "7. إنشاء بنية Media Library..." -ForegroundColor Cyan

# التحقق من مجلد Project في Media Library
$projectMediaPath = "/sitecore/media library/Project"
$projectMediaFolder = Get-Item -Path $projectMediaPath -Database $database -ErrorAction SilentlyContinue
if (-not $projectMediaFolder) {
    Write-Host "✓ إنشاء مجلد Project في Media Library" -ForegroundColor Green
    $projectMediaFolder = New-Item -Path "/sitecore/media library" -Name "Project" -ItemType "System/Media/Media folder" -Database $database
}

# إنشاء مجلد BMC في Media Library
$bmcMediaPath = "/sitecore/media library/Project/BMC"
$bmcMediaFolder = Get-Item -Path $bmcMediaPath -Database $database -ErrorAction SilentlyContinue
if (-not $bmcMediaFolder) {
    Write-Host "✓ إنشاء مجلد BMC في Media Library" -ForegroundColor Green
    $bmcMediaFolder = New-Item -Path $projectMediaPath -Name "BMC" -ItemType "System/Media/Media folder" -Database $database
}

# إنشاء مجلد BmcBlog في Media Library
$blogMediaFolder = Get-Item -Path $mediaLibraryPath -Database $database -ErrorAction SilentlyContinue
if (-not $blogMediaFolder) {
    Write-Host "✓ إنشاء مجلد BmcBlog في Media Library" -ForegroundColor Green
    $blogMediaFolder = New-Item -Path $bmcMediaPath -Name "BmcBlog" -ItemType "System/Media/Media folder" -Database $database
}
else {
    Write-Host "✓ مجلد BmcBlog موجود في Media Library" -ForegroundColor Yellow
}

# إنشاء مجلدات فرعية في Media Library
$mediaSubfolders = @("Posts", "Authors", "Categories", "General")
foreach ($subfolder in $mediaSubfolders) {
    $subfolderPath = "$mediaLibraryPath/$subfolder"
    $subfolderItem = Get-Item -Path $subfolderPath -Database $database -ErrorAction SilentlyContinue
    if (-not $subfolderItem) {
        Write-Host "✓ إنشاء مجلد $subfolder في Media Library" -ForegroundColor Green
        New-Item -Path $mediaLibraryPath -Name $subfolder -ItemType "System/Media/Media folder" -Database $database | Out-Null
    }
}

# ====================================
# 8. تحديث معرف مكتبة الوسائط في Site Item
# ====================================
Write-Host ""
Write-Host "8. تحديث معرف مكتبة الوسائط..." -ForegroundColor Cyan

if ($siteItem -and $blogMediaFolder) {
    $siteItem.Editing.BeginEdit()
    try {
        if ($siteItem.Fields["Site Media Library"]) {
            $siteItem.Fields["Site Media Library"].Value = $blogMediaFolder.ID
            Write-Host "✓ تم ربط مكتبة الوسائط بالموقع" -ForegroundColor Green
        }
        $siteItem.Editing.EndEdit() | Out-Null
    }
    catch {
        $siteItem.Editing.CancelEdit()
        Write-Host "✗ خطأ في تحديث معرف مكتبة الوسائط" -ForegroundColor Red
    }
}

# ====================================
# 9. إنشاء Index Configuration Items (اختياري)
# ====================================
Write-Host ""
Write-Host "9. التحقق من إعدادات الفهرسة..." -ForegroundColor Cyan
Write-Host "✓ يُنصح بإعداد Indexes يدوياً في Sitecore Content Search" -ForegroundColor Yellow

# ====================================
# النتيجة النهائية
# ====================================
Write-Host ""
Write-Host "=====================================" -ForegroundColor Green
Write-Host "تم إكمال إنشاء بنية BMC Blog بنجاح!" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green
Write-Host ""

# عرض ملخص بالعناصر المنشأة
Write-Host "ملخص العناصر المنشأة:" -ForegroundColor Cyan
Write-Host "  ✓ موقع BmcBlog: $sitePath" -ForegroundColor White
Write-Host "  ✓ صفحة Home: $homePath" -ForegroundColor White
Write-Host "  ✓ Blog Root: $blogPath" -ForegroundColor White
Write-Host "  ✓ التصنيفات: $($sampleCategories.Count)" -ForegroundColor White
Write-Host "  ✓ الكتّاب: $($sampleAuthors.Count)" -ForegroundColor White
Write-Host "  ✓ المقالات: $($samplePosts.Count)" -ForegroundColor White
Write-Host "  ✓ مكتبة الوسائط: $mediaLibraryPath" -ForegroundColor White
Write-Host ""

Write-Host "الخطوات التالية:" -ForegroundColor Yellow
Write-Host "  1. التحقق من Template IDs في الملف وتحديثها بالقيم الفعلية" -ForegroundColor Gray
Write-Host "  2. إضافة الصور المميزة للمقالات في Media Library" -ForegroundColor Gray
Write-Host "  3. تكوين Publishing targets" -ForegroundColor Gray
Write-Host "  4. اختبار الموقع على: https://abdo.sc" -ForegroundColor Gray
Write-Host "  5. نشر العناصر إلى قاعدة بيانات web" -ForegroundColor Gray
Write-Host ""

# عرض معلومات إضافية
Write-Host "لنشر جميع العناصر، استخدم الأمر التالي:" -ForegroundColor Cyan
Write-Host '  Publish-Item -Path "/sitecore/content/BMC/BmcBlog" -Recurse -PublishMode Full -Target "web"' -ForegroundColor White
Write-Host ""
