using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlaywrightTests.Pages;
using System.Threading.Tasks;

namespace PlaywrightTests
{
    [TestClass]
    public class Test3 : PageTest
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
        public async Task MultipleDeleteFunctionality()
        {
            // 1. Login
            var loginPage = new LoginPage(_page!);
            await loginPage.LoginAsync("davred", "Reinhart2244");

            // 2. Assert main page loads
            var mainPage = new MainPage(_page!);
            Assert.IsTrue(await mainPage.IsLoadedAsync(), "Main page did not load as expected after login.");

            // 3. Add four new records on the First Project window
            var records = new[] {
                new { Forename = "Sharon", FamilyName = "Jones", Gender = "Female", YearOfBirth = "2001" },
                new { Forename = "Andrew", FamilyName = "Franks", Gender = "Male", YearOfBirth = "1975" },
                new { Forename = "Jessica", FamilyName = "Fletcher", Gender = "Female", YearOfBirth = "1955" },
                new { Forename = "Bertholt", FamilyName = "Brancy", Gender = "Prefer not to say", YearOfBirth = "2002" }
            };
            foreach (var rec in records)
            {
                await _page!.FillAsync("input[name='Forename'], input#Forename", rec.Forename);
                await _page!.FillAsync("input[name='FamilyName'], input#FamilyName", rec.FamilyName);
                await _page!.SelectOptionAsync("select[name='Gender'], select#Gender", new[] { rec.Gender });
                await _page!.FillAsync("input[name='YearOfBirth'], input#YearOfBirth", rec.YearOfBirth);
                await _page!.Locator("button[type='submit']:visible, button:has-text('SUBMIT'):visible").First.ClickAsync();
                // Wait for success message
                var successMessage = await _page!.WaitForSelectorAsync("text=/record added successfully/i");
                Assert.IsNotNull(successMessage, $"Success message not rendered for {rec.Forename} {rec.FamilyName}");
                // Optionally, wait a moment before next entry
                await Task.Delay(500);
            }

            // 4. Assert that each new record appears in the main data grid
            // (Record creation assertion skipped as per user request)

            // Navigate to View People before attempting to delete
            await mainPage.ClickViewPeopleAsync();
            await _page!.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            await _page.WaitForSelectorAsync("text=Database Records");

            // 5. Select and delete the four new records
            // Navigate to View People if needed (uncomment if required)
            // await mainPage.ClickViewPeopleAsync();
            // await _page!.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            // await _page.WaitForSelectorAsync("text=Database Records");

            // Select checkboxes for the four new records

            // Click the 'Delete Records' button first
            // Try to find the button by exact text
            var deleteRecordsBtn = await _page.QuerySelectorAsync("button:has-text('Delete Records')");
            if (deleteRecordsBtn == null)
            {
                // Try any element with the text 'Delete Records' (not just <button>)
                deleteRecordsBtn = await _page.QuerySelectorAsync(":text('Delete Records')");
            }
            if (deleteRecordsBtn == null)
            {
                // Output all button HTML for debugging
                var allButtons = await _page.QuerySelectorAllAsync("button");
                var buttonHtml = new System.Text.StringBuilder();
                foreach (var btn in allButtons)
                {
                    var html = await btn.InnerHTMLAsync();
                    buttonHtml.AppendLine($"Button HTML: '{html}'");
                }
                Assert.Fail($"Delete Records button not found. Button HTML on page:\n{buttonHtml}");
            }
            await deleteRecordsBtn.ClickAsync();

            // Now select the records on the delete confirmation/select screen
            // Output all row texts for debugging
            var debugRows = await _page.QuerySelectorAllAsync("tbody tr");
            var debugText = new System.Text.StringBuilder();
            int rowIdx = 1;
            foreach (var row in debugRows)
            {
                var rowText = await row.InnerTextAsync();
                debugText.AppendLine($"Row {rowIdx++}: {rowText.Replace("\n", " | ")}");
            }
            System.Diagnostics.Debug.WriteLine($"Rows on multiple delete screen:\n{debugText}");
            // Optionally, also output to test failure for visibility
            Console.WriteLine($"Rows on multiple delete screen:\n{debugText}");
            var allRows = await _page.QuerySelectorAllAsync("tbody tr");
            int selectedCount = 0;
            foreach (var rec in records)
            {
                foreach (var row in allRows)
                {
                    var text = await row.InnerTextAsync();
                    // Split on whitespace, remove empty, trim, and lower
                    var columns = text.Split(new char[] {'\t', ' ', '\u00A0'}, System.StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim().ToLowerInvariant()).ToArray();
                    if (columns.Length < 4) continue;
                    var forename = rec.Forename.Trim().ToLowerInvariant();
                    var familyName = rec.FamilyName.Trim().ToLowerInvariant();
                    var gender = rec.Gender.Replace(" ", "").ToLowerInvariant();
                    var year = rec.YearOfBirth.Trim().ToLowerInvariant();
                    // Gender in grid may be concatenated (e.g. PreferNotToSay)
                    var rowGender = columns[2].Replace(" ", "");
                    if (columns[0] == familyName &&
                        columns[1] == forename &&
                        rowGender == gender &&
                        columns[3] == year)
                    {
                        var checkbox = await row.QuerySelectorAsync("input[type='checkbox']");
                        Assert.IsNotNull(checkbox, $"Checkbox not found for {rec.Forename} {rec.FamilyName} row.");
                        await checkbox.CheckAsync();
                        selectedCount++;
                        break;
                    }
                }
            }
            Assert.AreEqual(4, selectedCount, "Not all new records were selected for deletion.");

            // Assert that the Delete Selected (4) button is present
            var deleteSelectedBtn = await _page.QuerySelectorAsync("button:has-text('Delete Selected (4)')");
            Assert.IsNotNull(deleteSelectedBtn, "Delete Selected (4) button not found after selecting 4 records.");

            // Register dialog handler BEFORE clicking the button
            _page.Dialog += async (sender, dialog) =>
            {
                await dialog.AcceptAsync();
            };

            // Click the Delete Selected (4) button
            await deleteSelectedBtn.ClickAsync();

            // Wait for the success message (robust selector)
            var successMsg = await _page.WaitForSelectorAsync("text=/successfully\\s*deleted\\s*4\\s*records/i", new PageWaitForSelectorOptions { Timeout = 10000 });
            Assert.IsNotNull(successMsg, "Success message for deleting 4 records not found.");

            // Click Back to List button
            var backToListBtn = await _page.QuerySelectorAsync("button:has-text('Back to List'), a:has-text('Back to List')");
            Assert.IsNotNull(backToListBtn, "Back to List button not found after deletion.");
            await backToListBtn.ClickAsync();


            // Small pause to allow navigation
            await Task.Delay(1000);

            // Assert all deleted records are no longer present in the grid
            var rowsAfterDelete = await _page.QuerySelectorAllAsync("tbody tr");
            foreach (var rec in records)
            {
                bool stillPresent = false;
                foreach (var row in rowsAfterDelete)
                {
                    var text = await row.InnerTextAsync();
                    var columns = text.Split(new char[] {'\t', ' ', '\u00A0'}, System.StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim().ToLowerInvariant()).ToArray();
                    if (columns.Length < 4) continue;
                    var forename = rec.Forename.Trim().ToLowerInvariant();
                    var familyName = rec.FamilyName.Trim().ToLowerInvariant();
                    var gender = rec.Gender.Replace(" ", "").ToLowerInvariant();
                    var year = rec.YearOfBirth.Trim().ToLowerInvariant();
                    var rowGender = columns[2].Replace(" ", "");
                    if (columns[0] == familyName &&
                        columns[1] == forename &&
                        rowGender == gender &&
                        columns[3] == year)
                    {
                        stillPresent = true;
                        break;
                    }
                }
                Assert.IsFalse(stillPresent, $"Record for {rec.Forename} {rec.FamilyName} was not deleted from the grid.");
            }

            // Log out
            var userDropdownMenu = await _page.QuerySelectorAsync(":text('davred')");
            Assert.IsNotNull(userDropdownMenu, "User dropdown labeled with username 'davred' not found");
            await userDropdownMenu.ClickAsync();
            var logoutButton = await _page.QuerySelectorAsync("text=/logout/i");
            Assert.IsNotNull(logoutButton, "Logout button not found in user dropdown");
            await logoutButton.ClickAsync();
            var usernameInput = await _page.WaitForSelectorAsync("input[name='Username'], input#Username", new PageWaitForSelectorOptions { Timeout = 10000 });
            Assert.IsNotNull(usernameInput, "Username input was not rendered after logout");
            var loginButton = await _page.QuerySelectorAsync("button[type='submit'], button:has-text('Login')");
            Assert.IsNotNull(loginButton, "Login button was not rendered after logout");

            // Pause for manual inspection after selecting the 4 records
            await Task.Delay(3000); // 3 seconds

            // The rest of the test (navigate to View People, select, and delete) will follow in the next steps.
        }
    }
}
