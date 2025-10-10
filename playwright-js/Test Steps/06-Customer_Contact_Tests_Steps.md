# Customer Contact Tests - Test Steps (JavaScript)

## Test Suite: Record Customer Contact Functionality

### Purpose
Validates the complete customer contact recording system including navigation from dashboard, form functionality, data validation, and contact management workflows.

### Prerequisites
- Application running at `https://localhost:7031`
- Valid admin credentials available
- Customer Contact functionality accessible from dashboard
- Clean database state for testing

---

## Test 1: Dashboard Navigation to Customer Contact Form

### Purpose
Validates navigation from main dashboard to customer contact creation form at `/contact/create`.

### Test Steps

1. **Authentication Setup**
   - Launch browser (headed mode, 1920x1080 viewport)
   - Navigate to application home page
   - Complete login with Admin credentials
   - Verify successful authentication and dashboard access

2. **Dashboard Contact Access Discovery**
   - Verify current location is dashboard/home page
   - Scan dashboard for Customer Contact access options
   - Look for various possible labels:
     - "Record Customer Contact"
     - "Customer Contact" 
     - "Record Contact"
     - Cards or buttons containing "Customer" or "Contact"

3. **Navigation Testing**
   - **Primary Method**: Click dashboard Customer Contact link/button
   - **Fallback Method**: Direct navigation to `/contact/create` if link not found
   - Wait for page load completion
   - Verify successful navigation to contact form

4. **URL Validation**
   - Confirm current URL contains `/contact/create`
   - Verify page loads without errors
   - Document navigation path for future reference

**Expected Results**: Successfully navigate from dashboard to customer contact form at `/contact/create`.

---

## Test 2: Customer Contact Form Structure and Default Values Validation

### Purpose
Validates the structure, elements, default values, and auto-population behavior of the customer contact form.

### Test Steps

1. **Form Access**
   - Navigate directly to `/contact/create`
   - Wait for complete page load
   - Verify form accessibility

2. **Button Validation**
   - **Expected Buttons**:
     - "Back to Customer Calls" - Navigation button
     - "Reset Form" - Form reset functionality
     - "Record Customer Call" - Submit button (primary action)
   - Verify all buttons are present and visible
   - Check button styling and accessibility

3. **Auto-Populated Call Number Validation**
   - **Format**: Must be `yyyymmdd-nnn` (e.g., 20241010-001)
   - **Date Component**: Must match today's date (yyyymmdd)
   - **Sequence Component**: Three-digit number (nnn)
   - **Field Location**: Look for input fields with names/IDs containing "call" or "number"
   - **Validation**: Assert format matches regex `^\d{8}-\d{3}$`

4. **Contact Date and Time Validation**
   - **Default Value**: Must show today's date
   - **Time Component**: Should include current time (if datetime field)
   - **Field Type**: Look for `input[type="datetime-local"]` or `input[type="date"]`
   - **Format Validation**: Verify date format matches ISO standard

5. **Status Dropdown Validation**
   - **Default Selection**: Must be set to "Open"
   - **Field Location**: Look for select elements with names/IDs containing "status"
   - **Value Check**: Verify selected value is "Open" (case-insensitive)

6. **RED "Open" Label Validation**
   - **Visual Indicator**: Must be a RED colored label showing "Open"
   - **Location**: Adjacent to or near the status dropdown
   - **Styling**: Look for CSS classes or inline styles indicating red color:
     - `.text-danger`, `.text-red`
     - `style*="red"`, `style*="color: red"`
     - `.badge-danger`, `.alert-danger`
   - **Visibility**: Label must be visible and properly styled

7. **Additional Form Fields Discovery**
   - **Customer Name Field**: Text input for customer identification
   - **Contact Type Selection**: Dropdown for contact method/reason
   - **Notes/Description Area**: Textarea for detailed contact information
   - **Field Validation**: Check for proper labels, placeholders, and requirements

**Expected Results**: 
- ✅ All three buttons present and functional
- ✅ Call number auto-populated in correct format with today's date
- ✅ Contact date shows today's date and current time
- ✅ Status dropdown defaults to "Open"
- ✅ RED "Open" label visible and properly styled
- ✅ Form structure supports complete contact recording workflow

---

## Test 3: Record Customer Contact - Complete Workflow

### Purpose
Tests the complete process of recording a customer contact from form filling through successful submission.

