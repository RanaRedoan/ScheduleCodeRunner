param(
    [string]$Configuration = "Release",
    [string]$RuntimeIdentifier = "win-x64"
)

$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
$artifactsRoot = Join-Path $root "artifacts"
$publishRoot = Join-Path $artifactsRoot "publish"
$appPublishDir = Join-Path $publishRoot "app"
$workerPublishDir = Join-Path $publishRoot "worker"
$installerOutputDir = Join-Path $artifactsRoot "installer"

New-Item -ItemType Directory -Force -Path $publishRoot, $appPublishDir, $workerPublishDir, $installerOutputDir | Out-Null

function Invoke-Step {
    param(
        [string]$Label,
        [scriptblock]$Command
    )

    & $Command
    if ($LASTEXITCODE -ne 0)
    {
        throw "$Label failed with exit code $LASTEXITCODE."
    }
}

Invoke-Step "Restore app" {
    dotnet restore (Join-Path $root "src\ScheduleCodeRunner.App\ScheduleCodeRunner.App.csproj") -r $RuntimeIdentifier
}

Invoke-Step "Restore worker" {
    dotnet restore (Join-Path $root "src\ScheduleCodeRunner.Worker\ScheduleCodeRunner.Worker.csproj") -r $RuntimeIdentifier
}

Invoke-Step "Publish app" {
    dotnet publish (Join-Path $root "src\ScheduleCodeRunner.App\ScheduleCodeRunner.App.csproj") `
        -c $Configuration `
        -r $RuntimeIdentifier `
        --no-restore `
        --self-contained true `
        -o $appPublishDir
}

Invoke-Step "Publish worker" {
    dotnet publish (Join-Path $root "src\ScheduleCodeRunner.Worker\ScheduleCodeRunner.Worker.csproj") `
        -c $Configuration `
        -r $RuntimeIdentifier `
        --no-restore `
        --self-contained true `
        -o $workerPublishDir
}

if (Get-Command iscc -ErrorAction SilentlyContinue)
{
    & iscc (Join-Path $PSScriptRoot "ScheduleCodeRunner.iss")
}
else
{
    $localIscc = Join-Path $env:LOCALAPPDATA "Programs\Inno Setup 6\ISCC.exe"
    if (Test-Path $localIscc)
    {
        & $localIscc (Join-Path $PSScriptRoot "ScheduleCodeRunner.iss")
    }
    else
    {
        Write-Host "Inno Setup compiler not found. Publish output is ready, but installer build was skipped."
    }
}
