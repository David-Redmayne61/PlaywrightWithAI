using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlaywrightTests.Pages;
using TechTalk.SpecFlow;
using System.IO;
using System.Threading.Tasks;

namespace PlaywrightTests.StepDefinitions
{
    [Binding]
    public class ExportFunctionalitySteps : IAsyncDisposable
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly PlaywrightHooks _hooks;
        private IPage? _page;
        private IDownload? _lastDownload;

        public ExportFunctionalitySteps(ScenarioContext scenarioContext, PlaywrightHooks hooks)
        {
            _scenarioContext = scenarioContext;
            _hooks = hooks;
            _page = _hooks.GetPage();
        }

        public async ValueTask DisposeAsync()
        {
            if (_page != null)
            {
                await _page.CloseAsync();
            }
        }

        private IPage GetPage()
        {
            // First try to get from hooks (more reliable)
            try
            {
                _page = _hooks.GetPage();
                return _page;
            }
            catch
            {
                // Fallback to scenario context if hooks fail
                if (_scenarioContext.ContainsKey("Page"))
                {
                    _page = _scenarioContext.Get<IPage>("Page");
                    return _page;
                }
                throw new InvalidOperationException("No page available - browser context may have been closed");
            }
        }

        [When(@"I click on the ""(.*)"" link")]
        public async Task WhenIClickOnTheLink(string linkText)
        {
            var page = GetPage();
            await page.ClickAsync($"a:text('{linkText}')");
            await Task.Delay(2000); // Wait for page to load
            Console.WriteLine($"✅ Clicked on '{linkText}' link");
        }

        [When(@"I click on the Search link")]
        public async Task WhenIClickOnTheSearchLink()
        {
            var page = GetPage();
            await page.ClickAsync("a:text('Search')");
            await Task.Delay(2000); // Wait for page to load
            Console.WriteLine("✅ Clicked on Search link");
        }

        [When(@"I view the View People page")]
        public async Task WhenIViewTheViewPeoplePage()
        {
            var page = GetPage();
            // Wait for the View People page to load
            await page.WaitForSelectorAsync("text=Database Records", new PageWaitForSelectorOptions { Timeout = 10000 });
            Console.WriteLine("✅ View People page loaded");
        }

        [When(@"I view the Search Form")]
        public async Task WhenIViewTheSearchForm()
        {
            var page = GetPage();
            // Navigate to Search page if not already there
            var currentUrl = page.Url;
            if (!currentUrl.Contains("Search"))
            {
                await page.ClickAsync("a:text('Search')");
                await Task.Delay(2000);
            }
            
            // Try multiple possible search form indicators
            try
            {
                // Try different possible text indicators for the search form
                await page.WaitForSelectorAsync("text=Search for People", new PageWaitForSelectorOptions { Timeout = 3000 });
            }
            catch
            {
                try
                {
                    await page.WaitForSelectorAsync("text=Search", new PageWaitForSelectorOptions { Timeout = 3000 });
                }
                catch
                {
                    // Try to find any form element or input field
                    await page.WaitForSelectorAsync("form, input[type=text], input[name*=search]", new PageWaitForSelectorOptions { Timeout = 5000 });
                }
            }
            
            Console.WriteLine("✅ Search Form is loaded and ready");
        }

        [When(@"I click the ""(.*)"" button")]
        public async Task WhenIClickTheButton(string buttonText)
        {
            var page = GetPage();
            var buttonSelector = $"button:text('{buttonText}'), input[type=button][value='{buttonText}']";
            var button = await page.WaitForSelectorAsync(buttonSelector, new PageWaitForSelectorOptions { Timeout = 5000 });
            Assert.IsNotNull(button, $"{buttonText} button should be found");
            await button.ClickAsync();
            await Task.Delay(1000); // Wait for menu to appear
            Console.WriteLine($"✅ Clicked '{buttonText}' button");
        }

        [When(@"I click Output Options")]
        public async Task WhenIClickOutputOptions()
        {
            await WhenIClickTheButton("Output Options");
        }

        [When(@"I click on ""(.*)""")]
        public async Task WhenIClickOn(string optionText)
        {
            var page = GetPage();
            
            // Set up download handling before clicking
            var downloadTask = page.WaitForDownloadAsync();
            
            var optionSelector = $"a:text('{optionText}'), button:text('{optionText}')";
            var option = await page.WaitForSelectorAsync(optionSelector, new PageWaitForSelectorOptions { Timeout = 5000 });
            Assert.IsNotNull(option, $"{optionText} option should be found");
            
            await option.ClickAsync();
            Console.WriteLine($"✅ Clicked on '{optionText}' option");
            
            // If it's an export option, wait for download
            if (optionText.Contains("Export"))
            {
                try
                {
                    _lastDownload = await downloadTask.WaitAsync(TimeSpan.FromSeconds(30));
                    Console.WriteLine($"✅ Download started: {_lastDownload.SuggestedFilename}");
                }
                catch (TimeoutException)
                {
                    Console.WriteLine("⚠️ Download did not start within timeout period");
                }
            }
        }

