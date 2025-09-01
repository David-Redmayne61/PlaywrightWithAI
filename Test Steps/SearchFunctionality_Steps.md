# SearchFunctionality.feature Step-by-Step Test Flow

## Feature Overview

**Purpose**: Test the search functionality to find specific person records
**Type**: BDD/Gherkin Test
**Test Framework**: SpecFlow with Playwright
**Expected Duration**: ~10-15 seconds

## Background Steps (Common Setup)

1. Launch browser and log in as "davred" with password "Reinhart2244".
2. Assert that the main page loads successfully after login.
3. Click on the "Search" link in the navigation menu.
4. Assert that the Search page is loaded and ready for input.

## Scenario: Search for a record

### Test Steps

1. **Navigate to Search Form**: Verify the Search form is displayed with the Family Name input field.
2. **Enter Search Criteria**: Type "Burch" into the Family Name field.
3. **Execute Search**: Click the "Search" button to perform the search operation.
4. **Verify Search Results**: Assert that exactly one row of data is returned with the following details:

   - **Forename**: "Dorothy"
   - **Family Name**: "Burch"
   - **Gender**: "Female"
   - **Year of Birth**: "1901"

### Expected Outcome

- The search should return a single matching record for the specified family name.
- - The search functionality should be responsive and return results quickly.

### Technical Notes

- **Browser**: Chrome (non-headless mode)
- - **Step Definitions**: Located in `StepDefinitions/SearchFunctionalitySteps.cs`
- - **Assertions**: Uses MSTest Assert methods for validation

### Dependencies

- Web application must be running and accessible
- - Login functionality must be working

-

---
*This BDD test demonstrates the search capability and validates that users can successfully find specific person records using family name criteria.*
