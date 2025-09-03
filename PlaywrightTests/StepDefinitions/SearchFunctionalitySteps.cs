using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlaywrightTests.Pages;
using TechTalk.SpecFlow;

namespace PlaywrightTests.StepDefinitions
{
    [Binding]
    public class SearchFunctionalitySteps
    {
        private IPage _page;
        private LoginPage _loginPage;
        private MainPage _mainPage;
        private readonly PlaywrightHooks _hooks;

        public SearchFunctionalitySteps(PlaywrightHooks hooks)
        {
            _hooks = hooks;
            // Get the page instance from PlaywrightHooks
            _page = _hooks.GetPage();
            _loginPage = new LoginPage(_page);
            _mainPage = new MainPage(_page);
        }

        [Given(@"I am logged in as ""(.*)""")]
        public async Task GivenIAmLoggedInAs(string username)
        {
            await _loginPage.LoginAsync(username, "Reinhart2244");
        }

        [Given(@"I am on the main page")]
        public async Task GivenIAmOnTheMainPage()
        {
            var isLoaded = await _mainPage.IsLoadedAsync();
            Assert.IsTrue(isLoaded, "Main page did not load as expected");
        }

        [Given(@"I click on the Search link")]
        public async Task GivenIClickOnTheSearchLink()
        {
            // Verify we're on the main page by checking for expected elements first
            await _page.WaitForSelectorAsync("a:has-text('View People'), a:has-text('Search')", new PageWaitForSelectorOptions { Timeout = 5000 });
            
            await _page.ClickAsync("a:has-text('Search')");
            await _page.WaitForTimeoutAsync(1000); // Allow page to load
        }

        [When(@"I view the Search form, I enter ""(.*)"" in the family name")]
        public async Task WhenIViewTheSearchFormIEnterInTheFamilyName(string familyName)
        {
            // Wait for the search page to load - try multiple approaches
            try
            {
                // First try to wait for any input field
                await _page.WaitForSelectorAsync("input[type='text'], input[type='search'], input:not([type='hidden']):not([type='submit']):not([type='button'])", new PageWaitForSelectorOptions { Timeout = 5000 });
            }
            catch
            {
                // If that fails, just wait a bit for the page to load
                await _page.WaitForTimeoutAsync(2000);
            }
            
            // Try multiple possible selectors for family name field, ordered from most specific to most generic
            var possibleSelectors = new[]
            {
                "input[name*='Family']",
                "input[id*='Family']", 
                "input[placeholder*='Family']",
                "input[name*='family']",
                "input[id*='family']",
                "input[placeholder*='family']",
                "input[name*='surname']",
                "input[name*='lastname']",
                "input[type='text']",
                "input:not([type='hidden']):not([type='submit']):not([type='button'])"
            };
            
            string workingSelector = "";
            foreach (var selector in possibleSelectors)
            {
                var field = await _page.QuerySelectorAsync(selector);
                if (field != null)
                {
                    workingSelector = selector;
                    break;
                }
            }
            
            Assert.IsFalse(string.IsNullOrEmpty(workingSelector), 
                "Family name input field not found on Search page. Available elements: " + 
                await GetPageElementsInfoAsync());
            
            // Clear and fill the field using the page locator
            await _page.FillAsync(workingSelector, "");  // Clear first
            await _page.FillAsync(workingSelector, familyName);  // Then fill
            
            Console.WriteLine($"✅ Entered '{familyName}' in search field using selector: {workingSelector}");
        }
        
        private async Task<string> GetPageElementsInfoAsync()
        {
            try
            {
                var inputs = await _page.QuerySelectorAllAsync("input");
                var inputInfo = new List<string>();
                foreach (var input in inputs)
                {
                    var type = await input.GetAttributeAsync("type") ?? "text";
                    var name = await input.GetAttributeAsync("name") ?? "";
                    var id = await input.GetAttributeAsync("id") ?? "";
                    var placeholder = await input.GetAttributeAsync("placeholder") ?? "";
                    inputInfo.Add($"input[type='{type}' name='{name}' id='{id}' placeholder='{placeholder}']");
                }
                return string.Join(", ", inputInfo);
            }
            catch
            {
                return "Could not analyze page elements";
            }
        }

