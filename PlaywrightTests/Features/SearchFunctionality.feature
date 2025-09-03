Feature: Search Functionality
    As a logged-in user
    I want to navigate to the Search page
    So that I can find a person record

Background:
    Given I am logged in as "davred"
    And I am on the main page
    And I click on the Search link

Scenario: Search for a record
    When I view the Search form, I enter "Burch" in the family name
    And then I click Search
    Then I should see a single row of data with Forename "Dorothy", Family Name "Burch", Gender "Female" and Year of Birth "1901"

Scenario: Search for records using wildcards
    When I view the search form
    And I click Clear
    Then I should see a blank search form
    When I select "Female" from the Gender selector and I click Search
    Then I should see 45 records all of whom are Gender = Female
