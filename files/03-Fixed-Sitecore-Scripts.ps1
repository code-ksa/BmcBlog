# ====================================================================
# BMC Blog - سكريبتات Sitecore المُحدَّثة (بناءً على Diagnostic Results)
# ====================================================================
# التشخيص أظهر:
# - ✗ Layout غير موجود
# - ✗ Templates غير موجودة  
# - ✗ Renderings غير موجودة
# - 393 صفحة بدون Layout
# - 14 عنصر غير منشور
# ====================================================================

Write-Host "=====================================================================" -ForegroundColor Cyan
Write-Host "BMC Blog - Sitecore Setup Scripts (Updated)" -ForegroundColor Cyan
Write-Host "=====================================================================" -ForegroundColor Cyan
Write-Host ""

# ====================================================================
# SCRIPT 1: إنشاء Layout Item (محسّن)
# ====================================================================
# الهدف: إنشاء Blog Layout في المسار الصحيح
# ====================================================================

Write-Host "========================================" -ForegroundColor Yellow
Write-Host "SCRIPT 1: Creating Layout Item" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Yellow
Write-Host ""

# المسارات
$layoutFolderPath = "/sitecore/layout/Layouts"
$bmcFolderPath = "$layoutFolderPath/BMC"
$layoutItemPath = "$bmcFolderPath/Blog Layout"
$layoutFilePath = "~/Views/Layouts/BlogLayout.cshtml"

Write-Host "[+] Checking layout structure..." -ForegroundColor Yellow

# التحقق من وجود Layout folder
$layoutFolder = Get-Item -Path "master:$layoutFolderPath" -ErrorAction SilentlyContinue

if (-not $layoutFolder) {
    Write-Host "[✗] CRITICAL: Layout folder not found!" -ForegroundColor Red
    Write-Host "    This is a core Sitecore folder and should exist." -ForegroundColor Red
    exit
}

Write-Host "[✓] Layout folder exists" -ForegroundColor Green

# إنشاء BMC folder
$bmcFolder = Get-Item -Path "master:$bmcFolderPath" -ErrorAction SilentlyContinue

if (-not $bmcFolder) {
    Write-Host "[+] Creating BMC folder..." -ForegroundColor Yellow
    $bmcFolder = New-Item -Path "master:$layoutFolderPath" -Name "BMC" -ItemType "Common/Folder"
    Write-Host "[✓] BMC folder created: $($bmcFolder.ID)" -ForegroundColor Green
} else {
    Write-Host "[✓] BMC folder already exists" -ForegroundColor Green
}

# التحقق من وجود Layout item
$layoutItem = Get-Item -Path "master:$layoutItemPath" -ErrorAction SilentlyContinue

if ($layoutItem) {
    Write-Host "[!] Layout already exists. Do you want to recreate it? (y/n)" -ForegroundColor Yellow
    $recreate = Read-Host
    
    if ($recreate -eq 'y') {
        Write-Host "[+] Removing old layout..." -ForegroundColor Yellow
        Remove-Item -Path "master:$layoutItemPath" -Permanently
        $layoutItem = $null
    } else {
        Write-Host "[!] Keeping existing layout" -ForegroundColor Yellow
    }
}

