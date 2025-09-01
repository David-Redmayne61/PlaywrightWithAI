# Sequential Test Execution: Traditional C# Tests followed by BDD Tests
# This script runs Test1.cs, Test2.cs, Test3.cs first, then all BDD feature tests
# 
# Usage: .\Scripts\run-all-tests-sequential.ps1 [-NoOpen]
# 
# Parameters:
#   -NoOpen : Skip opening reports automatically

param(
    [switch]$NoOpen = $false
)

# Color functions
function Write-Success { param($msg) Write-Host "✓ $msg" -ForegroundColor Green }
function Write-Error { param($msg) Write-Host "✗ $msg" -ForegroundColor Red }
function Write-Info { param($msg) Write-Host "ℹ $msg" -ForegroundColor Cyan }
function Write-Warning { param($msg) Write-Host "⚠ $msg" -ForegroundColor Yellow }

# Get script directory and project root
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Split-Path -Parent $scriptDir
$playwrightTestsPath = Join-Path $projectRoot "PlaywrightTests"

# Validate paths
if (-not (Test-Path $playwrightTestsPath)) {
    Write-Error "PlaywrightTests directory not found at: $playwrightTestsPath"
    Write-Warning "Please run this script from the project root or Scripts folder."
    return
}

Write-Host ""
Write-Host "🚀 SEQUENTIAL TEST EXECUTION" -ForegroundColor Magenta
Write-Host "========================================" -ForegroundColor Magenta
Write-Info "Running Traditional C# tests first, then BDD tests"
Write-Host "PlaywrightTests path: $playwrightTestsPath" -ForegroundColor Gray
Write-Host ""

# Change to PlaywrightTests directory
Push-Location $playwrightTestsPath

# Clean up old results
if (Test-Path "TestResults") {
    Remove-Item "TestResults\*.html", "TestResults\*.trx" -ErrorAction SilentlyContinue
}

# ========================
# PHASE 1: Traditional C# Tests
# ========================
Write-Host "📋 PHASE 1: Traditional C# Tests (Test1.cs → Test2.cs → Test3.cs)" -ForegroundColor Yellow
Write-Host "=================================================================" -ForegroundColor Yellow

# Build test command for TRADITIONAL C# tests only
$traditionalTestCommand = @(
    "test"
    "--filter"
    "FullyQualifiedName~PlaywrightTests.Test1|FullyQualifiedName~PlaywrightTests.Test2|FullyQualifiedName~PlaywrightTests.Test3"
    "--logger:html;LogFileName=Traditional_Tests_Report.html"
    "--logger:trx;LogFileName=Traditional_Tests_Results.trx"
    "--logger:console;verbosity=normal"
    "--settings:playwright.runsettings"
)

Write-Info "Running traditional tests..."
& dotnet $traditionalTestCommand
$traditionalExitCode = $LASTEXITCODE

# Check traditional test results
$traditionalHtmlReport = "TestResults\Traditional_Tests_Report.html"

if ($traditionalExitCode -eq 0) {
    Write-Success "Traditional C# tests completed successfully"
} else {
    Write-Error "Traditional C# tests failed (Exit Code: $traditionalExitCode)"
    Write-Warning "Continuing with BDD tests anyway..."
}

Write-Host ""

# ========================
# PHASE 2: BDD Feature Tests
# ========================
Write-Host "🥒 PHASE 2: BDD Feature Tests (Gherkin Scenarios)" -ForegroundColor Yellow
Write-Host "=================================================" -ForegroundColor Yellow

# Build test command for BDD feature tests
$bddTestCommand = @(
    "test"
    "--filter"
    "FullyQualifiedName~Features"
    "--logger:html;LogFileName=BDD_Tests_Report.html"
    "--logger:trx;LogFileName=BDD_Tests_Results.trx"
    "--logger:console;verbosity=normal"
    "--settings:playwright.runsettings"
)

Write-Info "Running BDD feature tests..."
& dotnet $bddTestCommand
$bddExitCode = $LASTEXITCODE

# Check BDD test results
$bddHtmlReport = "TestResults\BDD_Tests_Report.html"

if ($bddExitCode -eq 0) {
    Write-Success "BDD feature tests completed successfully"
} else {
    Write-Error "BDD feature tests failed (Exit Code: $bddExitCode)"
}

Write-Host ""

# ========================
# SUMMARY & REPORTS
# ========================
Write-Host "📊 EXECUTION SUMMARY" -ForegroundColor Magenta
Write-Host "====================" -ForegroundColor Magenta

# Traditional test summary
if (Test-Path $traditionalHtmlReport) {
    Write-Success "Traditional C# Tests Report: $traditionalHtmlReport"
    $fullTraditionalPath = (Get-Item $traditionalHtmlReport).FullName
} else {
    Write-Error "Traditional C# Tests Report not found"
    $fullTraditionalPath = $null
}

# BDD test summary  
if (Test-Path $bddHtmlReport) {
    Write-Success "BDD Feature Tests Report: $bddHtmlReport"
    $fullBddPath = (Get-Item $bddHtmlReport).FullName
} else {
    Write-Error "BDD Feature Tests Report not found"
    $fullBddPath = $null
}

# Overall result
$overallSuccess = ($traditionalExitCode -eq 0) -and ($bddExitCode -eq 0)

Write-Host ""
if ($overallSuccess) {
    Write-Success "🎉 ALL TESTS COMPLETED SUCCESSFULLY!"
} else {
    Write-Warning "⚠️  Some tests failed - check individual reports"
}

# Open reports if requested
if (-not $NoOpen) {
    Write-Host ""
    Write-Info "Opening test reports..."
    
    if ($fullTraditionalPath) {
        Write-Host "📄 Opening Traditional Tests Report..." -ForegroundColor Gray
        Start-Process $fullTraditionalPath
        Start-Sleep -Seconds 2  # Brief delay between opening reports
    }
    
    if ($fullBddPath) {
        Write-Host "📄 Opening BDD Tests Report..." -ForegroundColor Gray  
        Start-Process $fullBddPath
    }
}

Write-Host ""
Write-Host "📄 Report Locations:" -ForegroundColor Cyan
if ($fullTraditionalPath) {
    Write-Host "   Traditional C#: $fullTraditionalPath" -ForegroundColor White
}
if ($fullBddPath) {
    Write-Host "   BDD Features:   $fullBddPath" -ForegroundColor White
}

Write-Host ""
Write-Host "🔧 Quick Commands:" -ForegroundColor Gray
Write-Host "   Traditional only: .\Scripts\run-tests-with-reports.ps1" -ForegroundColor DarkGray
Write-Host "   BDD only:         .\Scripts\run-bdd-tests.ps1" -ForegroundColor DarkGray
Write-Host "   Sequential (all): .\Scripts\run-all-tests-sequential.ps1" -ForegroundColor DarkGray

# Return to original location
Pop-Location

Write-Host ""
Write-Host "Sequential test execution completed!" -ForegroundColor Green

# Set exit code based on overall result
if (-not $overallSuccess) {
    exit 1
}