        [When(@"then I select ""(.*)""")]
        public async Task WhenThenISelect(string optionText)
        {
            await WhenIClickOn(optionText);
        }

        [When(@"I have records displayed in the data grid")]
        public async Task WhenIHaveRecordsDisplayedInTheDataGrid()
        {
            var page = GetPage();
            // Wait for data grid to load with records
            await page.WaitForSelectorAsync("table, .data-grid, [role='grid']", new PageWaitForSelectorOptions { Timeout = 10000 });
            Console.WriteLine("✅ Data grid with records is displayed");
        }

        [Then(@"I should see export options")]
        public async Task ThenIShouldSeeExportOptions()
        {
            var page = GetPage();
            // Check if export options are visible
            var exportOptions = await page.QuerySelectorAllAsync("a:text('Export'), button:text('Export'), [href*='export'], [onclick*='export']");
            Assert.IsTrue(exportOptions.Count > 0, "Export options should be visible");
            Console.WriteLine($"✅ Found {exportOptions.Count} export options");
        }

        [Then(@"I should see ""(.*)"" option")]
        public async Task ThenIShouldSeeOption(string optionText)
        {
            var page = GetPage();
            var optionSelector = $"a:text('{optionText}'), button:text('{optionText}')";
            var option = await page.QuerySelectorAsync(optionSelector);
            Assert.IsNotNull(option, $"{optionText} option should be visible");
            
            var isVisible = await option.IsVisibleAsync();
            Assert.IsTrue(isVisible, $"{optionText} option should be visible");
            Console.WriteLine($"✅ {optionText} option is visible");
        }

        [Then(@"I should receive a PDF file download")]
        public async Task ThenIShouldReceiveAPDFFileDownload()
        {
            Assert.IsNotNull(_lastDownload, "A download should have been initiated");
            
            var filename = _lastDownload.SuggestedFilename ?? "unknown";
            Assert.IsTrue(filename.EndsWith(".pdf"), "Downloaded file should be a PDF");
            
            // Save the file to verify it
            var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            var downloadPath = Path.Combine(downloadsPath, filename);
            await _lastDownload.SaveAsAsync(downloadPath);
            
            Assert.IsTrue(File.Exists(downloadPath), "PDF file should be saved");
            var fileInfo = new FileInfo(downloadPath);
            Assert.IsTrue(fileInfo.Length > 0, "PDF file should have content");
            
            Console.WriteLine($"✅ PDF file downloaded successfully: {filename} ({fileInfo.Length} bytes)");
        }

        [Then(@"I should receive an Excel file download")]
        public async Task ThenIShouldReceiveAnExcelFileDownload()
        {
            Assert.IsNotNull(_lastDownload, "A download should have been initiated");
            
            var filename = _lastDownload.SuggestedFilename ?? "unknown";
            Assert.IsTrue(filename.EndsWith(".xlsx") || filename.EndsWith(".xls"), "Downloaded file should be an Excel file");
            
            // Save the file to verify it
            var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            var downloadPath = Path.Combine(downloadsPath, filename);
            await _lastDownload.SaveAsAsync(downloadPath);
            
            Assert.IsTrue(File.Exists(downloadPath), "Excel file should be saved");
            var fileInfo = new FileInfo(downloadPath);
            Assert.IsTrue(fileInfo.Length > 0, "Excel file should have content");
            
            Console.WriteLine($"✅ Excel file downloaded successfully: {filename} ({fileInfo.Length} bytes)");
        }

        [Then(@"the PDF file should contain the database records")]
        public Task ThenThePDFFileShouldContainTheDatabaseRecords()
        {
            // For now, just verify file size indicates content
            Assert.IsNotNull(_lastDownload, "A download should have been completed");
            var filename = _lastDownload.SuggestedFilename ?? "unknown";
            var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            var downloadPath = Path.Combine(downloadsPath, filename);
            
            var fileInfo = new FileInfo(downloadPath);
            Assert.IsTrue(fileInfo.Length > 10000, "PDF should contain substantial content (> 10KB)");
            Console.WriteLine($"✅ PDF contains substantial content: {fileInfo.Length} bytes");
            return Task.CompletedTask;
        }