### Test Steps

1. **Form Access and Preparation**
   - Navigate to `/contact/create`
   - Verify form is ready for input
   - Prepare test contact data

2. **Test Data Preparation**
   ```javascript
   const testContactData = {
     customerName: 'Test Customer Contact',
     contactType: 'Phone Call', 
     contactDate: '2024-10-10',
     notes: 'Customer called regarding product inquiry. Provided information about services and pricing. Follow-up scheduled.'
   };
   ```

3. **Form Completion Process**
   - **Customer Name Entry**:
     - Fill customer name field with test data
     - Verify text input acceptance
     - Check for real-time validation
   
   - **Contact Type Selection**:
     - Select appropriate contact type from dropdown
     - Verify selection is properly displayed
     - Test different contact type options
   
   - **Date Selection**:
     - Set contact date using date picker
     - Verify date format compliance
     - Test current date default (if applicable)
   
   - **Notes/Description Entry**:
     - Fill notes field with detailed contact information
     - Test character limits and formatting
     - Verify text preservation

4. **Form Submission**
   - Click Submit/Save button
   - Monitor form processing
   - Wait for submission completion

5. **Success Validation**
   - **Success Message Detection**:
     - Look for confirmation messages containing:
       - "successfully"
       - "created" 
       - "saved"
       - "recorded"
   
   - **Page Redirection**:
     - Verify navigation away from create form
     - Check for contact details page or list view
     - Document final URL destination
   
   - **Contact ID Extraction**:
     - Extract contact ID from URL for cleanup
     - Store ID for later deletion/verification

6. **Data Persistence Verification**
   - If redirected to contact details, verify data accuracy
   - Check that all entered information is correctly displayed
   - Validate proper data formatting and storage

**Expected Results**: Customer contact successfully recorded with proper system feedback and workflow completion.

---

## Test 3a: Dashboard Metrics Validation - Business Impact Verification

### Purpose
Validates that creating a customer contact record properly impacts dashboard metrics and business indicators.

### Test Steps

1. **Pre-Test Dashboard Metrics Capture**
   - Navigate to main dashboard (/)
   - Locate customer call widgets and capture baseline values:
     - **Total Customer Calls**: Overall count of all customer contacts
     - **Open Customer Calls**: Count of contacts with "Open" status  
     - **Aged Open Calls**: Count of older open contacts
     - **Pending Customer Calls**: Count of pending contacts
     - **Closed Customer Calls**: Count of completed contacts
   - Store all baseline metrics for comparison

2. **Contact Creation Process**
   - Execute complete contact creation workflow (Test 3)
   - Ensure successful contact record creation
   - Note contact creation timestamp

3. **Post-Test Dashboard Metrics Verification**
   - Return to main dashboard
   - Allow time for metrics refresh (2-3 seconds)
   - Re-capture all customer call widget values

4. **Business Impact Analysis**
   - **Total Customer Calls**: Must increment by exactly +1
   - **Open Customer Calls**: Must increment by exactly +1 (new contacts default to "Open")
   - **Other Metrics**: Should remain unchanged unless business rules dictate otherwise
   - **Metrics Consistency**: Verify mathematical relationships between metrics

5. **Data Integrity Validation**
   - Compare before/after snapshots
   - Document exact increment values
   - Verify no unintended metric changes
   - Confirm dashboard accurately reflects system state

**Expected Results**: 
- ✅ Total Customer Calls increases by exactly 1
- ✅ Open Customer Calls increases by exactly 1  
- ✅ Other metrics remain stable
- ✅ Dashboard provides accurate business intelligence

---

---

## Test 4: Contact Form Validation - Required Fields

### Purpose
Validates form validation behavior for required fields and error handling.

### Test Steps

1. **Empty Form Submission**
   - Navigate to contact form
   - Leave all fields empty
   - Attempt form submission
   - Monitor validation response

2. **Validation Response Analysis**
   - **Error Message Detection**:
     - Look for validation messages containing:
       - "required"
       - "Please"
       - "error"
       - "must"
   
   - **Form Behavior**:
     - Verify form remains on create page
     - Check that submission is prevented
     - Validate error message placement and styling

3. **Field-Specific Validation**
   - **Customer Name Validation**:
     - Test empty customer name submission
     - Verify required field error
     - Test minimum length requirements (if any)
   
   - **Contact Type Validation**:
     - Test submission without contact type selection
     - Verify dropdown validation
   
   - **Date Validation**:
     - Test invalid date formats
     - Check future date restrictions (if any)
     - Verify required date selection