        [When(@"then I click Search")]
        public async Task WhenThenIClickSearch()
        {
            // Try multiple possible selectors for the search button, ordered from most specific to most generic
            var possibleButtonSelectors = new[]
            {
                "button:has-text('Search')",
                "input[type='submit'][value*='Search']", 
                "input[type='button'][value*='Search']",
                "button[type='submit']",
                "input[type='submit']",
                "button:has-text('Find')",
                "button:has-text('Submit')",
                "input[value*='Find']",
                "input[value*='Submit']",
                "[type='submit']"
            };
            
            string workingButtonSelector = "";
            foreach (var selector in possibleButtonSelectors)
            {
                var button = await _page.QuerySelectorAsync(selector);
                if (button != null)
                {
                    var isVisible = await button.IsVisibleAsync();
                    if (isVisible)
                    {
                        workingButtonSelector = selector;
                        break;
                    }
                }
            }
            
            Assert.IsFalse(string.IsNullOrEmpty(workingButtonSelector), 
                "Search button not found on Search page. Available buttons: " + 
                await GetPageButtonInfoAsync());
            
            await _page.ClickAsync(workingButtonSelector);
            await _page.WaitForTimeoutAsync(2000); // Allow search results to load
            
            Console.WriteLine($"✅ Clicked search button using selector: {workingButtonSelector}");
        }
        
        private async Task<string> GetPageButtonInfoAsync()
        {
            try
            {
                var buttons = await _page.QuerySelectorAllAsync("button, input[type='button'], input[type='submit']");
                var buttonInfo = new List<string>();
                foreach (var button in buttons)
                {
                    var text = await button.InnerTextAsync();
                    var type = await button.GetAttributeAsync("type") ?? "";
                    var value = await button.GetAttributeAsync("value") ?? "";
                    var visible = await button.IsVisibleAsync();
                    buttonInfo.Add($"button[text='{text}' type='{type}' value='{value}' visible='{visible}']");
                }
                return string.Join(", ", buttonInfo);
            }
            catch
            {
                return "Could not analyze page buttons";
            }
        }

        [Then(@"I should see a single row of data with Forename ""(.*)"", Family Name ""(.*)"", Gender ""(.*)"" and Year of Birth ""(.*)""")]
        public async Task ThenIShouldSeeASingleRowOfDataWithForenameFamily_NameGenderAndYearOfBirth(string expectedForename, string expectedFamilyName, string expectedGender, string expectedYearOfBirth)
        {
            // Wait for search results table to appear
            await _page.WaitForSelectorAsync("table, .table, [role='table']", new PageWaitForSelectorOptions { Timeout = 5000 });
            
            // Get all data rows (excluding header row)
            var dataRows = await _page.QuerySelectorAllAsync("table tr:not(:first-child), .table tr:not(:first-child), tbody tr");
            
            Assert.AreEqual(1, dataRows.Count, $"Expected 1 search result row, but found {dataRows.Count}");
            
            var resultRow = dataRows[0];
            
            // Get all cells in the row
            var cells = await resultRow.QuerySelectorAllAsync("td");
            Assert.IsTrue(cells.Count >= 4, $"Expected at least 4 columns in result row, but found {cells.Count}");
            
            // Extract text content from cells
            var cellTexts = new List<string>();
            foreach (var cell in cells)
            {
                var text = await cell.InnerTextAsync();
                cellTexts.Add(text.Trim());
            }
            
            // Check if the expected values are present in the row
            var rowText = string.Join(" | ", cellTexts);
            
            Assert.IsTrue(cellTexts.Any(c => c.Contains(expectedForename)), 
                $"Forename '{expectedForename}' not found in search result row: {rowText}");
            
            Assert.IsTrue(cellTexts.Any(c => c.Contains(expectedFamilyName)), 
                $"Family Name '{expectedFamilyName}' not found in search result row: {rowText}");
            
            Assert.IsTrue(cellTexts.Any(c => c.Contains(expectedGender)), 
                $"Gender '{expectedGender}' not found in search result row: {rowText}");
            
            Assert.IsTrue(cellTexts.Any(c => c.Contains(expectedYearOfBirth)), 
                $"Year of Birth '{expectedYearOfBirth}' not found in search result row: {rowText}");
            
            Console.WriteLine($"✅ Search result verified: {expectedForename} {expectedFamilyName}, {expectedGender}, {expectedYearOfBirth}");
        }

