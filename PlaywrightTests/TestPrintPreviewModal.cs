using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlaywrightTests.Pages;
using System.Threading.Tasks;

namespace PlaywrightTests
{
    [TestClass]
    public class TestPrintPreviewModal : PageTest
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
        public async Task InvestigatePrintPreviewModal()
        {
            // Login
            var loginPage = new LoginPage(_page!);
            await loginPage.LoginAsync("davred", "Reinhart2244");

            var mainPage = new MainPage(_page!);
            Assert.IsTrue(await mainPage.IsLoadedAsync(), "Main page did not load as expected after login.");

            // Navigate to Search page
            await _page!.ClickAsync("a:text('Search')");
            await Task.Delay(2000);

            // Click Output Options to reveal the menu
            var outputOptionsBtn = await _page!.QuerySelectorAsync("button:text('Output Options'), input[type=button][value='Output Options']");
            Assert.IsNotNull(outputOptionsBtn, "Output Options button not found on Search page");
            await outputOptionsBtn.ClickAsync();

            // Wait for the export options to appear
            await Task.Delay(1000);

            Console.WriteLine("=== INVESTIGATING AVAILABLE ELEMENTS ===");

            // Look for ALL buttons and links on the page
            var allButtons = await _page!.QuerySelectorAllAsync("button, input[type='button'], input[type='submit']");
            var allLinks = await _page!.QuerySelectorAllAsync("a");

            Console.WriteLine($"Found {allButtons.Count} buttons on page:");
            foreach (var button in allButtons)
            {
                var text = await button.InnerTextAsync();
                var isVisible = await button.IsVisibleAsync();
                if (isVisible && !string.IsNullOrWhiteSpace(text))
                {
                    Console.WriteLine($"  Button: '{text.Trim()}'");
                }
            }

            Console.WriteLine($"Found {allLinks.Count} links on page:");
            foreach (var link in allLinks)
            {
                var text = await link.InnerTextAsync();
                var href = await link.GetAttributeAsync("href");
                var isVisible = await link.IsVisibleAsync();
                if (isVisible && !string.IsNullOrWhiteSpace(text))
                {
                    Console.WriteLine($"  Link: '{text.Trim()}' (href: {href})");
                }
            }

            // Take screenshot before clicking anything
            await _page!.ScreenshotAsync(new PageScreenshotOptions 
            { 
                Path = "before_any_modal_interaction.png",
                FullPage = true 
            });

            // Find the Print Preview option
            var printPreviewLink = await _page!.QuerySelectorAsync("a:text('Print Preview'), button:text('Print Preview')");
            Assert.IsNotNull(printPreviewLink, "Print Preview option not found");

            Console.WriteLine("=== ABOUT TO CLICK PRINT PREVIEW ===");
            Console.WriteLine("Looking for modal elements BEFORE clicking...");

            // Check for any existing modal elements
            var existingModals = await _page!.QuerySelectorAllAsync("[role='dialog'], .modal, iframe, [data-modal]");
            Console.WriteLine($"Existing modals before click: {existingModals.Count}");

            // NOW click Print Preview and immediately start looking for modal
            await printPreviewLink.ClickAsync();
            Console.WriteLine("✅ Print Preview clicked - immediately checking for modal elements...");

            // Check every 500ms for modal elements appearing
            for (int i = 0; i < 10; i++) // Check for 5 seconds total
            {
                await Task.Delay(500);
                
                // Look for modal elements
                var modals = await _page!.QuerySelectorAllAsync("[role='dialog'], .modal, .print-preview, iframe");
                var cancelButtons = await _page!.QuerySelectorAllAsync("button:text('Cancel'), button:text('Close'), .cancel, .close");
                var printButtons = await _page!.QuerySelectorAllAsync("button:text('Print'), input[value='Print']");
                
                Console.WriteLine($"Check #{i + 1}: Modals: {modals.Count}, Cancel buttons: {cancelButtons.Count}, Print buttons: {printButtons.Count}");
                
                if (cancelButtons.Count > 0)
                {
                    Console.WriteLine("=== FOUND CANCEL BUTTONS! ===");
                    foreach (var cancelBtn in cancelButtons)
                    {
                        var text = await cancelBtn.InnerTextAsync();
                        var isVisible = await cancelBtn.IsVisibleAsync();
                        Console.WriteLine($"  Cancel button: '{text}' - Visible: {isVisible}");
                        
                        if (isVisible)
                        {
                            // Take screenshot showing the modal
                            await _page!.ScreenshotAsync(new PageScreenshotOptions 
                            { 
                                Path = $"modal_with_cancel_button_check_{i}.png",
                                FullPage = true 
                            });
                            
                            // Click the cancel button
                            await cancelBtn.ClickAsync();
                            Console.WriteLine("✅ Clicked Cancel button!");
                            
                            // Take screenshot after clicking cancel
                            await _page!.ScreenshotAsync(new PageScreenshotOptions 
                            { 
                                Path = "after_clicking_cancel.png",
                                FullPage = true 
                            });
                            
                            return; // Test successful
                        }
                    }
                }
                
                // If we found modals but no cancel buttons, investigate further
                if (modals.Count > 0)
                {
                    Console.WriteLine("=== FOUND MODAL ELEMENTS! ===");
                    foreach (var modal in modals)
                    {
                        var tagName = await modal.EvaluateAsync<string>("el => el.tagName");
                        var className = await modal.GetAttributeAsync("class");
                        var isVisible = await modal.IsVisibleAsync();
                        Console.WriteLine($"  Modal: {tagName} - Class: {className} - Visible: {isVisible}");
                        
                        if (isVisible)
                        {
                            // Take screenshot of the modal
                            await modal.ScreenshotAsync(new ElementHandleScreenshotOptions 
                            { 
                                Path = $"modal_element_check_{i}.png"
                            });
                            
                            // Look for buttons within this modal
                            var modalButtons = await modal.QuerySelectorAllAsync("button, input[type='button']");
                            Console.WriteLine($"    Buttons in modal: {modalButtons.Count}");
                            foreach (var btn in modalButtons)
                            {
                                var btnText = await btn.InnerTextAsync();
                                Console.WriteLine($"      Modal button: '{btnText}'");
                            }
                        }
                    }
                }
            }

            Console.WriteLine("=== MODAL INVESTIGATION COMPLETE ===");
            Console.WriteLine("If no modal was found, Print Preview likely opens browser print dialog");
            
            // Final screenshot
            await _page!.ScreenshotAsync(new PageScreenshotOptions 
            { 
                Path = "final_modal_investigation.png",
                FullPage = true 
            });
        }
    }
}
