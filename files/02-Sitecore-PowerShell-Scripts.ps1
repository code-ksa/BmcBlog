# ====================================================================
# BMC Blog - Sitecore PowerShell Scripts (خطوة بخطوة)
# ====================================================================
# هذا الملف يحتوي على جميع السكريبتات مرتبة بالتسلسل الصحيح
# استخدم كل سكريبت على حدة من داخل Sitecore PowerShell ISE
# ====================================================================

# ====================================================================
# SCRIPT 1: إنشاء Layout Item في Sitecore
# ====================================================================
# الغرض: إنشاء Blog Layout item وربطه بملف BlogLayout.cshtml
# متى نستخدمه: قبل أي شيء آخر
# ====================================================================

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "STEP 1: Creating Layout Item" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# المسارات
$layoutFolderPath = "/sitecore/layout/Layouts"
$bmcFolderPath = "$layoutFolderPath/BMC"
$layoutItemPath = "$bmcFolderPath/Blog Layout"
$layoutFilePath = "~/Views/Layouts/BlogLayout.cshtml"

# التحقق من وجود Layout folder
$layoutFolder = Get-Item -Path "master:$layoutFolderPath" -ErrorAction SilentlyContinue

if (-not $layoutFolder) {
    Write-Host "[✗] Layout folder not found!" -ForegroundColor Red
    exit
}

# إنشاء BMC folder إذا لم يكن موجوداً
$bmcFolder = Get-Item -Path "master:$bmcFolderPath" -ErrorAction SilentlyContinue

if (-not $bmcFolder) {
    Write-Host "[+] Creating BMC folder..." -ForegroundColor Yellow
    $bmcFolder = New-Item -Path "master:$layoutFolderPath" -Name "BMC" -ItemType "Common/Folder"
    Write-Host "[✓] BMC folder created" -ForegroundColor Green
} else {
    Write-Host "[✓] BMC folder exists" -ForegroundColor Green
}

# التحقق من وجود Layout item
$layoutItem = Get-Item -Path "master:$layoutItemPath" -ErrorAction SilentlyContinue

