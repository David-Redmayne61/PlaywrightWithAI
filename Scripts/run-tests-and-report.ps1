# Runs Playwright MSTest tests, generates HTML report, and opens it in the default browser

$projectPath = "..\PlaywrightTests\PlaywrightTests.csproj"
$reportName = "Traditional_Tests_Report.html"
$reportPath = "..\PlaywrightTests\TestResults\$reportName"

# Run tests and generate HTML report
Write-Host "Running tests and generating HTML report..."
dotnet test $projectPath --logger:"html;LogFileName=$reportName"

# Check if report exists and open it
if (Test-Path $reportPath) {
    Write-Host "Opening HTML report: $reportPath"
    Start-Process $reportPath
} else {
    Write-Host "Report not found: $reportPath"
}
