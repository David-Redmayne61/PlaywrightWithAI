# Customer Contact Records Management Tests - Test Steps (JavaScript)

## Test Suite: Customer Contact Records Management and Editing

### Purpose
Validates the complete customer contact records management system including contact list structure, record navigation methods, edit mode verification, and comprehensive edit workflow with dashboard metrics validation.

### Prerequisites
- Application running at `https://localhost:7031`
- Valid admin credentials available
- Existing customer contact records (created via 06-customer-contact.spec.js)
- Clean database state with known test records
- Customer contact functionality accessible

---

## Test 1: Contact List and Verify Page Structure

### Purpose
Validates the customer contact list page structure, widget presence, and navigation elements.

### Test Steps

1. **Authentication Setup**
   - Launch browser (headed mode, 1920x1080 viewport)
   - Navigate to application home page
   - Complete login with Admin credentials
   - Verify successful authentication and dashboard access

2. **Dashboard Metrics Capture**
   - Navigate to main dashboard
   - Locate and capture values from dashboard widgets:
     - Total Customer Calls
     - Open Customer Calls  
     - Pending Customer Calls
     - Today's Customer Calls
   - Store metrics for later validation

3. **Contact List Navigation**
   - Navigate to `/contact` endpoint
   - Wait for page load and network idle state
   - Verify successful arrival at contact list page

4. **Page Structure Validation**
   - Verify presence of 4 main widgets:
     - Total Contacts
     - Today's Contacts  
     - This Week
     - Unique Customers
   - Validate navigation buttons and controls
   - Confirm data grid/table presence with customer contact records
   - Verify page layout and functionality

### Expected Results
- Contact list page loads successfully
- All 4 widgets are present and functional
- Data grid displays existing customer contact records
- Navigation elements are accessible

---

## Test 2: Record Navigation Methods - Call Number Link, Eye Icon, and Pencil Icon

### Purpose
Validates three different methods for accessing customer contact record details: call number links, eye icons, and pencil icons.

### Test Steps

1. **Test Data Setup**
   - Navigate to customer contact list
   - Identify target record with call number "20251010-001"
   - Capture original record state for verification

2. **Method 1: Call Number Link Navigation**
   - Click on call number text "20251010-001"
   - Verify navigation to record details page
   - Confirm URL pattern `/Contact/Details/7` (case-sensitive)
   - Validate record information display
   - Return to contact list

3. **Method 2: Eye Icon Navigation**
   - Locate FontAwesome eye icon in record row
   - Test multiple eye icon selectors:
     - `i.fa-eye`
     - `i[class*="eye"]`
     - `.fa-eye`
   - Click eye icon and verify navigation
   - Confirm same record details page access
   - Return to contact list

4. **Method 3: Pencil Icon Navigation**
   - Locate FontAwesome pencil icon in record row
   - Test multiple pencil icon selectors:
     - `i.fa-pencil`
     - `i[class*="pencil"]`
     - `.fa-pencil`
   - Click pencil icon and verify navigation
   - Confirm navigation to edit mode
   - Validate edit functionality access

### Expected Results
- All three navigation methods successfully access record details
- Call number link provides view-only access
- Eye icon provides view-only access  
- Pencil icon provides edit access
- Navigation URLs are consistent and correct

---

## Test 3: Edit Mode Entry and Verification

### Purpose
Validates edit mode entry and verification of edit indicators including visual cues and timestamp display.

### Test Steps

1. **Edit Mode Access**
   - Navigate to contact list
   - Click pencil icon for target record
   - Verify successful navigation to edit mode
   - Confirm URL pattern `/Contact/Edit/7`

2. **Edit Mode Indicator Verification**
   - Search for edit mode visual indicators:
     - Underlined text elements indicating editable fields
     - Form elements in edit state
     - Edit-specific UI controls
   - Validate edit mode accessibility

3. **Timestamp Verification**
   - Look for "Editing on dd/mm/yyyy" timestamp display
   - Verify current date appears in editing timestamp
   - Confirm user context (Admin user mentioned)
   - Validate timestamp format and accuracy

