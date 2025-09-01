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
