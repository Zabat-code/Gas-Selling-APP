@echo off
echo === Step 1: checking .NET and Node.js ===
powershell -ExecutionPolicy Bypass -File "%~dp0install-requirements.ps1"
echo.
echo === Step 2: installing project dependencies ===
powershell -ExecutionPolicy Bypass -File "%~dp0install-dependencies.ps1"
pause
