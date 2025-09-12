# PersonDuplicateTests - Test Steps

## Duplicate Prevention

1. Login as Admin and render dashboard.
2. Click Add Person button from Quick Actions.
3. Enter Forename 'John', Family Name 'Smith', Gender 'Male', Year of Birth '1976'.
4. Submit Add Person form.
5. Assert success message for new person.
6. Attempt to add duplicate person (John Smith).
7. Assert duplicate entry warning message is rendered.
8. Return to dashboard.
