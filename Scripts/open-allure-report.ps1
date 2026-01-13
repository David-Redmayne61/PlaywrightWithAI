# Open Existing Allure Report
# This script opens the Allure report without running tests

param(
    [switch]$Help
)

if ($Help) {
    Write-Host "Open Existing Allure Report" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Usage:" -ForegroundColor Yellow
    Write-Host "  .\open-allure-report.ps1      # Open the existing Allure report"
    Write-Host "  .\open-allure-report.ps1 -Help # Show this help"
    Write-Host ""
    Write-Host "Note:" -ForegroundColor Yellow
    Write-Host "  This opens the last generated report without running tests."
    Write-Host "  To run tests and generate a new report, use run-playwright-allure.ps1"
    Write-Host ""
    return
}

# Determine paths
$scriptPath = $PSScriptRoot
$projectRoot = Split-Path $scriptPath -Parent
$playwrightJsPath = Join-Path $projectRoot "playwright-js"
$allureReportPath = Join-Path $playwrightJsPath "allure-report"

# Check if playwright-js directory exists
if (-not (Test-Path $playwrightJsPath)) {
    Write-Host "Error: playwright-js directory not found at: $playwrightJsPath" -ForegroundColor Red
    exit 1
}

# Check if allure-report exists
if (-not (Test-Path $allureReportPath)) {
    Write-Host "Error: No Allure report found at: $allureReportPath" -ForegroundColor Red
    Write-Host ""
    Write-Host "To generate a report, run:" -ForegroundColor Yellow
    Write-Host "  .\Scripts\run-playwright-allure.ps1" -ForegroundColor Cyan
    Write-Host ""
    exit 1
}

Write-Host "Opening Allure report..." -ForegroundColor Cyan
Write-Host ""

Push-Location $playwrightJsPath
try {
    npx allure open allure-report
} catch {
    Write-Host "Error opening Allure report: $_" -ForegroundColor Red
    Pop-Location
    exit 1
}
Pop-Location
