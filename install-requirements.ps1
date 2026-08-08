# Checks whether you have what the project needs (.NET 8 and Node.js)
# and installs them automatically if missing (using winget, built into Windows).
#
# Usage: right-click this file > "Run with PowerShell"
# or, from a terminal: powershell -ExecutionPolicy Bypass -File .\install-requirements.ps1

function Test-Command($cmd) {
    return $null -ne (Get-Command $cmd -ErrorAction SilentlyContinue)
}

Write-Host "== Checking requirements ==" -ForegroundColor Cyan

$installedSomething = $false

if (Test-Command "dotnet") {
    Write-Host "[OK] .NET SDK already installed ($(dotnet --version))" -ForegroundColor Green
} else {
    Write-Host "[MISSING] .NET SDK 8 not found. Installing..." -ForegroundColor Yellow
    winget install --id Microsoft.DotNet.SDK.8 -e --accept-package-agreements --accept-source-agreements
    $installedSomething = $true
}

if (Test-Command "node") {
    Write-Host "[OK] Node.js already installed ($(node --version))" -ForegroundColor Green
} else {
    Write-Host "[MISSING] Node.js not found. Installing..." -ForegroundColor Yellow
    winget install --id OpenJS.NodeJS.LTS -e --accept-package-agreements --accept-source-agreements
    $installedSomething = $true
}

Write-Host ""
if ($installedSomething) {
    Write-Host "Something new was installed. CLOSE this terminal and open a new one" -ForegroundColor Cyan
    Write-Host "(or restart VS Code) so it picks up the newly installed tools." -ForegroundColor Cyan
    Write-Host "Then run: .\install-dependencies.ps1" -ForegroundColor Cyan
} else {
    Write-Host "You already have everything needed. Run: .\install-dependencies.ps1" -ForegroundColor Green
}
