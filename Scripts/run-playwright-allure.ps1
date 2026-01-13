# Run Playwright Tests with Allure Reporting
# This script runs the JavaScript Playwright tests and generates/opens the Allure report

param(
    [switch]$NoOpen,
    [switch]$Help
)

if ($Help) {
    Write-Host "Run Playwright Tests with Allure Reports" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Usage:" -ForegroundColor Yellow
    Write-Host "  .\run-playwright-allure.ps1           # Run tests and open Allure report"
    Write-Host "  .\run-playwright-allure.ps1 -NoOpen   # Run tests without opening report"
    Write-Host "  .\run-playwright-allure.ps1 -Help     # Show this help"
    Write-Host ""
    Write-Host "Description:" -ForegroundColor Yellow
    Write-Host "  1. Runs all Playwright tests (JavaScript)"
    Write-Host "  2. Generates Allure report from results"
    Write-Host "  3. Opens report in browser (unless -NoOpen is specified)"
    Write-Host ""
    return
}

# Determine paths
$scriptPath = $PSScriptRoot
$projectRoot = Split-Path $scriptPath -Parent
$playwrightJsPath = Join-Path $projectRoot "playwright-js"

# Check if playwright-js directory exists
if (-not (Test-Path $playwrightJsPath)) {
    Write-Host "Error: playwright-js directory not found at: $playwrightJsPath" -ForegroundColor Red
    exit 1
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Playwright Tests with Allure Reporting" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Run Playwright tests
Write-Host "[Step 1/3] Running Playwright tests..." -ForegroundColor Yellow
Push-Location $playwrightJsPath
try {
    npx playwright test
    $testExitCode = $LASTEXITCODE
    
    if ($testExitCode -ne 0) {
        Write-Host ""
        Write-Host "Warning: Some tests failed (exit code: $testExitCode)" -ForegroundColor Yellow
        Write-Host "Continuing to generate report..." -ForegroundColor Yellow
    } else {
        Write-Host ""
        Write-Host "All tests passed!" -ForegroundColor Green
    }
} catch {
    Write-Host "Error running tests: $_" -ForegroundColor Red
    Pop-Location
    exit 1
}

# Step 2: Generate Allure report
Write-Host ""
Write-Host "[Step 2/3] Generating Allure report..." -ForegroundColor Yellow
try {
    npx allure generate allure-results --clean -o allure-report
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Error generating Allure report" -ForegroundColor Red
        Pop-Location
        exit 1
    }
    Write-Host "Report generated successfully!" -ForegroundColor Green
} catch {
    Write-Host "Error generating Allure report: $_" -ForegroundColor Red
    Pop-Location
    exit 1
}

# Step 3: Open Allure report (unless -NoOpen is specified)
if (-not $NoOpen) {
    Write-Host ""
    Write-Host "[Step 3/3] Opening Allure report in browser..." -ForegroundColor Yellow
    try {
        Start-Process "npx" -ArgumentList "allure", "open", "allure-report" -NoNewWindow
        Write-Host "Allure report server started!" -ForegroundColor Green
        Write-Host ""
        Write-Host "Press Ctrl+C to stop the Allure server when done." -ForegroundColor Cyan
    } catch {
        Write-Host "Error opening Allure report: $_" -ForegroundColor Red
        Write-Host "You can manually open it with: npx allure open allure-report" -ForegroundColor Yellow
    }
} else {
    Write-Host ""
    Write-Host "Report generated. To view it, run:" -ForegroundColor Yellow
    Write-Host "  cd playwright-js" -ForegroundColor Cyan
    Write-Host "  npx allure open allure-report" -ForegroundColor Cyan
}

Pop-Location

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Summary:" -ForegroundColor Cyan
Write-Host "  Test Exit Code: $testExitCode" -ForegroundColor $(if ($testExitCode -eq 0) { "Green" } else { "Yellow" })
Write-Host "  Report Location: playwright-js/allure-report" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
