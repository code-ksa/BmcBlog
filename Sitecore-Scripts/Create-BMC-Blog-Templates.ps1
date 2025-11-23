<#
.SYNOPSIS
    Creates BMC Blog templates in Sitecore

.DESCRIPTION
    This script creates all required templates for the BMC Blog feature including:
    - Blog Post template
    - Blog Root template
    - Category template
    - Author template
    - Site template

.NOTES
    Author: BMC Blog Team
    Date: 2025-11-23
    Version: 1.0

.EXAMPLE
    # Run in Sitecore PowerShell ISE
    .\Create-BMC-Blog-Templates.ps1
#>

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "BMC Blog Templates Creator" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

$database = Get-Database "master"

# مسارات القوالب
$projectTemplatePath = "/sitecore/templates/Project/BMC"
$featureTemplatePath = "/sitecore/templates/Feature/BMC/Blog"

# دالة لإنشاء Template
function Create-Template {
    param(
        [string]$Path,
        [string]$Name,
        [string]$DisplayName,
        [string]$BaseTemplate = "/sitecore/templates/System/Templates/Standard template"
    )

    $parentPath = Split-Path $Path -Parent
    $templateName = Split-Path $Path -Leaf

    # التحقق من وجود Template
    $existingTemplate = Get-Item -Path $Path -Database $database -ErrorAction SilentlyContinue

    if ($existingTemplate) {
        Write-Host "✓ Template موجود مسبقاً: $Name" -ForegroundColor Yellow
        return $existingTemplate
    }

    # التحقق من المجلد الأب
    $parentFolder = Get-Item -Path $parentPath -Database $database -ErrorAction SilentlyContinue
    if (-not $parentFolder) {
        Write-Host "✗ المجلد الأب غير موجود: $parentPath" -ForegroundColor Red
        return $null
    }

    # إنشاء Template
    Write-Host "✓ إنشاء Template: $Name" -ForegroundColor Green
    $template = New-Item -Path $parentPath -Name $templateName -ItemType "/sitecore/templates/System/Templates/Template" -Database $database

    # تعيين Base Template
    if ($template) {
        $template.Editing.BeginEdit()
        try {
            $template.Fields["__Base template"].Value = $BaseTemplate
            if ($DisplayName) {
                $template.Fields["__Display name"].Value = $DisplayName
            }
            $template.Editing.EndEdit() | Out-Null
        }
        catch {
            $template.Editing.CancelEdit()
        }
    }

    return $template
}

# دالة لإنشاء Template Section
function Create-TemplateSection {
    param(
        [object]$Template,
        [string]$SectionName
    )

    $sectionPath = "$($Template.Paths.FullPath)/$SectionName"
    $section = Get-Item -Path $sectionPath -Database $database -ErrorAction SilentlyContinue

    if ($section) {
        Write-Host "  └─ Section موجود: $SectionName" -ForegroundColor Gray
        return $section
    }

    Write-Host "  └─ إنشاء Section: $SectionName" -ForegroundColor Green
    $section = New-Item -Path $Template.Paths.FullPath -Name $SectionName -ItemType "/sitecore/templates/System/Templates/Template section" -Database $database

    return $section
}

# دالة لإنشاء Template Field
function Create-TemplateField {
    param(
        [object]$Section,
        [string]$FieldName,
        [string]$FieldType = "Single-Line Text",
        [string]$DisplayName = ""
    )

    $fieldPath = "$($Section.Paths.FullPath)/$FieldName"
    $field = Get-Item -Path $fieldPath -Database $database -ErrorAction SilentlyContinue

    if ($field) {
        Write-Host "    └─ Field موجود: $FieldName" -ForegroundColor Gray
        return $field
    }

    Write-Host "    └─ إنشاء Field: $FieldName ($FieldType)" -ForegroundColor Green
    $field = New-Item -Path $Section.Paths.FullPath -Name $FieldName -ItemType "/sitecore/templates/System/Templates/Template field" -Database $database

    if ($field) {
        $field.Editing.BeginEdit()
        try {
            $field.Fields["Type"].Value = $FieldType
            if ($DisplayName) {
                $field.Fields["__Display name"].Value = $DisplayName
            }
            $field.Editing.EndEdit() | Out-Null
        }
        catch {
            $field.Editing.CancelEdit()
        }
    }

    return $field
}

