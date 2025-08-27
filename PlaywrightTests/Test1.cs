using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlaywrightTests.Pages;
using System.Threading.Tasks;

namespace PlaywrightTests
{
    [TestClass]
    public class Test1 : PageTest
    {
        private static IPlaywright? _playwright;
        private static IBrowser? _browser;
        private static IBrowserContext? _context;
        private static IPage? _page;

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
            _page.SetViewportSizeAsync(1920, 1080).GetAwaiter().GetResult();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _context?.CloseAsync().GetAwaiter().GetResult();
            _browser?.CloseAsync().GetAwaiter().GetResult();
            _playwright?.Dispose();
        }

        [TestMethod]
        public async Task LoginWithAdminCredentials()
        {
            var loginPage = new LoginPage(_page!);
            await loginPage.LoginAsync("davred", "Reinhart2244");

            var mainPage = new MainPage(_page!);
            Assert.IsTrue(await mainPage.IsLoadedAsync(), "Main page did not load as expected after login.");

            // Assert that fields with labels Forename, Family Name, Gender, and Year of Birth exist
            var forenameField = await _page!.QuerySelectorAsync("label:text('Forename')");
            Assert.IsNotNull(forenameField, "Forename field not found");
            var familyNameField = await _page!.QuerySelectorAsync("label:text('Family Name')");
            Assert.IsNotNull(familyNameField, "Family Name field not found");
            var genderField = await _page!.QuerySelectorAsync("label:text('Gender')");
            Assert.IsNotNull(genderField, "Gender field not found");
            var yearOfBirthField = await _page!.QuerySelectorAsync("label:text('Year of Birth')");
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

            // Use a flexible, case-insensitive, partial match selector for the duplicate entry error message
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
        }
    }
}