if (-not $layoutItem) {
    Write-Host "[+] Creating Blog Layout item..." -ForegroundColor Yellow
    
    # الحصول على Layout template
    $layoutTemplate = Get-Item -Path "master:/sitecore/templates/System/Layout/Layout"
    
    if (-not $layoutTemplate) {
        Write-Host "[✗] Layout template not found!" -ForegroundColor Red
        exit
    }
    
    # إنشاء Layout item
    $layoutItem = New-Item -Path "master:$bmcFolderPath" -Name "Blog Layout" -ItemType $layoutTemplate.ID
    
    if (-not $layoutItem) {
        Write-Host "[✗] Failed to create layout item!" -ForegroundColor Red
        exit
    }
    
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
Write-Host "Layout ID: $($layoutItem.ID)" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# حفظ Layout ID للاستخدام في السكريبتات التالية
$global:BlogLayoutId = $layoutItem.ID

# ====================================================================
# SCRIPT 2: إنشاء Rendering Items (محسّن)
# ====================================================================
# الهدف: إنشاء جميع الـ Renderings المطلوبة
# ====================================================================

Write-Host "========================================" -ForegroundColor Yellow
Write-Host "SCRIPT 2: Creating Rendering Items" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Yellow
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

Write-Host "[✓] Renderings folder exists" -ForegroundColor Green

# إنشاء BMC Renderings folder
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

if (-not $controllerRenderingTemplate) {
    Write-Host "[✗] Controller Rendering template not found!" -ForegroundColor Red
    exit
}

# تعريف الـ Renderings المطلوبة
$renderings = @(
    @{
        Name = "Header"
        Controller = "BmcNavigation"
        Action = "Header"
        Placeholder = "header"
    },
    @{
        Name = "Footer"
        Controller = "BmcNavigation"
        Action = "Footer"
        Placeholder = "footer"
    },
    @{
        Name = "Breadcrumb"
        Controller = "BmcNavigation"
        Action = "Breadcrumb"
        Placeholder = "breadcrumb"
    },
    @{
        Name = "Newsletter Subscribe"
        Controller = "BmcNewsletter"
        Action = "Subscribe"
        Placeholder = "newsletter"
    }
)

$createdRenderings = @{}

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
        
        $createdRenderings[$rendering.Name] = $existingRendering
        Write-Host "[✓] $($rendering.Name) updated" -ForegroundColor Green
    } else {
        Write-Host "[+] Creating $($rendering.Name)..." -ForegroundColor Yellow
        
        $newRendering = New-Item -Path "master:$bmcRenderingsFolderPath" -Name $rendering.Name -ItemType $controllerRenderingTemplate.ID
        
        if (-not $newRendering) {
            Write-Host "[✗] Failed to create $($rendering.Name)!" -ForegroundColor Red
            continue
        }
        
        $newRendering.Editing.BeginEdit()
        $newRendering["Controller"] = $rendering.Controller
        $newRendering["Controller Action"] = $rendering.Action
        $newRendering.Editing.EndEdit() | Out-Null
        
        $createdRenderings[$rendering.Name] = $newRendering
        Write-Host "[✓] $($rendering.Name) created" -ForegroundColor Green
        Write-Host "    ID: $($newRendering.ID)" -ForegroundColor Gray
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Renderings creation completed!" -ForegroundColor Green
Write-Host "Total Renderings: $($createdRenderings.Count)" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# ====================================================================
# SCRIPT 3: ربط Layout بالصفحات (مُحسَّن بشكل كبير)
# ====================================================================
# الهدف: ربط Blog Layout بجميع الصفحات التي تحتاج Layout
# ====================================================================

Write-Host "========================================" -ForegroundColor Yellow
Write-Host "SCRIPT 3: Assigning Layout to Pages" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Yellow
Write-Host ""

# التحقق من وجود Layout
$layoutItem = Get-Item -Path "master:$layoutItemPath" -ErrorAction SilentlyContinue

if (-not $layoutItem) {
    Write-Host "[✗] Layout not found! Please run SCRIPT 1 first!" -ForegroundColor Red
    exit
}

Write-Host "[✓] Layout found: $($layoutItem.ID)" -ForegroundColor Green
Write-Host ""

# الحصول على Device item
$deviceItem = Get-Item -Path "master:/sitecore/layout/Devices/Default" -ErrorAction SilentlyContinue

if (-not $deviceItem) {
    Write-Host "[✗] Default device not found!" -ForegroundColor Red
    exit
}

Write-Host "[✓] Default device found" -ForegroundColor Green
Write-Host ""

# Function لتطبيق Layout على item واحد
function Set-ItemLayout {
    param(
        [Parameter(Mandatory=$true)]
        $Item,
        [Parameter(Mandatory=$true)]
        $LayoutItem,
        [Parameter(Mandatory=$true)]
        $DeviceItem
    )
    
    try {
        # الحصول على Layout Definition الحالية
        $layoutField = $Item.Fields["__Renderings"]
        
        if ($layoutField -eq $null) {
            Write-Host "    [✗] Cannot access layout field" -ForegroundColor Red
            return $false
        }
        
        # إنشاء XML للـ Layout
        $layoutXml = "<r xmlns:xsd=`"http://www.w3.org/2001/XMLSchema`"><d id=`"$($DeviceItem.ID)`" l=`"$($LayoutItem.ID)`" /></r>"
        
        # تطبيق Layout
        $Item.Editing.BeginEdit()
        $Item.Fields["__Renderings"].Value = $layoutXml
        $Item.Editing.EndEdit() | Out-Null
        
        return $true
    }
    catch {
        Write-Host "    [✗] Error: $_" -ForegroundColor Red
        if ($Item.Editing.IsEditing) {
            $Item.Editing.CancelEdit()
        }
        return $false
    }
}

# سؤال المستخدم عن الصفحات المراد تطبيق Layout عليها
Write-Host "Which pages do you want to assign layout to?" -ForegroundColor Cyan
Write-Host "1. Home page only" -ForegroundColor White
Write-Host "2. Home + Direct children (Blog, etc.)" -ForegroundColor White
Write-Host "3. All pages (393 items - RECOMMENDED)" -ForegroundColor White
Write-Host "4. Custom path" -ForegroundColor White
Write-Host ""
$choice = Read-Host "Enter your choice (1-4)"

$pagesToProcess = @()

switch ($choice) {
    "1" {
        # Home page only
        $homePage = Get-Item -Path "master:/sitecore/content/BMC/BmcBlog/Home" -ErrorAction SilentlyContinue
        if ($homePage) {
            $pagesToProcess += $homePage
        }
    }
    "2" {
        # Home + Direct children
        $homePage = Get-Item -Path "master:/sitecore/content/BMC/BmcBlog/Home" -ErrorAction SilentlyContinue
        if ($homePage) {
            $pagesToProcess += $homePage
            $pagesToProcess += Get-ChildItem -Path "master:/sitecore/content/BMC/BmcBlog/Home" -ErrorAction SilentlyContinue
        }
    }
    "3" {
        # All pages
        Write-Host "[+] Getting all items under Home..." -ForegroundColor Yellow
        $allItems = Get-Item -Path "master:/sitecore/content/BMC/BmcBlog/Home" -ErrorAction SilentlyContinue
        if ($allItems) {
            $pagesToProcess += Get-ChildItem -Path "master:/sitecore/content/BMC/BmcBlog/Home" -Recurse -ErrorAction SilentlyContinue
        }
    }
    "4" {
        # Custom path
        $customPath = Read-Host "Enter the Sitecore path (e.g., /sitecore/content/BMC/BmcBlog/Home/Blog)"
        $customItem = Get-Item -Path "master:$customPath" -ErrorAction SilentlyContinue
        if ($customItem) {
            $applyToChildren = Read-Host "Apply to all children too? (y/n)"
            if ($applyToChildren -eq 'y') {
                $pagesToProcess += Get-ChildItem -Path "master:$customPath" -Recurse -ErrorAction SilentlyContinue
            } else {
                $pagesToProcess += $customItem
            }
        }
    }
    default {
        Write-Host "[✗] Invalid choice!" -ForegroundColor Red
        exit
    }
}

if ($pagesToProcess.Count -eq 0) {
    Write-Host "[✗] No pages found to process!" -ForegroundColor Red
    exit
}

Write-Host ""
Write-Host "[+] Found $($pagesToProcess.Count) page(s) to process" -ForegroundColor Yellow
Write-Host "[+] Starting layout assignment..." -ForegroundColor Yellow
Write-Host ""

$successCount = 0
$failCount = 0
$skipCount = 0

foreach ($page in $pagesToProcess) {
    # تخطي العناصر التي ليست صفحات (مثل Folders, Data folders, etc.)
    $skipTemplates = @(
        "Folder",
        "DataFolder", 
        "MediaVirtualFolder",
        "Dictionary Domain",
        "Presentation",
        "Settings"
    )
    
    if ($page.TemplateName -in $skipTemplates) {
        $skipCount++
        continue
    }
    
    Write-Host "[+] Processing: $($page.Name)" -ForegroundColor Gray
    
    $result = Set-ItemLayout -Item $page -LayoutItem $layoutItem -DeviceItem $deviceItem
    
    if ($result) {
        Write-Host "    [✓] Layout assigned" -ForegroundColor Green
        $successCount++
    } else {
        Write-Host "    [✗] Failed" -ForegroundColor Red
        $failCount++
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Layout Assignment Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Total Items: $($pagesToProcess.Count)" -ForegroundColor White
Write-Host "Success: $successCount" -ForegroundColor Green
Write-Host "Failed: $failCount" -ForegroundColor Red
Write-Host "Skipped: $skipCount" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# ====================================================================
# SCRIPT 4: إضافة Renderings إلى الصفحات (محسّن)
# ====================================================================
# الهدف: إضافة Header, Footer, Breadcrumb إلى الصفحات
# ====================================================================

Write-Host "========================================" -ForegroundColor Yellow
Write-Host "SCRIPT 4: Adding Renderings to Pages" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Yellow
Write-Host ""

# التحقق من وجود Renderings
$renderingsToAdd = @("Header", "Footer", "Breadcrumb")
$missingRenderings = @()

foreach ($renderingName in $renderingsToAdd) {
    $rendering = Get-Item -Path "master:$bmcRenderingsFolderPath/$renderingName" -ErrorAction SilentlyContinue
    if (-not $rendering) {
        $missingRenderings += $renderingName
    }
}

if ($missingRenderings.Count -gt 0) {
    Write-Host "[✗] Missing renderings: $($missingRenderings -join ', ')" -ForegroundColor Red
    Write-Host "    Please run SCRIPT 2 first!" -ForegroundColor Red
    exit
}

Write-Host "[✓] All required renderings found" -ForegroundColor Green
Write-Host ""

# Function لإضافة Rendering لـ item
function Add-RenderingToItem {
    param(
        [Parameter(Mandatory=$true)]
        $Item,
        [Parameter(Mandatory=$true)]
        $Rendering,
        [Parameter(Mandatory=$true)]
        $Placeholder,
        [Parameter(Mandatory=$true)]
        $DeviceItem
    )
    
    try {
        # الحصول على Layout الحالي
        $layoutField = $Item.Fields["__Renderings"]
        
        if ($layoutField -eq $null -or [string]::IsNullOrEmpty($layoutField.Value)) {
            Write-Host "    [!] Item has no layout, skipping" -ForegroundColor Yellow
            return $false
        }
        
        # Parse XML الحالي
        [xml]$layoutXml = $layoutField.Value
        
        # البحث عن Device
        $deviceNode = $layoutXml.SelectSingleNode("//d[@id='$($DeviceItem.ID)']")
        
        if ($deviceNode -eq $null) {
            Write-Host "    [!] Device not found in layout" -ForegroundColor Yellow
            return $false
        }
        
        # التحقق من عدم وجود Rendering مسبقاً
        $existingRendering = $deviceNode.SelectSingleNode("r[@id='$($Rendering.ID)']")
        if ($existingRendering -ne $null) {
            Write-Host "    [!] Rendering already exists" -ForegroundColor Yellow
            return $false
        }
        
        # إنشاء Rendering node جديد
        $renderingNode = $layoutXml.CreateElement("r")
        $renderingNode.SetAttribute("id", $Rendering.ID.ToString())
        $renderingNode.SetAttribute("ph", $Placeholder)
        $renderingNode.SetAttribute("uid", [Guid]::NewGuid().ToString())
        
        # إضافة Rendering
        $deviceNode.AppendChild($renderingNode) | Out-Null
        
        # حفظ التعديلات
        $Item.Editing.BeginEdit()
        $Item.Fields["__Renderings"].Value = $layoutXml.OuterXml
        $Item.Editing.EndEdit() | Out-Null
        
        return $true
    }
    catch {
        Write-Host "    [✗] Error: $_" -ForegroundColor Red
        if ($Item.Editing.IsEditing) {
            $Item.Editing.CancelEdit()
        }
        return $false
    }
}

# سؤال المستخدم
Write-Host "Do you want to add renderings to:" -ForegroundColor Cyan
Write-Host "1. Pages that have layout already" -ForegroundColor White
Write-Host "2. Specific page" -ForegroundColor White
Write-Host ""
$renderingChoice = Read-Host "Enter your choice (1-2)"

$itemsToAddRenderings = @()

switch ($renderingChoice) {
    "1" {
        # جميع الصفحات التي لها Layout
        Write-Host "[+] Finding pages with layout..." -ForegroundColor Yellow
        $allItems = Get-ChildItem -Path "master:/sitecore/content/BMC/BmcBlog/Home" -Recurse -ErrorAction SilentlyContinue
        
        foreach ($item in $allItems) {
            $layoutField = $item.Fields["__Renderings"]
            if ($layoutField -ne $null -and ![string]::IsNullOrEmpty($layoutField.Value)) {
                $itemsToAddRenderings += $item
            }
        }
    }
    "2" {
        # صفحة محددة
        $pagePath = Read-Host "Enter page path (e.g., /sitecore/content/BMC/BmcBlog/Home)"
        $page = Get-Item -Path "master:$pagePath" -ErrorAction SilentlyContinue
        if ($page) {
            $itemsToAddRenderings += $page
        }
    }
    default {
        Write-Host "[✗] Invalid choice!" -ForegroundColor Red
        exit
    }
}

if ($itemsToAddRenderings.Count -eq 0) {
    Write-Host "[!] No pages found with layout" -ForegroundColor Yellow
    exit
}

Write-Host ""
Write-Host "[+] Found $($itemsToAddRenderings.Count) page(s)" -ForegroundColor Yellow
Write-Host "[+] Adding renderings..." -ForegroundColor Yellow
Write-Host ""

$renderingsConfig = @(
    @{Name = "Header"; Placeholder = "header"},
    @{Name = "Breadcrumb"; Placeholder = "breadcrumb"},
    @{Name = "Footer"; Placeholder = "footer"}
)

$renderingStats = @{
    TotalItems = $itemsToAddRenderings.Count
    TotalRenderings = 0
    SuccessCount = 0
    FailCount = 0
    SkipCount = 0
}

foreach ($item in $itemsToAddRenderings) {
    Write-Host "[+] Processing: $($item.Name)" -ForegroundColor Gray
    
    foreach ($config in $renderingsConfig) {
        $rendering = Get-Item -Path "master:$bmcRenderingsFolderPath/$($config.Name)" -ErrorAction SilentlyContinue
        
        if ($rendering) {
            $renderingStats.TotalRenderings++
            $result = Add-RenderingToItem -Item $item -Rendering $rendering -Placeholder $config.Placeholder -DeviceItem $deviceItem
            
            if ($result) {
                Write-Host "    [✓] $($config.Name) added" -ForegroundColor Green
                $renderingStats.SuccessCount++
            } elseif ($result -eq $false) {
                $renderingStats.SkipCount++
            } else {
                $renderingStats.FailCount++
            }
        }
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Rendering Assignment Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Total Pages: $($renderingStats.TotalItems)" -ForegroundColor White
Write-Host "Total Rendering Operations: $($renderingStats.TotalRenderings)" -ForegroundColor White
Write-Host "Success: $($renderingStats.SuccessCount)" -ForegroundColor Green
Write-Host "Skipped: $($renderingStats.SkipCount)" -ForegroundColor Yellow
Write-Host "Failed: $($renderingStats.FailCount)" -ForegroundColor Red
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# ====================================================================
# SCRIPT 5: Publish Items (محسّن)
# ====================================================================
# الهدف: نشر جميع التغييرات
# ====================================================================

Write-Host "========================================" -ForegroundColor Yellow
Write-Host "SCRIPT 5: Publishing Items" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Yellow
Write-Host ""

$confirm = Read-Host "Do you want to publish all changes now? (y/n)"

if ($confirm -ne 'y') {
    Write-Host "[!] Publishing skipped" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Setup Complete!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Cyan
    exit
}

Write-Host ""
Write-Host "[+] Starting publish operations..." -ForegroundColor Yellow
Write-Host ""

# نشر Layout
Write-Host "[1/4] Publishing Layout..." -ForegroundColor Yellow
$layoutItem = Get-Item -Path "master:$layoutItemPath" -ErrorAction SilentlyContinue
if ($layoutItem) {
    Publish-Item -Item $layoutItem -Target "web" -PublishMode Full -Recurse
    Write-Host "    [✓] Layout published" -ForegroundColor Green
}

# نشر Renderings
Write-Host "[2/4] Publishing Renderings..." -ForegroundColor Yellow
$renderingsFolder = Get-Item -Path "master:$bmcRenderingsFolderPath" -ErrorAction SilentlyContinue
if ($renderingsFolder) {
    Publish-Item -Item $renderingsFolder -Target "web" -PublishMode Full -Recurse
    Write-Host "    [✓] Renderings published" -ForegroundColor Green
}

# نشر Content
Write-Host "[3/4] Publishing Content..." -ForegroundColor Yellow
$homeFolder = Get-Item -Path "master:/sitecore/content/BMC/BmcBlog/Home" -ErrorAction SilentlyContinue
if ($homeFolder) {
    Publish-Item -Item $homeFolder -Target "web" -PublishMode Full -Recurse
    Write-Host "    [✓] Content published" -ForegroundColor Green
}

# نشر Blog Posts المحددة
Write-Host "[4/4] Publishing specific blog items..." -ForegroundColor Yellow
$blogItems = @(
    "/sitecore/content/BMC/BmcBlog/Home/Blog",
    "/sitecore/content/BMC/BmcBlog/Home/Blog/Authors",
    "/sitecore/content/BMC/BmcBlog/Home/Blog/Categories",
    "/sitecore/content/BMC/BmcBlog/Home/Blog/Posts"
)

foreach ($path in $blogItems) {
    $item = Get-Item -Path "master:$path" -ErrorAction SilentlyContinue
    if ($item) {
        Publish-Item -Item $item -Target "web" -PublishMode Full -Recurse
        Write-Host "    [✓] Published: $($item.Name)" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Publishing completed!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# ====================================================================
# SCRIPT 6: Final Verification (جديد)
# ====================================================================
# الهدف: التحقق النهائي من كل شيء
# ====================================================================

Write-Host "========================================" -ForegroundColor Yellow
Write-Host "SCRIPT 6: Final Verification" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Yellow
Write-Host ""

$verificationResults = @{
    LayoutExists = $false
    RenderingsCount = 0
    PagesWithLayout = 0
    PagesWithRenderings = 0
    PublishedItems = 0
}

# التحقق من Layout
Write-Host "1. Checking Layout..." -ForegroundColor Yellow
$layoutItem = Get-Item -Path "master:$layoutItemPath" -ErrorAction SilentlyContinue
if ($layoutItem) {
    $verificationResults.LayoutExists = $true
    Write-Host "   [✓] Layout exists" -ForegroundColor Green
    
    # التحقق من النشر
    $webLayoutItem = Get-Item -Path "web:$layoutItemPath" -ErrorAction SilentlyContinue
    if ($webLayoutItem) {
        $verificationResults.PublishedItems++
        Write-Host "   [✓] Layout published to web" -ForegroundColor Green
    } else {
        Write-Host "   [!] Layout NOT published to web" -ForegroundColor Yellow
    }
} else {
    Write-Host "   [✗] Layout NOT found" -ForegroundColor Red
}

# التحقق من Renderings
Write-Host "2. Checking Renderings..." -ForegroundColor Yellow
$renderingItems = Get-ChildItem -Path "master:$bmcRenderingsFolderPath" -ErrorAction SilentlyContinue
if ($renderingItems) {
    $verificationResults.RenderingsCount = $renderingItems.Count
    Write-Host "   [✓] Found $($renderingItems.Count) renderings" -ForegroundColor Green
    
    foreach ($rendering in $renderingItems) {
        $webRendering = Get-Item -Path "web:$bmcRenderingsFolderPath/$($rendering.Name)" -ErrorAction SilentlyContinue
        if ($webRendering) {
            $verificationResults.PublishedItems++
        }
    }
} else {
    Write-Host "   [!] No renderings found" -ForegroundColor Yellow
}

# التحقق من الصفحات
Write-Host "3. Checking Pages..." -ForegroundColor Yellow
$allPages = Get-ChildItem -Path "master:/sitecore/content/BMC/BmcBlog/Home" -Recurse -ErrorAction SilentlyContinue

foreach ($page in $allPages) {
    $layoutField = $page.Fields["__Renderings"]
    if ($layoutField -ne $null -and ![string]::IsNullOrEmpty($layoutField.Value)) {
        $verificationResults.PagesWithLayout++
        
        # التحقق من وجود Renderings
        [xml]$xml = $layoutField.Value
        $renderings = $xml.SelectNodes("//r")
        if ($renderings.Count -gt 0) {
            $verificationResults.PagesWithRenderings++
        }
    }
}

Write-Host "   [✓] Pages with layout: $($verificationResults.PagesWithLayout)" -ForegroundColor Green
Write-Host "   [✓] Pages with renderings: $($verificationResults.PagesWithRenderings)" -ForegroundColor Green

# ملخص نهائي
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "FINAL VERIFICATION SUMMARY" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Layout Exists: $(if($verificationResults.LayoutExists){'✓ Yes'}else{'✗ No'})" -ForegroundColor $(if($verificationResults.LayoutExists){'Green'}else{'Red'})
Write-Host "Renderings Created: $($verificationResults.RenderingsCount)" -ForegroundColor $(if($verificationResults.RenderingsCount -gt 0){'Green'}else{'Red'})
Write-Host "Pages with Layout: $($verificationResults.PagesWithLayout)" -ForegroundColor $(if($verificationResults.PagesWithLayout -gt 0){'Green'}else{'Yellow'})
Write-Host "Pages with Renderings: $($verificationResults.PagesWithRenderings)" -ForegroundColor $(if($verificationResults.PagesWithRenderings -gt 0){'Green'}else{'Yellow'})
Write-Host "Published Items: $($verificationResults.PublishedItems)" -ForegroundColor $(if($verificationResults.PublishedItems -gt 0){'Green'}else{'Yellow'})
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

if ($verificationResults.LayoutExists -and $verificationResults.RenderingsCount -gt 0 -and $verificationResults.PagesWithLayout -gt 0) {
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "✓ SETUP COMPLETED SUCCESSFULLY!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next Steps:" -ForegroundColor Cyan
    Write-Host "1. Test your site: http://abdo.sc/home" -ForegroundColor White
    Write-Host "2. Check Experience Editor" -ForegroundColor White
    Write-Host "3. Verify renderings are showing" -ForegroundColor White
    Write-Host ""
} else {
    Write-Host "========================================" -ForegroundColor Yellow
    Write-Host "! SETUP INCOMPLETE" -ForegroundColor Yellow
    Write-Host "========================================" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Please review the results above and:" -ForegroundColor Yellow
    Write-Host "1. Re-run any failed scripts" -ForegroundColor White
    Write-Host "2. Check Sitecore logs for errors" -ForegroundColor White
    Write-Host "3. Verify file permissions" -ForegroundColor White
    Write-Host ""
}

# ====================================================================
# END OF SCRIPTS
# ====================================================================
