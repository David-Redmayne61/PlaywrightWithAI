# BDD Exploration Script for PlaywrightWithAI
# This script helps evaluate adding BDD/Gherkin to your existing project

param(
    [switch]$Help,
    [switch]$ShowExample,
    [switch]$SetupSpecFlow
)

if ($Help) {
    Write-Host "BDD Exploration Helper" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Usage:" -ForegroundColor Yellow
    Write-Host "  .\explore-bdd.ps1              # Show BDD information and options"
    Write-Host "  .\explore-bdd.ps1 -ShowExample # Display BDD vs Traditional comparison"
    Write-Host "  .\explore-bdd.ps1 -SetupSpecFlow # Guide to add SpecFlow to project"
    Write-Host "  .\explore-bdd.ps1 -Help        # Show this help"
    Write-Host ""
    return
}

Write-Host "🥒 BDD/Gherkin Exploration for PlaywrightWithAI" -ForegroundColor Green
Write-Host "=" * 50

if ($ShowExample) {
    Write-Host ""
    Write-Host "📊 Quick Comparison:" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "TRADITIONAL C# (Current):" -ForegroundColor Yellow
    Write-Host "await loginPage.LoginAsync(`"davred`", `"password`");" -ForegroundColor White
    Write-Host "Assert.IsTrue(await mainPage.IsLoadedAsync());" -ForegroundColor White
    Write-Host ""
    Write-Host "BDD GHERKIN (Alternative):" -ForegroundColor Yellow
    Write-Host "Given I am on the login page" -ForegroundColor White
    Write-Host "When I enter username `"davred`" and password `"password`"" -ForegroundColor White
    Write-Host "Then I should see the main page" -ForegroundColor White
    Write-Host ""
    Write-Host "📖 See BDD-PILOT-EXAMPLE.md for detailed comparison" -ForegroundColor Gray
    return
}

if ($SetupSpecFlow) {
    Write-Host ""
    Write-Host "🛠️ Setting up SpecFlow (BDD for .NET):" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "1. Add SpecFlow packages:" -ForegroundColor Yellow
    Write-Host "   dotnet add package SpecFlow.Plus.LivingDocPlugin" -ForegroundColor Gray
    Write-Host "   dotnet add package SpecFlow.MsTest" -ForegroundColor Gray
    Write-Host "   dotnet add package SpecFlow.Tools.MsBuild.Generation" -ForegroundColor Gray
    Write-Host ""
    Write-Host "2. Create Features folder and add .feature files" -ForegroundColor Yellow
    Write-Host "3. Create StepDefinitions folder for step implementations" -ForegroundColor Yellow
    Write-Host "4. Reuse existing Page Object classes in step definitions" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "📖 Full setup guide available in documentation" -ForegroundColor Gray
    return
}

Write-Host ""
Write-Host "🎯 Current Project Analysis:" -ForegroundColor Cyan

# Check current project structure
$hasTests = Test-Path "../PlaywrightTests/*.cs"
$hasPageObjects = Test-Path "../PlaywrightTests/Pages/*.cs"
$hasTestSteps = Test-Path "../Test Steps/*.md"

if ($hasTests) { Write-Host "✅ Existing C# tests found" -ForegroundColor Green }
if ($hasPageObjects) { Write-Host "✅ Page Object Model in place" -ForegroundColor Green }
if ($hasTestSteps) { Write-Host "✅ Test step documentation available" -ForegroundColor Green }

Write-Host ""
Write-Host "🤔 BDD Evaluation Questions:" -ForegroundColor Cyan
Write-Host ""

$questions = @(
    "Do product owners/business analysts need to review test scenarios?",
    "Would non-technical stakeholders benefit from readable test specs?", 
    "Is your team comfortable with Gherkin syntax learning curve?",
    "Do you need living documentation that stays current?",
    "Would you benefit from shared step definitions across scenarios?"
)

foreach ($question in $questions) {
    Write-Host "   • $question" -ForegroundColor White
}

Write-Host ""
Write-Host "📋 Next Steps:" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. 📖 Review comparison documents:" -ForegroundColor Yellow
Write-Host "   • BDD-COMPARISON.md - Detailed pros/cons analysis" -ForegroundColor White
Write-Host "   • BDD-PILOT-EXAMPLE.md - Your Test1 in both formats" -ForegroundColor White
Write-Host ""
Write-Host "2. 🎯 Team Discussion:" -ForegroundColor Yellow
Write-Host "   • Which approach feels more natural?" -ForegroundColor White
Write-Host "   • Who will be reading/maintaining the tests?" -ForegroundColor White
Write-Host "   • What are your team's pain points with current approach?" -ForegroundColor White
Write-Host ""
Write-Host "3. 🧪 Pilot Option:" -ForegroundColor Yellow
Write-Host "   • Convert one scenario to BDD format" -ForegroundColor White
Write-Host "   • Keep existing tests running" -ForegroundColor White
Write-Host "   • Evaluate which approach works better" -ForegroundColor White
Write-Host ""
Write-Host "💡 Remember: You don't have to choose just one approach!" -ForegroundColor Green
Write-Host "   Many teams successfully use both traditional and BDD tests." -ForegroundColor White
