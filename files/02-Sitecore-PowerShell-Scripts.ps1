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
# SCRIPT 3: Assign Layout to Pages (Fixed & Optimized)
# ====================================================================

Write-Host ""
Write-Host "========================================" -ForegroundColor Yellow
Write-Host "SCRIPT 3: Assigning Layout to Pages" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Yellow
Write-Host ""

# Configuration
$layoutPath = "/sitecore/layout/Layouts/BMC/Blog Layout"
$sitePath = "/sitecore/content/BMC/BmcBlog"

# Check Layout
Write-Host "[1/4] Checking requirements..." -ForegroundColor Cyan
$layoutItem = Get-Item -Path "master:$layoutPath" -ErrorAction SilentlyContinue
if (-not $layoutItem) {
    Write-Host "[✗] Layout not found! Run SCRIPT 1 first." -ForegroundColor Red
    exit
}
Write-Host "    [✓] Layout found: $($layoutItem.ID)" -ForegroundColor Green

# Check Device
$defaultDevice = Get-Item -Path "master:/sitecore/layout/Devices/Default" -ErrorAction SilentlyContinue
if (-not $defaultDevice) {
    Write-Host "[✗] Default device not found!" -ForegroundColor Red
    exit
}
Write-Host "    [✓] Default device found" -ForegroundColor Green

# Check Site
$siteRoot = Get-Item -Path "master:$sitePath" -ErrorAction SilentlyContinue
if (-not $siteRoot) {
    Write-Host "[✗] Site root not found!" -ForegroundColor Red
    exit
}
Write-Host "    [✓] Site root found" -ForegroundColor Green
Write-Host ""

# Get items to process
Write-Host "[2/4] Select pages:" -ForegroundColor Cyan
Write-Host "    1. Home page only" -ForegroundColor White
Write-Host "    2. Home + Direct children" -ForegroundColor White
Write-Host "    3. All pages (Recommended)" -ForegroundColor Green
Write-Host "    4. Custom path" -ForegroundColor White
Write-Host ""
$choice = Read-Host "Enter choice (1-4)"

$itemsToProcess = @()

switch ($choice) {
    "1" {
        $itemsToProcess = @(Get-Item -Path "master:$sitePath")
    }
    "2" {
        $itemsToProcess = @(Get-Item -Path "master:$sitePath")
        $itemsToProcess += Get-ChildItem -Path "master:$sitePath"
    }
    "3" {
        Write-Host "    [+] Getting all items..." -ForegroundColor Yellow
        $itemsToProcess = @(Get-Item -Path "master:$sitePath")
        $itemsToProcess += Get-ChildItem -Path "master:$sitePath" -Recurse
    }
    "4" {
        $customPath = Read-Host "    Enter path"
        $recurse = Read-Host "    Include children? (y/n)"
        
        if ($recurse -eq 'y') {
            $itemsToProcess = @(Get-Item -Path "master:$customPath" -ErrorAction SilentlyContinue)
            $itemsToProcess += Get-ChildItem -Path "master:$customPath" -Recurse -ErrorAction SilentlyContinue
        } else {
            $itemsToProcess = @(Get-Item -Path "master:$customPath" -ErrorAction SilentlyContinue)
        }
    }
    default {
        Write-Host "[✗] Invalid choice!" -ForegroundColor Red
        exit
    }
}

$itemsToProcess = $itemsToProcess | Where-Object { $_ -ne $null }

if ($itemsToProcess.Count -eq 0) {
    Write-Host "[✗] No items found!" -ForegroundColor Red
    exit
}

Write-Host "    [✓] Found $($itemsToProcess.Count) items" -ForegroundColor Green
Write-Host ""

# Handle existing
Write-Host "[3/4] Handle existing layouts:" -ForegroundColor Cyan
Write-Host "    1. Skip items with layout" -ForegroundColor White
Write-Host "    2. Update/Replace (Recommended)" -ForegroundColor Green
Write-Host "    3. Remove old then add new" -ForegroundColor White
Write-Host ""
$handleExisting = Read-Host "Enter choice (1-3)"
Write-Host ""

# Process items
Write-Host "[4/4] Processing items..." -ForegroundColor Cyan
Write-Host ""

$skipTemplates = @("Common/Folder", "System/Layout/Device", "System/Dictionary/Dictionary Domain")
$skipNames = @("Media", "Data", "Presentation", "Settings", "BmcBlog Dictionary")

$processedCount = 0
$skippedCount = 0
$successCount = 0
$errorCount = 0
$updatedCount = 0

