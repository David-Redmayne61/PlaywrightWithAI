# Dashboard Tests - Test Steps (JavaScript)

## Test: Login_ShowsDashboardWithExpectedItems

### Purpose
Validates that the dashboard displays correctly after login with all expected navigation elements and cards.

### Prerequisites
- Application running at `https://localhost:7031`
- Valid admin credentials available

### Test Steps

1. **Browser Setup**
   - Launch Chromium browser in headed mode
   - Set viewport to 1920x1080 for consistent rendering
   - Enable trace recording for debugging

2. **Navigation & Login**
   - Navigate to `https://localhost:7031`
   - Wait for login page to load (timeout: 20 seconds)
   - Locate username input field: `input[name="Username"]`
   - Fill username: `Admin`
   - Locate password input field: `input[name="Password"]`
   - Fill password: `Admin123!`
   - Click submit button: `button[type="submit"]`
   - Wait for navigation completion (wildcard URL match)

3. **Post-Login Validation**
   - Verify URL does not contain `/Login` (successful redirect)
   - Wait for page load state: 'networkidle'
   - Capture current URL for debugging

4. **Dashboard Cards Validation**
   - Locate and validate each dashboard card:
     - **Search People Card**: Title and "Search Now" button (btn-primary)
     - **Add Person Card**: Title and "Add Now" button (btn-success)
     - **Customer Calls Card**: Title and "View Calls" button (btn-info)
     - **Reports Card**: Title and "View Reports" button (btn-warning)
     - **Import Data Card**: Title and "Import Now" button (btn-secondary)
     - **Export Data Card**: Title and "Export Now" button (btn-dark)
   - Assert each card has proper styling and is clickable

5. **Top Navigation Validation**
   - Verify presence of 9 navigation links:
     1. Home
     2. User Management
     3. View People
     4. Add Person
     5. Search
     6. Customer Calls
     7. Reports
     8. Import Data
     9. Export Data
   - Assert each link is visible and properly labeled
   - Verify total count matches expected (9 links)

6. **User Authentication Display**
   - Locate user display area (top-right)
   - Verify "Admin" username is displayed
   - Locate user dropdown trigger
   - Click to open user dropdown menu

7. **User Dropdown Validation**
   - Verify dropdown contains 3 options:
     1. Change Admin Password
     2. About
     3. Logout
   - Assert each option is present and clickable
   - Verify total count matches expected (3 options)

8. **About Page Flow**
   - Click "About" option from dropdown
   - Wait for About page to display
   - Verify About page content is visible
   - Locate "Return to Home" button
   - Click "Return to Home"
   - Verify return to dashboard (URL and content validation)

9. **Test Cleanup**
   - Save Playwright trace file for debugging
   - Capture final screenshot
   - Close browser session

### Expected Results
- ✅ Successful login and dashboard display
- ✅ All 6 dashboard cards present with correct styling
- ✅ All 9 navigation links present and properly labeled
- ✅ User dropdown with 3 options functions correctly
- ✅ About page navigation works bidirectionally
- ✅ No console errors or broken elements

### Data Captured
- Initial page HTML for selector analysis
- Current URL at each navigation step
- Playwright trace for debugging
- Screenshots on any failures