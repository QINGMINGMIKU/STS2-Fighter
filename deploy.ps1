# Fighter mod deploy script
# Builds, waits for game to close, copies DLL+PCK, optionally relaunches
param(
    [switch]$Launch,      # relaunch game after deploy
    [switch]$NoBuild,     # skip build (DLL already compiled)
    [switch]$NoWait       # fail immediately if game is running
)

$ErrorActionPreference = "Stop"
$project = $PSScriptRoot
$gameDir = "C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2"
$modDir = "$gameDir\mods\Fighter"
$godotExe = "C:\Tools\Godot_v4.5.1-stable_mono_win64\Godot_v4.5.1-stable_mono_win64.exe"
$steamExe = "C:\Program Files (x86)\Steam\steam.exe"

# 1. Build
if (-not $NoBuild) {
    Write-Host "[1/3] Building..." -ForegroundColor Cyan
    Push-Location $project
    $result = dotnet build -c Release 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Host "BUILD FAILED:" -ForegroundColor Red
        $result | Select-String "error CS" | ForEach-Object { Write-Host $_ -ForegroundColor Red }
        Pop-Location
        exit 1
    }
    Write-Host "  Build OK" -ForegroundColor Green
    Pop-Location
} else {
    Write-Host "[1/3] Build skipped (--NoBuild)" -ForegroundColor DarkGray
}

# 2. Wait for game to close, then copy
Write-Host "[2/3] Deploying..." -ForegroundColor Cyan
$dll = "$project\.godot\mono\temp\bin\Release\Fighter.dll"
$pck = "$project\Fighter.pck"

# Export PCK (can do while game is running)
Push-Location $project
& $godotExe --headless --export-pack "BasicExport" $pck 2>&1 | Out-Null
Pop-Location

# Try copy DLL - if locked, wait
$copied = $false
while (-not $copied) {
    try {
        $stream = [System.IO.File]::Open("$modDir\Fighter.dll", [System.IO.FileMode]::Open, [System.IO.FileAccess]::ReadWrite, [System.IO.FileShare]::None)
        $stream.Close()
        Copy-Item $dll -Destination $modDir -Force
        Copy-Item $pck -Destination $modDir -Force
        Write-Host "  DLL + PCK deployed" -ForegroundColor Green
        $copied = $true
    } catch {
        if ($NoWait) {
            Write-Host "  Game is running. Close it and re-run." -ForegroundColor Yellow
            exit 1
        }
        Write-Host "  Waiting for game to close... (Ctrl+C to cancel)" -ForegroundColor Yellow
        Start-Sleep -Seconds 3
    }
}

# 3. Launch
if ($Launch) {
    Write-Host "[3/3] Launching..." -ForegroundColor Cyan
    Start-Process $steamExe -ArgumentList "-applaunch 2868840"
    Write-Host "  Launched" -ForegroundColor Green
}

Write-Host "Done." -ForegroundColor Green
