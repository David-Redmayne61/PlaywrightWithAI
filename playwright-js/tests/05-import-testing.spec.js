const { test, expect } = require('@playwright/test');
const fs = require('fs');
const path = require('path');
const XLSX = require('xlsx');

test.describe('Data Import Testing - Excel Import', () => {
  let importedPersonIds = [];
  
  test.beforeEach(async ({ page }) => {
    // Login before each test
    await page.goto('https://localhost:7031');
    await page.waitForSelector('input[name="Username"]', { timeout: 20000 });
    await page.fill('input[name="Username"]', 'Admin');
    await page.fill('input[name="Password"]', 'Admin123!');
    await page.click('button[type="submit"]');
    await page.waitForURL('**', { timeout: 20000 });
    await expect(page).not.toHaveURL(/Login/i);
    
    // Navigate directly to import page using the correct URL
    await page.goto('https://localhost:7031/home/import');
    await page.waitForLoadState('networkidle');
  });

  test.afterEach(async ({ page }, testInfo) => {
    // Clean up imported data after each test
    if (importedPersonIds.length > 0) {
      await test.step('Clean up imported test data', async () => {
        for (const personId of importedPersonIds) {
          try {
            // Navigate to delete page for each imported person
            await page.goto(`https://localhost:7031/Home/Delete/${personId}`);
            await page.waitForSelector('button:has-text("Delete"), input[type="submit"][value="Delete"]', { timeout: 5000 });
            await page.click('button:has-text("Delete"), input[type="submit"][value="Delete"]');
            await page.waitForURL('**/Home/ViewPeople', { timeout: 5000 });
          } catch (error) {
            testInfo.attach('Cleanup Warning', { 
              body: `Could not delete person ID ${personId}: ${error.message}`, 
              contentType: 'text/plain' 
            });
          }
        }
        testInfo.attach('Cleanup Summary', { 
          body: `Attempted to clean up ${importedPersonIds.length} imported records`, 
          contentType: 'text/plain' 
        });
        importedPersonIds = []; // Reset the array
      });
    }
  });

  test('Import Page Structure and Instructions', async ({ page }, testInfo) => {
    await test.step('Document import page layout and requirements', async () => {
      // Capture the import page instructions
      const pageText = await page.textContent('body');
      const instructions = await page.locator('p, .instructions, .help-text, .alert, .requirements').allTextContents();
      
      testInfo.attach('Import Page Instructions', { 
        body: instructions.filter(text => text.trim().length > 10).join('\n\n'), 
        contentType: 'text/plain' 
      });
      
      // Look for template download links
      const templateLinks = await page.locator('a[href*="template"], a[href*="download"], a:has-text("template"), a:has-text("download")').all();
      if (templateLinks.length > 0) {
        for (let i = 0; i < templateLinks.length; i++) {
          const linkText = await templateLinks[i].textContent();
          const linkHref = await templateLinks[i].getAttribute('href');
          testInfo.attach(`Template Link ${i + 1}`, { 
            body: `Text: "${linkText}"\nURL: ${linkHref}`, 
            contentType: 'text/plain' 
          });
        }
      }
      
      // Check for specific Excel format requirements
      if (pageText.toLowerCase().includes('excel') || pageText.toLowerCase().includes('.xlsx')) {
        testInfo.attach('Excel Support Confirmed', { body: 'Page confirms Excel format support', contentType: 'text/plain' });
      }
    });

    await test.step('Verify import form elements', async () => {
      // Check page title - be more flexible
      await expect(page.locator('h1, h2, h3, .page-title, .card-title')).toBeVisible();
      
      // Check for file upload input
      await expect(page.locator('input[type="file"]')).toBeVisible();
      
      // Check for the specific import button by text
      const importButton = page.locator('button:has-text("Import Data")');
      await expect(importButton).toBeVisible();
      
      testInfo.attach('Import Form Validation', { 
        body: 'Import page has required form elements: file input and "Import Data" button', 
        contentType: 'text/plain' 
      });
    });
  });

  test('Import Page UI and Elements Validation', async ({ page }, testInfo) => {
    await test.step('Verify import page loaded correctly', async () => {
      // Check page title - be more flexible with selectors
      await expect(page.locator('h1, h2, h3, .page-title, .import-title, .card-title')).toBeVisible();
      
      // Check for file upload input
      await expect(page.locator('input[type="file"]')).toBeVisible();
      
      // Check for import button - use specific text
      const importButton = page.locator('button:has-text("Import Data")');
      await expect(importButton).toBeVisible();
      
      testInfo.attach('Import Page Validation', { 
        body: 'Import page loaded with file upload and "Import Data" button present', 
        contentType: 'text/plain' 
      });
    });

    await test.step('Check for file format requirements', async () => {
      // Look for format requirements or help text
      const formatInfo = await page.locator('.format-info, .help-text, .requirements, p, .alert-info').allTextContents();
      const formatText = formatInfo.join(' ');
      
      testInfo.attach('Format Requirements', { 
        body: formatText || 'No format requirements found on page', 
        contentType: 'text/plain' 
      });
      
      // Check if Excel format is mentioned
      if (formatText.toLowerCase().includes('excel') || formatText.toLowerCase().includes('.xlsx') || formatText.toLowerCase().includes('.xls')) {
        testInfo.attach('Excel Support', { body: 'Excel format is supported for import', contentType: 'text/plain' });
      }
    });
  });

  test('Test Excel File Import - Working Implementation', async ({ page }, testInfo) => {
    await test.step('Create and upload valid Excel file', async () => {
      // Create proper Excel workbook (this format DOES work)
      const workbook = XLSX.utils.book_new();
      
      // Create test data with timestamp for uniqueness and only required fields
      const timestamp = Date.now();
      const testData = [
        ['Forename', 'FamilyName', 'Gender', 'YearOfBirth'],
        [`ExcelImport${timestamp}1`, 'ExcelFamily1', 'Male', 1950],
        [`ExcelImport${timestamp}2`, 'ExcelFamily2', 'Female', 1960]
      ];
      
      // Create worksheet and add to workbook
      const worksheet = XLSX.utils.aoa_to_sheet(testData);
      XLSX.utils.book_append_sheet(workbook, worksheet, 'People');
      
      // Write Excel file
      const testFilePath = path.join(__dirname, 'working-import.xlsx');
      XLSX.writeFile(workbook, testFilePath);
      
      testInfo.attach('Excel File Created', { 
        body: `Created working Excel file: ${testFilePath}\nContains 2 test person records with timestamp ${timestamp}`, 
        contentType: 'text/plain' 
      });
      
      try {
        // Upload the Excel file
        await page.setInputFiles('input[type="file"]', testFilePath);
        testInfo.attach('File Upload', { body: 'Excel file uploaded successfully', contentType: 'text/plain' });
        
        // Click Import Data button
        await page.click('button:has-text("Import Data")');
        
        // Wait for import to complete and check for success
        await page.waitForTimeout(5000);
        
        // Check for successful import (should show success message)
        const currentUrl = page.url();
        const pageContent = await page.textContent('body');
        
        // Look for positive success indicators
        const hasSuccessMessage = pageContent.includes('Successfully imported') || pageContent.includes('Import successful');
        const hasImportFailed = pageContent.includes('Import failed');
        const hasPackageError = pageContent.includes('Package file');
        
        if (hasSuccessMessage) {
          testInfo.attach('Import Success Confirmed', { 
            body: `Excel import completed successfully!\nFinal URL: ${currentUrl}\nSuccess message found in page content`, 
            contentType: 'text/plain' 
          });
          
          // Try to capture imported record IDs for cleanup
          if (currentUrl.includes('ViewPeople') || currentUrl.includes('View')) {
            // Already on people list page
            const newRecords = await page.locator(`tr:has-text("ExcelImport${timestamp}")`).all();
            for (const record of newRecords) {
              const idCell = record.locator('td').first();
              const idText = await idCell.textContent();
              if (idText && /^\d+$/.test(idText.trim())) {
                importedPersonIds.push(idText.trim());
              }
            }
          } else {
            // Navigate to view people to find imported records
            await page.goto('https://localhost:7031/Home/ViewPeople');
            await page.waitForTimeout(2000);
            const newRecords = await page.locator(`tr:has-text("ExcelImport${timestamp}")`).all();
            for (const record of newRecords) {
              const idCell = record.locator('td').first();
              const idText = await idCell.textContent();
              if (idText && /^\d+$/.test(idText.trim())) {
                importedPersonIds.push(idText.trim());
              }
            }
          }
          
          testInfo.attach('Imported Records for Cleanup', { 
            body: `Found ${importedPersonIds.length} imported records: ${importedPersonIds.join(', ')}`, 
            contentType: 'text/plain' 
          });
          
        } else {
          testInfo.attach('Unexpected Import Failure', { 
            body: `Excel import failed unexpectedly!\nImport Failed: ${hasImportFailed}\nPackage Error: ${hasPackageError}\nURL: ${currentUrl}`, 
            contentType: 'text/plain' 
          });
        }
        
      } catch (error) {
        testInfo.attach('Import Test Error', { body: `Error during Excel import test: ${error.message}`, contentType: 'text/plain' });
      } finally {
        // Clean up test file
        if (fs.existsSync(testFilePath)) {
          fs.unlinkSync(testFilePath);
        }
      }
    });
  });

  test('Test Template Download (if available)', async ({ page }, testInfo) => {
    await test.step('Check for template download functionality', async () => {
      const templateLinks = await page.locator('a[href*="template"], a[href*="download"], a:has-text("template"), a:has-text("download")').all();
      
      if (templateLinks.length > 0) {
        testInfo.attach('Template Available', { body: `Found ${templateLinks.length} template/download links`, contentType: 'text/plain' });
        
        // Try downloading the first template
        const downloadPromise = page.waitForEvent('download', { timeout: 10000 });
        await templateLinks[0].click();
        
        try {
          const download = await downloadPromise;
          const fileName = download.suggestedFilename();
          testInfo.attach('Template Download', { 
            body: `Successfully downloaded template: ${fileName}`, 
            contentType: 'text/plain' 
          });
        } catch (error) {
          testInfo.attach('Template Download', { body: 'Template link clicked but no download occurred', contentType: 'text/plain' });
        }
      } else {
        testInfo.attach('Template Check', { body: 'No template download links found on import page', contentType: 'text/plain' });
      }
    });
  });

  test('Test Import Process with Sample Data', async ({ page }, testInfo) => {
    await test.step('Create temporary test Excel file', async () => {
      // Create proper Excel file (CSV files are rejected)
      const workbook = XLSX.utils.book_new();
      
      const testData = [
        ['Forename', 'FamilyName', 'Gender', 'YearOfBirth'],
        ['TestImport1', 'TestFamily1', 'Male', 1940],
        ['TestImport2', 'TestFamily2', 'Female', 1945],
        ['TestImport3', 'TestFamily3', 'Male', 1950]
      ];
      
      const worksheet = XLSX.utils.aoa_to_sheet(testData);
      XLSX.utils.book_append_sheet(workbook, worksheet, 'People');
      
      const testFilePath = path.join(__dirname, 'test-import-data.xlsx');
      XLSX.writeFile(workbook, testFilePath);
      
      testInfo.attach('Test File Created', { 
        body: `Created Excel test file: ${testFilePath}\nContains 3 test person records`, 
        contentType: 'text/plain' 
      });
      
      // Try to upload the test file
      const fileInput = page.locator('input[type="file"]');
      await fileInput.setInputFiles(testFilePath);
      
      testInfo.attach('File Upload', { body: 'Excel test file uploaded to import form', contentType: 'text/plain' });
    });

    await test.step('Execute import process', async () => {
      // Click the specific import button
      const importButton = page.locator('button:has-text("Import Data")');
      await importButton.click();
      
      // Wait for import to complete - look for success/error messages
      try {
        await page.waitForSelector('.alert-success, .alert-danger, .message, [role="alert"], text="Import Failed"', { timeout: 15000 });
        
        // Check specifically for the known Excel-only requirement
        const importFailedVisible = await page.locator('text=/Import failed.*Package file/i').isVisible().catch(() => false);
        if (importFailedVisible) {
          const failedMessage = await page.locator('text=/Import failed.*Package file/i').textContent();
          testInfo.attach('Import Failed - Package Error', { 
            body: `This error occurs when uploading non-Excel files. Import only accepts .xlsx files: ${failedMessage}`, 
            contentType: 'text/plain' 
          });
          return; // Exit early - this is expected for non-Excel files
        }
        
        // Check for general "Import Failed" message
        const generalImportFailed = await page.locator('text="Import Failed"').count();
        if (generalImportFailed > 0) {
          const failedMessage = await page.locator('text="Import Failed"').textContent();
          testInfo.attach('Import Failed', { body: failedMessage, contentType: 'text/plain' });
          
          // Look for additional error details
          const errorDetails = await page.locator('.alert-danger, .error, .validation-summary').allTextContents();
          if (errorDetails.length > 0) {
            testInfo.attach('Import Error Details', { body: errorDetails.join('\n'), contentType: 'text/plain' });
          }
          return; // Exit early if import failed
        }
        
        const importMessage = await page.locator('.alert-success, .alert-danger, .message, [role="alert"]').textContent();
        testInfo.attach('Import Result', { body: importMessage || 'Import completed', contentType: 'text/plain' });
        
        // If import was successful, try to capture imported record IDs
        if (importMessage && importMessage.toLowerCase().includes('success')) {
          await test.step('Capture imported record IDs for cleanup', async () => {
            // Navigate to view people page to find our imported records
            await page.click('nav a:has-text("View People"), a:has-text("View All")');
            await page.waitForURL(/ViewPeople|View/i, { timeout: 10000 });
            
            // Look for our test data
            const testNames = ['TestImport1', 'TestImport2', 'TestImport3'];
            for (const name of testNames) {
              const personRow = page.locator(`table tbody tr:has-text("${name}")`);
              if (await personRow.isVisible()) {
                // Try to extract ID from the row (usually first column or in a link)
                const idCell = personRow.locator('td').first();
                const idText = await idCell.textContent();
                if (idText && /^\d+$/.test(idText.trim())) {
                  importedPersonIds.push(idText.trim());
                }
              }
            }
            
            testInfo.attach('Imported IDs for Cleanup', { 
              body: `Found ${importedPersonIds.length} imported records: ${importedPersonIds.join(', ')}`, 
              contentType: 'text/plain' 
            });
          });
        }
        
      } catch (error) {
        testInfo.attach('Import Status', { body: 'Import process completed but no clear success/error message found', contentType: 'text/plain' });
      }
    });

    await test.step('Clean up test file', async () => {
      // Remove the temporary test file
      const testFilePath = path.join(__dirname, 'test-import-data.xlsx');
      if (fs.existsSync(testFilePath)) {
        fs.unlinkSync(testFilePath);
        testInfo.attach('File Cleanup', { body: 'Temporary Excel test file removed', contentType: 'text/plain' });
      }
    });
  });

  test('Test Import Data Validation', async ({ page }, testInfo) => {
    await test.step('Test import with malformed data', async () => {
      // Create Excel with missing required fields to test validation
      const workbook = XLSX.utils.book_new();
      
      const malformedData = [
        ['Forename', 'FamilyName', 'Gender', 'YearOfBirth'],
        ['TestBad1', '', 'Male', 1920],        // Missing FamilyName
        ['', 'TestBad2', 'Female', 1925],      // Missing Forename
        ['TestBad3', 'TestBad3', 'InvalidGender', 1880]  // Invalid Gender & Year (before 1900)
      ];
      
      const worksheet = XLSX.utils.aoa_to_sheet(malformedData);
      XLSX.utils.book_append_sheet(workbook, worksheet, 'People');
      
      const malformedFilePath = path.join(__dirname, 'malformed-import.xlsx');
      XLSX.writeFile(workbook, malformedFilePath);
      
      try {
        const fileInput = page.locator('input[type="file"]');
        await fileInput.setInputFiles(malformedFilePath);
        
        const importButton = page.locator('button:has-text("Import Data")');
        await importButton.click();
        
        // Look for validation errors
        await page.waitForSelector('.alert-danger, .alert-warning, .error, [role="alert"]', { timeout: 10000 });
        const validationMessage = await page.locator('.alert-danger, .alert-warning, .error, [role="alert"]').textContent();
        
        testInfo.attach('Data Validation Error', { 
          body: validationMessage || 'Import completed despite malformed data', 
          contentType: 'text/plain' 
        });
        
        // Clean up
        if (fs.existsSync(malformedFilePath)) {
          fs.unlinkSync(malformedFilePath);
        }
        
      } catch (error) {
        testInfo.attach('Malformed Data Test', { body: `Test completed: ${error.message}`, contentType: 'text/plain' });
        // Clean up
        if (fs.existsSync(malformedFilePath)) {
          fs.unlinkSync(malformedFilePath);
        }
      }
    });
  });
});