4. **Error Recovery Testing**
   - Fill required fields after validation errors
   - Verify error messages clear appropriately
   - Test successful submission after error correction

5. **Validation Message Quality**
   - Verify error messages are clear and helpful
   - Check that validation guides user to correct action
   - Ensure consistent validation styling

**Expected Results**: Form properly validates required fields and provides clear, helpful error messages.

---

## Test 5: Contact History/List View Access

### Purpose
Validates access to contact history or list view to manage existing customer contacts.

### Test Steps

1. **Dashboard List Access Search**
   - Return to dashboard/home page
   - Look for contact list/history access options:
     - "Contact History"
     - "View Contacts" 
     - "Customer Contacts"
     - Generic "Contacts" links

2. **Navigation Testing**
   - **Dashboard Links**: Test any found contact list links
   - **Menu Navigation**: Check main navigation menu for contact options
   - **Direct URL Access**: Try common contact list URLs:
     - `/contact`
     - `/contact/list`
     - `/contact/index`
     - `/contacts`

3. **List View Validation**
   - **Page Access**: Verify successful access to contact list
   - **Content Structure**: Check for contact list/table display
   - **Search Functionality**: Test contact search features (if available)
   - **Sorting Options**: Verify list sorting capabilities

4. **Contact Management Features**
   - **View Details**: Test individual contact viewing
   - **Edit Functionality**: Check contact editing capabilities
   - **Delete Options**: Verify contact deletion features
   - **Filtering**: Test contact filtering by date, type, etc.

5. **Data Display Validation**
   - Verify contact information displays correctly
   - Check date formatting and sorting
   - Validate contact type categorization
   - Ensure notes/descriptions are accessible

**Expected Results**: Contact list/history is accessible with appropriate management features for viewing and organizing customer contacts.

---

## Shared Test Utilities

### Contact Data Template
```javascript
const generateContactData = (suffix = '') => ({
  customerName: `Test Customer ${suffix}`,
  contactType: 'Phone Call',
  contactDate: new Date().toISOString().split('T')[0],
  notes: `Test contact record ${suffix} - automated test data`
});
```

### Success Detection Helper
```javascript
const checkContactSuccess = async (page) => {
  const pageContent = await page.textContent('body');
  return pageContent.includes('successfully') || 
         pageContent.includes('created') ||
         pageContent.includes('saved') ||
         pageContent.includes('recorded');
};
```

### Contact ID Extraction
```javascript
const extractContactId = (url) => {
  const match = url.match(/\/contact\/(\d+)|\/contact\/details\/(\d+)|\/(\d+)/);
  return match ? (match[1] || match[2] || match[3]) : null;
};
```

---

## Data Requirements & Business Rules

### Required Fields (Common)
- **Customer Name**: Text, required, non-empty
- **Contact Type**: Selection from predefined options
- **Contact Date**: Valid date, typically current or past dates
- **Notes/Description**: Text area for contact details

### Contact Types (Expected Options)
- Phone Call
- Email
- In-Person Visit
- Online Chat
- Support Ticket
- Follow-up
- Complaint
- Inquiry

### Data Validation Rules
- **Customer Name**: Minimum length, special character handling
- **Date**: Valid date format, reasonable date range
- **Notes**: Character limits, formatting preservation
- **Contact Type**: Must be from predefined list

### Success Indicators
- **Confirmation Message**: "Contact successfully recorded" or similar
- **Page Redirection**: To contact details or list view
- **Data Persistence**: Information stored and retrievable

---

## Performance & Quality Metrics

### Execution Timing
- **Form Load**: Under 3 seconds
- **Form Submission**: Under 5 seconds
- **List View Access**: Under 3 seconds
- **Complete Workflow**: Under 15 seconds

### Quality Checkpoints
- ✅ Form validation prevents invalid submissions
- ✅ Success confirmation for valid submissions
- ✅ Data integrity maintained
- ✅ Proper navigation and URL handling
- ✅ Contact management features accessible
- ✅ Clean test data removal

### Business Value Validation
- Contact recording supports customer service workflows
- Data capture enables customer interaction tracking
- List/history views support contact management
- Validation ensures data quality and consistency