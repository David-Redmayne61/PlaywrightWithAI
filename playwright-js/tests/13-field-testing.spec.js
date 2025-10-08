const { test, expect } = require('@playwright/test');
const fs = require('fs');
const path = require('path');
const XLSX = require('xlsx');

test.describe('Excel Import Field Testing', () => {
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

  test('Test Different Column Header Variations', async ({ page }, testInfo) => {
    await test.step('Try with YearOfBirth instead of DateOfBirth', async () => {
      const workbook = XLSX.utils.book_new();
      
      // Try with YearOfBirth column instead
      const testData = [
        ['Forename', 'FamilyName', 'Gender', 'EmailAddress', 'YearOfBirth'],
        ['HeaderTest1', 'HeaderFamily1', 'Male', 'header1@test.com', '1990'],
        ['HeaderTest2', 'HeaderFamily2', 'Female', 'header2@test.com', '1985']
      ];
      
      const worksheet = XLSX.utils.aoa_to_sheet(testData);
      XLSX.utils.book_append_sheet(workbook, worksheet, 'People');
      
      const testFilePath = path.join(__dirname, 'header-test.xlsx');
      XLSX.writeFile(workbook, testFilePath);
      
      try {
        await page.setInputFiles('input[type="file"]', testFilePath);
        await page.click('button:has-text("Import Data")');
        await page.waitForTimeout(3000);
        
        const pageContent = await page.textContent('body');
        const hasPackageError = pageContent.includes('Package file');
        const hasYearError = pageContent.toLowerCase().includes('year') && pageContent.toLowerCase().includes('birth');
        const hasDateError = pageContent.toLowerCase().includes('date');
        
        testInfo.attach('YearOfBirth Header Test', { 
          body: `Using YearOfBirth column:\nPackage Error: ${hasPackageError}\nYear Error: ${hasYearError}\nDate Error: ${hasDateError}`, 
          contentType: 'text/plain' 
        });
        
      } finally {
        if (fs.existsSync(testFilePath)) {
          fs.unlinkSync(testFilePath);
        }
      }
    });

    await test.step('Try with minimal required fields only', async () => {
      const workbook = XLSX.utils.book_new();
      
      // Try with just the basic required fields
      const testData = [
        ['Forename', 'FamilyName', 'Gender'],
        ['MinimalTest1', 'MinimalFamily1', 'Male'],
        ['MinimalTest2', 'MinimalFamily2', 'Female']
      ];
      
      const worksheet = XLSX.utils.aoa_to_sheet(testData);
      XLSX.utils.book_append_sheet(workbook, worksheet, 'People');
      
      const testFilePath = path.join(__dirname, 'minimal-test.xlsx');
      XLSX.writeFile(workbook, testFilePath);
      
      try {
        await page.setInputFiles('input[type="file"]', testFilePath);
        await page.click('button:has-text("Import Data")');
        await page.waitForTimeout(3000);
        
        const pageContent = await page.textContent('body');
        const hasErrors = pageContent.includes('Import failed') || pageContent.includes('error');
        const currentUrl = page.url();
        
        testInfo.attach('Minimal Fields Test', { 
          body: `Using minimal fields (Forename, FamilyName, Gender):\nHas Errors: ${hasErrors}\nFinal URL: ${currentUrl}`, 
          contentType: 'text/plain' 
        });
        
        if (!hasErrors && currentUrl.includes('View')) {
          testInfo.attach('Minimal Import Success', { 
            body: 'Minimal field import appears successful!', 
            contentType: 'text/plain' 
          });
        }
        
      } finally {
        if (fs.existsSync(testFilePath)) {
          fs.unlinkSync(testFilePath);
        }
      }
    });
  });
});