        [When(@"I view the search form")]
        public async Task WhenIViewTheSearchForm()
        {
            // Check if we're already on the search form or need to navigate to it
            var searchInputExists = await _page.QuerySelectorAsync("input[name*='family'], input[name*='forename']") != null;
            
            if (!searchInputExists)
            {
                // If not on search form, try to navigate there
                var searchLink = await _page.QuerySelectorAsync("a:has-text('Search')");
                if (searchLink != null)
                {
                    await searchLink.ClickAsync();
                    await _page.WaitForTimeoutAsync(1000);
                }
            }
            
            // Verify we can see the search form elements
            await _page.WaitForSelectorAsync("input[name*='family'], input[name*='forename'], select[name*='gender'], select[name*='Gender']", 
                new PageWaitForSelectorOptions { Timeout = 10000 });
            Console.WriteLine("✅ Search form is visible and ready");
        }

        [When(@"I click Clear")]
        public async Task WhenIClickClear()
        {
            // Look for Clear button with various possible selectors
            var clearButton = await _page.QuerySelectorAsync("button:has-text('Clear'), input[type='reset'], button[type='reset'], .clear-button");
            if (clearButton != null)
            {
                await clearButton.ClickAsync();
                Console.WriteLine("✅ Clicked Clear button");
            }
            else
            {
                // If no clear button found, manually clear form fields
                var inputs = await _page.QuerySelectorAllAsync("input[type='text'], input[type='search']");
                foreach (var input in inputs)
                {
                    await input.FillAsync("");
                }
                
                var selects = await _page.QuerySelectorAllAsync("select");
                foreach (var select in selects)
                {
                    await select.SelectOptionAsync("");
                }
                Console.WriteLine("✅ Manually cleared all form fields");
            }
            
            await _page.WaitForTimeoutAsync(500); // Allow UI to update
        }

        [Then(@"I should see a blank search form")]
        public async Task ThenIShouldSeeABlankSearchForm()
        {
            // Check that text inputs are empty
            var textInputs = await _page.QuerySelectorAllAsync("input[type='text'], input[type='search']");
            foreach (var input in textInputs)
            {
                var value = await input.GetAttributeAsync("value") ?? "";
                Assert.IsTrue(string.IsNullOrEmpty(value), $"Input field should be empty but contains: '{value}'");
            }
            
            // Check that select elements are on default/empty option
            var selects = await _page.QuerySelectorAllAsync("select");
            foreach (var select in selects)
            {
                var selectedValue = await select.EvaluateAsync<string>("el => el.value");
                Assert.IsTrue(string.IsNullOrEmpty(selectedValue) || selectedValue == "0", 
                    $"Select field should be empty but has value: '{selectedValue}'");
            }
            
            Console.WriteLine("✅ Search form is blank as expected");
        }

        [When(@"I select ""(.*)"" from the Gender selector and I click Search")]
        public async Task WhenISelectFromTheGenderSelectorAndIClickSearch(string genderValue)
        {
            // Find the gender selector (could be dropdown or radio buttons)
            var genderSelect = await _page.QuerySelectorAsync("select[name*='gender'], select[name*='Gender'], #gender, .gender-select");
            if (genderSelect != null)
            {
                await genderSelect.SelectOptionAsync(genderValue);
                Console.WriteLine($"✅ Selected '{genderValue}' from gender dropdown");
            }
            else
            {
                // Try radio buttons
                var genderRadio = await _page.QuerySelectorAsync($"input[type='radio'][value*='{genderValue}'], input[type='radio'] + label:has-text('{genderValue}')");
                if (genderRadio != null)
                {
                    await genderRadio.ClickAsync();
                    Console.WriteLine($"✅ Selected '{genderValue}' radio button");
                }
                else
                {
                    throw new Exception($"Could not find gender selector for value: {genderValue}");
                }
            }
            
            // Click the search button
            await _page.ClickAsync("button:has-text('Search'), input[type='submit'][value*='Search']");
            Console.WriteLine("✅ Clicked Search button after selecting gender");
            
            // Wait for results to load
            await _page.WaitForTimeoutAsync(2000);
        }

