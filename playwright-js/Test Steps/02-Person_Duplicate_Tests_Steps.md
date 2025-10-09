# Person Duplicate Tests - Test Steps (JavaScript)

## Test: DuplicatePersonDetectionAndPrevention

### Purpose
Validates that the system correctly prevents creation of duplicate person records and displays appropriate error messages.

### Prerequisites
- Application running at `https://localhost:7031`
- Valid admin credentials available
- Clean database state for testing

### Test Steps

1. **Browser Setup & Authentication**
   - Launch Chromium browser (headed mode, 1920x1080 viewport)
   - Navigate to application home page
   - Complete login process with Admin credentials
   - Verify successful authentication and dashboard access

2. **Create Initial Person Record**
   - Navigate to Add Person page
   - Fill form with test data:
     - Forename: "DuplicateTest"
     - Family Name: "Original"
     - Gender: "Female"
     - Year of Birth: "1990"
   - Submit form and verify successful creation
   - Note the person ID for later cleanup
   - Verify person appears in system (View People page)

3. **Attempt Duplicate Creation - Exact Match**
   - Navigate back to Add Person page
   - Fill form with identical data:
     - Forename: "DuplicateTest"
     - Family Name: "Original"
     - Gender: "Female"
     - Year of Birth: "1990"
   - Submit form
   - **Expected**: System should prevent creation

4. **Duplicate Detection Validation**
   - Verify duplicate error message is displayed
   - Common error messages to check for:
     - "Person already exists"
     - "Duplicate person detected"
     - "A person with these details already exists"
   - Verify form remains on Add Person page
   - Verify no new record was created in database

5. **Case Sensitivity Testing**
   - Test duplicate detection with different case:
     - Forename: "duplicatetest" (lowercase)
     - Family Name: "original" (lowercase)
     - Same Gender and Year of Birth
   - Submit form
   - Verify system handles case variations appropriately

6. **Partial Duplicate Testing**
   - **Test Different Forename**:
     - Forename: "DifferentName"
     - Family Name: "Original"
     - Same Gender and Year of Birth
     - Verify this is allowed (not a duplicate)
   
   - **Test Different Family Name**:
     - Forename: "DuplicateTest"
     - Family Name: "DifferentFamily"
     - Same Gender and Year of Birth
     - Verify this is allowed (not a duplicate)
   
   - **Test Different Gender**:
     - Same Forename and Family Name
     - Gender: "Male" (different from original)
     - Same Year of Birth
     - Verify system behavior (may or may not be considered duplicate)

7. **Year of Birth Variation Testing**
   - Use same Forename, Family Name, and Gender
   - Change Year of Birth: "1991" (different from 1990)
   - Submit form
   - Verify system behavior for year variations

8. **Multiple Duplicate Attempts**
   - Attempt to create the exact duplicate 3 times
   - Verify error message consistency
   - Verify system remains stable
   - Check for any memory leaks or performance issues

9. **Database Integrity Verification**
   - Navigate to View People page
   - Search for "DuplicateTest Original"
   - Verify only ONE record exists (the original)
   - Verify no partial or corrupted records were created

10. **Error Message Testing**
    - Verify error messages are:
      - Clearly displayed to user
      - Descriptive and helpful
      - Properly styled (error styling)
      - Dismissible or automatically cleared

11. **Navigation After Duplicate Error**
    - After receiving duplicate error, test navigation:
      - Can navigate away from form
      - Can return to dashboard
      - Can access other features
      - System remains responsive

12. **Edge Case Testing**
    - **Whitespace Testing**:
      - Try "DuplicateTest " (with trailing space)
      - Try " DuplicateTest" (with leading space)
      - Verify trimming behavior
    
    - **Special Characters**:
      - Test names with apostrophes, hyphens
      - Verify duplicate detection with special characters

13. **Test Cleanup**
    - Delete the original test person record
    - Verify successful deletion
    - Confirm person is removed from View People page
    - Clear any test data created during partial duplicate tests

14. **Final System State Verification**
    - Return to dashboard
    - Verify system is responsive and clean
    - No test data remains in system
    - Save trace files for debugging

### Expected Results
- ✅ System prevents exact duplicate person creation
- ✅ Clear error messages displayed for duplicate attempts
- ✅ Database integrity maintained (only one record exists)
- ✅ Partial duplicates handled according to business rules
- ✅ System remains stable after multiple duplicate attempts
- ✅ Error messages are user-friendly and properly styled
- ✅ Navigation works correctly after duplicate errors
- ✅ Edge cases (whitespace, case sensitivity) handled appropriately

### Test Data Used
- **Original Person**: DuplicateTest Original, Female, 1990
- **Duplicate Attempts**: Various combinations of same data
- **Partial Duplicates**: Different names, genders, years for comparison

### Business Rules Validated
- Exact duplicates (all fields match) must be prevented
- Case sensitivity handling
- Whitespace trimming behavior
- Definition of what constitutes a "duplicate"

### Cleanup Actions
- Delete original test person
- Remove any partially created test records
- Verify clean system state