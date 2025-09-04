# Export Functionality BDD Test Steps

## Test Suite Overview
**Purpose**: Validate export functionality (Print Preview, PDF Export, Excel Export) from the View People page  
**Framework**: SpecFlow BDD with Playwright  
**Test Count**: 3 scenarios  
**Feature File**: `ExportFunctionality.feature`  

---

## Scenario 1: Access Print Preview from View People page Output Options

### Test Objective
Verify that users can access the Print Preview modal from the View People page Output Options menu.

### Step-by-Step Flow
1. **Given** I am logged in as "davred"
   - Launch browser and navigate to login page
   - Enter username "davred" and password
   - Submit login form and verify successful authentication

2. **And** I am on the main page
   - Verify main page loads correctly after login
   - Confirm page title and basic UI elements are present

3. **When** I click on the "View People" link
   - Locate and click the "View People" navigation link
   - Wait for page transition to complete

4. **And** I view the View People page
   - Verify the View People page loads successfully
   - Confirm data grid with people records is displayed

5. **And** I click the "Output Options" button
   - Locate the "Output Options" button on the page
   - Click to open the export options menu

6. **And** I click on "Print Preview"
   - Select "Print Preview" from the available export options
   - Trigger the browser's native print preview dialog

7. **Then** I should see the Browser Print preview modal
   - Verify that the browser's print preview functionality opens
   - Note: This opens the browser's native print dialog (cannot be fully automated)

8. **And** then I will click Cancel to close the modal
   - Press ESC key or equivalent action to close the print dialog
   - Return to the normal View People page view

### Expected Results
- ✅ Print Preview modal opens successfully
- ✅ User can close the modal and return to normal view
- ✅ No errors or crashes occur during the process

---

## Scenario 2: Export data to Excel format from View People

### Test Objective
Verify that users can successfully export people data to Excel format and receive a valid Excel file download.

### Step-by-Step Flow
1. **Given** I am logged in as "davred"
   - Launch browser and navigate to login page
   - Enter username "davred" and password
   - Submit login form and verify successful authentication

2. **And** I am on the main page
   - Verify main page loads correctly after login
   - Confirm page title and basic UI elements are present

3. **When** I click on the "View People" link
   - Locate and click the "View People" navigation link
   - Wait for page transition to complete

4. **And** I view the View People page
   - Verify the View People page loads successfully
   - Confirm data grid with people records is displayed

5. **And** I click the "Output Options" button
   - Locate the "Output Options" button on the page
   - Click to open the export options menu

6. **And** I click on "Export to Excel"
   - Select "Export to Excel" from the available export options
   - Trigger the Excel export functionality

7. **Then** I should receive an Excel file download
   - Intercept the download using Playwright's download handling
   - Verify that a file download is initiated
   - Confirm the downloaded file has an Excel filename (e.g., `people_export_YYYYMMDD_HHMMSS.xlsx`)

8. **And** the Excel file should contain the database records
   - Verify the downloaded file exists in the Downloads folder
   - Check that the file size indicates substantial content (> 1KB)
   - Confirm the file contains data from the people database

9. **And** the Excel file should have proper column headers
   - Validate that the Excel file has the correct ZIP signature (PK bytes)
   - Verify the file is a valid Excel format
   - Confirm proper file structure

### Expected Results
- ✅ Excel file downloads successfully with timestamped filename
- ✅ File size indicates substantial content (typically 6KB+ for test data)
- ✅ File has valid Excel format signature
- ✅ Download completes without errors

### Technical Validation
- **Endpoint**: `/Home/ExportToExcel`
- **File Format**: `.xlsx` (Excel 2007+ format)
- **File Signature**: PK (ZIP-based format validation)
- **Typical Size**: 6,000+ bytes for test dataset

---

## Scenario 3: Export data to PDF format from View People

### Test Objective
Verify that users can successfully export people data to PDF format and receive a valid PDF file download.

### Step-by-Step Flow
1. **Given** I am logged in as "davred"
   - Launch browser and navigate to login page
   - Enter username "davred" and password
   - Submit login form and verify successful authentication

