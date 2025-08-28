# Test2.cs Step-by-Step Test Flow

1. Launch browser and log in as "davred".
2. Assert the main page loads successfully after login.
3. Navigate to the "View People" tab.
4. Assert the page title and presence of "Database Records".
5. Assert required buttons ("Output Options", "Return to Home", "Delete Records") are present.
6. Assert the data grid and required column headings exist.
7. Assert the "Family Name" column is sorted A→Z.
8. Click "Year of Birth" column to sort and assert sorting.
9. Click "Family Name" column to return sort and assert sorting.
10. Find the "William Smith, Male, 1985" record and click Edit.
11. On the Edit screen, change Forename to "William T." and Year of Birth to "1990", then save.
12. Assert the updated record ("William T. Smith, Male, 1990") appears in the grid.
13. Edit the record again, revert Forename to "William" and Year of Birth to "1985", then save.
14. Assert the reverted record ("William Smith, Male, 1985") appears in the grid.
15. Click Delete for "William Smith, Male, 1985".
16. On the Delete Confirmation screen, assert Forename, Family Name, and Gender.
17. Click "Back to List" and assert return to the main grid.
18. Click Delete for "William Smith, Male, 1985" again.
19. On the Delete Confirmation screen, confirm deletion.
20. Assert the user is returned to the main grid and the record is deleted.
21. Click the user dropdown and log out.
22. Assert the login screen is displayed.
