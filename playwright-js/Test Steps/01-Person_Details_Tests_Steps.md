# Person Details Tests - Test Steps (JavaScript)

## Test: PersonDetailsFormValidationAndSubmission

### Purpose
Validates person details form functionality including field validation, form submission, and proper data handling.

### Prerequisites
- Application running at `https://localhost:7031`
- Valid admin credentials available
- Clean database state for consistent testing

### Test Steps

1. **Browser Setup & Login**
   - Launch Chromium browser (headed mode, 1920x1080 viewport)
   - Navigate to `https://localhost:7031`
   - Perform login with Admin credentials
   - Wait for successful authentication and dashboard display

2. **Navigation to Add Person Page**
   - Click "Add Person" from dashboard cards or navigation menu
   - Wait for Add Person page to load
   - Verify URL contains `/Home/Create` or similar
   - Wait for form elements to be fully loaded

3. **Form Field Validation**
   - **Forename Field Testing**:
     - Locate forename input field
     - Test empty submission (should show validation error)
     - Test valid input (e.g., "TestFirstName")
     - Verify field accepts alphanumeric characters
   
   - **Family Name Field Testing**:
     - Locate family name input field
     - Test empty submission (should show validation error)
     - Test valid input (e.g., "TestLastName")
     - Verify field accepts alphanumeric characters
   
   - **Gender Selection Testing**:
     - Locate gender dropdown/radio buttons
     - Verify all available options (Male, Female, Other)
     - Test selection functionality
     - Verify required field validation

   - **Year of Birth Field Testing**:
     - Locate year of birth input field
     - Test invalid years (e.g., future dates, before 1900)
     - Test valid year (e.g., 1980)
     - Verify numeric input validation

4. **Form Submission Process**
   - Fill all required fields with valid test data:
     - Forename: "AutoTest"
     - Family Name: "PersonDetails"
     - Gender: "Male"
     - Year of Birth: "1985"
   - Locate and click Submit/Save button
   - Wait for form processing

5. **Success Validation**
   - Verify successful submission message/notification
   - Check for proper page redirection (usually to View People or confirmation page)
   - Verify no error messages are displayed
   - Capture the new person's ID for cleanup

6. **Data Verification**
   - Navigate to View People page or person list
   - Search for the newly created person
   - Verify all entered data is correctly displayed:
     - Forename matches input
     - Family Name matches input
     - Gender matches selection
     - Year of Birth matches input

7. **Error Handling Testing**
   - **Test Missing Required Fields**:
     - Navigate back to Add Person form
     - Submit form with missing forename
     - Verify appropriate validation message
     - Submit form with missing family name
     - Verify appropriate validation message
   
   - **Test Invalid Data**:
     - Enter invalid year of birth (e.g., 1850, 2030)
     - Verify validation prevents submission
     - Test special characters in name fields if restricted

8. **Form Reset/Cancel Testing**
   - Fill form with test data
   - Click Cancel button (if available)
   - Verify form data is cleared or user is redirected
   - Test form reset functionality (if available)

9. **Test Cleanup**
   - Delete the test person record created during testing
   - Navigate to person details or delete option
   - Confirm deletion
   - Verify person is removed from system

10. **Final Validation**
    - Return to dashboard
    - Verify system is in clean state
    - Save trace file and any failure screenshots

### Expected Results
- ✅ All form fields validate correctly
- ✅ Required field validation prevents empty submissions
- ✅ Valid data submission creates new person record
- ✅ Data integrity maintained (input matches stored data)
- ✅ Error messages display for invalid inputs
- ✅ Form reset/cancel functionality works correctly
- ✅ Test data cleanup completes successfully

### Test Data Used
- **Valid Person**: AutoTest PersonDetails, Male, 1985
- **Invalid Years**: 1850 (too old), 2030 (future)
- **Empty Fields**: Test each required field individually

### Cleanup Actions
- Delete test person record
- Verify removal from database
- Return system to clean state