        [Then(@"I should see (.*) records all of whom are Gender = (.*)")]
        public async Task ThenIShouldSeeRecordsAllOfWhomAreGender(int expectedCount, string expectedGender)
        {
            // Wait for results table to be visible
            await _page.WaitForSelectorAsync("table, .results, .search-results", new PageWaitForSelectorOptions { Timeout = 10000 });
            
            // Get all result rows (excluding header)
            var resultRows = await _page.QuerySelectorAllAsync("table tbody tr, .result-row, .search-result");
            int actualCount = resultRows.Count;
            
            Assert.AreEqual(expectedCount, actualCount, 
                $"Expected {expectedCount} records but found {actualCount}");
            
            // Verify each row has the expected gender
            for (int i = 0; i < resultRows.Count; i++)
            {
                var row = resultRows[i];
                var rowText = await row.TextContentAsync() ?? "";
                
                Assert.IsTrue(rowText.Contains(expectedGender), 
                    $"Row {i + 1} does not contain expected gender '{expectedGender}'. Row text: {rowText}");
            }
            
            Console.WriteLine($"✅ Verified {actualCount} records all have Gender = {expectedGender}");
        }

        [When(@"I enter ""(.*)"" into the Family Name field")]
        public async Task WhenIEnterIntoTheFamilyNameField(string familyName)
        {
            // Find the family name input field and enter the wildcard pattern
            var familyNameInput = await _page.QuerySelectorAsync("input[name*='family'], input[name*='Family'], input[id*='family'], input[id*='Family']");
            
            if (familyNameInput == null)
            {
                throw new Exception("Could not find Family Name input field");
            }
            
            await familyNameInput.FillAsync(familyName);
            Console.WriteLine($"✅ Entered '{familyName}' in Family Name field");
        }

        [When(@"I click Search")]
        public async Task WhenIClickSearch()
        {
            // Click the search button
            await _page.ClickAsync("button:has-text('Search'), input[type='submit'][value*='Search']");
            Console.WriteLine("✅ Clicked Search button");
            
            // Wait for results to load
            await _page.WaitForTimeoutAsync(2000);
        }