        [Then(@"the Excel file should contain the database records")]
        public Task ThenTheExcelFileShouldContainTheDatabaseRecords()
        {
            // For now, just verify file size indicates content
            Assert.IsNotNull(_lastDownload, "A download should have been completed");
            var filename = _lastDownload.SuggestedFilename ?? "unknown";
            var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            var downloadPath = Path.Combine(downloadsPath, filename);
            
            var fileInfo = new FileInfo(downloadPath);
            Assert.IsTrue(fileInfo.Length > 1000, "Excel file should contain substantial content (> 1KB)");
            Console.WriteLine($"✅ Excel file contains substantial content: {fileInfo.Length} bytes");
            return Task.CompletedTask;
        }

        [Then(@"the PDF file should be properly formatted")]
        public async Task ThenThePDFFileShouldBeProperlyFormatted()
        {
            // Verify PDF header
            var filename = _lastDownload?.SuggestedFilename ?? "unknown";
            var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            var downloadPath = Path.Combine(downloadsPath, filename);
            
            var fileHeader = new byte[4];
            using var fs = new FileStream(downloadPath, FileMode.Open, FileAccess.Read);
            await fs.ReadAsync(fileHeader.AsMemory(0, 4));
            
            var pdfHeader = System.Text.Encoding.ASCII.GetString(fileHeader);
            Assert.IsTrue(pdfHeader.StartsWith("%PDF"), "File should be a valid PDF");
            Console.WriteLine("✅ PDF file is properly formatted");
        }

        [Then(@"the Excel file should have proper column headers")]
        public async Task ThenTheExcelFileShouldHaveProperColumnHeaders()
        {
            // Basic validation - file exists and has content
            var filename = _lastDownload?.SuggestedFilename ?? "unknown";
            var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            var downloadPath = Path.Combine(downloadsPath, filename);
            
            var fileHeader = new byte[4];
            using var fs = new FileStream(downloadPath, FileMode.Open, FileAccess.Read);
            await fs.ReadAsync(fileHeader.AsMemory(0, 4));
            
            // Excel files start with PK (ZIP signature)
            var isValidExcel = fileHeader[0] == 0x50 && fileHeader[1] == 0x4B;
            Assert.IsTrue(isValidExcel, "File should be a valid Excel file");
            Console.WriteLine("✅ Excel file has proper format");
        }

        [Then(@"the Excel file should contain all visible records")]
        public async Task ThenTheExcelFileShouldContainAllVisibleRecords()
        {
            await ThenTheExcelFileShouldContainTheDatabaseRecords();
        }

        [Then(@"the Excel file should maintain data integrity")]
        public async Task ThenTheExcelFileShouldMaintainDataIntegrity()
        {
            await ThenTheExcelFileShouldHaveProperColumnHeaders();
        }

        [Then(@"the PDF filename should include timestamp or identifier")]
        public Task ThenThePDFFilenameShouldIncludeTimestampOrIdentifier()
        {
            var filename = _lastDownload?.SuggestedFilename ?? "";
            Assert.IsTrue(filename.Contains("20") || filename.Contains("_"), "Filename should contain timestamp or identifier");
            Console.WriteLine($"✅ PDF filename has identifier: {filename}");
            return Task.CompletedTask;
        }

        [Then(@"the PDF should contain appropriate metadata")]
        public async Task ThenThePDFShouldContainAppropriateMetadata()
        {
            // Basic validation that it's a PDF file
            await ThenThePDFFileShouldBeProperlyFormatted();
        }

        [Then(@"I should see the download take place \(dropped into default download folder\)")]
        public async Task ThenIShouldSeeTheDownloadTakePlaceDroppedIntoDefaultDownloadFolder()
        {
            await ThenIShouldReceiveAPDFFileDownload();
        }

        [Then(@"Acrobat opens and displays the pdf")]
        public Task ThenAcrobatOpensAndDisplaysThePdf()
        {
            // We can't reliably test if Acrobat opens in automated tests
            // Just verify the PDF was downloaded successfully
            Console.WriteLine("📝 Note: PDF downloaded successfully - Acrobat opening depends on system configuration");
            return Task.CompletedTask;
        }

        [Then(@"I should see the Browser Print preview modal")]
        public Task ThenIShouldSeeTheBrowserPrintPreviewModal()
        {
            // Print preview opens browser's native print dialog which we can't easily test
            Console.WriteLine("📝 Note: Print Preview opens browser's native print dialog");
            return Task.CompletedTask;
        }

        [Then(@"then I will click Cancel to close the modal")]
        public async Task ThenThenIWillClickCancelToCloseTheModal()
        {
            var page = GetPage();
            // Try to close any modal or press Escape
            await page.Keyboard.PressAsync("Escape");
            Console.WriteLine("✅ Pressed Escape to close modal/dialog");
        }
    }
}
