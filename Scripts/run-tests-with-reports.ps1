# Run TRADITIONAL C# Playwright tests with comprehensive reporting
# This script runs Test1.cs, Test2.cs, Test3.cs (excludes BDD tests)
# For BDD tests, use: .\Scripts\run-bdd-tests.ps1

param(
    [switch]$Help,
    [switch]$NoOpen
)

if ($Help) {
    Write-Host "Traditional C# Playwright Test Runner with Reporting" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Usage:" -ForegroundColor Yellow
    Write-Host "  .\run-tests-with-reports.ps1         # Run traditional tests and open HTML report"
    Write-Host "  .\run-tests-with-reports.ps1 -NoOpen # Run traditional tests without opening report"
    Write-Host "  .\run-tests-with-reports.ps1 -Help   # Show this help"
    Write-Host ""
    Write-Host "Note: This runs ONLY traditional C# tests (Test1.cs, Test2.cs, Test3.cs)" -ForegroundColor Gray
    Write-Host "      For BDD tests, use: .\Scripts\run-bdd-tests.ps1" -ForegroundColor Gray
    Write-Host ""
    return
}

# Determine the correct path to PlaywrightTests directory
$scriptPath = $PSScriptRoot
$projectRoot = Split-Path $scriptPath -Parent
$playwrightTestsPath = Join-Path $projectRoot "PlaywrightTests"

if (-not (Test-Path $playwrightTestsPath)) {
    Write-Host "Error: PlaywrightTests directory not found at: $playwrightTestsPath" -ForegroundColor Red
    Write-Host "Please run this script from the project root or Scripts folder." -ForegroundColor Yellow
    return
}

Write-Host "Running Playwright tests with reporting..." -ForegroundColor Green
Write-Host "PlaywrightTests path: $playwrightTestsPath" -ForegroundColor Gray

# Change to PlaywrightTests directory
Push-Location $playwrightTestsPath

# Clean up old results
if (Test-Path "TestResults") {
    Remove-Item "TestResults\*.html", "TestResults\*.trx" -ErrorAction SilentlyContinue
}

# Build test command for TRADITIONAL C# tests only (exclude BDD tests)
$testCommand = @(
    "test"
    "--filter"
    "FullyQualifiedName~PlaywrightTests.Test1|FullyQualifiedName~PlaywrightTests.Test2|FullyQualifiedName~PlaywrightTests.Test3"  # Only traditional C# tests  
    "--logger:html;LogFileName=PlaywrightReport.html"
    "--logger:trx;LogFileName=PlaywrightResults.trx"
    "--logger:console;verbosity=normal"
    "--settings:playwright.runsettings"
)

# Run tests with multiple loggers
& dotnet $testCommand

# Check if reports were generated
$htmlReport = "TestResults\PlaywrightReport.html"
$trxReport = "TestResults\PlaywrightResults.trx"

if (Test-Path $htmlReport) {
    Write-Host "✓ HTML Report generated: $htmlReport" -ForegroundColor Green
    
    # Get the full path for easier access
    $fullHtmlPath = (Get-Item $htmlReport).FullName
    
    if (-not $NoOpen) {
        Write-Host "Opening HTML report..." -ForegroundColor Yellow
        Start-Process $fullHtmlPath
    }
    
    Write-Host ""
    Write-Host "📄 Report Locations:" -ForegroundColor Cyan
    Write-Host "   HTML: $fullHtmlPath" -ForegroundColor White
    Write-Host "   You can also open it with: code '$fullHtmlPath'" -ForegroundColor Gray
} else {
    Write-Host "✗ HTML Report not found" -ForegroundColor Red
}

if (Test-Path $trxReport) {
    Write-Host "✓ TRX Report generated: $trxReport" -ForegroundColor Green
} else {
    Write-Host "✗ TRX Report not found" -ForegroundColor Red
}

# Show test results summary
Write-Host "`nTest execution completed!" -ForegroundColor Cyan
Write-Host "Check the TestResults folder for detailed reports." -ForegroundColor White

# Return to original directory
Pop-Location