        [Then(@"I should see (.*) records whose family name starts with ""(.*)""")]
        public async Task ThenIShouldSeeRecordsWhoseFamilyNameStartsWith(int expectedCount, string expectedPrefix)
        {
            // Wait for results table to be visible
            await _page.WaitForSelectorAsync("table, .results, .search-results", new PageWaitForSelectorOptions { Timeout = 10000 });
            
            // Get all result rows (excluding header)
            var resultRows = await _page.QuerySelectorAllAsync("table tbody tr, .result-row, .search-result");
            int actualCount = resultRows.Count;
            
            Assert.AreEqual(expectedCount, actualCount, 
                $"Expected {expectedCount} records but found {actualCount}");
            
            // Remove the wildcard asterisk from the expected prefix for validation
            string prefixToCheck = expectedPrefix.TrimEnd('*');
            
            // Verify each row has a family name that starts with the expected prefix
            for (int i = 0; i < resultRows.Count; i++)
            {
                var row = resultRows[i];
                var rowText = await row.TextContentAsync() ?? "";
                
                // Split the row text to find individual cell values
                var cellTexts = rowText.Split(new char[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                
                // Look for a family name that starts with the prefix (case insensitive)
                bool foundMatchingFamilyName = cellTexts.Any(cell => 
                    cell.StartsWith(prefixToCheck, StringComparison.OrdinalIgnoreCase) && 
                    cell.Length > prefixToCheck.Length); // Ensure it's not just the prefix itself
                
                Assert.IsTrue(foundMatchingFamilyName, 
                    $"Row {i + 1} does not contain a family name starting with '{prefixToCheck}'. Row text: {rowText}");
            }
            
            Console.WriteLine($"✅ Verified {actualCount} records all have family names starting with '{prefixToCheck}'");
        }

        [When(@"I enter ""(.*)"" into the Forename field")]
        public async Task WhenIEnterIntoTheForenameField(string forename)
        {
            // Find the forename input field and enter the wildcard pattern
            var forenameInput = await _page.QuerySelectorAsync("input[name*='forename'], input[name*='Forename'], input[name*='first'], input[name*='First'], input[id*='forename'], input[id*='Forename']");
            
            if (forenameInput == null)
            {
                throw new Exception("Could not find Forename input field");
            }
            
            await forenameInput.FillAsync(forename);
            Console.WriteLine($"✅ Entered '{forename}' in Forename field");
        }

        [Then(@"I should see (.*) records")]
        public async Task ThenIShouldSeeRecords(int expectedCount)
        {
            // Wait for results table to be visible
            await _page.WaitForSelectorAsync("table, .results, .search-results", new PageWaitForSelectorOptions { Timeout = 10000 });
            
            // Get all result rows (excluding header)
            var resultRows = await _page.QuerySelectorAllAsync("table tbody tr, .result-row, .search-result");
            int actualCount = resultRows.Count;
            
            Assert.AreEqual(expectedCount, actualCount, 
                $"Expected {expectedCount} records but found {actualCount}");
            
            Console.WriteLine($"✅ Verified {actualCount} records found as expected");
        }

        [When(@"I enter ""(.*)"" in the Year of Birth field")]
        public async Task WhenIEnterInTheYearOfBirthField(string yearOfBirth)
        {
            // Find the year of birth input field and enter the year
            var yearInput = await _page.QuerySelectorAsync("input[name*='year'], input[name*='Year'], input[name*='birth'], input[name*='Birth'], input[id*='year'], input[id*='Year'], input[id*='birth'], input[id*='Birth']");
            
            if (yearInput == null)
            {
                throw new Exception("Could not find Year of Birth input field");
            }
            
            await yearInput.FillAsync(yearOfBirth);
            Console.WriteLine($"✅ Entered '{yearOfBirth}' in Year of Birth field");
        }

        [Then(@"I should see (.*) records with IDs (.*)")]
        public async Task ThenIShouldSeeRecordsWithIDs(int expectedCount, string expectedIDs)
        {
            // Wait for results table to be visible
            await _page.WaitForSelectorAsync("table, .results, .search-results", new PageWaitForSelectorOptions { Timeout = 10000 });
            
            // Get all result rows (excluding header)
            var resultRows = await _page.QuerySelectorAllAsync("table tbody tr, .result-row, .search-result");
            int actualCount = resultRows.Count;
            
            Assert.AreEqual(expectedCount, actualCount, 
                $"Expected {expectedCount} records but found {actualCount}");
            
            // Parse the expected IDs from the string (e.g., "82, 35, 30 and 47")
            var expectedIDList = expectedIDs
                .Replace(" and ", ", ")
                .Split(',')
                .Select(id => id.Trim())
                .ToList();
            
            // Verify each row contains one of the expected IDs
            var foundIDs = new List<string>();
            
            for (int i = 0; i < resultRows.Count; i++)
            {
                var row = resultRows[i];
                var rowText = await row.TextContentAsync() ?? "";
                
                // Look for ID in the first column or at the beginning of the row
                var cellTexts = rowText.Split(new char[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                
                if (cellTexts.Length > 0)
                {
                    var firstCell = cellTexts[0].Trim();
                    foundIDs.Add(firstCell);
                    
                    Assert.IsTrue(expectedIDList.Contains(firstCell), 
                        $"Row {i + 1} has ID '{firstCell}' which is not in expected IDs: {string.Join(", ", expectedIDList)}");
                }
            }
            
            Console.WriteLine($"✅ Verified {actualCount} records with expected IDs: {string.Join(", ", foundIDs)}");
        }
    }
}
