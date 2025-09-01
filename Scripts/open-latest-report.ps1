# Quick script to open the latest Playwright test report
# This script finds and opens the most recent HTML test report

param(
    [switch]$Help
)

if ($Help) {
    Write-Host "Open Latest Playwright Report" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Usage:" -ForegroundColor Yellow
    Write-Host "  .\open-latest-report.ps1      # Open the latest HTML test report"
    Write-Host "  .\open-latest-report.ps1 -Help # Show this help"
    Write-Host ""
    return
}

# Determine the correct path to PlaywrightTests directory
$scriptPath = $PSScriptRoot
$projectRoot = Split-Path $scriptPath -Parent
$playwrightTestsPath = Join-Path $projectRoot "PlaywrightTests"

if (-not (Test-Path $playwrightTestsPath)) {
    Write-Host "Error: PlaywrightTests directory not found at: $playwrightTestsPath" -ForegroundColor Red
    return
}

# Look for HTML reports
$testResultsPath = Join-Path $playwrightTestsPath "TestResults"
if (-not (Test-Path $testResultsPath)) {
    Write-Host "No TestResults directory found. Run tests first." -ForegroundColor Yellow
    return
}

# Find the most recent HTML report
$htmlReports = Get-ChildItem $testResultsPath -Filter "*.html" -ErrorAction SilentlyContinue | Sort-Object LastWriteTime -Descending

if ($htmlReports) {
    $latestReport = $htmlReports[0]
    Write-Host "Opening latest report: $($latestReport.Name)" -ForegroundColor Green
    Write-Host "Location: $($latestReport.FullName)" -ForegroundColor Gray
    
    # Open the report
    Start-Process $latestReport.FullName
    
    Write-Host ""
    Write-Host "💡 Pro tips:" -ForegroundColor Cyan
    Write-Host "   • Pin this report to your browser for quick access" -ForegroundColor White
    Write-Host "   • Use Ctrl+R to refresh after running new tests" -ForegroundColor White
    Write-Host "   • Open in VS Code with: code '$($latestReport.FullName)'" -ForegroundColor White
} else {
    Write-Host "No HTML reports found in TestResults." -ForegroundColor Yellow
    Write-Host "Run tests first with: .\run-tests-with-reports.ps1" -ForegroundColor Gray
}
