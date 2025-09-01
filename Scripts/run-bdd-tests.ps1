# Run BDD (SpecFlow) tests specifically
# This script runs only the Gherkin/BDD tests, separate from traditional C# tests

param(
    [switch]$Help,
    [switch]$AllTests,
    [switch]$Traditional
)

if ($Help) {
    Write-Host "BDD Test Runner for PlaywrightWithAI" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Usage:" -ForegroundColor Yellow
    Write-Host "  .\run-bdd-tests.ps1            # Run only BDD/SpecFlow tests"
    Write-Host "  .\run-bdd-tests.ps1 -AllTests  # Run all tests (BDD + Traditional)"
    Write-Host "  .\run-bdd-tests.ps1 -Traditional # Run only traditional C# tests"
    Write-Host "  .\run-bdd-tests.ps1 -Help      # Show this help"
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

Write-Host "🥒 Running BDD Tests for PlaywrightWithAI" -ForegroundColor Green
Write-Host "PlaywrightTests path: $playwrightTestsPath" -ForegroundColor Gray

# Change to PlaywrightTests directory
Push-Location $playwrightTestsPath

try {
    # Clean up old results
    if (Test-Path "TestResults") {
        Remove-Item "TestResults\*BDD*", "TestResults\*SpecFlow*" -ErrorAction SilentlyContinue
    }

    if ($AllTests) {
        Write-Host "Running ALL tests (BDD + Traditional)..." -ForegroundColor Yellow
        dotnet test --logger:"html;LogFileName=AllTestsReport.html" --logger:"console;verbosity=normal"
    }
    elseif ($Traditional) {
        Write-Host "Running TRADITIONAL C# tests only..." -ForegroundColor Yellow
        dotnet test --filter "FullyQualifiedName!~StepDefinitions" --logger:"html;LogFileName=TraditionalTestsReport.html" --logger:"console;verbosity=normal"
    }
    else {
        Write-Host "Running BDD/SpecFlow tests only..." -ForegroundColor Yellow
        dotnet test --filter "FullyQualifiedName~StepDefinitions" --logger:"html;LogFileName=BDDTestsReport.html" --logger:"console;verbosity=normal"
    }

    # Check results
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "✅ Tests completed successfully!" -ForegroundColor Green
        
        if ($AllTests) {
            Write-Host "📊 Report: TestResults\AllTestsReport.html" -ForegroundColor Cyan
        }
        elseif ($Traditional) {
            Write-Host "📊 Report: TestResults\TraditionalTestsReport.html" -ForegroundColor Cyan
        }
        else {
            Write-Host "📊 Report: TestResults\BDDTestsReport.html" -ForegroundColor Cyan
        }
        
        Write-Host ""
        Write-Host "🔍 Test Type Comparison:" -ForegroundColor White
        Write-Host "• Traditional C# tests: Direct, fast, developer-focused" -ForegroundColor Gray
        Write-Host "• BDD/Gherkin tests: Business-readable, stakeholder-friendly" -ForegroundColor Gray
        Write-Host "• Both approaches can coexist in the same project!" -ForegroundColor Gray
    }
    else {
        Write-Host ""
        Write-Host "❌ Some tests failed. Check the output above for details." -ForegroundColor Red
    }
}
finally {
    # Return to original directory
    Pop-Location
}
