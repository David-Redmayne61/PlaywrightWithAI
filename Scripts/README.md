# Scripts Directory

This folder contains PowerShell scripts for various project automation tasks.

## Scripts Overview

### 🧪 Test Automation

- **`run-tests-with-reports.ps1`** - C# Playwright test runner with comprehensive reporting
  - Run tests: `.\run-tests-with-reports.ps1`
  - Run without auto-opening report: `.\run-tests-with-reports.ps1 -NoOpen`

- **`run-playwright-allure.ps1`** - JavaScript Playwright tests with Allure reporting
  - Run tests and open Allure report: `.\run-playwright-allure.ps1`
  - Run without auto-opening report: `.\run-playwright-allure.ps1 -NoOpen`

- **`open-latest-report.ps1`** - Quick access to the most recent C# test report
  - Open latest report: `.\open-latest-report.ps1`

- **`open-allure-report.ps1`** - Open existing Allure report without running tests
  - Open Allure report: `.\open-allure-report.ps1`

### 📄 Documentation Processing

- - **`cleanup-stepmd.ps1`** - Cleans up and formats markdown test step files

### 📊 Coverage & Guidance

- - **`explore-bdd.ps1`** - Interactive guide for exploring BDD/Gherkin options for your project

## Usage

### From VS Code (Recommended)

Use the Command Palette (`Ctrl+Shift+P`) and search for "Tasks: Run Task", then select:

- **Run Playwright Tests** - Runs tests and opens the report
- - **Open Latest Test Report** - Just opens the most recent report

### From Terminal

To run any script from the project root:

```powershell
# Navigate to the project root
cd "c:\Users\David.Redmayne\VSC\PlaywrightWithAI"
# Run a script
.\Scripts\script-name.ps1
```text

Or run from within the Scripts folder:

```powershell
# Navigate to Scripts folder
cd Scripts
# Run a script
.\script-name.ps1
```text

## Notes

- - Make sure PowerShell execution policy allows script execution: `Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser`
- For meaningful code coverage, use the web-app-coverage-guide to set up proper unit and integration tests
