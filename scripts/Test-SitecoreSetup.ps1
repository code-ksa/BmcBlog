# ====================================================================
# BMC Blog - Test Sitecore Setup Script
# ====================================================================
# Purpose: Verify that everything is deployed and configured correctly
# ====================================================================

param(
    [string]$SitecorePath = "C:\inetpub\wwwroot\abdo.sc",
    [switch]$Detailed
)

# ====================================================================
# Configuration
# ====================================================================

$ErrorActionPreference = "Continue"

# Colors
$ColorSuccess = "Green"
$ColorWarning = "Yellow"
$ColorError = "Red"
$ColorInfo = "Cyan"

# Test results
$TestResults = @{
    Total = 0
    Passed = 0
    Failed = 0
    Warnings = 0
}

# ====================================================================
# Helper Functions
# ====================================================================

function Write-TestResult {
    param(
        [string]$TestName,
        [bool]$Passed,
        [string]$Message = "",
        [bool]$IsWarning = $false
    )
    
    $TestResults.Total++
    
    if ($Passed) {
        $TestResults.Passed++
        Write-Host "[?] $TestName" -ForegroundColor $ColorSuccess
        if ($Message -and $Detailed) {
            Write-Host "    $Message" -ForegroundColor Gray
        }
    } elseif ($IsWarning) {
        $TestResults.Warnings++
        Write-Host "[!] $TestName" -ForegroundColor $ColorWarning
        if ($Message) {
            Write-Host "    $Message" -ForegroundColor $ColorWarning
        }
    } else {
        $TestResults.Failed++
        Write-Host "[?] $TestName" -ForegroundColor $ColorError
        if ($Message) {
            Write-Host "    $Message" -ForegroundColor $ColorError
        }
    }
}

function Test-FileExists {
    param(
        [string]$Path,
        [string]$Description
    )
    
    $exists = Test-Path $Path
    Write-TestResult -TestName $Description -Passed $exists -Message $Path
    return $exists
}

# ====================================================================
# Main Tests
# ====================================================================

Write-Host ""
Write-Host "========================================" -ForegroundColor $ColorInfo
Write-Host "BMC Blog - Sitecore Setup Verification" -ForegroundColor $ColorInfo
Write-Host "========================================" -ForegroundColor $ColorInfo
Write-Host ""
Write-Host "Sitecore Path: $SitecorePath" -ForegroundColor Gray
Write-Host ""

# ====================================================================
# Section 1: File System Tests
# ====================================================================

Write-Host "????????????????????????????????????????" -ForegroundColor $ColorInfo
Write-Host "SECTION 1: File System Tests" -ForegroundColor $ColorInfo
Write-Host "????????????????????????????????????????" -ForegroundColor $ColorInfo
Write-Host ""

# Test Sitecore path
Test-FileExists -Path $SitecorePath -Description "Sitecore installation path exists"

# Test DLLs
Write-Host ""
Write-Host "Testing DLL deployment..." -ForegroundColor $ColorInfo

$requiredDLLs = @(
    "BMC.Foundation.SitecoreExtensions.dll",
    "BMC.Foundation.DependencyInjection.dll",
    "BMC.Foundation.Indexing.dll",
    "BMC.Foundation.Multisite.dll",
    "BMC.Foundation.Caching.dll",
    "BMC.Feature.Navigation.dll",
    "BMC.Feature.Newsletter.dll",
    "BMC.Feature.Blog.dll",
    "BMC.Feature.Hero.dll",
    "BMC.Feature.Search.dll",
    "BMC.Feature.Identity.dll",
    "BMC.Feature.Comments.dll",
    "BMC.Project.BlogSite.dll"
)

$binPath = Join-Path $SitecorePath "bin"
$dllCount = 0

foreach ($dll in $requiredDLLs) {
    $dllPath = Join-Path $binPath $dll
    if (Test-FileExists -Path $dllPath -Description "  DLL: $dll") {
        $dllCount++
    }
}

Write-Host ""
Write-Host "DLLs found: $dllCount / $($requiredDLLs.Count)" -ForegroundColor $(if ($dllCount -eq $requiredDLLs.Count) { $ColorSuccess } else { $ColorWarning })

# Test Views
Write-Host ""
Write-Host "Testing Views deployment..." -ForegroundColor $ColorInfo

$viewsToTest = @{
    "Layouts\BlogLayout.cshtml" = "Blog Layout View"
    "BmcNavigation\Header.cshtml" = "Header View"
    "BmcNavigation\Footer.cshtml" = "Footer View"
    "BmcNavigation\Breadcrumb.cshtml" = "Breadcrumb View"
    "BmcNewsletter\Subscribe.cshtml" = "Newsletter Subscribe View"
}

$viewsPath = Join-Path $SitecorePath "Views"
$viewCount = 0

