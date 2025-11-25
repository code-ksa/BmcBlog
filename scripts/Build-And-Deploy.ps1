# ====================================================================
# BMC Blog - Build and Deploy Script
# ====================================================================
# Purpose: Build the solution and deploy to Sitecore instance
# ====================================================================

param(
    [string]$SitecorePath = "C:\inetpub\wwwroot\abdo.sc",
    [string]$Configuration = "Debug",
    [switch]$SkipBuild,
    [switch]$Verbose
)

# ====================================================================
# Configuration
# ====================================================================

$ErrorActionPreference = "Stop"
$ScriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$SolutionPath = Split-Path -Parent $ScriptPath
$SolutionFile = Join-Path $SolutionPath "BmcBlog.sln"

# Sitecore paths
$SitecoreBinPath = Join-Path $SitecorePath "bin"
$SitecoreViewsPath = Join-Path $SitecorePath "Views"
$SitecoreConfigPath = Join-Path $SitecorePath "App_Config\Include\Foundation"

# Build output paths
$BuildOutputPath = Join-Path $SolutionPath "bin\$Configuration"

# Colors for output
$ColorSuccess = "Green"
$ColorWarning = "Yellow"
$ColorError = "Red"
$ColorInfo = "Cyan"

# ====================================================================
# Helper Functions
# ====================================================================

function Write-Log {
    param(
        [string]$Message,
        [string]$Level = "INFO"
    )
    
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $color = switch ($Level) {
        "SUCCESS" { $ColorSuccess }
        "WARNING" { $ColorWarning }
        "ERROR"   { $ColorError }
        default   { $ColorInfo }
    }
    
    Write-Host "[$timestamp] [$Level] $Message" -ForegroundColor $color
}

function Test-Path-Safe {
    param([string]$Path)
    
    if (-not (Test-Path $Path)) {
        Write-Log "Path not found: $Path" "ERROR"
        return $false
    }
    return $true
}

function Ensure-Directory {
    param([string]$Path)
    
    if (-not (Test-Path $Path)) {
        Write-Log "Creating directory: $Path" "INFO"
        New-Item -ItemType Directory -Path $Path -Force | Out-Null
    }
}

function Copy-Files-Safe {
    param(
        [string]$Source,
        [string]$Destination,
        [string]$Filter = "*.*",
        [switch]$Recurse
    )
    
    try {
        if (-not (Test-Path $Source)) {
            Write-Log "Source not found: $Source" "WARNING"
            return $false
        }
        
        Ensure-Directory $Destination
        
        $params = @{
            Path = (Join-Path $Source $Filter)
            Destination = $Destination
            Force = $true
            ErrorAction = "Stop"
        }
        
        if ($Recurse) {
            $params.Recurse = $true
        }
        
        Copy-Item @params
        return $true
    }
    catch {
        Write-Log "Error copying files: $_" "ERROR"
        return $false
    }
}

# ====================================================================
# Main Script
# ====================================================================

Write-Log "========================================" "INFO"
Write-Log "BMC Blog - Build and Deploy Script" "INFO"
Write-Log "========================================" "INFO"
Write-Log ""

# Step 1: Validate paths
Write-Log "Step 1: Validating paths..." "INFO"

if (-not (Test-Path-Safe $SolutionFile)) {
    Write-Log "Solution file not found: $SolutionFile" "ERROR"
    exit 1
}

if (-not (Test-Path-Safe $SitecorePath)) {
    Write-Log "Sitecore path not found: $SitecorePath" "ERROR"
    Write-Log "Please specify correct path using -SitecorePath parameter" "ERROR"
    exit 1
}

Write-Log "Solution: $SolutionFile" "SUCCESS"
Write-Log "Sitecore: $SitecorePath" "SUCCESS"
Write-Log ""

