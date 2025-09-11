using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace PlaywrightTests
{
    [TestClass]
    public class LoginAndDashboardTests
    {
        private IBrowser? _browser;
        private IPage? _page;
        public TestContext? TestContext { get; set; }

        [TestInitialize]
        public async Task Setup()
        {
            var playwright = await Playwright.CreateAsync();
            _browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions {
                Headless = false
            });
            var context = await _browser.NewContextAsync(new BrowserNewContextOptions {
                IgnoreHTTPSErrors = true,
                ViewportSize = new ViewportSize { Width = 1920, Height = 1080 }
            });
            _page = await context.NewPageAsync();
            // Start Playwright tracing
            await _page.Context.Tracing.StartAsync(new TracingStartOptions
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });
        }
        [TestCleanup]
        public async Task Cleanup()
        {
            if (_page != null)
            {
                string traceName = "trace.zip";
                if (TestContext != null && !string.IsNullOrEmpty(TestContext.TestName))
                    traceName = $"trace_{TestContext.TestName}.zip";
                await _page.Context.Tracing.StopAsync(new TracingStopOptions
                {
                    Path = traceName
                });
                await _page.CloseAsync();
            }
            if (_browser != null)
                await _browser.CloseAsync();
        }

        [TestMethod]
        public async Task Login_ShowsDashboardWithExpectedItems()
        {
            if (_page == null) { Assert.Fail("Playwright page not initialized"); }
            // Navigate to login page
            await _page.GotoAsync("https://localhost:7031", new PageGotoOptions { Timeout = 30000 });
            Assert.IsTrue(_page.Url.Contains("localhost:7031"), $"Did not navigate to login page, current URL: {_page.Url}");
            var html = await _page.ContentAsync();
            System.IO.File.WriteAllText("LoginPage.html", html);
            Assert.IsTrue(html.Contains("Login"), "Login page HTML does not contain expected content.");
            // Wait for login form to be visible
            await _page.WaitForSelectorAsync("input[name='Username']", new PageWaitForSelectorOptions { Timeout = 20000 });
            await _page.WaitForSelectorAsync("input[name='Password']", new PageWaitForSelectorOptions { Timeout = 20000 });
            await _page.FillAsync("input[name='Username']", "Admin");
            await _page.FillAsync("input[name='Password']", "Admin123!");
            await _page.ClickAsync("button[type='submit']");
            await _page.WaitForURLAsync("**", new PageWaitForURLOptions { Timeout = 20000 });
            // Log post-login page
            var postLoginHtml = await _page.ContentAsync();
            System.IO.File.WriteAllText("PostLoginPage.html", postLoginHtml);
            Assert.IsFalse(_page.Url.Contains("Login"), "Still on login page after submitting credentials.");
            // Assert dashboard card titles and action buttons
            var dashboardItems = new[] {
                new { Title = "Search People", ButtonText = "Search Now", ButtonClass = "btn-primary" },
                new { Title = "Add Person", ButtonText = "Add Person", ButtonClass = "btn-success" },
                new { Title = "View All", ButtonText = "View All", ButtonClass = "btn-info" },
                new { Title = "Import Data", ButtonText = "Import", ButtonClass = "btn-warning" },
                new { Title = "Record Customer Call", ButtonText = "New Call", ButtonClass = "btn-danger" },
                new { Title = "View all Customer Calls", ButtonText = "View List", ButtonClass = "btn-info" },
                new { Title = "Export Data", ButtonText = "Export", ButtonClass = "btn-secondary" },
                new { Title = "Reports", ButtonText = "View Reports", ButtonClass = "btn-primary" }
            };
            foreach (var item in dashboardItems)
            {
                var titleSelector = $"h5.card-title:has-text('{item.Title}')";
                var titleExists = await _page.QuerySelectorAsync(titleSelector) != null;
                Assert.IsTrue(titleExists, $"Dashboard card title '{item.Title}' not found");
                var buttonSelector = $"a.btn.{item.ButtonClass}:has-text('{item.ButtonText}')";
                var buttonExists = await _page.QuerySelectorAsync(buttonSelector) != null;
                Assert.IsTrue(buttonExists, $"Dashboard action button '{item.ButtonText}' not found");
            }
            // Assert navigation links
            var expectedNavLinks = new[] {
                "Home", "User Management", "View People", "Add Person", "Search", "Customer Calls", "Reports", "Import Data", "Export Data"
            };
            var navLinks = await _page.Locator("nav a, .navbar a, .nav-link, .top-nav a").AllTextContentsAsync();
            foreach (var link in expectedNavLinks)
            {
                Assert.IsTrue(navLinks.Any(x => x.Trim() == link), $"Navigation link '{link}' not found at top of page");
            }
            Assert.AreEqual(9, navLinks.Count(x => expectedNavLinks.Contains(x.Trim())), "Expected 9 navigation links at top of page");
            // Assert logged in user's name (Admin) is present at top right
            var userNameSelector = ".dropdown-toggle, .user-name";
            var userNameLocator = _page.Locator(userNameSelector);
            var userNameText = await userNameLocator.TextContentAsync();
            Assert.IsTrue(userNameText != null && userNameText.Trim().ToLower().Contains("admin"), "Logged in user name 'Admin' not found at top right");
            // Assert dropdown to right of name with 3 options
            await userNameLocator.ClickAsync(); // Open dropdown
            var dropdownOptionsLocator = _page.Locator(".dropdown-menu a, .dropdown-menu button, .dropdown-menu li");
            var dropdownOptions = await dropdownOptionsLocator.AllTextContentsAsync();
            var expectedOptions = new[] { "Change Admin Password", "About", "Logout" };
            foreach (var option in expectedOptions)
            {
                Assert.IsTrue(dropdownOptions.Any(x => x.Trim() == option), $"Dropdown option '{option}' not found");
            }
            // Click About and wait for navigation
            // Open dropdown and force menu to stay open
            await _page.ClickAsync("#accountDropdown");
            await _page.EvaluateAsync("document.querySelector('.dropdown-menu[aria-labelledby=accountDropdown]').classList.add('show')");
            await _page.WaitForSelectorAsync("a.dropdown-item[href='/Home/About']", new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible, Timeout = 3000 });
            await _page.ClickAsync("a.dropdown-item[href='/Home/About']");
            await _page.WaitForURLAsync("**/Home/About");
            var aboutPageContent = await _page.ContentAsync();
            System.IO.File.WriteAllText("AboutPage.html", aboutPageContent);
            Assert.IsTrue(aboutPageContent.Contains("About"), "About page did not load correctly after clicking About.");
            // Click Return to Home button on About page and assert dashboard navigation
            var returnHomeButton = _page.Locator("a:has-text('Return to Home'), button:has-text('Return to Home')");
            await returnHomeButton.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await returnHomeButton.ClickAsync();
            var currentUrl = _page.Url;
            Console.WriteLine($"Current URL after Return to Home: {currentUrl}");
            var dashboardTitle = await _page.TextContentAsync("h1, .dashboard-title");
            Assert.IsTrue(dashboardTitle != null && dashboardTitle.Contains("Dashboard"), $"Dashboard not visible after returning home. Current URL: {currentUrl}");
        }

        }
    }
