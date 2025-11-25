# =============================================================================
# سكربت نشر مشروع Sitecore Helix (نهائي - بدون تكرار ملفات المشروع الرئيسي)
# =============================================================================

# 1. إعدادات المسارات
# -----------------------------------------------------------------------------
$publishTarget = "D:\test"
$configuration = "Debug"
$solutionRoot = Get-Location 

# البحث عن MSBuild
$msBuildPath = "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
if (-not (Test-Path $msBuildPath)) {
    $msBuildPath = "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"
}
if (-not (Test-Path $msBuildPath)) {
    $msBuildPath = "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
}

if (-not (Test-Path $msBuildPath)) {
    Write-Host "Error: Could not find MSBuild.exe." -ForegroundColor Red
    exit
}

Write-Host "Deploying to: $publishTarget" -ForegroundColor Yellow
Write-Host ""

# 2. بناء ونشر المشاريع (يقوم بنشر المشروع الرئيسي كاملاً + DLLs للمشاريع الأخرى)
# -----------------------------------------------------------------------------
$projects = Get-ChildItem -Path ".\" -Recurse -Filter "*.csproj" | 
            Where-Object { $_.FullName -notmatch "Tests" -and $_.FullName -notmatch "Demo" }

foreach ($project in $projects) {
    Write-Host "Building: $($project.BaseName)..." -ForegroundColor Cyan
    $buildArgs = @(
        $project.FullName,
        "/p:Configuration=$configuration",
        "/p:DeployOnBuild=true",
        "/p:WebPublishMethod=FileSystem",
        "/p:publishUrl=$publishTarget",
        "/p:DeployDefaultTarget=WebPublish",
        "/v:m", "/nologo"
    )
    $process = Start-Process -FilePath $msBuildPath -ArgumentList $buildArgs -Wait -NoNewWindow -PassThru
}

# 3. خطوة إضافية: نسخ ملفات Configs و Views من الـ Features و Foundations فقط
# -----------------------------------------------------------------------------
Write-Host ""
Write-Host "----------------------------------------" -ForegroundColor Yellow
Write-Host "Starting Asset Copy (Features & Foundations)..." -ForegroundColor Yellow
Write-Host "----------------------------------------" -ForegroundColor Yellow

# دالة لنسخ الملفات
function Copy-HelixAssets ($sourceParam, $destinationParam) {
    if (Test-Path $sourceParam) {
        Write-Host "  Copying from: $sourceParam" -ForegroundColor Gray
        Copy-Item -Path "$sourceParam\*" -Destination $destinationParam -Recurse -Force
    }
}

# تم استبعاد "Project" من هنا لتجنب نسخ الـ web.config
$layers = @("Feature", "Foundation")

foreach ($layer in $layers) {
    $layerPath = Join-Path $solutionRoot $layer
    if (Test-Path $layerPath) {
        # الحصول على كل مجلدات المشاريع داخل الطبقة
        $modules = Get-ChildItem -Path $layerPath -Directory
        
        foreach ($module in $modules) {
            # 1. نسخ App_Config
            $configSource = Join-Path $module.FullName "App_Config"
            $configDest = Join-Path $publishTarget "App_Config"
            Copy-HelixAssets $configSource $configDest

            # 2. نسخ Views
            $viewSource = Join-Path $module.FullName "Views"
            $viewDest = Join-Path $publishTarget "Views"
            Copy-HelixAssets $viewSource $viewDest
        }
    }
}

Write-Host ""
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "Deployment Completed Successfully!" -ForegroundColor Green
Write-Host "All Assets are now in: $publishTarget" -ForegroundColor Yellow
Write-Host "==========================================" -ForegroundColor Cyan