# Step 2: Build Solution
if (-not $SkipBuild) {
    Write-Log "Step 2: Building solution..." "INFO"
    
    # Find MSBuild
    $msbuildPath = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" `
        -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe `
        | Select-Object -First 1
    
    if (-not $msbuildPath) {
        Write-Log "MSBuild not found. Please ensure Visual Studio is installed." "ERROR"
        exit 1
    }
    
    Write-Log "Using MSBuild: $msbuildPath" "INFO"
    
    $buildArgs = @(
        $SolutionFile,
        "/p:Configuration=$Configuration",
        "/t:Clean;Build",
        "/m",
        "/v:m"
    )
    
    if ($Verbose) {
        $buildArgs += "/v:d"
    }
    
    & $msbuildPath $buildArgs
    
    if ($LASTEXITCODE -ne 0) {
        Write-Log "Build failed with exit code: $LASTEXITCODE" "ERROR"
        exit $LASTEXITCODE
    }
    
    Write-Log "Build completed successfully!" "SUCCESS"
    Write-Log ""
} else {
    Write-Log "Step 2: Skipping build (SkipBuild flag set)" "WARNING"
    Write-Log ""
}

# Step 3: Deploy DLLs
Write-Log "Step 3: Deploying DLLs..." "INFO"

$projectsToDeploy = @(
    "Foundation\BMC.Foundation.SitecoreExtensions",
    "Foundation\BMC.Foundation.DependencyInjection",
    "Foundation\BMC.Foundation.Indexing",
    "Foundation\BMC.Foundation.Multisite",
    "Foundation\BMC.Foundation.Caching",
    "Feature\BMC.Feature.Navigation",
    "Feature\BMC.Feature.Newsletter",
    "Feature\BMC.Feature.Blog",
    "Feature\BMC.Feature.Hero",
    "Feature\BMC.Feature.Search",
    "Feature\BMC.Feature.Identity",
    "Feature\BMC.Feature.Comments",
    "Project\BMC.Project.BlogSite"
)

$deployedCount = 0
$failedCount = 0

foreach ($project in $projectsToDeploy) {
    $projectPath = Join-Path $SolutionPath $project
    $binPath = Join-Path $projectPath "bin\$Configuration"
    
    if (-not (Test-Path $binPath)) {
        Write-Log "Bin path not found: $binPath" "WARNING"
        $failedCount++
        continue
    }
    
    # Get DLL name from project folder name
    $projectName = Split-Path $project -Leaf
    $dllPath = Join-Path $binPath "$projectName.dll"
    $pdbPath = Join-Path $binPath "$projectName.pdb"
    
    if (Test-Path $dllPath) {
        Copy-Item $dllPath $SitecoreBinPath -Force
        Write-Log "Deployed: $projectName.dll" "SUCCESS"
        $deployedCount++
        
        if (Test-Path $pdbPath) {
            Copy-Item $pdbPath $SitecoreBinPath -Force
        }
    } else {
        Write-Log "DLL not found: $dllPath" "WARNING"
        $failedCount++
    }
}

Write-Log "DLLs deployed: $deployedCount, Failed: $failedCount" "INFO"
Write-Log ""

# Step 4: Deploy Views
Write-Log "Step 4: Deploying Views..." "INFO"

$viewsToDeploy = @{
    "Feature\BMC.Feature.Navigation\Views" = "Views"
    "Feature\BMC.Feature.Newsletter\Views" = "Views"
    "Project\BMC.Project.BlogSite\Views" = "Views"
}

$viewsDeployedCount = 0

foreach ($viewSource in $viewsToDeploy.Keys) {
    $sourcePath = Join-Path $SolutionPath $viewSource
    $destPath = Join-Path $SitecorePath $viewsToDeploy[$viewSource]
    
    if (Test-Path $sourcePath) {
        if (Copy-Files-Safe -Source $sourcePath -Destination $destPath -Filter "*.*" -Recurse) {
            Write-Log "Deployed views from: $viewSource" "SUCCESS"
            $viewsDeployedCount++
        }
    } else {
        Write-Log "Views path not found: $sourcePath" "WARNING"
    }
}

Write-Log "View folders deployed: $viewsDeployedCount" "INFO"
Write-Log ""

# Step 5: Deploy Config Files
Write-Log "Step 5: Deploying Config files..." "INFO"

$configsToDeploy = @(
    "Foundation\BMC.Foundation.SitecoreExtensions\App_Config\Include\Foundation"
)

$configsDeployedCount = 0

foreach ($configSource in $configsToDeploy) {
    $sourcePath = Join-Path $SolutionPath $configSource
    
    if (Test-Path $sourcePath) {
        $configFiles = Get-ChildItem -Path $sourcePath -Filter "*.config" -Recurse
        
        foreach ($configFile in $configFiles) {
            $destPath = Join-Path $SitecoreConfigPath $configFile.Name
            Copy-Item $configFile.FullName $destPath -Force
            Write-Log "Deployed config: $($configFile.Name)" "SUCCESS"
            $configsDeployedCount++
        }
    }
}

Write-Log "Config files deployed: $configsDeployedCount" "INFO"
Write-Log ""

# Step 6: Clean temp folders
Write-Log "Step 6: Cleaning temporary folders..." "INFO"

$tempFolders = @(
    (Join-Path $SitecorePath "App_Data\MediaCache"),
    (Join-Path $SitecorePath "App_Data\ViewState"),
    (Join-Path $SitecorePath "temp")
)

foreach ($tempFolder in $tempFolders) {
    if (Test-Path $tempFolder) {
        try {
            Get-ChildItem -Path $tempFolder -Recurse -File | Remove-Item -Force -ErrorAction SilentlyContinue
            Write-Log "Cleaned: $tempFolder" "SUCCESS"
        }
        catch {
            Write-Log "Could not clean: $tempFolder - $_" "WARNING"
        }
    }
}

Write-Log ""

# Step 7: Recycle App Pool (optional)
Write-Log "Step 7: Recycling Application Pool..." "INFO"

try {
    # Get the app pool name from IIS
    Import-Module WebAdministration -ErrorAction SilentlyContinue
    
    $site = Get-Website | Where-Object { $_.physicalPath -eq $SitecorePath } | Select-Object -First 1
    
    if ($site) {
        $appPoolName = $site.applicationPool
        Write-Log "Recycling App Pool: $appPoolName" "INFO"
        Restart-WebAppPool -Name $appPoolName
        Write-Log "App Pool recycled successfully" "SUCCESS"
    } else {
        Write-Log "Could not find IIS site for path: $SitecorePath" "WARNING"
        Write-Log "Please recycle the app pool manually" "WARNING"
    }
}
catch {
    Write-Log "Could not recycle app pool: $_" "WARNING"
    Write-Log "Please recycle the app pool manually from IIS Manager" "WARNING"
}

Write-Log ""

# ====================================================================
# Summary
# ====================================================================

Write-Log "========================================" "INFO"
Write-Log "DEPLOYMENT SUMMARY" "INFO"
Write-Log "========================================" "INFO"
Write-Log "DLLs deployed: $deployedCount" "SUCCESS"
Write-Log "View folders deployed: $viewsDeployedCount" "SUCCESS"
Write-Log "Config files deployed: $configsDeployedCount" "SUCCESS"
Write-Log ""

if ($failedCount -eq 0) {
    Write-Log "Deployment completed successfully! ?" "SUCCESS"
    Write-Log ""
    Write-Log "Next Steps:" "INFO"
    Write-Log "1. Open your browser: http://abdo.sc/home" "INFO"
    Write-Log "2. Run Sitecore PowerShell scripts (02 or 03)" "INFO"
    Write-Log "3. Test the site functionality" "INFO"
} else {
    Write-Log "Deployment completed with $failedCount warning(s)" "WARNING"
    Write-Log "Please review the warnings above" "WARNING"
}

Write-Log "========================================" "INFO"
