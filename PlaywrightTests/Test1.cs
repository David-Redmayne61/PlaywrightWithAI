
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
        if (_page == null)
            Assert.Fail("Page was not initialized.");
        await _page.GotoAsync("https://localhost:7031");
        // Enter 'Admin' in the Username field
    await _page.FillAsync("input[name='Username'], input#Username", "davred");
    // Enter 'Reinhart2244' in the Password field
    await _page.FillAsync("input[name='Password'], input#Password", "Reinhart2244");
        // Click the Login button
        await _page.ClickAsync("button[type='submit'], button:has-text('Login')");

        // Wait for navigation after login
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        // Check the new tab title
        var newTitle = await _page.TitleAsync();
        Assert.IsTrue(string.Equals(newTitle, "First Project - First Project", System.StringComparison.OrdinalIgnoreCase));
    }

    [ClassInitialize]
    public static async Task ClassInit(TestContext context)
    {
        _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions {
            Headless = false,
            Args = new[] { "--start-maximized" }
        });
        _context = await _browser.NewContextAsync(new BrowserNewContextOptions {
            ViewportSize = null
        });
        _page = await _context.NewPageAsync();
        // Set viewport to 1920x1080 to simulate maximized window
        await _page.SetViewportSizeAsync(1920, 1080);
        await _context.Tracing.StartAsync(new TracingStartOptions
        {
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });
    }

    [ClassCleanup]
    public static async Task ClassCleanup()
    {
        // Wait 5 seconds before closing the browser so the window stays open briefly after tests
        await Task.Delay(5000);
        try
        {
            if (_context != null)
            {
                // Export trace.zip to the test results directory
                var resultsDir = System.IO.Path.Combine(System.Environment.CurrentDirectory, "TestResults");
                if (!System.IO.Directory.Exists(resultsDir))
                    System.IO.Directory.CreateDirectory(resultsDir);
                var tracePath = System.IO.Path.Combine(resultsDir, "trace.zip");
                await _context.Tracing.StopAsync(new TracingStopOptions { Path = tracePath });
                Console.WriteLine($"Trace exported to: {tracePath}");
                await _context.CloseAsync();
            }
            if (_browser != null)
                await _browser.CloseAsync();
            _playwright?.Dispose();
        }
        catch (Exception ex)
        {
            // Optionally log or handle cleanup errors
            Console.WriteLine($"Cleanup error: {ex}");
        }
    }
    [TestMethod]
    public async Task HasTitle()
    {
        if (_page == null)
            Assert.Fail("Page was not initialized.");
        await _page.GotoAsync("https://localhost:7031");
        var title = await _page.TitleAsync();
        Assert.IsTrue(!string.IsNullOrEmpty(title));
    }

    [TestMethod]
    public async Task TabTitleIsLoginFirstProject()
    {
        if (_page == null)
            Assert.Fail("Page was not initialized.");
        await _page.GotoAsync("https://localhost:7031");
        var title = await _page.TitleAsync();
        Assert.IsTrue(string.Equals(title, "login - First Project", System.StringComparison.OrdinalIgnoreCase));
    }



}