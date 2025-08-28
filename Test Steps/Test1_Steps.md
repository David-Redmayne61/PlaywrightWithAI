# Test1.cs Step-by-Step Test Flow

1. Launch browser and log in as "davred".
2. Assert the main page loads successfully after login.
3. Assert that fields with labels Forename, Family Name, Gender, and Year of Birth exist.
4. Fill in the Forename field with "William".
5. Fill in the Family Name field with "Smith".
6. Select 'Male' from the Gender options.
7. Enter '1985' into Year of Birth.
8. Click the SUBMIT button to add the record.
9. Assert that the success message 'Record added successfully!' is rendered.
10. Wait 1 second to allow UI/backend to process before duplicate entry.
11. Reload the page to reset the form.
12. Re-enter the same data to trigger duplicate entry error.
13. Assert that the duplicate entry error message is rendered.
14. Assert that the top menu bar contains the required links: FirstProject, Home, View People, Search, Import Data.
15. Assert that a dropdown exists labeled with the current username (davred).
16. Click the user dropdown (davred).
17. Click the Logout option.
18. Assert that the login screen is rendered (Username field and Login button).

---

For all future test files, a similar step-by-step .md file will be generated and stored in the project root.
