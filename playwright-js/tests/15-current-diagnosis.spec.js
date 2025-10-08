const { test, expect } = require('@playwright/test');
const fs = require('fs');
const path = require('path');
const XLSX = require('xlsx');

test.describe('Current Import Error Diagnosis', () => {
  test.beforeEach(async ({ page }) => {
    // Login
    await page.goto('https://localhost:7031');
    await page.waitForSelector('input[name="Username"]', { timeout: 20000 });
    await page.fill('input[name="Username"]', 'Admin');
    await page.fill('input[name="Password"]', 'Admin123!');
    await page.click('button[type="submit"]');
    await page.waitForURL('**', { timeout: 20000 });
    
    // Navigate to import page
    await page.goto('https://localhost:7031/home/import');
    await page.waitForLoadState('networkidle');
  });

  test('Diagnose Current Import Errors', async ({ page }, testInfo) => {
    // Create Excel with the format we think is correct
    const workbook = XLSX.utils.book_new();
    
    const testData = [
      ['Forename', 'FamilyName', 'Gender', 'YearOfBirth'],
      ['DiagTest1', 'DiagFamily1', 'Male', 1990],
      ['DiagTest2', 'DiagFamily2', 'Female', 1985]
    ];
    
    const worksheet = XLSX.utils.aoa_to_sheet(testData);
    XLSX.utils.book_append_sheet(workbook, worksheet, 'People');
    
    const testFilePath = path.join(__dirname, 'current-diagnosis.xlsx');
    XLSX.writeFile(workbook, testFilePath);
    
    try {
      console.log('📁 Uploading Excel file with YearOfBirth format...');
      await page.setInputFiles('input[type="file"]', testFilePath);
      
      console.log('🔘 Clicking Import Data button...');
      await page.click('button:has-text("Import Data")');
      
      console.log('⏳ Waiting for import response...');
      await page.waitForTimeout(3000);
      
      console.log('⏸️  PAUSING - Please copy the EXACT error messages you see!');
      console.log('📋 Look for ALL error messages on the page');
      
      // PAUSE to see current errors
      await page.pause();
      
      // After resume, capture everything
      const pageContent = await page.textContent('body');
      testInfo.attach('Current Page Content', { 
        body: pageContent, 
        contentType: 'text/plain' 
      });
      
      // Look for specific error patterns
      const packageError = pageContent.includes('Package file');
      const yearError = pageContent.toLowerCase().includes('year');
      const birthError = pageContent.toLowerCase().includes('birth');
      const validationError = pageContent.toLowerCase().includes('validation');
      const importFailed = pageContent.includes('Import failed');
      
      testInfo.attach('Error Pattern Analysis', { 
        body: `Error Analysis:
Package Error: ${packageError}
Year Error: ${yearError}
Birth Error: ${birthError}
Validation Error: ${validationError}
Import Failed: ${importFailed}

Please copy the exact error messages from the browser for accurate diagnosis.`, 
        contentType: 'text/plain' 
      });
      
    } catch (error) {
      testInfo.attach('Test Error', { body: error.message, contentType: 'text/plain' });
    } finally {
      if (fs.existsSync(testFilePath)) {
        fs.unlinkSync(testFilePath);
      }
    }
  });
});