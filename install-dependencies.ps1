# Installs the project's dependencies (NuGet packages, npm) and prepares the database.
# Run this ONCE (or every time you download a new version of the project).
#
# Usage: powershell -ExecutionPolicy Bypass -File .\install-dependencies.ps1

$root = $PSScriptRoot

Write-Host "== Backend (.NET) ==" -ForegroundColor Cyan
Set-Location "$root\GasStationBilling.Api"
dotnet restore

if (-not (Get-Command dotnet-ef -ErrorAction SilentlyContinue)) {
    Write-Host "Installing dotnet-ef tool..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-ef
}

if (-not (Test-Path "Migrations")) {
    Write-Host "Creating the initial database migration..." -ForegroundColor Yellow
    dotnet ef migrations add InitialCreate
} else {
    Write-Host "A migration already exists, skipping." -ForegroundColor Green
}

Write-Host ""
Write-Host "== Frontend (React) ==" -ForegroundColor Cyan
Set-Location "$root\frontend"
npm install

# Newer npm versions block package install scripts (like esbuild's) until
# approved. Approve them automatically so Vite works out of the box.
npm approve-scripts --all 2>$null

if (-not (Test-Path "node_modules\.bin\vite.cmd")) {
    Write-Host "Retrying npm install after approving scripts..." -ForegroundColor Yellow
    npm install
}

Set-Location $root
Write-Host ""
Write-Host "Done. Now run: .\start.ps1" -ForegroundColor Green