2. **And** I am on the main page
   - Verify main page loads correctly after login
   - Confirm page title and basic UI elements are present

3. **When** I click on the "View People" link
   - Locate and click the "View People" navigation link
   - Wait for page transition to complete

4. **And** I view the View People page
   - Verify the View People page loads successfully
   - Confirm data grid with people records is displayed

5. **And** I click the "Output Options" button
   - Locate the "Output Options" button on the page
   - Click to open the export options menu

6. **And** I click on "Export to PDF"
   - Select "Export to PDF" from the available export options
   - Trigger the PDF export functionality

7. **Then** I should receive a PDF file download
   - Intercept the download using Playwright's download handling
   - Verify that a file download is initiated
   - Confirm the downloaded file has a PDF filename (e.g., `export_YYYYMMDD_HHMMSS.pdf`)

8. **And** the PDF file should contain the database records
   - Verify the downloaded file exists in the Downloads folder
   - Check that the file size indicates substantial content (> 10KB)
   - Confirm the file contains data from the people database

9. **And** the PDF file should be properly formatted
   - Validate that the PDF file has the correct PDF header (%PDF)
   - Verify the file is a valid PDF format
   - Confirm proper file structure

### Expected Results
- ✅ PDF file downloads successfully with timestamped filename
- ✅ File size indicates substantial content (typically 60KB+ for test data)
- ✅ File has valid PDF format signature (%PDF)
- ✅ Download completes without errors

### Technical Validation
- **Endpoint**: `/Home/ExportToPdf`
- **File Format**: `.pdf` (Portable Document Format)
- **File Signature**: %PDF (standard PDF header validation)
- **Typical Size**: 60,000+ bytes for test dataset

---

## Test Environment Requirements

### Prerequisites
- ✅ Application running and accessible
- ✅ User account "davred" exists and is active
- ✅ Database populated with test people records
- ✅ Export functionality enabled in backend
- ✅ Browser with download permissions

### Technical Setup
- **Framework**: SpecFlow 3.9.74 + Playwright
- **Browser**: Chromium (configurable)
- **Download Folder**: `%USERPROFILE%\Downloads`
- **Reporting**: HTML reports with SpecFlow LivingDoc
- **Test Parallelization**: Disabled (ClassLevel scope)

### File Validation Methods
1. **Download Interception**: Playwright's built-in download handling
2. **File Existence**: Verify files appear in Downloads folder
3. **Size Validation**: Ensure files contain substantial content
4. **Format Validation**: Check file signatures (PDF: %PDF, Excel: PK)
5. **Cleanup**: Files remain in Downloads for manual verification

---

## Reporting and Execution

### Test Execution Commands
```powershell
# Run export tests only
dotnet test --filter "FullyQualifiedName~ExportFunctionality"

# Run all BDD tests (including export)
.\Scripts\run-bdd-tests.ps1

# Run complete test suite with reporting
.\Scripts\run-all-tests-sequential.ps1
```

### Test Reports
- **BDD HTML Report**: `TestResults\BDD_Tests_Report.html`
- **Cucumber Output**: Console with 🥒 step-by-step execution
- **TRX Report**: `TestResults\BDD_Tests_Results.trx`

### Success Criteria
- ✅ All 3 export scenarios pass
- ✅ 2 download files created (1 Excel + 1 PDF)
- ✅ No browser context conflicts
- ✅ Files have valid format signatures
- ✅ Beautiful BDD reporting generated

---

## Troubleshooting Guide

### Common Issues
1. **Multiple Downloads**: Ensure only 3 scenarios exist in feature file
2. **Browser Context Conflicts**: Use single browser instance via PlaywrightHooks
3. **Download Failures**: Check backend export endpoints are functional
4. **File Validation Errors**: Verify Downloads folder permissions

### Debug Commands
```powershell
# List all tests
dotnet test --list-tests

# Check export test count
dotnet test --filter "FullyQualifiedName~ExportFunctionality" --list-tests

# Verbose execution
dotnet test --filter "FullyQualifiedName~ExportFunctionality" --logger "console;verbosity=detailed"
```

For all future export functionality updates, this comprehensive test documentation should be maintained alongside the BDD feature files and step definitions.
