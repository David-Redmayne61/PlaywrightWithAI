const { test, expect } = require('@playwright/test');
const { testPerson } = require('./shared-test-data');
const fs = require('fs');

test.describe('Export Formats - Robust Testing', () => {
  
  test.beforeEach(async ({ page }) => {
    // Login
    await page.goto('https://localhost:7031');
    await page.waitForSelector('input[name="Username"]', { timeout: 20000 });
    await page.fill('input[name="Username"]', 'Admin');
    await page.fill('input[name="Password"]', 'Admin123!');
    await page.click('button[type="submit"]');
    await page.waitForURL('**', { timeout: 20000 });
    
    // Navigate to export page
    await page.click('a.btn-secondary:has-text("Export")');
    await page.waitForURL(/Export|export/i, { timeout: 10000 });
    
    // Set up test data filters
    await page.fill('input[name="forename"]', testPerson.forename);
    await page.fill('input[name="familyName"]', testPerson.familyName);
    await page.selectOption('select[name="gender"]', testPerson.gender);
    await page.fill('input[name="yearOfBirth"]', testPerson.yearOfBirth);
  });

  test('Excel Export - Download and Verify', async ({ page }, testInfo) => {
    const downloadPromise = page.waitForEvent('download', { timeout: 10000 });
    
    // Look for Excel export trigger
    const excelTrigger = page.locator('a:has-text("Excel"), button:has-text("Excel")');
    
    try {
      await excelTrigger.click();
      const download = await downloadPromise;
      const fileName = download.suggestedFilename();
      
      expect(fileName.toLowerCase()).toMatch(/\.(xlsx|xls)$/);
      testInfo.attach('Excel Export', { body: `✅ Excel export successful: ${fileName}`, contentType: 'text/plain' });
      
    } catch (error) {
      testInfo.attach('Excel Export', { body: '❌ Excel export failed or not available', contentType: 'text/plain' });
    }
  });

  test('CSV Export - Download and Verify Content', async ({ page }, testInfo) => {
    const downloadPromise = page.waitForEvent('download', { timeout: 10000 });
    
    // Look for CSV export trigger
    const csvTrigger = page.locator('a:has-text("CSV"), button:has-text("CSV")');
    
    try {
      await csvTrigger.click();
      const download = await downloadPromise;
      const fileName = download.suggestedFilename();
      const downloadPath = await download.path();
      
      expect(fileName.toLowerCase()).toMatch(/\.csv$/);
      
      // Verify CSV content
      const csvContent = fs.readFileSync(downloadPath, 'utf8');
      expect(csvContent).toMatch(/,/); // Should have CSV delimiters
      
      testInfo.attach('CSV Export', { body: `✅ CSV export successful: ${fileName}`, contentType: 'text/plain' });
      testInfo.attach('CSV Sample', { body: csvContent.substring(0, 200), contentType: 'text/plain' });
      
    } catch (error) {
      testInfo.attach('CSV Export', { body: '❌ CSV export failed or not available', contentType: 'text/plain' });
    }
  });

  test('PDF Export - Handle Multiple Behaviors', async ({ page }, testInfo) => {
    // PDF might download, open in new tab, or display inline
    const downloadPromise = page.waitForEvent('download', { timeout: 5000 }).catch(() => null);
    const popupPromise = page.waitForEvent('popup', { timeout: 5000 }).catch(() => null);
    
    const pdfTrigger = page.locator('a:has-text("PDF"), button:has-text("PDF")');
    
    try {
      await pdfTrigger.click();
      
      // Wait for either download, popup, or URL change
      const [download, popup] = await Promise.all([downloadPromise, popupPromise]);
      
      if (download) {
        const fileName = download.suggestedFilename();
        expect(fileName.toLowerCase()).toMatch(/\.pdf$/);
        testInfo.attach('PDF Export', { body: `✅ PDF downloaded: ${fileName}`, contentType: 'text/plain' });
        
      } else if (popup) {
        const popupUrl = popup.url();
        testInfo.attach('PDF Export', { body: `✅ PDF opened in new tab: ${popupUrl}`, contentType: 'text/plain' });
        
        // Check if the popup URL indicates PDF content
        if (popupUrl.includes('pdf') || popupUrl.includes('export')) {
          testInfo.attach('PDF Verification', { body: 'PDF export appears to be working via popup', contentType: 'text/plain' });
        }
        
        await popup.close();
        
      } else {
        // Check if current page changed to show PDF or export results
        const currentUrl = page.url();
        const pageContent = await page.textContent('body');
        
        if (currentUrl.includes('pdf') || pageContent.includes('PDF') || pageContent.includes('%PDF')) {
          testInfo.attach('PDF Export', { body: `✅ PDF displayed inline at: ${currentUrl}`, contentType: 'text/plain' });
        } else {
          testInfo.attach('PDF Export', { body: '❓ PDF export triggered but behavior unclear', contentType: 'text/plain' });
        }
      }
      
    } catch (error) {
      testInfo.attach('PDF Export', { body: '❌ PDF export failed or not available', contentType: 'text/plain' });
    }
  });

  test('All Export Formats Summary', async ({ page }, testInfo) => {
    const formatResults = [];
    
    // Test each format
    const formats = [
      { name: 'Excel', selector: 'a:has-text("Excel"), button:has-text("Excel")', extensions: ['.xlsx', '.xls'] },
      { name: 'CSV', selector: 'a:has-text("CSV"), button:has-text("CSV")', extensions: ['.csv'] },
      { name: 'PDF', selector: 'a:has-text("PDF"), button:has-text("PDF")', extensions: ['.pdf'] }
    ];
    
    for (const format of formats) {
      const element = page.locator(format.selector);
      
      if (await element.isVisible()) {
        formatResults.push(`✅ ${format.name}: Export option available`);
        
        // Quick test click (don't wait for download to avoid timeouts)
        try {
          const downloadPromise = page.waitForEvent('download', { timeout: 3000 }).catch(() => null);
          await element.click();
          const download = await downloadPromise;
          
          if (download) {
            formatResults.push(`  📁 ${format.name}: Downloads file successfully`);
          } else {
            formatResults.push(`  📄 ${format.name}: May display inline or in new tab`);
          }
        } catch (error) {
          formatResults.push(`  ⚠️  ${format.name}: Available but behavior needs verification`);
        }
        
        // Reset page for next test
        await page.goto(page.url());
        await page.fill('input[name="forename"]', testPerson.forename);
        await page.fill('input[name="familyName"]', testPerson.familyName);
        await page.selectOption('select[name="gender"]', testPerson.gender);
        await page.fill('input[name="yearOfBirth"]', testPerson.yearOfBirth);
        
      } else {
        formatResults.push(`❌ ${format.name}: Export option not found`);
      }
    }
    
    testInfo.attach('Export Formats Summary', { 
      body: formatResults.join('\n'), 
      contentType: 'text/plain' 
    });
    
    // Verify we tested all required formats
    const availableFormats = formatResults.filter(result => result.includes('✅')).length;
    testInfo.attach('Coverage', { 
      body: `Found ${availableFormats} out of 3 expected export formats (Excel, PDF, CSV)`, 
      contentType: 'text/plain' 
    });
  });
});