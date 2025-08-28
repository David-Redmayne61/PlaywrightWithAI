# Test Steps for Test3.cs: Multiple Delete Functionality

1. **Login**
   - Navigate to the login page.
   - Enter username and password.
   - Submit the login form.

2. **Verify Main Page Loads**
   - Assert that the main page is loaded after login.

3. **Add Four New Records**
   - For each of the following records, fill in Forename, Family Name, Gender, and Year of Birth, then submit:
     - Sharon Jones, Female, 2001
     - Andrew Franks, Male, 1975
     - Jessica Fletcher, Female, 1955
     - Bertholt Brancy, Prefer not to say, 2002

4. **Navigate to View People**
   - Click the 'View People' option to display the main data grid.

5. **Click Delete Records**
   - Click the red 'Delete Records' button beneath the grid to open the multiple delete screen.

6. **Select the Four New Records**
   - For each of the four new records, locate the corresponding row and check its selection box.
   - Assert that exactly four records are selected.

7. **Assert Delete Selected Button**
   - Verify that the 'Delete Selected (4)' button is visible after selection.

8. **Delete the Selected Records**
   - Click the 'Delete Selected (4)' button.
   - Handle the browser confirmation popup by accepting it.

9. **Assert Success Message**
   - Wait for and assert the appearance of the message: 'Successfully deleted 4 records'.

10. **Return to List**
    - Click the 'Back to List' button to return to the main data grid.
    - Wait briefly for navigation.

11. **Assert Records Are Deleted**
    - Verify that none of the four deleted records are present in the grid.

12. **Logout**
    - Open the user dropdown menu.
    - Click 'Logout'.
    - Assert that the login screen is displayed.

---

**End of Test3.cs Steps**