foreach ($view in $viewsToTest.Keys) {
    $viewPath = Join-Path $viewsPath $view
    if (Test-FileExists -Path $viewPath -Description "  View: $($viewsToTest[$view])") {
        $viewCount++
    }
}

Write-Host ""
Write-Host "Views found: $viewCount / $($viewsToTest.Count)" -ForegroundColor $(if ($viewCount -eq $viewsToTest.Count) { $ColorSuccess } else { $ColorWarning })

# Test Config Files
Write-Host ""
Write-Host "Testing Config files..." -ForegroundColor $ColorInfo

$configPath = Join-Path $SitecorePath "App_Config\Include\Foundation"
if (Test-Path $configPath) {
    $configFiles = Get-ChildItem -Path $configPath -Filter "*.config" -ErrorAction SilentlyContinue
    Write-TestResult -TestName "  Config folder exists" -Passed $true -Message "Found $($configFiles.Count) config file(s)"
    
    if ($Detailed) {
        foreach ($config in $configFiles) {
            Write-Host "    - $($config.Name)" -ForegroundColor Gray
        }
    }
} else {
    Write-TestResult -TestName "  Config folder exists" -Passed $false -Message $configPath
}

# ====================================================================
# Section 2: Web Server Tests
# ====================================================================

Write-Host ""
Write-Host "????????????????????????????????????????" -ForegroundColor $ColorInfo
Write-Host "SECTION 2: Web Server Tests" -ForegroundColor $ColorInfo
Write-Host "????????????????????????????????????????" -ForegroundColor $ColorInfo
Write-Host ""

# Test IIS Module
try {
    Import-Module WebAdministration -ErrorAction Stop
    Write-TestResult -TestName "IIS PowerShell Module" -Passed $true
    
    # Find website
    $site = Get-Website | Where-Object { $_.physicalPath -eq $SitecorePath } | Select-Object -First 1
    
    if ($site) {
        Write-TestResult -TestName "IIS Website found" -Passed $true -Message "Site: $($site.name), App Pool: $($site.applicationPool)"
        
        # Check if site is started
        $isStarted = $site.state -eq "Started"
        Write-TestResult -TestName "Website is running" -Passed $isStarted -Message "State: $($site.state)"
        
        # Check App Pool
        $appPool = Get-WebAppPoolState -Name $site.applicationPool
        $poolRunning = $appPool.Value -eq "Started"
        Write-TestResult -TestName "Application Pool is running" -Passed $poolRunning -Message "State: $($appPool.Value)"
    } else {
        Write-TestResult -TestName "IIS Website found" -Passed $false -Message "No website found for path: $SitecorePath"
    }
}
catch {
    Write-TestResult -TestName "IIS PowerShell Module" -Passed $false -Message $_.Exception.Message -IsWarning $true
}

# Test web connectivity
Write-Host ""
Write-Host "Testing web connectivity..." -ForegroundColor $ColorInfo

$urlsToTest = @(
    "http://abdo.sc",
    "http://abdo.sc/home"
)

foreach ($url in $urlsToTest) {
    try {
        $response = Invoke-WebRequest -Uri $url -UseBasicParsing -TimeoutSec 10 -ErrorAction Stop
        $passed = $response.StatusCode -eq 200
        Write-TestResult -TestName "  URL accessible: $url" -Passed $passed -Message "Status: $($response.StatusCode)"
    }
    catch {
        Write-TestResult -TestName "  URL accessible: $url" -Passed $false -Message $_.Exception.Message -IsWarning $true
    }
}

# ====================================================================
# Section 3: Database Tests (If Sitecore is accessible)
# ====================================================================

Write-Host ""
Write-Host "????????????????????????????????????????" -ForegroundColor $ColorInfo
Write-Host "SECTION 3: Sitecore Database Tests" -ForegroundColor $ColorInfo
Write-Host "????????????????????????????????????????" -ForegroundColor $ColorInfo
Write-Host ""
Write-Host "[INFO] These tests require Sitecore PowerShell ISE" -ForegroundColor $ColorWarning
Write-Host "[INFO] Run the following script in Sitecore PowerShell ISE:" -ForegroundColor $ColorWarning
Write-Host ""
Write-Host "# Check Layout Item" -ForegroundColor Gray
Write-Host '$layout = Get-Item -Path "master:/sitecore/layout/Layouts/BMC/Blog Layout" -ErrorAction SilentlyContinue' -ForegroundColor Gray
Write-Host 'if ($layout) { Write-Host "[?] Layout exists" -ForegroundColor Green } else { Write-Host "[?] Layout not found" -ForegroundColor Red }' -ForegroundColor Gray
Write-Host ""
Write-Host "# Check Rendering Items" -ForegroundColor Gray
Write-Host '$renderings = Get-ChildItem -Path "master:/sitecore/layout/Renderings/BMC" -ErrorAction SilentlyContinue' -ForegroundColor Gray
Write-Host 'Write-Host "Renderings found: $($renderings.Count)"' -ForegroundColor Gray
Write-Host ""
Write-Host "# Check Home Page Presentation" -ForegroundColor Gray
Write-Host '$home = Get-Item -Path "master:/sitecore/content/BMC/BmcBlog/Home" -ErrorAction SilentlyContinue' -ForegroundColor Gray
Write-Host '$hasLayout = $home.Fields["__Renderings"].Value' -ForegroundColor Gray
Write-Host 'if ($hasLayout) { Write-Host "[?] Home has presentation" -ForegroundColor Green } else { Write-Host "[?] Home has no presentation" -ForegroundColor Red }' -ForegroundColor Gray

