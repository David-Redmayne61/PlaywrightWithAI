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
    }
}