### Expected Results
- Edit mode is successfully entered
- Visual edit indicators are present and clear
- Editing timestamp displays current date
- User context is properly shown

---

## Test 4: Edit Record - Change Status and Add Comment (Complete Workflow)

### Purpose
Validates the complete edit workflow including status changes, comment addition, save operations, and dashboard metrics validation.

### Test Steps

1. **Dashboard Metrics Baseline**
   - Navigate to main dashboard
   - Capture baseline metrics:
     - Total Customer Calls
     - Open Customer Calls
     - Pending Customer Calls
   - Store values for comparison

2. **Record Access and Initial State**
   - Navigate to customer contact list
   - Capture original record state (call number "20251010-001")
   - Click pencil icon to enter edit mode
   - Verify successful edit mode entry

3. **Status Change Operation**
   - Locate status dropdown/select field using multiple selectors:
     - `select[name*="status" i]`
     - `select[id*="status" i]`
     - `select:has(option:text-matches("open|pending|closed", "i"))`
   - Capture current status value (should be "0" for Open)
   - Change status from "Open" to "Pending"
   - Verify status change to value "1"

4. **Visual Status Verification**
   - Look for yellow Pending label appearance
   - Test selectors:
     - `.text-warning:has-text("Pending")`
     - `.badge-warning:has-text("Pending")`
     - `[class*="yellow"]:has-text("Pending")`
   - Confirm visual status indicator update

5. **Comment Addition to RTF Field**
   - Locate Reason for Contact RTF field using selectors:
     - `[contenteditable="true"]`
     - `iframe[title*="Rich Text"]`
     - `.ql-editor`
   - Handle different RTF editor types (iframe, contenteditable)
   - Append comment "Waiting to hear back from her" to existing content
   - Verify comment addition without overwriting existing text

6. **Save Operation**
   - Click "Update Customer Call" button
   - Verify navigation back to contact list
   - Look for success confirmation message
   - Validate successful save operation

7. **Record Update Verification**
   - Confirm record shows "Pending" status in list
   - Verify Last Modified date is updated to current date
   - Validate record changes are persisted

8. **Dashboard Metrics Validation**
   - Navigate back to main dashboard
   - Wait for metrics to refresh (5 second delay + page reload)
   - Capture updated dashboard metrics
   - Verify expected changes:
     - Total Customer Calls: unchanged
     - Open Customer Calls: decreased by 1
     - Pending Customer Calls: increased by 1
   - Validate business logic accuracy

### Expected Results
- Status successfully changes from Open to Pending
- Yellow Pending label appears
- Comment is properly appended to existing RTF content
- Save operation completes successfully
- Record updates are persisted and visible
- Dashboard metrics accurately reflect the status change:
  - Open Customer Calls: -1
  - Pending Customer Calls: +1

---

## Technical Implementation Notes

### RTF Field Handling
- Supports multiple RTF editor types: iframe-based, contenteditable, Quill editor
- Uses proper append logic to add content after existing text
- Handles both HTML and text content verification

### Selector Resilience
- Multiple selector strategies for each element type
- FontAwesome icon detection with fallback selectors
- Case-insensitive and flexible matching patterns

### Dashboard Metrics Integration
- Baseline capture before operations
- Wait periods for metric refresh
- Page reload to ensure latest data
- Comprehensive before/after comparison

### Error Handling
- Graceful fallback between selector strategies
- Comprehensive error logging and attachments
- Visual verification through screenshots and videos

---

## Test Data Requirements

### Required Test Records
- Customer contact record with call number "20251010-001"
- Record must be in "Open" status initially
- Record must have existing RTF content in Reason for Contact field
- Valid customer association and contact details

### Dashboard State
- Functional widget system showing accurate metrics
- Real-time or near-real-time metric updates
- Proper status-based counting logic

---

## Dependencies

### Test Suite Dependencies
- **06-customer-contact.spec.js**: Creates test records in Open status
- Authentication system with Admin/Admin123! credentials
- Functional customer contact management system

### Technical Dependencies
- Playwright JavaScript framework
- Browser automation capabilities
- Network monitoring and wait strategies
- Screenshot and video capture for debugging