# ====================================================================
# Section 4: Build Verification
# ====================================================================

Write-Host ""
Write-Host "????????????????????????????????????????" -ForegroundColor $ColorInfo
Write-Host "SECTION 4: Build Verification" -ForegroundColor $ColorInfo
Write-Host "????????????????????????????????????????" -ForegroundColor $ColorInfo
Write-Host ""

# Check for PDB files (indicates debug build)
$pdbFiles = Get-ChildItem -Path $binPath -Filter "BMC.*.pdb" -ErrorAction SilentlyContinue
$hasDebugSymbols = $pdbFiles.Count -gt 0
Write-TestResult -TestName "Debug symbols deployed" -Passed $hasDebugSymbols -Message "$($pdbFiles.Count) PDB files found" -IsWarning (-not $hasDebugSymbols)

# Check DLL timestamps
$newestDll = Get-ChildItem -Path $binPath -Filter "BMC.*.dll" -ErrorAction SilentlyContinue | 
    Sort-Object LastWriteTime -Descending | 
    Select-Object -First 1

if ($newestDll) {
    $age = (Get-Date) - $newestDll.LastWriteTime
    $isRecent = $age.TotalMinutes -lt 60
    Write-TestResult -TestName "DLLs are recent" -Passed $isRecent -Message "Last deployed: $($newestDll.LastWriteTime) ($([math]::Round($age.TotalMinutes, 1)) minutes ago)" -IsWarning (-not $isRecent)
}

# ====================================================================
# Final Summary
# ====================================================================

Write-Host ""
Write-Host "========================================" -ForegroundColor $ColorInfo
Write-Host "TEST SUMMARY" -ForegroundColor $ColorInfo
Write-Host "========================================" -ForegroundColor $ColorInfo
Write-Host ""
Write-Host "Total Tests: $($TestResults.Total)" -ForegroundColor White
Write-Host "Passed:      $($TestResults.Passed)" -ForegroundColor $ColorSuccess
Write-Host "Failed:      $($TestResults.Failed)" -ForegroundColor $ColorError
Write-Host "Warnings:    $($TestResults.Warnings)" -ForegroundColor $ColorWarning
Write-Host ""

$passPercentage = if ($TestResults.Total -gt 0) { 
    [math]::Round(($TestResults.Passed / $TestResults.Total) * 100, 1) 
} else { 
    0 
}

Write-Host "Pass Rate: $passPercentage%" -ForegroundColor $(
    if ($passPercentage -ge 90) { $ColorSuccess }
    elseif ($passPercentage -ge 70) { $ColorWarning }
    else { $ColorError }
)

Write-Host ""

if ($TestResults.Failed -eq 0 -and $TestResults.Warnings -eq 0) {
    Write-Host "? All tests passed!" -ForegroundColor $ColorSuccess
    Write-Host ""
    Write-Host "NEXT STEPS:" -ForegroundColor $ColorInfo
    Write-Host "1. Open Sitecore PowerShell ISE" -ForegroundColor White
    Write-Host "2. Run scripts from: files\03-Fixed-Sitecore-Scripts.ps1" -ForegroundColor White
    Write-Host "3. Test your site: http://abdo.sc/home" -ForegroundColor White
} elseif ($TestResults.Failed -eq 0) {
    Write-Host "! Tests completed with warnings" -ForegroundColor $ColorWarning
    Write-Host "Please review warnings above" -ForegroundColor $ColorWarning
} else {
    Write-Host "? Some tests failed" -ForegroundColor $ColorError
    Write-Host "Please fix the issues and run again" -ForegroundColor $ColorError
    Write-Host ""
    Write-Host "TROUBLESHOOTING:" -ForegroundColor $ColorInfo
    Write-Host "1. Run Build-And-Deploy.ps1 script again" -ForegroundColor White
    Write-Host "2. Check Sitecore logs in: $SitecorePath\App_Data\logs" -ForegroundColor White
    Write-Host "3. Verify IIS application pool is running" -ForegroundColor White
    Write-Host "4. Check file permissions on Sitecore folders" -ForegroundColor White
}

Write-Host ""
Write-Host "========================================" -ForegroundColor $ColorInfo

# Return exit code based on results
if ($TestResults.Failed -gt 0) {
    exit 1
} else {
    exit 0
}
