# Import Testing - Test Steps (JavaScript)

## Comprehensive Excel Import Testing Suite

### Purpose
Validates complete Excel import functionality including file upload, data validation, error handling, and success detection across 6 comprehensive test scenarios.

### Prerequisites
- Application running at `https://localhost:7031`
- Valid admin credentials available
- xlsx npm library installed for Excel file generation
- Clean database state for consistent testing

---

## Test 1: Import Page Structure and Instructions

### Purpose
Validates import page UI, instructions, and basic accessibility.

### Test Steps
1. **Authentication & Navigation**
   - Login with Admin credentials
   - Navigate to import page (`/home/import`)
   - Wait for page load completion

2. **Page Structure Validation**
   - Verify page title indicates import functionality
   - Check for import instructions/help text
   - Validate file upload area is present
   - Confirm import button is available

3. **UI Element Testing**
   - Test file input field functionality
   - Verify acceptable file types are indicated
   - Check for progress indicators or status areas
   - Validate page responsive design

**Expected Results**: Import page loads correctly with clear instructions and functional UI elements.

---

## Test 2: Import Page UI and Elements Validation

### Purpose
Validates all interactive elements and their behavior on the import page.

### Test Steps
1. **Element Identification**
   - Locate file input: `input[type="file"]`
   - Locate import button: `button:has-text("Import Data")`
   - Check for additional UI elements (progress bars, help text)

2. **File Input Testing**
   - Test file selection dialog opens
   - Verify file type restrictions (if any)
   - Test file selection and display

3. **Button State Testing**
   - Verify import button initial state
   - Test button state after file selection
   - Check button accessibility and styling

**Expected Results**: All UI elements are functional and provide appropriate user feedback.

---

## Test 3: Excel File Import - Working Implementation

### Purpose
Tests successful Excel file import with valid data and success message detection.

### Test Steps
1. **Excel File Generation**
   - Create new Excel workbook using xlsx library
   - Generate test data with required fields:
     ```javascript
     ['Forename', 'FamilyName', 'Gender', 'YearOfBirth']
     ['ExcelTest', 'ImportUser', 'Male', 1975]
     ```
   - Save as .xlsx file with proper formatting

2. **File Upload Process**
   - Select generated Excel file using file input
   - Verify file selection is displayed
   - Click "Import Data" button
   - Wait for processing completion

3. **Success Detection**
   - Monitor page content for success message
   - Look for "Successfully imported" text
   - Verify proper page redirection
   - Capture success confirmation

4. **Data Verification**
   - Navigate to View People page
   - Search for imported person: "ExcelTest ImportUser"
   - Verify all data fields match input
   - Note person ID for cleanup

5. **Test Cleanup**
   - Delete imported test person
   - Verify successful removal
   - Clean up temporary Excel file

**Expected Results**: Excel file imports successfully with "Successfully imported" message and data appears correctly in system.

---

## Test 4: Template Download Testing

### Purpose
Validates template download functionality (if available) for user guidance.

### Test Steps
1. **Template Availability Check**
   - Look for template download link/button
   - Verify template accessibility
   - Test download functionality

2. **Template Validation**
   - Download template file
   - Verify file format (.xlsx)
   - Check template structure and headers
   - Validate example data (if provided)

3. **Template Usage**
   - Use template as basis for test import
   - Verify template compatibility
   - Test import with template-based data

**Expected Results**: Template (if available) downloads correctly and provides proper guidance for import format.

---

## Test 5: Import Process with Sample Data

### Purpose
Tests import process with multiple records and various data combinations.

### Test Steps
1. **Multi-Record Excel Creation**
   - Generate Excel with multiple test records:
     ```javascript
     ['Sample1', 'Test1', 'Female', 1980]
     ['Sample2', 'Test2', 'Male', 1985]
     ['Sample3', 'Test3', 'Other', 1990]
     ```

2. **Import Execution**
   - Upload multi-record Excel file
   - Execute import process
   - Monitor for success message
   - Verify batch processing

3. **Batch Verification**
   - Verify all records imported successfully
   - Check import count matches file records
   - Validate data integrity for each record
   - Test different data combinations (gender, years)

4. **Cleanup Process**
   - Delete all imported test records
   - Verify complete cleanup
   - Confirm system state is clean

**Expected Results**: Multiple records import successfully with accurate data preservation and proper success reporting.

---

## Test 6: Import Data Validation

### Purpose
Tests import error handling with malformed/invalid data.

### Test Steps
1. **Malformed Data Creation**
   - Generate Excel with validation test cases:
     ```javascript
     ['TestBad1', '', 'Male', 1920]           // Missing FamilyName
     ['', 'TestBad2', 'Female', 1925]         // Missing Forename  
     ['TestBad3', 'TestBad3', 'InvalidGender', 1880] // Invalid Gender & Year
     ```

2. **Import Attempt**
   - Upload malformed data file
   - Execute import process
   - Monitor for validation errors

3. **Error Detection**
   - Look for validation error messages
   - Check for field-specific errors
   - Verify helpful error descriptions
   - Confirm import prevention for bad data

4. **Error Recovery**
   - Verify system remains stable after errors
   - Test navigation after validation failure
   - Confirm no partial data import occurred

**Expected Results**: System properly validates data, prevents import of invalid records, and displays helpful error messages.

---

## Shared Test Utilities

### Excel File Generation Function
```javascript
const createExcelFile = (data, filename) => {
  const workbook = XLSX.utils.book_new();
  const worksheet = XLSX.utils.aoa_to_sheet(data);
  XLSX.utils.book_append_sheet(workbook, worksheet, 'People');
  XLSX.writeFile(workbook, filename);
};
```

### Success Detection Logic
```javascript
const checkImportSuccess = async (page) => {
  const pageContent = await page.textContent('body');
  return pageContent.includes('Successfully imported') || 
         pageContent.includes('Import successful');
};
```

### Cleanup Helper
```javascript
const cleanupImportedPerson = async (page, personId) => {
  await page.goto(`https://localhost:7031/Home/Delete/${personId}`);
  await page.click('button:has-text("Delete")');
  await page.waitForURL('**/Home/ViewPeople');
};
```

---

## Data Requirements & Validation Rules

### Required Fields
- **Forename**: Text, required, non-empty
- **FamilyName**: Text, required, non-empty  
- **Gender**: Selection (Male/Female/Other), required
- **YearOfBirth**: 4-digit number, ≥1900, ≤current year

### File Format Requirements
- **File Type**: Excel (.xlsx) only - CSV files rejected
- **Sheet Name**: "People" (or default first sheet)
- **Header Row**: Must include exact field names
- **Data Format**: Proper Excel cell formatting

### Success Indicators
- **Success Message**: "Successfully imported X records"
- **Page Redirection**: Usually to View People or confirmation page
- **Data Appearance**: Records visible in system immediately

### Error Scenarios
- **Package File Error**: CSV files or corrupted Excel files
- **Validation Errors**: Missing required fields, invalid years
- **Format Errors**: Incorrect column headers, wrong data types

---

## Performance & Quality Metrics

### Execution Time
- **Single Test**: ~4-10 seconds per test
- **Complete Suite**: ~25-30 seconds total
- **File Generation**: <1 second per Excel file

### Success Criteria
- ✅ All 6 tests pass consistently
- ✅ Import success detection works reliably
- ✅ Data validation prevents bad imports
- ✅ Cleanup completes successfully
- ✅ No test data remains in system

### Monitoring Points
- Success message detection accuracy
- Data integrity verification
- Error message clarity and helpfulness
- System stability after imports
- Complete test data cleanup