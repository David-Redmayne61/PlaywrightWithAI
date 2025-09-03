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

Scenario: Search for records using wildcards 1
    When I view the search form
    And I click Clear
    Then I should see a blank search form
    When I select "Female" from the Gender selector and I click Search
    Then I should see 45 records all of whom are Gender = Female

Scenario: Search for records using wildcards 2
    When I view the search form
    And I enter "A*" into the Family Name field
    When I click Search
    Then I should see 5 records whose family name starts with "A"

Scenario: Search for records using wildcards 3
    When I view the search form
    And I enter "Al*" into the Family Name field
    When I click Search
    Then I should see 3 records whose family name starts with "Al"

Scenario: Search for records using wildcards 4
    When I view the search form
    And I enter "D*" into the Family Name field
    And I enter "I*" into the Forename field
    When I click Search
    Then I should see 2 records

Scenario: Search for records using wildcards 5
    When I view the search form
    And I enter "1941" in the Year of Birth field
    When I click Search
    Then I should see 4 records with IDs 82, 35, 30 and 47