if ($layoutItem) {
    Write-Host "[!] Layout already exists. Updating..." -ForegroundColor Yellow
    
    # تحديث Path
    $layoutItem.Editing.BeginEdit()
    $layoutItem["Path"] = $layoutFilePath
    $layoutItem.Editing.EndEdit() | Out-Null
    
    Write-Host "[✓] Layout updated" -ForegroundColor Green
} else {
    Write-Host "[+] Creating Blog Layout item..." -ForegroundColor Yellow
    
    # الحصول على Layout template
    $layoutTemplate = Get-Item -Path "master:/sitecore/templates/System/Layout/Layout"
    
    # إنشاء Layout item
    $layoutItem = New-Item -Path "master:$bmcFolderPath" -Name "Blog Layout" -ItemType $layoutTemplate.ID
    
    # تعيين Path
    $layoutItem.Editing.BeginEdit()
    $layoutItem["Path"] = $layoutFilePath
    $layoutItem.Editing.EndEdit() | Out-Null
    
    Write-Host "[✓] Blog Layout created successfully!" -ForegroundColor Green
    Write-Host "    ID: $($layoutItem.ID)" -ForegroundColor Gray
    Write-Host "    Path: $layoutFilePath" -ForegroundColor Gray
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Layout creation completed!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan

# ====================================================================
# SCRIPT 2: إنشاء Rendering Items
# ====================================================================
# الغرض: إنشاء Controller Renderings للمكونات (Header, Footer, etc.)
# متى نستخدمه: بعد إنشاء Layout
# ====================================================================

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "STEP 2: Creating Rendering Items" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# المسارات
$renderingsFolderPath = "/sitecore/layout/Renderings"
$bmcRenderingsFolderPath = "$renderingsFolderPath/BMC"

# التحقق من وجود Renderings folder
$renderingsFolder = Get-Item -Path "master:$renderingsFolderPath" -ErrorAction SilentlyContinue

if (-not $renderingsFolder) {
    Write-Host "[✗] Renderings folder not found!" -ForegroundColor Red
    exit
}

# إنشاء BMC folder للـ Renderings
$bmcRenderingsFolder = Get-Item -Path "master:$bmcRenderingsFolderPath" -ErrorAction SilentlyContinue

if (-not $bmcRenderingsFolder) {
    Write-Host "[+] Creating BMC Renderings folder..." -ForegroundColor Yellow
    $bmcRenderingsFolder = New-Item -Path "master:$renderingsFolderPath" -Name "BMC" -ItemType "Common/Folder"
    Write-Host "[✓] BMC Renderings folder created" -ForegroundColor Green
} else {
    Write-Host "[✓] BMC Renderings folder exists" -ForegroundColor Green
}

# الحصول على Controller Rendering template
$controllerRenderingTemplate = Get-Item -Path "master:/sitecore/templates/System/Layout/Renderings/Controller rendering"

# تعريف الـ Renderings المطلوبة
$renderings = @(
    @{
        Name = "Header"
        Controller = "BmcNavigation"
        Action = "Header"
    },
    @{
        Name = "Footer"
        Controller = "BmcNavigation"
        Action = "Footer"
    },
    @{
        Name = "Breadcrumb"
        Controller = "BmcNavigation"
        Action = "Breadcrumb"
    },
    @{
        Name = "Newsletter Subscribe"
        Controller = "BmcNewsletter"
        Action = "Subscribe"
    }
)

# إنشاء كل Rendering
foreach ($rendering in $renderings) {
    $renderingPath = "$bmcRenderingsFolderPath/$($rendering.Name)"
    $existingRendering = Get-Item -Path "master:$renderingPath" -ErrorAction SilentlyContinue
    
    if ($existingRendering) {
        Write-Host "[!] $($rendering.Name) already exists. Updating..." -ForegroundColor Yellow
        
        $existingRendering.Editing.BeginEdit()
        $existingRendering["Controller"] = $rendering.Controller
        $existingRendering["Controller Action"] = $rendering.Action
        $existingRendering.Editing.EndEdit() | Out-Null
        
        Write-Host "[✓] $($rendering.Name) updated" -ForegroundColor Green
    } else {
        Write-Host "[+] Creating $($rendering.Name)..." -ForegroundColor Yellow
        
        $newRendering = New-Item -Path "master:$bmcRenderingsFolderPath" -Name $rendering.Name -ItemType $controllerRenderingTemplate.ID
        
        $newRendering.Editing.BeginEdit()
        $newRendering["Controller"] = $rendering.Controller
        $newRendering["Controller Action"] = $rendering.Action
        $newRendering.Editing.EndEdit() | Out-Null
        
        Write-Host "[✓] $($rendering.Name) created" -ForegroundColor Green
        Write-Host "    ID: $($newRendering.ID)" -ForegroundColor Gray
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Renderings creation completed!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan

# ====================================================================
# SCRIPT 3: ربط Layout بـ Home Page
# ====================================================================
# الغرض: ربط Blog Layout بصفحة Home
# متى نستخدمه: بعد إنشاء Layout و Renderings
# ====================================================================

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "STEP 3: Assigning Layout to Home Page" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# المسارات
$homePagePath = "/sitecore/content/BMC/BmcBlog/Home"
$layoutItemPath = "/sitecore/layout/Layouts/BMC/Blog Layout"

# الحصول على الصفحة
$homePage = Get-Item -Path "master:$homePagePath" -ErrorAction SilentlyContinue

if (-not $homePage) {
    Write-Host "[✗] Home page not found at: $homePagePath" -ForegroundColor Red
    exit
}

# الحصول على Layout
$layoutItem = Get-Item -Path "master:$layoutItemPath" -ErrorAction SilentlyContinue

if (-not $layoutItem) {
    Write-Host "[✗] Layout not found. Please run SCRIPT 1 first!" -ForegroundColor Red
    exit
}

Write-Host "[+] Assigning layout to Home page..." -ForegroundColor Yellow

# الحصول على shared layout field
$sharedLayoutField = $homePage.Fields["__Renderings"]
$finalLayoutField = $homePage.Fields["__Final Renderings"]

# إنشاء Layout definition
$layoutDefinition = New-Object Sitecore.Data.Fields.LayoutField $sharedLayoutField

# إنشاء Device definition
$deviceItem = Get-Item -Path "master:/sitecore/layout/Devices/Default"
$device = $layoutDefinition.GetDefinition().GetDevice($deviceItem.ID.ToString())

if ($device -eq $null) {
    $device = New-Object Sitecore.Data.Fields.DeviceDefinition
    $device.ID = $deviceItem.ID.ToString()
}

# تعيين Layout
$device.Layout = $layoutItem.ID.ToString()

# حفظ التعديلات
$homePage.Editing.BeginEdit()
try {
    $layoutDefinition.Value = $device.ToString()
    $homePage.Editing.EndEdit() | Out-Null
    Write-Host "[✓] Layout assigned successfully!" -ForegroundColor Green
} catch {
    $homePage.Editing.CancelEdit()
    Write-Host "[✗] Error: $_" -ForegroundColor Red
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Layout assignment completed!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan

# ====================================================================
# SCRIPT 4: إضافة Renderings إلى Home Page
# ====================================================================
# الغرض: إضافة Header, Footer, Breadcrumb إلى صفحة Home
# متى نستخدمه: بعد ربط Layout
# ====================================================================

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "STEP 4: Adding Renderings to Home Page" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# المسارات
$homePagePath = "/sitecore/content/BMC/BmcBlog/Home"
$renderingsBasePath = "/sitecore/layout/Renderings/BMC"

# الحصول على الصفحة
$homePage = Get-Item -Path "master:$homePagePath"

# تعريف الـ Renderings المطلوب إضافتها مع Placeholders
$renderingsToAdd = @(
    @{
        Name = "Header"
        Placeholder = "header"
    },
    @{
        Name = "Breadcrumb"
        Placeholder = "breadcrumb"
    },
    @{
        Name = "Footer"
        Placeholder = "footer"
    }
)

Write-Host "[+] Adding renderings to Home page..." -ForegroundColor Yellow

foreach ($renderingInfo in $renderingsToAdd) {
    $renderingPath = "$renderingsBasePath/$($renderingInfo.Name)"
    $rendering = Get-Item -Path "master:$renderingPath" -ErrorAction SilentlyContinue
    
    if (-not $rendering) {
        Write-Host "[✗] Rendering not found: $($renderingInfo.Name)" -ForegroundColor Red
        continue
    }
    
    Write-Host "[+] Adding $($renderingInfo.Name) to placeholder: $($renderingInfo.Placeholder)..." -ForegroundColor Yellow
    
    # إضافة Rendering باستخدام Add-Rendering cmdlet
    Add-Rendering -Item $homePage -PlaceHolder $renderingInfo.Placeholder -Rendering $rendering -Parameter @{} | Out-Null
    
    Write-Host "[✓] $($renderingInfo.Name) added successfully" -ForegroundColor Green
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Renderings added successfully!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan

# ====================================================================
# SCRIPT 5: نسخ إعدادات Presentation لصفحات أخرى
# ====================================================================
# الغرض: نسخ Layout و Renderings من Home إلى صفحات أخرى
# متى نستخدمه: بعد إعداد Home page بالكامل
# ====================================================================

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "STEP 5: Copying Presentation to Other Pages" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# المسارات
$homePagePath = "/sitecore/content/BMC/BmcBlog/Home"
$blogPagePath = "/sitecore/content/BMC/BmcBlog/Home/Blog"

# الحصول على الصفحات
$homePage = Get-Item -Path "master:$homePagePath"
$blogPage = Get-Item -Path "master:$blogPagePath" -ErrorAction SilentlyContinue

if (-not $blogPage) {
    Write-Host "[✗] Blog page not found" -ForegroundColor Red
    exit
}

Write-Host "[+] Copying presentation from Home to Blog page..." -ForegroundColor Yellow

# نسخ Shared Layout
$sharedLayout = $homePage.Fields["__Renderings"].Value
$blogPage.Editing.BeginEdit()
$blogPage.Fields["__Renderings"].Value = $sharedLayout
$blogPage.Editing.EndEdit() | Out-Null

Write-Host "[✓] Presentation copied successfully!" -ForegroundColor Green

# نسخ لجميع الصفحات الفرعية (اختياري)
$applyToAllPages = Read-Host "Do you want to apply to ALL pages under Blog? (y/n)"

if ($applyToAllPages -eq 'y') {
    Write-Host "[+] Applying to all pages..." -ForegroundColor Yellow
    
    $allPages = Get-ChildItem -Path "master:/sitecore/content/BMC/BmcBlog/Home" -Recurse | Where-Object { $_.TemplateID -ne "{folder-template-id}" }
    
    foreach ($page in $allPages) {
        if ($page.ID -ne $homePage.ID) {
            Write-Host "  [+] Processing: $($page.Name)..." -ForegroundColor Gray
            
            $page.Editing.BeginEdit()
            $page.Fields["__Renderings"].Value = $sharedLayout
            $page.Editing.EndEdit() | Out-Null
        }
    }
    
    Write-Host "[✓] Applied to all pages!" -ForegroundColor Green
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Presentation copy completed!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan

# ====================================================================
# SCRIPT 6: Publish All Changes
# ====================================================================
# الغرض: نشر جميع التغييرات إلى Web database
# متى نستخدمه: بعد الانتهاء من جميع التغييرات
# ====================================================================

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "STEP 6: Publishing Changes" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$confirm = Read-Host "Do you want to publish all changes now? (y/n)"

if ($confirm -eq 'y') {
    Write-Host "[+] Publishing Layout items..." -ForegroundColor Yellow
    
    $layoutItem = Get-Item -Path "master:/sitecore/layout/Layouts/BMC/Blog Layout"
    Publish-Item -Item $layoutItem -Recurse -Target "web"
    
    Write-Host "[+] Publishing Rendering items..." -ForegroundColor Yellow
    
    $renderingsFolder = Get-Item -Path "master:/sitecore/layout/Renderings/BMC"
    Publish-Item -Item $renderingsFolder -Recurse -Target "web"
    
    Write-Host "[+] Publishing content items..." -ForegroundColor Yellow
    
    $homeFolder = Get-Item -Path "master:/sitecore/content/BMC/BmcBlog/Home"
    Publish-Item -Item $homeFolder -Recurse -Target "web"
    
    Write-Host "[✓] All items published successfully!" -ForegroundColor Green
} else {
    Write-Host "[!] Publishing skipped" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Publishing completed!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan

# ====================================================================
# SCRIPT 7: Verification Script
# ====================================================================
# الغرض: التحقق من أن كل شيء تم إعداده بشكل صحيح
# متى نستخدمه: في النهاية للتأكد من كل شيء
# ====================================================================

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "STEP 7: Verification" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$issues = @()

# التحقق من Layout
Write-Host "1. Checking Layout..." -ForegroundColor Yellow
$layoutItem = Get-Item -Path "master:/sitecore/layout/Layouts/BMC/Blog Layout" -ErrorAction SilentlyContinue
if ($layoutItem) {
    Write-Host "[✓] Layout exists" -ForegroundColor Green
} else {
    $issues += "Layout not found"
    Write-Host "[✗] Layout not found" -ForegroundColor Red
}

# التحقق من Renderings
Write-Host "2. Checking Renderings..." -ForegroundColor Yellow
$renderingNames = @("Header", "Footer", "Breadcrumb", "Newsletter Subscribe")
foreach ($name in $renderingNames) {
    $rendering = Get-Item -Path "master:/sitecore/layout/Renderings/BMC/$name" -ErrorAction SilentlyContinue
    if ($rendering) {
        Write-Host "[✓] $name exists" -ForegroundColor Green
    } else {
        $issues += "$name rendering not found"
        Write-Host "[✗] $name not found" -ForegroundColor Red
    }
}

# التحقق من Presentation في Home
Write-Host "3. Checking Home Page Presentation..." -ForegroundColor Yellow
$homePage = Get-Item -Path "master:/sitecore/content/BMC/BmcBlog/Home"
$hasLayout = $homePage.Fields["__Renderings"].Value
if ($hasLayout) {
    Write-Host "[✓] Home page has presentation" -ForegroundColor Green
} else {
    $issues += "Home page has no presentation"
    Write-Host "[✗] Home page has no presentation" -ForegroundColor Red
}

# التحقق من Publishing
Write-Host "4. Checking Publishing Status..." -ForegroundColor Yellow
$webLayoutItem = Get-Item -Path "web:/sitecore/layout/Layouts/BMC/Blog Layout" -ErrorAction SilentlyContinue
if ($webLayoutItem) {
    Write-Host "[✓] Layout published to web" -ForegroundColor Green
} else {
    $issues += "Layout not published"
    Write-Host "[!] Layout not published to web" -ForegroundColor Yellow
}

# ملخص
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "VERIFICATION SUMMARY" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

if ($issues.Count -eq 0) {
    Write-Host "[✓] All checks passed!" -ForegroundColor Green
    Write-Host "Your blog is ready to use!" -ForegroundColor Green
} else {
    Write-Host "[✗] Found $($issues.Count) issue(s):" -ForegroundColor Red
    foreach ($issue in $issues) {
        Write-Host "  - $issue" -ForegroundColor Red
    }
    Write-Host ""
    Write-Host "Please fix the issues and run verification again." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Setup Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan

# ====================================================================
# END OF SCRIPTS
# ====================================================================
