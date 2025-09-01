# BDD Pilot Example: Test1 Scenario

This shows how your existing Test1 scenario could look in BDD format, demonstrating the difference in approaches.

## Original Test Steps (from Test1_Steps.md)

```text
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
```text

## BDD Version (Gherkin Format)

```gherkin
Feature: Person Record Management
  As a logged-in user
  I want to add person records to the system
  So that I can maintain a database of people
Background:
  Given I am logged in as "davred"
  And I am on the main page
Scenario: Successfully add a new person record
  When I fill in the person form with:
    | Field         | Value   |
    | Forename      | William |
    | Family Name   | Smith   |
    | Gender        | Male    |
    | Year of Birth | 1985    |
  And I click the SUBMIT button
  Then I should see the success message "Record added successfully!"
Scenario: Prevent duplicate person records
  Given I have added a person record with:
    | Field         | Value   |
    | Forename      | William |
    | Family Name   | Smith   |
    | Gender        | Male    |
    | Year of Birth | 1985    |
  When I try to add the same person record again
  Then I should see a duplicate entry error message
Scenario: Verify navigation menu and user controls
  Then I should see the navigation menu with links:
    | Link        |
    | FirstProject|
    | Home        |
    | View People |
    | Search      |
    | Import Data |
  And I should see a user dropdown labeled "davred"
Scenario: User logout functionality
  When I click the user dropdown "davred"
  And I click the "Logout" option
  Then I should be redirected to the login page
  And I should see the username field and login button
```text

## Comparison: Same Functionality, Different Perspectives
### Traditional C# Approach

- - **Focuses on implementation**: Page objects, assertions, waits
- **Technical accuracy**: Exact element selectors and methods

### BDD Approach

- - **Focuses on behavior**: User goals and expected outcomes
- **Business language**: Terms stakeholders understand

## Implementation Strategy

If you wanted to pilot BDD, you could:
1. **Keep your existing Test1.cs** (it works great!)
2. **Add a Feature file** for the same scenario
3. **Create step definitions** that reuse your existing Page Objects
4. **Compare** which approach your team prefers

## Step Definitions Would Look Like

```csharp
[Given(@"I am logged in as ""(.*)""")]
public async Task GivenIAmLoggedInAs(string username)
{
    var loginPage = new LoginPage(_page);
    await loginPage.LoginAsync(username, "Reinhart2244");
}
[When(@"I fill in the person form with:")]
public async Task WhenIFillInThePersonFormWith(Table table)
{
    var data = table.Rows.First();
    await _page.FillAsync("input[name='Forename']", data["Value"]);
    // etc...
}
```text

## Team Decision Points

Ask your team:
1. **Which format is easier to review** during sprint planning?
2. **Which helps stakeholders validate requirements** better?
3. **Which is easier to maintain** when requirements change?
4.
Both approaches can coexist - you don't have to choose just one! 🤝

```text
