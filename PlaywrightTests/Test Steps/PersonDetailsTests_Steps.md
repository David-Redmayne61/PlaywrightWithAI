# PersonDetailsTests - Test Steps

## Add and Delete Person

1. Login as Admin and render dashboard.
2. Click Add Person button from Quick Actions.
3. Assert Add Person form fields are visible.
4. Enter Forename 'John', Family Name 'Smith', Gender 'Male', Year of Birth '1976'.
5. Submit Add Person form.
6. Assert success message for new person.
7. Assert Total People value incremented by one.
8. Attempt to add duplicate person (John Smith).
9. Assert duplicate entry warning message is rendered.
10. Return to dashboard.
11. Click Search link in top nav.
12. Enter 'Smith' in Family Name field.
13. Click Search button.
14. Find John Smith row and note ID.
15. Click Delete on John Smith row.
16. Assert correct ID in URL and confirmation details (Forename John, Family Name Smith, Gender Male).
17. Click Delete to confirm.
18. Assert navigation to ViewPeople page.
