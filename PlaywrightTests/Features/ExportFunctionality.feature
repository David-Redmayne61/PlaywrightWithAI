Feature: Export Functionality
    As a logged-in user
    I want to export data from various pages
    So that I can save records in different formats for external use

Background:
    Given I am logged in as "davred"
    And I am on the main page

Scenario: Access Print Preview from View People page Output Options
    When I click on the "View People" link
    And I view the View People page
    And I click the "Output Options" button
    And I click on "Print Preview"
    Then I should see the Browser Print preview modal
    And then I will click Cancel to close the modal

Scenario: Export data to Excel format from View People
    When I click on the "View People" link
    And I view the View People page
    And I click the "Output Options" button
    And I click on "Export to Excel"
    Then I should receive an Excel file download
    And the Excel file should contain the database records
    And the Excel file should have proper column headers

Scenario: Export data to PDF format from View People
    When I click on the "View People" link
    And I view the View People page
    And I click the "Output Options" button
    And I click on "Export to PDF"
    Then I should receive a PDF file download
    And the PDF file should contain the database records
    And the PDF file should be properly formatted
