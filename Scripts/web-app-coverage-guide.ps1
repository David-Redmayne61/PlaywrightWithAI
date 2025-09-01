# Web Application Code Coverage Setup Guide
# This script demonstrates how to set up proper code coverage for web applications

Write-Host "Web Application Code Coverage Setup" -ForegroundColor Cyan
Write-Host "====================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Current Issue:" -ForegroundColor Yellow
Write-Host "• Playwright tests run in browser and test UI workflows" -ForegroundColor White
Write-Host "• Code coverage collector only measures the test project" -ForegroundColor White
Write-Host "• Your web application code (at https://localhost:7031) is not measured" -ForegroundColor White
Write-Host ""

Write-Host "Solutions for Web App Coverage:" -ForegroundColor Green
Write-Host ""

Write-Host "1. Integration Tests with TestServer (Recommended)" -ForegroundColor Cyan
Write-Host "   • Create integration tests using Microsoft.AspNetCore.Mvc.Testing" -ForegroundColor White
Write-Host "   • Test your controllers and business logic directly" -ForegroundColor White
Write-Host "   • Get meaningful code coverage of your business logic" -ForegroundColor White
Write-Host ""

Write-Host "2. Unit Tests for Business Logic" -ForegroundColor Cyan
Write-Host "   • Create unit tests for your services, repositories, and business logic" -ForegroundColor White
Write-Host "   • These will provide the best code coverage metrics" -ForegroundColor White
Write-Host ""

Write-Host "3. Web Application Runtime Coverage (Advanced)" -ForegroundColor Cyan
Write-Host "   • Configure coverage collection in your web application" -ForegroundColor White
Write-Host "   • Use tools like Coverlet with your web app" -ForegroundColor White
Write-Host "   • Requires significant setup and may impact performance" -ForegroundColor White
Write-Host ""

Write-Host "Recommendation:" -ForegroundColor Green
Write-Host "• Keep Playwright tests for UI/workflow testing (no coverage needed)" -ForegroundColor White
Write-Host "• Add integration tests for API endpoints and business logic" -ForegroundColor White
Write-Host "• Add unit tests for individual components" -ForegroundColor White
Write-Host "• Use code coverage on integration and unit tests, not UI tests" -ForegroundColor White
Write-Host ""

Write-Host "Example Project Structure:" -ForegroundColor Yellow
Write-Host "YourSolution/" -ForegroundColor Gray
Write-Host "├── YourWebApp/                 # Your ASP.NET Core app" -ForegroundColor Gray
Write-Host "├── YourWebApp.Tests/           # Unit tests (with coverage)" -ForegroundColor Gray
Write-Host "├── YourWebApp.IntegrationTests/# Integration tests (with coverage)" -ForegroundColor Gray
Write-Host "└── PlaywrightTests/            # UI tests (no coverage needed)" -ForegroundColor Gray
Write-Host ""

$response = Read-Host "Would you like to see example integration test setup? (y/n)"
if ($response -eq 'y' -or $response -eq 'Y') {
    Write-Host ""
    Write-Host "Example Integration Test Project Setup:" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "1. Create new test project:" -ForegroundColor Yellow
    Write-Host "   dotnet new mstest -n YourWebApp.IntegrationTests" -ForegroundColor Gray
    Write-Host ""
    Write-Host "2. Add packages:" -ForegroundColor Yellow
    Write-Host "   dotnet add package Microsoft.AspNetCore.Mvc.Testing" -ForegroundColor Gray
    Write-Host "   dotnet add package coverlet.collector" -ForegroundColor Gray
    Write-Host ""
    Write-Host "3. Example test:" -ForegroundColor Yellow
    Write-Host @"
[TestClass]
public class ApiControllerTests
{
    private readonly WebApplicationFactory<Program> _factory;
    
    public ApiControllerTests()
    {
        _factory = new WebApplicationFactory<Program>();
    }
    
    [TestMethod]
    public async Task GetUsers_ReturnsSuccess()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/users");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }
}
"@ -ForegroundColor Gray
}

Write-Host ""
Write-Host "This approach gives you meaningful coverage of your actual business logic!" -ForegroundColor Green