# ====================================
# 1. إنشاء مجلدات Templates
# ====================================
Write-Host "1. إنشاء مجلدات Templates..." -ForegroundColor Cyan

# مجلد Project/BMC
$projectBmcPath = "/sitecore/templates/Project/BMC"
$projectBmcFolder = Get-Item -Path $projectBmcPath -Database $database -ErrorAction SilentlyContinue
if (-not $projectBmcFolder) {
    $projectFolder = Get-Item -Path "/sitecore/templates/Project" -Database $database -ErrorAction SilentlyContinue
    if (-not $projectFolder) {
        New-Item -Path "/sitecore/templates" -Name "Project" -ItemType "/sitecore/templates/System/Templates/Template folder" -Database $database | Out-Null
    }
    $projectBmcFolder = New-Item -Path "/sitecore/templates/Project" -Name "BMC" -ItemType "/sitecore/templates/System/Templates/Template folder" -Database $database
    Write-Host "✓ إنشاء مجلد: Project/BMC" -ForegroundColor Green
}

# مجلد Feature/BMC/Blog
$featureBmcPath = "/sitecore/templates/Feature/BMC"
$featureBmcFolder = Get-Item -Path $featureBmcPath -Database $database -ErrorAction SilentlyContinue
if (-not $featureBmcFolder) {
    $featureFolder = Get-Item -Path "/sitecore/templates/Feature" -Database $database -ErrorAction SilentlyContinue
    if (-not $featureFolder) {
        New-Item -Path "/sitecore/templates" -Name "Feature" -ItemType "/sitecore/templates/System/Templates/Template folder" -Database $database | Out-Null
    }
    $featureBmcFolder = New-Item -Path "/sitecore/templates/Feature" -Name "BMC" -ItemType "/sitecore/templates/System/Templates/Template folder" -Database $database
    Write-Host "✓ إنشاء مجلد: Feature/BMC" -ForegroundColor Green
}

$featureBlogPath = "/sitecore/templates/Feature/BMC/Blog"
$featureBlogFolder = Get-Item -Path $featureBlogPath -Database $database -ErrorAction SilentlyContinue
if (-not $featureBlogFolder) {
    $featureBlogFolder = New-Item -Path $featureBmcPath -Name "Blog" -ItemType "/sitecore/templates/System/Templates/Template folder" -Database $database
    Write-Host "✓ إنشاء مجلد: Feature/BMC/Blog" -ForegroundColor Green
}

Write-Host ""

# ====================================
# 2. إنشاء Site Template
# ====================================
Write-Host "2. إنشاء Site Template..." -ForegroundColor Cyan

$siteTemplate = Create-Template -Path "$projectBmcPath/Site" -Name "Site" -DisplayName "Site"

if ($siteTemplate) {
    $siteSection = Create-TemplateSection -Template $siteTemplate -SectionName "Site Settings"

    if ($siteSection) {
        Create-TemplateField -Section $siteSection -FieldName "Site Media Library" -FieldType "Droptree" -DisplayName "مكتبة الوسائط - Site Media Library"
        Create-TemplateField -Section $siteSection -FieldName "Host Name" -FieldType "Single-Line Text" -DisplayName "اسم المضيف - Host Name"
        Create-TemplateField -Section $siteSection -FieldName "Site Title" -FieldType "Single-Line Text" -DisplayName "عنوان الموقع - Site Title"
    }
}

Write-Host ""

# ====================================
# 3. إنشاء Blog Root Template
# ====================================
Write-Host "3. إنشاء Blog Root Template..." -ForegroundColor Cyan