foreach ($item in $itemsToProcess) {
    $processedCount++
    
    if ($processedCount % 10 -eq 0) {
        Write-Host "    Progress: $processedCount / $($itemsToProcess.Count)" -ForegroundColor Gray
    }
    
    # Skip unwanted items
    $shouldSkip = $false
    
    foreach ($skipTemplate in $skipTemplates) {
        if ($item.TemplateName -like "*$skipTemplate*") {
            $shouldSkip = $true
            break
        }
    }
    
    if ($item.Name -in $skipNames) {
        $shouldSkip = $true
    }
    
    if ($shouldSkip) {
        $skippedCount++
        continue
    }
    
    # Check current layout
    $currentLayout = $item | Get-Layout -Device $defaultDevice -ErrorAction SilentlyContinue
    $hasLayout = ($currentLayout -ne $null)
    
    $shouldProcess = $false
    
    switch ($handleExisting) {
        "1" {
            if (-not $hasLayout) {
                $shouldProcess = $true
            } else {
                $skippedCount++
            }
        }
        "2" {
            $shouldProcess = $true
        }
        "3" {
            if ($hasLayout) {
                try {
                    $item.Editing.BeginEdit()
                    $item["__Renderings"] = ""
                    $item.Editing.EndEdit() | Out-Null
                } catch {
                    if ($item.Editing.IsEditing) {
                        $item.Editing.CancelEdit()
                    }
                }
            }
            $shouldProcess = $true
        }
    }
    
    if ($shouldProcess) {
        try {
            # Use SPE cmdlet
            $item | Set-Layout -Device $defaultDevice -Layout $layoutItem
            
            if ($hasLayout) {
                $updatedCount++
            } else {
                $successCount++
            }
        } catch {
            # Fallback to XML method
            try {
                $item.Editing.BeginEdit()
                $layoutXml = "<r xmlns:xsd=`"http://www.w3.org/2001/XMLSchema`"><d id=`"{FE5D7FDF-89C0-4D99-9AA3-B5FBD009C9F3}`" l=`"$($layoutItem.ID)`" /></r>"
                $item["__Renderings"] = $layoutXml
                $item.Editing.EndEdit() | Out-Null
                
                if ($hasLayout) {
                    $updatedCount++
                } else {
                    $successCount++
                }
            } catch {
                if ($item.Editing.IsEditing) {
                    $item.Editing.CancelEdit()
                }
                $errorCount++
            }
        }
    }
}

# Summary
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Layout Assignment Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Total Items Processed: $processedCount" -ForegroundColor White
Write-Host "New Layouts Assigned: $successCount" -ForegroundColor Green
Write-Host "Layouts Updated: $updatedCount" -ForegroundColor Yellow
Write-Host "Skipped: $skippedCount" -ForegroundColor DarkGray
Write-Host "Errors: $errorCount" -ForegroundColor Red
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

