
using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
namespace PlaywrightTests;

[TestClass]
public class ExampleTest : PageTest
{
    private static IPlaywright? _playwright;
    private static IBrowser? _browser;
    private static IBrowserContext? _context;
    private static IPage? _page;

    [TestMethod]
    public async Task LoginWithAdminCredentials()
    {
        // Ensure _page is initialized before starting tracing
        if (_page == null)
            Assert.Fail("Page was not initialized.");

        // Start Playwright tracing for this test
        var projectRoot = AppContext.BaseDirectory;
        // Go up to the project root (assume bin/Debug/net9.0 or similar)
        for (int i = 0; i < 3; i++)
            projectRoot = System.IO.Directory.GetParent(projectRoot)!.FullName;
        var tracePath = System.IO.Path.Combine(projectRoot, "trace_LoginWithAdminCredentials.zip");
        try
        {
            Console.WriteLine($"[TRACE] Starting Playwright tracing for LoginWithAdminCredentials...");
            await _context!.Tracing.StartAsync(new TracingStartOptions
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TRACE] ERROR: Failed to start tracing: {ex}");
        }

        await _page.GotoAsync("https://localhost:7031");
        await _page.FillAsync("input[name='Username'], input#Username", "davred");
        await _page.FillAsync("input[name='Password'], input#Password", "Reinhart2244");
        await _page.ClickAsync("button[type='submit'], button:has-text('Login')");

        // Debug: Check for login error messages
        var loginError = await _page.QuerySelectorAsync(".validation-summary-errors, .text-danger, .alert-danger");
        if (loginError != null)
        {
            var errorText = await loginError.InnerTextAsync();
            Console.WriteLine($"[LOGIN DEBUG] Login error message: {errorText}");
        }

        // Wait for a reliable post-login element (menu link 'FirstProject')
        await _page.WaitForSelectorAsync("a:text('FirstProject')");

        // Check the new tab title
        var newTitle = await _page.TitleAsync();
        Console.WriteLine($"Page title after login: '{newTitle}'");
        StringAssert.Contains(newTitle, "First Project", "Page title did not contain 'First Project'. Actual title: '" + newTitle + "'");

        // Assert that fields with labels Forename, Family Name, Gender, and Year of Birth exist
        var forenameField = await _page.QuerySelectorAsync("label:text('Forename')");
        if (forenameField == null)
        {
            // Debug: print page content and take a screenshot if Forename field is not found
            var debugContent = await _page.ContentAsync();
            Console.WriteLine("[DEBUG] Page content after login (Forename field not found):\n" + debugContent);
            var screenshotPath = System.IO.Path.Combine(System.Environment.CurrentDirectory, "login_failure.png");
            await _page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath, FullPage = true });
            Console.WriteLine($"[DEBUG] Screenshot saved to: {screenshotPath}");
        }
        Assert.IsNotNull(forenameField, "Forename field not found");
        var familyNameField = await _page.QuerySelectorAsync("label:text('Family Name')");
        Assert.IsNotNull(familyNameField, "Family Name field not found");
        var genderField = await _page.QuerySelectorAsync("label:text('Gender')");
        Assert.IsNotNull(genderField, "Gender field not found");
        var yearOfBirthField = await _page.QuerySelectorAsync("label:text('Year of Birth')");
        Assert.IsNotNull(yearOfBirthField, "Year of Birth field not found");

        // Fill in the Forename field
        await _page.FillAsync("input[name='Forename'], input#Forename", "William");
        // Fill in the Family Name field
        await _page.FillAsync("input[name='FamilyName'], input#FamilyName", "Smith");
        // Select 'Male' from the Gender options
        await _page.SelectOptionAsync("select[name='Gender'], select#Gender", new[] { "Male" });
        // Enter '1985' into Year of Birth
        await _page.FillAsync("input[name='YearOfBirth'], input#YearOfBirth", "1985");

        // Click the SUBMIT button (first time)
        await _page.Locator("button[type='submit']:visible, button:has-text('SUBMIT'):visible").First.ClickAsync();

        // Debug: print page content after submit
        var pageContent = await _page.ContentAsync();
        Console.WriteLine("Page content after submit:\n" + pageContent);

        // Use a more robust selector for the success message (case-insensitive, correct spelling)
        var successMessage = await _page.WaitForSelectorAsync("text=/record added successfully/i");
        Assert.IsNotNull(successMessage, "Success message 'Record added successfully!' was not rendered");

        // Wait 1 second to allow UI/backend to fully process before duplicate entry
        await Task.Delay(1000);

        // Reload the page to reset the form before re-entering data
        await _page.ReloadAsync();

        // Re-enter the same data to trigger duplicate entry error
        await _page.FillAsync("input[name='Forename'], input#Forename", "William");
        await _page.FillAsync("input[name='FamilyName'], input#FamilyName", "Smith");
        await _page.SelectOptionAsync("select[name='Gender'], select#Gender", new[] { "Male" });
        await _page.FillAsync("input[name='YearOfBirth'], input#YearOfBirth", "1985");
        await _page.Locator("button[type='submit']:visible, button:has-text('SUBMIT'):visible").First.ClickAsync();

        // Debug: print page content after second submit
        var pageContent2 = await _page.ContentAsync();
        Console.WriteLine("Page content after second submit:\n" + pageContent2);

        // Use a flexible, case-insensitive, partial match selector for the duplicate entry error message
        // and increase the timeout to 10 seconds
        var duplicateError = await _page.WaitForSelectorAsync(
            "text=/duplicate|already exists|williams smith/i",
            new PageWaitForSelectorOptions { Timeout = 10000 }
        );
        Assert.IsNotNull(duplicateError, "Duplicate entry error message was not rendered");

        // Assert that the top menu bar contains the required links
        var firstProjectLink = await _page.QuerySelectorAsync("a:text('FirstProject')");
        Assert.IsNotNull(firstProjectLink, "FirstProject link not found in menu bar");
        var homeLink = await _page.QuerySelectorAsync("a:text('Home')");
        Assert.IsNotNull(homeLink, "Home link not found in menu bar");
        var viewPeopleLink = await _page.QuerySelectorAsync("a:text('View People')");
        Assert.IsNotNull(viewPeopleLink, "View People link not found in menu bar");
        var searchLink = await _page.QuerySelectorAsync("a:text('Search')");
        Assert.IsNotNull(searchLink, "Search link not found in menu bar");
        var importDataLink = await _page.QuerySelectorAsync("a:text('Import Data')");
        Assert.IsNotNull(importDataLink, "Import Data link not found in menu bar");

        // Assert that a dropdown exists labeled with the current username (davred)
        var userDropdown = await _page.QuerySelectorAsync(":text('davred')");
        Assert.IsNotNull(userDropdown, "User dropdown labeled with username 'davred' not found");

        // Final stage: Logout
        // Click the user dropdown (davred)
        var userDropdownMenu = await _page.QuerySelectorAsync(":text('davred')");
        Assert.IsNotNull(userDropdownMenu, "User dropdown labeled with username 'davred' not found");
        await userDropdownMenu.ClickAsync();

        // Click the Logout option (case-insensitive, robust selector)
        var logoutButton = await _page.QuerySelectorAsync("text=/logout/i");
        Assert.IsNotNull(logoutButton, "Logout button not found in user dropdown");
        await logoutButton.ClickAsync();

    // Wait for the login screen to render (look for Username field or login button)
    var usernameInput = await _page.WaitForSelectorAsync("input[name='Username'], input#Username", new PageWaitForSelectorOptions { Timeout = 10000 });
    Assert.IsNotNull(usernameInput, "Username input was not rendered after logout");

    // Optionally, check for a login button or heading
    var loginButton = await _page.QuerySelectorAsync("button[type='submit'], button:has-text('Login')");
    Assert.IsNotNull(loginButton, "Login button was not rendered after logout");

        // Stop Playwright tracing for this test
        try
        {
            Console.WriteLine($"[TRACE] Stopping Playwright tracing. Will export to: {tracePath}");
            await _context!.Tracing.StopAsync(new TracingStopOptions { Path = tracePath });
            Console.WriteLine($"[TRACE] Trace exported to: {tracePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TRACE] ERROR: Failed to stop tracing or export trace: {ex}");
        }
    }

    [ClassInitialize]
    public static void ClassInit(TestContext context)
    {
        _playwright = Microsoft.Playwright.Playwright.CreateAsync().GetAwaiter().GetResult();
        _browser = _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions {
            Headless = false,
            Args = new[] { "--start-maximized" }
        }).GetAwaiter().GetResult();
        _context = _browser.NewContextAsync(new BrowserNewContextOptions {
            ViewportSize = null
        }).GetAwaiter().GetResult();
        _page = _context.NewPageAsync().GetAwaiter().GetResult();
        // Set viewport to 1920x1080 to simulate maximized window
        _page.SetViewportSizeAsync(1920, 1080).GetAwaiter().GetResult();
        // Start Playwright tracing for the whole test class
        try
        {
            Console.WriteLine("[TRACE] Attempting to start Playwright tracing (per class)...");
            _context!.Tracing.StartAsync(new TracingStartOptions
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true
            }).GetAwaiter().GetResult();
            Console.WriteLine("[TRACE] Tracing started (per class).");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TRACE] ERROR: Failed to start tracing: {ex}");
        }
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        // Wait 5 seconds before closing the browser so the window stays open briefly after tests
        System.Threading.Thread.Sleep(5000);
        try
        {
            if (_context != null)
            {
                var projectRoot = AppContext.BaseDirectory;
                for (int i = 0; i < 3; i++)
                    projectRoot = System.IO.Directory.GetParent(projectRoot)!.FullName;
                var tracePath = System.IO.Path.Combine(projectRoot, "trace.zip");
                Console.WriteLine($"[TRACE] Attempting to stop Playwright tracing. Will export to: {tracePath}");
                try
                {
                    _context!.Tracing.StopAsync(new TracingStopOptions { Path = tracePath }).GetAwaiter().GetResult();
                    Console.WriteLine($"[TRACE] Trace exported to: {tracePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[TRACE] ERROR: Failed to stop tracing or export trace: {ex}");
                }
                _context!.CloseAsync().GetAwaiter().GetResult();
            }
            if (_browser != null)
                _browser.CloseAsync().GetAwaiter().GetResult();
            _playwright?.Dispose();
        }
        catch (Exception ex)
        {
            // Optionally log or handle cleanup errors
            Console.WriteLine($"Cleanup error: {ex}");
        }
    }
}