$blogRootTemplate = Create-Template -Path "$featureBlogPath/Blog Root" -Name "Blog Root" -DisplayName "Blog Root"

if ($blogRootTemplate) {
    $blogRootSection = Create-TemplateSection -Template $blogRootTemplate -SectionName "Blog Root Settings"

    if ($blogRootSection) {
        Create-TemplateField -Section $blogRootSection -FieldName "Title" -FieldType "Single-Line Text" -DisplayName "العنوان - Title"
        Create-TemplateField -Section $blogRootSection -FieldName "Description" -FieldType "Multi-Line Text" -DisplayName "الوصف - Description"
    }
}

Write-Host ""

# ====================================
# 4. إنشاء Blog Post Template
# ====================================
Write-Host "4. إنشاء Blog Post Template..." -ForegroundColor Cyan

$blogPostTemplate = Create-Template -Path "$featureBlogPath/Blog Post" -Name "Blog Post" -DisplayName "مقالة المدونة - Blog Post"

if ($blogPostTemplate) {
    # Content Section
    $contentSection = Create-TemplateSection -Template $blogPostTemplate -SectionName "Content"

    if ($contentSection) {
        Create-TemplateField -Section $contentSection -FieldName "Title" -FieldType "Single-Line Text" -DisplayName "العنوان - Title"
        Create-TemplateField -Section $contentSection -FieldName "Content" -FieldType "Rich Text" -DisplayName "المحتوى - Content"
        Create-TemplateField -Section $contentSection -FieldName "Short Description" -FieldType "Multi-Line Text" -DisplayName "الملخص - Summary"
        Create-TemplateField -Section $contentSection -FieldName "Featured Image" -FieldType "Image" -DisplayName "الصورة البارزة - Featured Image"
    }

    # Metadata Section
    $metadataSection = Create-TemplateSection -Template $blogPostTemplate -SectionName "Metadata"

    if ($metadataSection) {
        Create-TemplateField -Section $metadataSection -FieldName "Publish Date" -FieldType "Datetime" -DisplayName "تاريخ النشر - Publish Date"
        Create-TemplateField -Section $metadataSection -FieldName "Author" -FieldType "Droptree" -DisplayName "الكاتب - Author"
        Create-TemplateField -Section $metadataSection -FieldName "Category" -FieldType "Droptree" -DisplayName "التصنيف - Category"
        Create-TemplateField -Section $metadataSection -FieldName "Tags" -FieldType "Multilist" -DisplayName "الوسوم - Tags"
        Create-TemplateField -Section $metadataSection -FieldName "View Count" -FieldType "Integer" -DisplayName "عدد المشاهدات - View Count"
    }
}

Write-Host ""

# ====================================
# 5. إنشاء Category Template
# ====================================
Write-Host "5. إنشاء Category Template..." -ForegroundColor Cyan

$categoryTemplate = Create-Template -Path "$featureBlogPath/Category" -Name "Category" -DisplayName "التصنيف - Category"

if ($categoryTemplate) {
    $categorySection = Create-TemplateSection -Template $categoryTemplate -SectionName "Category Information"

    if ($categorySection) {
        Create-TemplateField -Section $categorySection -FieldName "Category Name" -FieldType "Single-Line Text" -DisplayName "اسم التصنيف - Category Name"
        Create-TemplateField -Section $categorySection -FieldName "Category Description" -FieldType "Multi-Line Text" -DisplayName "وصف التصنيف - Description"
        Create-TemplateField -Section $categorySection -FieldName "Category Icon" -FieldType "Image" -DisplayName "أيقونة التصنيف - Icon"
    }
}

Write-Host ""

# ====================================
# 6. إنشاء Author Template
# ====================================
Write-Host "6. إنشاء Author Template..." -ForegroundColor Cyan

$authorTemplate = Create-Template -Path "$featureBlogPath/Author" -Name "Author" -DisplayName "الكاتب - Author"

