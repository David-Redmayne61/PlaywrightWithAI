# Run Playwright JS tests and open the report automatically
Write-Host "Running Playwright JS tests and opening report..."
Set-Location "$PSScriptRoot/../playwright-js"
npx playwright test
npx playwright show-report playwright-report