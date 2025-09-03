using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;

namespace PlaywrightTests.StepDefinitions
{
    [Binding]
    public class PlaywrightHooks : PageTest
    {
        private static IPlaywright? _playwright;
        private static IBrowser? _browser;
        private IBrowserContext? _context;
        private IPage? _page;

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            _playwright = Microsoft.Playwright.Playwright.CreateAsync().GetAwaiter().GetResult();
            _browser = _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                Args = new[] { "--start-maximized" }
            }).GetAwaiter().GetResult();
            Console.WriteLine("🎬 Browser started - Will be reused across all scenarios");
        }

        [BeforeScenario]
        public void BeforeScenario(ScenarioContext scenarioContext)
        {
            if (_browser == null)
                throw new InvalidOperationException("Browser not initialized. Make sure BeforeTestRun has been called.");

            _context = _browser.NewContextAsync(new BrowserNewContextOptions
            {
                ViewportSize = null
            }).GetAwaiter().GetResult();
            
            _page = _context.NewPageAsync().GetAwaiter().GetResult();
            _page.SetViewportSizeAsync(1920, 1080).GetAwaiter().GetResult();
            
            Console.WriteLine($"🥒 Starting scenario: {scenarioContext.ScenarioInfo.Title} - Fresh page context");
        }

        [AfterScenario]
        public void AfterScenario(ScenarioContext scenarioContext)
        {
            Console.WriteLine($"✅ Scenario completed: {scenarioContext.ScenarioInfo.Title} - Closing page context");
            _page?.CloseAsync().GetAwaiter().GetResult();
            _context?.CloseAsync().GetAwaiter().GetResult();
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            Console.WriteLine("🎬 All tests completed - Closing browser");
            _browser?.CloseAsync().GetAwaiter().GetResult();
            _playwright?.Dispose();
        }

        public IPage GetPage()
        {
            if (_page == null)
                throw new InvalidOperationException("Page not initialized. Make sure BeforeScenario has been called.");
            return _page;
        }
    }
}