if ($authorTemplate) {
    $authorSection = Create-TemplateSection -Template $authorTemplate -SectionName "Author Information"

    if ($authorSection) {
        Create-TemplateField -Section $authorSection -FieldName "Author Name" -FieldType "Single-Line Text" -DisplayName "اسم الكاتب - Author Name"
        Create-TemplateField -Section $authorSection -FieldName "Biography" -FieldType "Multi-Line Text" -DisplayName "السيرة الذاتية - Biography"
        Create-TemplateField -Section $authorSection -FieldName "Profile Image" -FieldType "Image" -DisplayName "صورة الملف الشخصي - Profile Image"
        Create-TemplateField -Section $authorSection -FieldName "Email" -FieldType "Single-Line Text" -DisplayName "البريد الإلكتروني - Email"
        Create-TemplateField -Section $authorSection -FieldName "Twitter Handle" -FieldType "Single-Line Text" -DisplayName "حساب تويتر - Twitter"
        Create-TemplateField -Section $authorSection -FieldName "LinkedIn Profile" -FieldType "General Link" -DisplayName "حساب لينكدإن - LinkedIn"
    }
}

Write-Host ""

# ====================================
# 7. إنشاء Blog Listing Template
# ====================================
Write-Host "7. إنشاء Blog Listing Template..." -ForegroundColor Cyan

$blogListingTemplate = Create-Template -Path "$featureBlogPath/Blog Listing" -Name "Blog Listing" -DisplayName "قائمة المدونة - Blog Listing"

if ($blogListingTemplate) {
    $listingSection = Create-TemplateSection -Template $blogListingTemplate -SectionName "Listing Settings"

    if ($listingSection) {
        Create-TemplateField -Section $listingSection -FieldName "Page Title" -FieldType "Single-Line Text" -DisplayName "عنوان الصفحة - Page Title"
        Create-TemplateField -Section $listingSection -FieldName "Items Per Page" -FieldType "Integer" -DisplayName "عدد المقالات في الصفحة - Items Per Page"
        Create-TemplateField -Section $listingSection -FieldName "Show Categories" -FieldType "Checkbox" -DisplayName "إظهار التصنيفات - Show Categories"
    }
}

Write-Host ""

# ====================================
# النتيجة النهائية
# ====================================
Write-Host "=====================================" -ForegroundColor Green
Write-Host "تم إنشاء جميع Templates بنجاح!" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green
Write-Host ""

Write-Host "ملخص Templates المنشأة:" -ForegroundColor Cyan
Write-Host "  ✓ Site Template" -ForegroundColor White
Write-Host "  ✓ Blog Root Template" -ForegroundColor White
Write-Host "  ✓ Blog Post Template" -ForegroundColor White
Write-Host "  ✓ Category Template" -ForegroundColor White
Write-Host "  ✓ Author Template" -ForegroundColor White
Write-Host "  ✓ Blog Listing Template" -ForegroundColor White
Write-Host ""

Write-Host "الخطوات التالية:" -ForegroundColor Yellow
Write-Host "  1. مراجعة Templates في Content Editor" -ForegroundColor Gray
Write-Host "  2. تشغيل Update-Template-IDs.ps1 لاستخراج IDs" -ForegroundColor Gray
Write-Host "  3. تشغيل Create-BMC-Blog-Structure.ps1 لإنشاء المحتوى" -ForegroundColor Gray
Write-Host "  4. نشر Templates إلى قاعدة بيانات web" -ForegroundColor Gray
Write-Host ""

Write-Host "لنشر Templates، استخدم:" -ForegroundColor Cyan
Write-Host '  Publish-Item -Path "/sitecore/templates/Project/BMC" -Recurse -PublishMode Full' -ForegroundColor White
Write-Host '  Publish-Item -Path "/sitecore/templates/Feature/BMC/Blog" -Recurse -PublishMode Full' -ForegroundColor White
Write-Host ""
