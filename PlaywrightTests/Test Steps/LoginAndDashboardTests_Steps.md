# LoginAndDashboardTests - Test Steps

## Login_ShowsDashboardWithExpectedItems

1. Launch Chromium browser (headed, 1920x1080 viewport).
2. Navigate to `https://localhost:7031`.
3. Save initial page HTML for selector analysis.
4. Fill in login credentials:
   - Username: `Admin`
   - Password: `Admin123!`
5. Click the submit button to log in.
6. Wait for navigation after login (wildcard URL match).
7. Save current URL for debugging.
8. For each dashboard item:
   - Assert card title is present (e.g., "Search People", "Add Person", etc.).
   - Assert action button is present with expected text and class (e.g., "Search Now" with `btn-primary`).
9. Assert there are 9 top navigation links with expected texts:
   - Home, User Management, View People, Add Person, Search, Customer Calls, Reports, Import Data, Export Data
   - Assert each link is present and total count matches.
10. Assert logged in user's name (Admin) is present at top right.
11. Open user dropdown and assert 3 options:
    - Change Admin Password, About, Logout
    - Assert each option is present and total count matches.
12. Click About and assert screen displays.
13. In About screen, click Return to Home and assert dashboard is visible again.
14. Save Playwright trace file for the test.
15. Close browser.
