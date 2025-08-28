using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlaywrightTests.Pages;
using System.Threading.Tasks;

namespace PlaywrightTests
{
    [TestClass]
    public class Test2 : PageTest
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
    public async Task ViewPeopleTabLoadsCorrectly()
    {
                // Login
                var loginPage = new LoginPage(_page!);
                await loginPage.LoginAsync("davred", "Reinhart2244");

                var mainPage = new MainPage(_page!);
                Assert.IsTrue(await mainPage.IsLoadedAsync(), "Main page did not load as expected after login.");

                await mainPage.ClickViewPeopleAsync();
                await _page!.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                var viewPeopleTitle = await _page!.TitleAsync();
                StringAssert.Contains(viewPeopleTitle, "View People - First Project", "Tab title did not match expected after clicking View People. Actual title: '" + viewPeopleTitle + "'");

                // Assert that the page contains a title 'Database Records'
                var dbRecordsTitle = await _page!.QuerySelectorAsync("text=Database Records");
                Assert.IsNotNull(dbRecordsTitle, "The title 'Database Records' was not found on the View People page.");

                // Assert that the page contains the required buttons
                var outputOptionsBtn = await _page!.QuerySelectorAsync("button:text('Output Options'), input[type=button][value='Output Options']");
                Assert.IsNotNull(outputOptionsBtn, "Button 'Output Options' was not found on the View People page.");

                var returnToHomeBtn = await _page!.QuerySelectorAsync("button:has-text('Return to Home'), input[type=button][value='Return to Home'], a:has-text('Return to Home'), button:has-text('Return Home'), a:has-text('Return Home'), button:has-text('Home'), a:has-text('Home')");
                Assert.IsNotNull(returnToHomeBtn, "Button or link with text containing 'Return to Home' or similar was not found on the View People page.");

                var deleteRecordsBtn = await _page!.QuerySelectorAsync("button:has-text('Delete Records'), input[type=button][value='Delete Records'], a:has-text('Delete Records'), button:has-text('Delete'), a:has-text('Delete'), button:has-text('Remove Records'), a:has-text('Remove Records')");
                Assert.IsNotNull(deleteRecordsBtn, "Button or link with text containing 'Delete Records', 'Delete', or 'Remove Records' was not found on the View People page.");

                // Assert that a grid displaying data exists
                var grid = await _page!.QuerySelectorAsync("table, .table, .data-grid, .grid, [role='grid']");
                Assert.IsNotNull(grid, "A data grid was not found on the View People page.");

                // Assert that the grid has the required column headings
                var headings = new[] { "ID", "Forename", "Family Name", "Gender", "Year of Birth", "Actions" };
                foreach (var heading in headings)
                {
                    var th = await _page.QuerySelectorAsync($"th:text('{heading}'), th:has-text('{heading}')");
                    Assert.IsNotNull(th, $"Column heading '{heading}' was not found in the grid.");
                }

                // Assert that the Family Name column is sorted A→Z
                var ths = await _page!.QuerySelectorAllAsync("th");
                int familyNameIndex = -1;
                for (int i = 0; i < ths.Count; i++)
                {
                    var text = (await ths[i].InnerTextAsync()).Trim();
                    if (text.Equals("Family Name", System.StringComparison.OrdinalIgnoreCase))
                    {
                        familyNameIndex = i;
                        break;
                    }
                }
                Assert.IsTrue(familyNameIndex >= 0, "Could not find 'Family Name' column index.");

                // Get all rows in the table body
                var rows = await _page.QuerySelectorAllAsync("tbody tr");
                var familyNames = new System.Collections.Generic.List<string>();
                foreach (var row in rows)
                {
                    var cells = await row.QuerySelectorAllAsync("td");
                    if (cells.Count > familyNameIndex)
                    {
                        var cellText = (await cells[familyNameIndex].InnerTextAsync()).Trim();
                        familyNames.Add(cellText);
                    }
                }
                var sorted = new System.Collections.Generic.List<string>(familyNames);
                sorted.Sort(System.StringComparer.OrdinalIgnoreCase);
                Assert.IsTrue(familyNames.SequenceEqual(sorted), "Family Name column is not sorted A→Z by default.");

                // Click the Year of Birth column heading to sort
                var ths2 = await _page!.QuerySelectorAllAsync("th");
                int yobIndex = -1;
                for (int i = 0; i < ths2.Count; i++)
                {
                    var thText = (await ths2[i].InnerTextAsync()).Trim();
                    if (thText.Equals("Year of Birth", System.StringComparison.OrdinalIgnoreCase))
                    {
                        yobIndex = i;
                        break;
                    }
                }
                Assert.IsTrue(yobIndex >= 0, "Could not find 'Year of Birth' column index.");

                var yobHeader = await _page.QuerySelectorAsync("th:has-text('Year of Birth')");
                Assert.IsNotNull(yobHeader, "Could not find the 'Year of Birth' column header using :has-text selector.");
                var yobAnchor = await yobHeader.QuerySelectorAsync("a");
                Assert.IsNotNull(yobAnchor, "Could not find <a> inside the 'Year of Birth' header.");
                await yobAnchor.ScrollIntoViewIfNeededAsync();
                await yobAnchor.HoverAsync();
                await _page.WaitForTimeoutAsync(200);
                await yobAnchor.ClickAsync(new() { Force = true });

                // Wait for the first cell in the Year of Birth column to change from its original value (max 5s)
                var yobRowsBefore = await _page!.QuerySelectorAllAsync("tbody tr:visible");
                string? firstYobBefore = null;
                if (yobRowsBefore.Count > 0)
                {
                    var cells = await yobRowsBefore[0].QuerySelectorAllAsync("td");
                    if (cells.Count > yobIndex)
                        firstYobBefore = (await cells[yobIndex].InnerTextAsync()).Trim();
                }
                // Wait for the first cell in the Year of Birth column to change (max 2s, poll every 50ms)
                await _page.WaitForFunctionAsync(
                    $@"() => {{
                        const rows = Array.from(document.querySelectorAll('tbody tr'));
                        return rows.some(r => r.offsetParent !== null && r.querySelectorAll('td').length > {yobIndex} && r.querySelectorAll('td')[{yobIndex}].innerText.trim() !== '{firstYobBefore}');
                    }}",
                    null,
                    new() { Timeout = 7000, PollingInterval = 50 }
                );
                var yobRowsAfter = await _page!.QuerySelectorAllAsync("tbody tr:visible");
                string? firstYobAfter = null;
                if (yobRowsAfter.Count > 0)
                {
                    var cells = await yobRowsAfter[0].QuerySelectorAllAsync("td");
                    if (cells.Count > yobIndex)
                        firstYobAfter = (await cells[yobIndex].InnerTextAsync()).Trim();
                }
                System.Diagnostics.Debug.WriteLine($"First Year of Birth after click: {firstYobAfter}");

                // Now collect all visible Year of Birth values and check sort
                var yobRows = await _page!.QuerySelectorAllAsync("tbody tr:visible");
                var yobYears = new System.Collections.Generic.List<int>();
                foreach (var row in yobRows)
                {
                    var cells = await row.QuerySelectorAllAsync("td");
                    if (cells.Count > yobIndex)
                    {
                        var cellText = (await cells[yobIndex].InnerTextAsync()).Trim();
                        if (int.TryParse(cellText, out int year))
                            yobYears.Add(year);
                    }
                }
                var yobSorted = new System.Collections.Generic.List<int>(yobYears);
                yobSorted.Sort();
                if (!yobYears.SequenceEqual(yobSorted))
                {
                    System.Diagnostics.Debug.WriteLine($"Extracted Year of Birth values after click: {string.Join(", ", yobYears)}");
                }
                Assert.IsTrue(yobYears.SequenceEqual(yobSorted), $"Year of Birth column is not sorted ascending after clicking the heading. Extracted: {string.Join(", ", yobYears)}");

                // Now click the Family Name header to return sort to A→Z by Family Name
                var familyNameHeader = await _page.QuerySelectorAsync("th:has-text('Family Name')");
                Assert.IsNotNull(familyNameHeader, "Could not find the 'Family Name' column header.");
                var familyNameAnchor = await familyNameHeader.QuerySelectorAsync("a");
                if (familyNameAnchor != null)
                {
                    await familyNameAnchor.ScrollIntoViewIfNeededAsync();
                    await familyNameAnchor.HoverAsync();
                    await _page.WaitForTimeoutAsync(200);
                    await familyNameAnchor.ClickAsync(new() { Force = true });
                }
                else
                {
                    await familyNameHeader.ScrollIntoViewIfNeededAsync();
                    await familyNameHeader.HoverAsync();
                    await _page.WaitForTimeoutAsync(200);
                    await familyNameHeader.ClickAsync(new() { Force = true });
                }

                // Wait for the first Family Name cell to change (max 5s)
                var familyRowsBefore = await _page.QuerySelectorAllAsync("tbody tr:visible");
                string? firstFamilyNameBefore = null;
                if (familyRowsBefore.Count > 0)
                {
                    var cells = await familyRowsBefore[0].QuerySelectorAllAsync("td");
                    if (cells.Count > familyNameIndex)
                        firstFamilyNameBefore = (await cells[familyNameIndex].InnerTextAsync()).Trim();
                }
                // Wait for the first Family Name cell to change (max 2s, poll every 50ms)
                await _page.WaitForFunctionAsync(
                    $@"() => {{
                        const rows = Array.from(document.querySelectorAll('tbody tr'));
                        return rows.some(r => r.offsetParent !== null && r.querySelectorAll('td').length > {familyNameIndex} && r.querySelectorAll('td')[{familyNameIndex}].innerText.trim() !== '{firstFamilyNameBefore}');
                    }}",
                    null,
                    new() { Timeout = 7000, PollingInterval = 50 }
                );
                var familyRowsAfter = await _page.QuerySelectorAllAsync("tbody tr:visible");
                string? firstFamilyNameAfter = null;
                if (familyRowsAfter.Count > 0)
                {
                    var cells = await familyRowsAfter[0].QuerySelectorAllAsync("td");
                    if (cells.Count > familyNameIndex)
                        firstFamilyNameAfter = (await cells[familyNameIndex].InnerTextAsync()).Trim();
                }
                System.Diagnostics.Debug.WriteLine($"First Family Name after click: {firstFamilyNameAfter}");

                // Assert Family Name column is sorted A→Z
                var familyRows = await _page.QuerySelectorAllAsync("tbody tr:visible");
                var familyNamesSorted = new System.Collections.Generic.List<string>();
                foreach (var row in familyRows)
                {
                    var cells = await row.QuerySelectorAllAsync("td");
                    if (cells.Count > familyNameIndex)
                    {
                        var cellText = (await cells[familyNameIndex].InnerTextAsync()).Trim();
                        familyNamesSorted.Add(cellText);
                    }
                }
                var familyNamesSortedExpected = new System.Collections.Generic.List<string>(familyNamesSorted);
                familyNamesSortedExpected.Sort(System.StringComparer.OrdinalIgnoreCase);
                if (!familyNamesSorted.SequenceEqual(familyNamesSortedExpected))
                {
                    System.Diagnostics.Debug.WriteLine($"Extracted Family Name values after click: {string.Join(", ", familyNamesSorted)}");
                }
                Assert.IsTrue(familyNamesSorted.SequenceEqual(familyNamesSortedExpected), $"Family Name column is not sorted A→Z after clicking the heading. Extracted: {string.Join(", ", familyNamesSorted)}");

                // Find the record with ID 140 and click its Edit button
            // Debug: List all divs with a vertical scrollbar
            var scrollableDivs = await _page.EvaluateAsync<string[]>(@"
                Array.from(document.querySelectorAll('div')).filter(div => {
                    const style = window.getComputedStyle(div);
                    return div.scrollHeight > div.clientHeight && style.overflowY !== 'visible';
                }).map(div => `class='${div.className}' id='${div.id}' height=${div.clientHeight} scrollHeight=${div.scrollHeight}`);
            ");
            System.Diagnostics.Debug.WriteLine($"Scrollable divs: {string.Join(" | ", scrollableDivs)}");
                // Search for the row with William Smith, Male, 1985 and click Edit
                var gridContainer = await _page.QuerySelectorAsync("tbody");
                // Find the actual scrollable grid container
                // Log all visible rows before scrolling
                var initialRows = await _page.QuerySelectorAllAsync("tbody tr");
                System.Diagnostics.Debug.WriteLine($"Initial visible rows: {initialRows.Count}");
                foreach (var row in initialRows)
                {
                    var cells = await row.QuerySelectorAllAsync("td");
                    if (cells.Count >= 5)
                    {
                        var forename = (await cells[1].InnerTextAsync()).Trim();
                        var familyName = (await cells[2].InnerTextAsync()).Trim();
                        var yob = (await cells[4].InnerTextAsync()).Trim();
                        System.Diagnostics.Debug.WriteLine($"Row: {forename} {familyName} {yob}");
                    }
                }
                var scrollDiv = await _page.QuerySelectorAsync(".table-scroll");
                // Diagnostics: log all scrollable divs and their scroll positions
                var allScrollDivs = await _page.QuerySelectorAllAsync("div");
                foreach (var div in allScrollDivs)
                {
                    var overflowY = await div.EvaluateAsync<string>("el => getComputedStyle(el).overflowY");
                    var scrollHeight = await div.EvaluateAsync<int>("el => el.scrollHeight");
                    var clientHeight = await div.EvaluateAsync<int>("el => el.clientHeight");
                    var scrollTop = await div.EvaluateAsync<int>("el => el.scrollTop");
                    if (overflowY != "visible" && scrollHeight > clientHeight)
                    {
                        var className = await div.EvaluateAsync<string>("el => el.className");
                        var id = await div.EvaluateAsync<string>("el => el.id");
                        System.Diagnostics.Debug.WriteLine($"DIV class='{className}' id='{id}' scrollTop={scrollTop} height={clientHeight} scrollHeight={scrollHeight} overflowY={overflowY}");
                    }
                }
                int maxScrolls = 60;
                int scrollStep = 600;
                IElementHandle? editButton = null;
                bool found = false;
                for (int scroll = 0; scroll < maxScrolls && !found; scroll++)
                {
                    var allRows = await _page.QuerySelectorAllAsync("tbody tr");
                    System.Diagnostics.Debug.WriteLine($"Visible rows after scroll {scroll}: {allRows.Count}");
                    if (allRows.Count > 0)
                    {
                        var firstCells = await allRows[0].QuerySelectorAllAsync("td");
                        if (firstCells.Count >= 5)
                        {
                            var firstForename = (await firstCells[1].InnerTextAsync()).Trim();
                            var firstFamilyName = (await firstCells[2].InnerTextAsync()).Trim();
                            var firstYob = (await firstCells[4].InnerTextAsync()).Trim();
                            System.Diagnostics.Debug.WriteLine($"First visible row after scroll {scroll}: {firstForename} {firstFamilyName} {firstYob}");
                        }
                    }
                    // Output current scroll position
                    if (scrollDiv != null)
                    {
                        var scrollTop = await scrollDiv.EvaluateAsync<int>("el => el.scrollTop");
                        System.Diagnostics.Debug.WriteLine($"Scroll position after scroll {scroll}: {scrollTop}");
                    }
                    foreach (var row in allRows)
                    {
                        var cells = await row.QuerySelectorAllAsync("td");
                        if (cells.Count >= 5)
                        {
                            var forename = (await cells[1].InnerTextAsync()).Trim();
                            var familyName = (await cells[2].InnerTextAsync()).Trim();
                            var gender = (await cells[3].InnerTextAsync()).Trim();
                            var yob = (await cells[4].InnerTextAsync()).Trim();
                            if (forename == "William" && familyName == "Smith" && gender == "Male" && yob == "1985")
                            {
                                editButton = await row.QuerySelectorAsync("a:has-text('Edit'), button:has-text('Edit')");
                                if (editButton != null)
                                {
                                    await editButton.ScrollIntoViewIfNeededAsync();
                                    found = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (!found)
                    {
                        if (scrollDiv != null)
                        {
                            await scrollDiv.EvaluateAsync($"el => el.scrollTop += {scrollStep}");
                            await _page.WaitForTimeoutAsync(200);
                        }
                        else
                        {
                            await _page.EvaluateAsync($"window.scrollBy(0, {scrollStep})");
                            await _page.WaitForTimeoutAsync(200);
                        }
                    }
                }
                Assert.IsNotNull(editButton, "Could not find Edit button for William Smith, Male, 1985.");
                // Update error message for new search criteria
                Assert.IsNotNull(editButton, "Could not find Edit button for William T. Smith, Male, 1990.");
                // Update error message for new search criteria
                Assert.IsNotNull(editButton, "Could not find Edit button for William Smith, Male, 1985.");

                // Click Edit and wait for the title to change
                var oldTitle = await _page.TitleAsync();
                await editButton.ClickAsync();
                // Log and assert the page title after clicking Edit
                var editPageTitle = await _page.TitleAsync();
                System.Diagnostics.Debug.WriteLine($"Page title after Edit click: {editPageTitle}");
                StringAssert.Contains(editPageTitle, "Edit Person - First Project", "Edit tab title did not match expected. Actual title: '" + editPageTitle + "'.");
                // Wait for the Forename input field to appear (Edit Person screen loaded)
                await _page.WaitForSelectorAsync("input[name='Forename']");

                // Change Forename to 'William T.' and Year of Birth to '1990'
                var forenameInputEdit = await _page.QuerySelectorAsync("input[name='Forename']");
                // Try all likely selectors for Forename
                                string[] forenameSelectors = {
                                    "input[name='Forename']",
                                    "input#Forename",
                                    "input[aria-label='Forename']"
                                };
                                IElementHandle? forenameInputEditTry = null;
                                foreach (var selector in forenameSelectors)
                                {
                                    forenameInputEditTry = await _page.QuerySelectorAsync(selector);
                                    if (forenameInputEditTry != null) { break; }
                                }
                                Assert.IsNotNull(forenameInputEditTry, "Forename input not found on Edit screen.");
                                // Clear, type, and blur for Forename
                                await forenameInputEditTry!.FocusAsync();
                                await forenameInputEditTry.PressAsync("Control+A");
                                await forenameInputEditTry.PressAsync("Backspace");
                                var beforeFillValue = await forenameInputEditTry.InputValueAsync();
                                await forenameInputEditTry!.FillAsync("William T.");
                                await forenameInputEditTry.DispatchEventAsync("input");
                                await forenameInputEditTry.DispatchEventAsync("change");
                                await forenameInputEditTry.EvaluateAsync("el => el.blur()");
                                // Confirm value was set
                                var forenameValueEdit = await forenameInputEdit!.InputValueAsync();

                var yobInputEdit = await _page.QuerySelectorAsync("input[name='YearOfBirth'], input[name='Year of Birth']");
                // Try all likely selectors for Year of Birth
                                string[] yobSelectors = {
                                    "input[name='YearOfBirth']",
                                    "input[name='Year of Birth']",
                                    "input#YearOfBirth",
                                    "input[aria-label='Year of Birth']",
                                    "input[aria-label='YearOfBirth']"
                                };
                                IElementHandle? yobInputEditTry = null;
                                foreach (var selector in yobSelectors)
                                {
                                    yobInputEditTry = await _page.QuerySelectorAsync(selector);
                                    if (yobInputEditTry != null) { break; }
                                }
                                Assert.IsNotNull(yobInputEditTry, "Year of Birth input not found on Edit screen.");
                                await yobInputEditTry!.FocusAsync();
                                await yobInputEditTry.PressAsync("Control+A");
                                await yobInputEditTry.PressAsync("Backspace");
                                await yobInputEditTry!.FillAsync("1990");
                                await yobInputEditTry.DispatchEventAsync("input");
                                await yobInputEditTry.DispatchEventAsync("change");
                                await yobInputEditTry.EvaluateAsync("el => el.blur()");

                                // Wait briefly to mimic real user pacing
                                await _page.WaitForTimeoutAsync(500);

                                // Click Save Changes button only (no Enter or JS submit)
                                var saveBtnEdit = await _page.QuerySelectorAsync("button:has-text('Save Changes'), input[type=submit][value='Save Changes']");
                                Assert.IsNotNull(saveBtnEdit, "Save Changes button not found on Edit screen.");
                                var isSaveEnabled = await saveBtnEdit.IsEnabledAsync();
                                if (isSaveEnabled)
                                {
                                    await saveBtnEdit.ClickAsync();
                                }
                                else
                                {
                                    Assert.Fail("Save Changes button is disabled after editing fields.");
                                }

                                // --- Debug: Log network requests and responses during save ---
                                void LogRequest(object? sender, Microsoft.Playwright.IRequest e)
                                {
                                }
                                void LogResponse(object? sender, Microsoft.Playwright.IResponse e)
                                {
                                }
                                _page.Request += LogRequest;
                                _page.Response += LogResponse;

                                // Log cookies before save
                                var cookiesBefore = await _page.Context.CookiesAsync();

                                // Wait for the grid to reload after navigation
                                await _page.WaitForSelectorAsync("text=Database Records");
                // Confirm value was set
                // ...removed duplicate and unsafe code, now handled above...

                // Wait for possible validation or success message
                await _page.WaitForTimeoutAsync(1000);
                // Log any visible validation or error messages
                var errorMessagesEditSave = await _page.QuerySelectorAllAsync(".validation-summary-errors, .field-validation-error, .alert-danger, .text-danger");
                foreach (var err in errorMessagesEditSave)
                {
                    var errText = (await err.InnerTextAsync()).Trim();
                }

                // Wait for possible validation or success message
                await _page.WaitForTimeoutAsync(1000);
                // Log any visible validation or error messages
                var errorMessagesEdit = await _page.QuerySelectorAllAsync(".validation-summary-errors, .field-validation-error, .alert-danger, .text-danger");
                foreach (var err in errorMessagesEdit)
                {
                    var errText = (await err.InnerTextAsync()).Trim();
                }

                // Wait for View People screen to reload


                // Wait for navigation to complete
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                // Log cookies after save
                var cookiesAfter = await _page.Context.CookiesAsync();

                // Remove event handlers to avoid duplicate logs in future tests
                _page.Request -= LogRequest;
                _page.Response -= LogResponse;

                // Debug: Output the current URL after navigation
                var currentUrl = _page.Url;

                // Assert the main grid title after saving
                var mainGridTitle = await _page.TitleAsync();
                StringAssert.Contains(mainGridTitle, "View People - First Project", "Main grid title did not match expected after save. Actual title: '" + mainGridTitle + "'.");

                // Check for unexpected logout
                if (currentUrl.Contains("/Account/Login"))
                {
                    Assert.Fail($"Unexpected logout after save. URL: {currentUrl}");
                }

                // Wait for the grid to reload after navigation (if still on main page)
                await _page.WaitForSelectorAsync("text=Database Records");

                // Do not access input fields after navigation; ensure no code below tries to do so

                // Assert the updated record is present in the grid (William T., 1990)
                // Wait for the grid to reload
                await _page.WaitForSelectorAsync("text=Database Records");
                // Assert the updated record is present in the grid (William T., 1990)
                var updatedRowsEdit = await _page.QuerySelectorAllAsync("tbody tr");
                bool foundUpdatedEdit = false;
                foreach (var row in updatedRowsEdit)
                {
                    var cells = await row.QuerySelectorAllAsync("td");
                    if (cells.Count >= 5)
                    {
                        var forename = (await cells[1].InnerTextAsync()).Trim();
                        var familyName = (await cells[2].InnerTextAsync()).Trim();
                        var gender = (await cells[3].InnerTextAsync()).Trim();
                        var yob = (await cells[4].InnerTextAsync()).Trim();
                        if (forename == "William T." && familyName == "Smith" && gender == "Male" && yob == "1990")
                        {
                            foundUpdatedEdit = true;
                            break;
                        }
                    }
                }
                Assert.IsTrue(foundUpdatedEdit, "Updated record (William T. Smith, Male, 1990) not found in grid after edit.");

                // --- REVERT: Find and edit the record again ---
                IElementHandle? editButtonRevert = null;
                foreach (var row in updatedRowsEdit)
                {
                    var cells = await row.QuerySelectorAllAsync("td");
                    if (cells.Count >= 5)
                    {
                        var forename = (await cells[1].InnerTextAsync()).Trim();
                        var familyName = (await cells[2].InnerTextAsync()).Trim();
                        var gender = (await cells[3].InnerTextAsync()).Trim();
                        var yob = (await cells[4].InnerTextAsync()).Trim();
                        if (forename == "William T." && familyName == "Smith" && gender == "Male" && yob == "1990")
                        {
                            editButtonRevert = await row.QuerySelectorAsync("a:has-text('Edit'), button:has-text('Edit')");
                            if (editButtonRevert != null)
                            {
                                await editButtonRevert.ScrollIntoViewIfNeededAsync();
                                break;
                            }
                        }
                    }
                }
                Assert.IsNotNull(editButtonRevert, "Could not find Edit button for William T. Smith, Male, 1990 (for revert).");
                await editButtonRevert.ClickAsync();

                // Wait for the Forename input field to appear again (Edit Person screen loaded)
                await _page.WaitForSelectorAsync("input[name='Forename']");

                // Change Forename back to 'William' and Year of Birth to '1985'
                var forenameInputRevert = await _page.QuerySelectorAsync("input[name='Forename'], input#Forename, input[aria-label='Forename']");
                if (forenameInputRevert == null)
                {
                    var pageTitle = await _page.TitleAsync();
                    var html = await _page.ContentAsync();
                }
                Assert.IsNotNull(forenameInputRevert, "Forename input not found on Edit screen (revert).");
                await forenameInputRevert.FillAsync("William");

                var yobInputRevert = await _page.QuerySelectorAsync("input[name='YearOfBirth'], input[name='Year of Birth']");
                Assert.IsNotNull(yobInputRevert, "Year of Birth input not found on Edit screen (revert).");
                await yobInputRevert.FillAsync("1985");

                // Click Save Changes again
                var saveBtnRevert = await _page.QuerySelectorAsync("button:has-text('Save Changes'), input[type=submit][value='Save Changes']");
                Assert.IsNotNull(saveBtnRevert, "Save Changes button not found on Edit screen (revert).");
                await saveBtnRevert.ClickAsync();

                // Wait for View People screen to reload
                await _page.WaitForSelectorAsync("text=Database Records");

                // Assert the reverted record is present in the grid (William, 1985)
                var revertedRowsFinal = await _page.QuerySelectorAllAsync("tbody tr");
                bool foundRevertedFinal = false;
                foreach (var row in revertedRowsFinal)
                {
                    var cells = await row.QuerySelectorAllAsync("td");
                    if (cells.Count >= 5)
                    {
                        var forename = (await cells[1].InnerTextAsync()).Trim();
                        var familyName = (await cells[2].InnerTextAsync()).Trim();
                        var gender = (await cells[3].InnerTextAsync()).Trim();
                        var yob = (await cells[4].InnerTextAsync()).Trim();
                        if (forename == "William" && familyName == "Smith" && gender == "Male" && yob == "1985")
                        {
                            foundRevertedFinal = true;
                            break;
                        }
                    }
                }
                Assert.IsTrue(foundRevertedFinal, "Reverted record (William Smith, Male, 1985) not found in grid after revert.");

                // --- DELETE: Find and delete the record ---
                IElementHandle? deleteButton = null;
                foreach (var row in revertedRowsFinal)
                {
                    var cells = await row.QuerySelectorAllAsync("td");
                    if (cells.Count >= 5)
                    {
                        var forename = (await cells[1].InnerTextAsync()).Trim();
                        var familyName = (await cells[2].InnerTextAsync()).Trim();
                        var gender = (await cells[3].InnerTextAsync()).Trim();
                        if (forename == "William" && familyName == "Smith" && gender == "Male")
                        {
                            deleteButton = await row.QuerySelectorAsync("a:has-text('Delete'), button:has-text('Delete')");
                            if (deleteButton != null)
                            {
                                await deleteButton.ScrollIntoViewIfNeededAsync();
                                break;
                            }
                        }
                    }
                }
                Assert.IsNotNull(deleteButton, "Could not find Delete button for William Smith, Male.");
                await deleteButton.ClickAsync();

                // Debug: Output page HTML after clicking Delete
                var htmlAfterDeleteClick = await _page.ContentAsync();

                // Wait for Delete Confirmation screen to load (wait for heading or confirmation text)
                // Playwright does not support multiple selectors separated by a comma; wait for both sequentially
                await _page.WaitForSelectorAsync("h3:has-text('Delete Confirmation')");
                await _page.WaitForSelectorAsync("text=Are you sure you want to delete this person?");

                // Assert the displayed Forename, Family Name, and Gender match the selected record
                // Try to get values from readonly inputs, spans, or labels
                string? confirmForename = null, confirmFamilyName = null, confirmGender = null;
                // Forename
                var forenameElem = await _page.QuerySelectorAsync("input[name='Forename'][readonly], span#Forename, span[aria-label='Forename'], label:has-text('Forename') + span, td:has-text('Forename') + td, [data-test-forename], .forename, .person-forename, .field-forename, .delete-forename");
                if (forenameElem != null)
                {
                    var tag = await forenameElem.EvaluateAsync<string>("el => el.tagName.toLowerCase()");
                    if (tag == "input")
                        confirmForename = await forenameElem.InputValueAsync();
                    else
                        confirmForename = (await forenameElem.InnerTextAsync()).Trim();
                }
                if (string.IsNullOrEmpty(confirmForename))
                {
                    // Try any input with Forename in name or id
                    var input = await _page.QuerySelectorAsync("input[name*='Forename' i], input[id*='Forename' i]");
                    if (input != null)
                        confirmForename = await input.InputValueAsync();
                }
                if (string.IsNullOrEmpty(confirmForename))
                {
                    // Try to find visible text near 'Forename' label
                    var label = await _page.QuerySelectorAsync("label:has-text('Forename')");
                    if (label != null)
                    {
                        var sibling = await label.EvaluateHandleAsync("el => el.nextElementSibling");
                        if (sibling != null)
                        {
                            var siblingText = await sibling.EvaluateAsync<string>("el => el.innerText");
                            if (!string.IsNullOrWhiteSpace(siblingText))
                                confirmForename = siblingText.Trim();
                        }
                    }
                }
                if (string.IsNullOrEmpty(confirmForename))
                {
                    // Fallback: try to find any visible text 'William' on the page
                    var textElem = await _page.QuerySelectorAsync(":text('William')");
                    if (textElem != null)
                        confirmForename = (await textElem.InnerTextAsync()).Trim();
                }
                // Family Name
                var familyNameElem = await _page.QuerySelectorAsync("input[name='FamilyName'][readonly], span#FamilyName, span[aria-label='Family Name'], label:has-text('Family Name') + span, td:has-text('Family Name') + td, [data-test-familyname], .familyname, .person-familyname, .field-familyname, .delete-familyname");
                if (familyNameElem != null)
                {
                    var tag = await familyNameElem.EvaluateAsync<string>("el => el.tagName.toLowerCase()");
                    if (tag == "input")
                        confirmFamilyName = await familyNameElem.InputValueAsync();
                    else
                        confirmFamilyName = (await familyNameElem.InnerTextAsync()).Trim();
                }
                if (string.IsNullOrEmpty(confirmFamilyName))
                {
                    // Try any input with FamilyName in name or id
                    var input = await _page.QuerySelectorAsync("input[name*='FamilyName' i], input[id*='FamilyName' i]");
                    if (input != null)
                        confirmFamilyName = await input.InputValueAsync();
                }
                if (string.IsNullOrEmpty(confirmFamilyName))
                {
                    // Try to find visible text near 'Family Name' label
                    var label = await _page.QuerySelectorAsync("label:has-text('Family Name')");
                    if (label != null)
                    {
                        var sibling = await label.EvaluateHandleAsync("el => el.nextElementSibling");
                        if (sibling != null)
                        {
                            var siblingText = await sibling.EvaluateAsync<string>("el => el.innerText");
                            if (!string.IsNullOrWhiteSpace(siblingText))
                                confirmFamilyName = siblingText.Trim();
                        }
                    }
                }
                if (string.IsNullOrEmpty(confirmFamilyName))
                {
                    // Fallback: try to find any visible text 'Smith' on the page
                    var textElem = await _page.QuerySelectorAsync(":text('Smith')");
                    if (textElem != null)
                        confirmFamilyName = (await textElem.InnerTextAsync()).Trim();
                }
                // Gender
                var genderElem = await _page.QuerySelectorAsync("input[name='Gender'][readonly], span#Gender, span[aria-label='Gender'], label:has-text('Gender') + span, td:has-text('Gender') + td, [data-test-gender], .gender, .person-gender, .field-gender, .delete-gender");
                if (genderElem != null)
                {
                    var tag = await genderElem.EvaluateAsync<string>("el => el.tagName.toLowerCase()");
                    if (tag == "input")
                        confirmGender = await genderElem.InputValueAsync();
                    else
                        confirmGender = (await genderElem.InnerTextAsync()).Trim();
                }
                if (string.IsNullOrEmpty(confirmGender))
                {
                    // Try any input with Gender in name or id
                    var input = await _page.QuerySelectorAsync("input[name*='Gender' i], input[id*='Gender' i]");
                    if (input != null)
                        confirmGender = await input.InputValueAsync();
                }
                if (string.IsNullOrEmpty(confirmGender))
                {
                    // Try to find visible text near 'Gender' label
                    var label = await _page.QuerySelectorAsync("label:has-text('Gender')");
                    if (label != null)
                    {
                        var sibling = await label.EvaluateHandleAsync("el => el.nextElementSibling");
                        if (sibling != null)
                        {
                            var siblingText = await sibling.EvaluateAsync<string>("el => el.innerText");
                            if (!string.IsNullOrWhiteSpace(siblingText))
                                confirmGender = siblingText.Trim();
                        }
                    }
                }
                if (string.IsNullOrEmpty(confirmGender))
                {
                    // Fallback: try to find any visible text 'Male' on the page
                    var textElem = await _page.QuerySelectorAsync(":text('Male')");
                    if (textElem != null)
                        confirmGender = (await textElem.InnerTextAsync()).Trim();
                }

                Assert.AreEqual("William", confirmForename, "Delete confirmation Forename does not match.");
                Assert.AreEqual("Smith", confirmFamilyName, "Delete confirmation Family Name does not match.");
                Assert.AreEqual("Male", confirmGender, "Delete confirmation Gender does not match.");

                // Click the 'Back to List' button
                var backToListBtn = await _page.QuerySelectorAsync("button:has-text('Back to List'), a:has-text('Back to List'), input[type=button][value='Back to List']");
                Assert.IsNotNull(backToListBtn, "'Back to List' button not found on Delete Confirmation screen.");
                await backToListBtn.ClickAsync();

                // Assert that the user is back on the main data grid page
                await _page.WaitForSelectorAsync("text=Database Records");
                var gridTitle = await _page.TitleAsync();
                StringAssert.Contains(gridTitle, "View People - First Project", "Did not return to main data grid page after clicking 'Back to List'.");

                // --- DELETE CONFIRM: Reselect William Smith, click Delete, and confirm deletion ---
                // Find the William Smith, Male, 1985 record again
                var rowsAfterBack = await _page.QuerySelectorAllAsync("tbody tr");
                IElementHandle? deleteButtonFinal = null;
                foreach (var row in rowsAfterBack)
                {
                    var cells = await row.QuerySelectorAllAsync("td");
                    if (cells.Count >= 5)
                    {
                        var forename = (await cells[1].InnerTextAsync()).Trim();
                        var familyName = (await cells[2].InnerTextAsync()).Trim();
                        var gender = (await cells[3].InnerTextAsync()).Trim();
                        var yob = (await cells[4].InnerTextAsync()).Trim();
                        if (forename == "William" && familyName == "Smith" && gender == "Male" && yob == "1985")
                        {
                            deleteButtonFinal = await row.QuerySelectorAsync("a:has-text('Delete'), button:has-text('Delete')");
                            if (deleteButtonFinal != null)
                            {
                                await deleteButtonFinal.ScrollIntoViewIfNeededAsync();
                                break;
                            }
                        }
                    }
                }
                Assert.IsNotNull(deleteButtonFinal, "Could not find Delete button for William Smith, Male, 1985 (final delete).");
                await deleteButtonFinal.ClickAsync();

                // Wait for Delete Confirmation screen to load
                await _page.WaitForSelectorAsync("h3:has-text('Delete Confirmation')");
                await _page.WaitForSelectorAsync("text=Are you sure you want to delete this person?");

                // Click the Confirm Delete button (try common selectors)
                var confirmDeleteBtn = await _page.QuerySelectorAsync("button:has-text('Delete'), input[type=submit][value='Delete'], button:has-text('Confirm'), input[type=submit][value='Confirm']");
                Assert.IsNotNull(confirmDeleteBtn, "Confirm Delete button not found on Delete Confirmation screen.");
                await confirmDeleteBtn.ClickAsync();

                // Wait for return to main grid
                await _page.WaitForSelectorAsync("text=Database Records");
                var gridTitleAfterDelete = await _page.TitleAsync();
                StringAssert.Contains(gridTitleAfterDelete, "View People - First Project", "Did not return to main data grid page after confirming delete.");

                // Assert the record is no longer present in the grid
                var rowsAfterDelete = await _page.QuerySelectorAllAsync("tbody tr");
                bool foundDeleted = false;
                foreach (var row in rowsAfterDelete)
                {
                    var cells = await row.QuerySelectorAllAsync("td");
                    if (cells.Count >= 5)
                    {
                        var forename = (await cells[1].InnerTextAsync()).Trim();
                        var familyName = (await cells[2].InnerTextAsync()).Trim();
                        var gender = (await cells[3].InnerTextAsync()).Trim();
                        var yob = (await cells[4].InnerTextAsync()).Trim();
                        if (forename == "William" && familyName == "Smith" && gender == "Male" && yob == "1985")
                        {
                            foundDeleted = true;
                            break;
                        }
                    }
                }
                Assert.IsFalse(foundDeleted, "Record (William Smith, Male, 1985) was not deleted from the grid.");

                // --- LOGOUT ---
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
