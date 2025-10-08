#!/usr/bin/env pwsh

param(
    [switch]$NoOpen
)

Write-Host "Running Playwright JavaScript Tests with Reports..." -ForegroundColor Green

# Change to the playwright-js directory
Set-Location "$PSScriptRoot\..\playwright-js"

# Run the tests with HTML report
Write-Host "Executing tests..." -ForegroundColor Yellow
npx playwright test tests/00-dashboard.spec.js tests/01-person-details.spec.js tests/02-person-duplicate.spec.js tests/03-export-final.spec.js tests/04-export-formats.spec.js tests/05-import-testing.spec.js

# Check if tests passed
if ($LASTEXITCODE -eq 0) {
    Write-Host "All tests passed successfully!" -ForegroundColor Green
} else {
    Write-Host "Some tests failed. Check the report for details." -ForegroundColor Red
}

# Open the HTML report unless -NoOpen is specified
if (-not $NoOpen) {
    Write-Host "Opening test report..." -ForegroundColor Cyan
    npx playwright show-report
} else {
    Write-Host "Test report available. Run 'npx playwright show-report' in the playwright-js directory to view." -ForegroundColor Cyan
}

Write-Host "Test execution completed." -ForegroundColor Green