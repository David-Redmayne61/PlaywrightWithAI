# Run Playwright JS tests and offer to open the report
Write-Host "Running Playwright JS tests..."
Set-Location "$PSScriptRoot/../playwright-js"
npx playwright test
Write-Host "To view the report, run: npx playwright show-report playwright-report"