if ($successCount -gt 0 -or $updatedCount -gt 0) {
    Write-Host "[✓] Layout assignment completed!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next: Run SCRIPT 4 to add Renderings" -ForegroundColor Cyan
} else {
    Write-Host "[!] No layouts were assigned." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "SCRIPT 3 Completed!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
# ====================================================================
# SCRIPT 4: Add Renderings to Pages (Fixed & Optimized)
# ====================================================================

Write-Host ""
Write-Host "========================================" -ForegroundColor Yellow
Write-Host "SCRIPT 4: Adding Renderings to Pages" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Yellow
Write-Host ""

# Configuration
$renderingsBasePath = "/sitecore/layout/Renderings/BMC"
$sitePath = "/sitecore/content/BMC/BmcBlog"

# Renderings to add
$renderingsConfig = @(
    @{Name = "Header"; Placeholder = "header"},
    @{Name = "Breadcrumb"; Placeholder = "breadcrumb"},
    @{Name = "Footer"; Placeholder = "footer"}
)

# Check requirements
Write-Host "[1/3] Checking requirements..." -ForegroundColor Cyan

$missingRenderings = @()
foreach ($config in $renderingsConfig) {
    $rendering = Get-Item -Path "master:$renderingsBasePath/$($config.Name)" -ErrorAction SilentlyContinue
    if (-not $rendering) {
        $missingRenderings += $config.Name
    }
}

if ($missingRenderings.Count -gt 0) {
    Write-Host "[✗] Missing renderings: $($missingRenderings -join ', ')" -ForegroundColor Red
    Write-Host "    Run SCRIPT 2 first!" -ForegroundColor Red
    exit
}
Write-Host "    [✓] All renderings found" -ForegroundColor Green

$defaultDevice = Get-Item -Path "master:/sitecore/layout/Devices/Default" -ErrorAction SilentlyContinue
if (-not $defaultDevice) {
    Write-Host "[✗] Default device not found!" -ForegroundColor Red
    exit
}
Write-Host "    [✓] Default device found" -ForegroundColor Green
Write-Host ""

# Get pages with layout
Write-Host "[2/3] Finding pages with layout..." -ForegroundColor Cyan

$allItems = Get-ChildItem -Path "master:$sitePath" -Recurse -ErrorAction SilentlyContinue
$pagesWithLayout = @()

foreach ($item in $allItems) {
    $layoutField = $item.Fields["__Renderings"]
    if ($layoutField -ne $null -and ![string]::IsNullOrEmpty($layoutField.Value)) {
        $pagesWithLayout += $item
    }
}

Write-Host "    [✓] Found $($pagesWithLayout.Count) pages with layout" -ForegroundColor Green
Write-Host ""

if ($pagesWithLayout.Count -eq 0) {
    Write-Host "[!] No pages with layout found. Run SCRIPT 3 first!" -ForegroundColor Yellow
    exit
}

# Process pages
Write-Host "[3/3] Adding renderings..." -ForegroundColor Cyan
Write-Host ""

$stats = @{
    TotalPages = $pagesWithLayout.Count
    TotalOperations = 0
    SuccessCount = 0
    SkipCount = 0
    ErrorCount = 0
}

$skipNames = @("Media", "Data", "Presentation", "Settings", "BmcBlog Dictionary")

foreach ($page in $pagesWithLayout) {
    # Skip system items
    if ($page.Name -in $skipNames) {
        continue
    }
    
    if ($stats.TotalOperations % 30 -eq 0 -and $stats.TotalOperations -gt 0) {
        Write-Host "    Progress: $($stats.TotalOperations) operations completed" -ForegroundColor Gray
    }
    
    # Get current layout XML
    $layoutField = $page.Fields["__Renderings"]
    if ($layoutField -eq $null -or [string]::IsNullOrEmpty($layoutField.Value)) {
        continue
    }
    
    try {
        [xml]$layoutXml = $layoutField.Value
    } catch {
        Write-Host "    [!] Skipping $($page.Name) - invalid layout XML" -ForegroundColor Yellow
        continue
    }
    
    # Find device node
    $deviceNode = $layoutXml.SelectSingleNode("//d[@id='$($defaultDevice.ID)']")
    if ($deviceNode -eq $null) {
        continue
    }
    
    $modified = $false
    
    # Add each rendering
    foreach ($config in $renderingsConfig) {
        $stats.TotalOperations++
        
        $rendering = Get-Item -Path "master:$renderingsBasePath/$($config.Name)" -ErrorAction SilentlyContinue
        if (-not $rendering) {
            continue
        }
        
        # Check if rendering already exists
        $existingRendering = $deviceNode.SelectSingleNode("r[@id='$($rendering.ID)']")
        if ($existingRendering -ne $null) {
            $stats.SkipCount++
            continue
        }
        
        # Create new rendering node
        $renderingNode = $layoutXml.CreateElement("r")
        $renderingNode.SetAttribute("id", $rendering.ID.ToString())
        $renderingNode.SetAttribute("ph", $config.Placeholder)
        $renderingNode.SetAttribute("uid", [Guid]::NewGuid().ToString())
        
        # Add to device
        $deviceNode.AppendChild($renderingNode) | Out-Null
        $modified = $true
        $stats.SuccessCount++
    }
    
    # Save if modified
    if ($modified) {
        try {
            $page.Editing.BeginEdit()
            $page.Fields["__Renderings"].Value = $layoutXml.OuterXml
            $page.Editing.EndEdit() | Out-Null
        } catch {
            if ($page.Editing.IsEditing) {
                $page.Editing.CancelEdit()
            }
            $stats.ErrorCount++
        }
    }
}

# Summary
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Rendering Addition Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Total Pages: $($stats.TotalPages)" -ForegroundColor White
Write-Host "Total Operations: $($stats.TotalOperations)" -ForegroundColor White
Write-Host "Renderings Added: $($stats.SuccessCount)" -ForegroundColor Green
Write-Host "Skipped (already exists): $($stats.SkipCount)" -ForegroundColor Yellow
Write-Host "Errors: $($stats.ErrorCount)" -ForegroundColor Red
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

if ($stats.SuccessCount -gt 0) {
    Write-Host "[✓] Renderings added successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next: Run SCRIPT 5 to publish changes" -ForegroundColor Cyan
} else {
    Write-Host "[!] No new renderings were added." -ForegroundColor Yellow
    Write-Host "    Possible reasons:" -ForegroundColor Cyan
    Write-Host "    - All pages already have these renderings" -ForegroundColor White
    Write-Host "    - Pages don't have layout assigned" -ForegroundColor White
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "SCRIPT 4 Completed!" -ForegroundColor Green
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
