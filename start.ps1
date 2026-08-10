# Starts the backend (C#) and the frontend (React) together, each in its
# own terminal window, and opens the browser. If dependencies are missing
# (e.g. first run, or a fresh copy of the project), it installs them first.
#
# Usage: powershell -ExecutionPolicy Bypass -File .\start.ps1

$root = $PSScriptRoot

# ---- Self-heal: make sure frontend dependencies are actually installed ----
$frontendDir = "$root\frontend"
$nodeModules = "$frontendDir\node_modules"
$viteBin = "$nodeModules\.bin\vite.cmd"

if (-not (Test-Path $nodeModules) -or -not (Test-Path $viteBin)) {
    Write-Host "Frontend dependencies are missing or incomplete. Installing now..." -ForegroundColor Yellow
    Push-Location $frontendDir
    npm install

    # Newer npm versions block package install scripts (like esbuild's)
    # until approved. Approve them automatically so Vite works out of the box.
    npm approve-scripts --all 2>$null

    if (-not (Test-Path $viteBin)) {
        Write-Host "Retrying npm install after approving scripts..." -ForegroundColor Yellow
        npm install
    }
    Pop-Location

    if (-not (Test-Path $viteBin)) {
        Write-Host "[WARNING] Vite still wasn't installed correctly." -ForegroundColor Red
        Write-Host "Open a terminal in the 'frontend' folder and run:" -ForegroundColor Red
        Write-Host "  npm install" -ForegroundColor Red
        Write-Host "  npm approve-scripts esbuild" -ForegroundColor Red
        Write-Host "then run start.bat again." -ForegroundColor Red
        Read-Host "Press Enter to close"
        exit 1
    }

    Write-Host "Frontend dependencies installed." -ForegroundColor Green
}

# ---- Self-heal: make sure the backend builds before starting it ----
# (dotnet run compiles on first launch, but doing it here surfaces errors
#  early and guarantees the project is restorable on a fresh copy.)
$backendDir = "$root\GasStationBilling.Api"
$backendDll = "$backendDir\bin\Debug\net8.0\GasStationBilling.Api.dll"

if (-not (Test-Path $backendDll)) {
    Write-Host "Backend isn't built yet. Restoring and building..." -ForegroundColor Yellow
    Push-Location $backendDir
    dotnet build
    Pop-Location
    if (-not (Test-Path $backendDll)) {
        Write-Host "[WARNING] Backend build failed." -ForegroundColor Red
        Write-Host "Open a terminal in 'GasStationBilling.Api' and run: dotnet build" -ForegroundColor Red
        Read-Host "Press Enter to close"
        exit 1
    }
    Write-Host "Backend built." -ForegroundColor Green
}

# ---- Run the backend test suite (validation of business rules) ----
$testsProject = "$root\GasStationBilling.Api.Tests\GasStationBilling.Api.Tests.csproj"
if (Test-Path $testsProject) {
    Write-Host "Running backend tests..." -ForegroundColor Cyan
    Push-Location $root
    dotnet test $testsProject --nologo -v q
    Pop-Location
    Write-Host ""
}

# ---- Start both servers ----
Write-Host "Starting backend on http://localhost:5000 ..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$backendDir'; dotnet run"

Write-Host "Starting frontend on http://localhost:5173 ..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$frontendDir'; npm run dev"

Write-Host ""
Write-Host "Waiting for both to start..." -ForegroundColor Cyan
Start-Sleep -Seconds 6

Start-Process "http://localhost:5173"

Write-Host ""
Write-Host "Done. 2 new windows opened (backend and frontend)." -ForegroundColor Green
Write-Host "To shut everything down, close those 2 windows." -ForegroundColor Green
