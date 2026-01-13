# PlaywrightWithAI

Modern automated testing project featuring comprehensive JavaScript Playwright tests with HTML reporting and trace analysis, alongside traditional C# Playwright tests for comparison and reference.

## 🚀 Quick Start

### Running JavaScript Tests (Primary Test Suite)

```bash
cd playwright-js
npx playwright test                                    # Run all JS tests
npx playwright test tests/05-import-testing.spec.js   # Run import tests only
npx playwright show-report                            # View HTML report

# Or use the automated script with Allure reporting
cd ..
.\Scripts\run-playwright-allure.ps1                   # Run tests and open Allure report
```

### Running C# Tests (Reference Implementation)

- **Traditional C# Tests**: `.\Scripts\run-tests-with-reports.ps1`
- **BDD/SpecFlow Tests**: `.\Scripts\run-bdd-tests.ps1`
- **All Tests Combined**: `.\Scripts\run-bdd-tests.ps1 -AllTests`

### Accessing Reports

📖 **For detailed instructions, see [QUICK-ACCESS-GUIDE.md](QUICK-ACCESS-GUIDE.md)**

## 🧪 Test Architecture

### JavaScript Playwright Tests (Primary)

- **00-dashboard.spec.js**: Dashboard functionality and navigation testing
- **01-person-details.spec.js**: Person details form validation and submission
- **02-person-duplicate.spec.js**: Duplicate person detection and prevention
- **03-export-final.spec.js**: Data export functionality testing
- **04-export-formats.spec.js**: Export format validation and download testing
- **05-import-testing.spec.js**: Comprehensive Excel import testing (6 test scenarios)
- **06-customer-contact.spec.js**: Customer contact creation and form validation (8 test scenarios)
- **07-customer-contact-records.spec.js**: Customer contact records management and editing (4 test scenarios)

### C# Playwright Tests (Reference)

- **DashboardTests.cs**: Dashboard and navigation validation
- **LoginAndDashboardTests.cs**: Authentication and UI testing
- **Test Steps/**: Detailed documentation of C# test flows

## 📁 Project Structure

```text
PlaywrightWithAI/
├── playwright-js/                   # Primary JavaScript test suite
│   ├── tests/                       # Test specifications
│   │   ├── 00-dashboard.spec.js     # Dashboard functionality
│   │   ├── 01-person-details.spec.js # Person form validation
│   │   ├── 02-person-duplicate.spec.js # Duplicate prevention
│   │   ├── 03-export-final.spec.js  # Export functionality
│   │   ├── 04-export-formats.spec.js # Export formats
│   │   ├── 05-import-testing.spec.js # Import testing (6 scenarios)
│   │   ├── 06-customer-contact.spec.js # Customer contact creation (8 tests)
│   │   ├── 07-customer-contact-records.spec.js # Contact records management (4 tests)
│   │   └── shared-test-data.js      # Common test utilities
│   ├── playwright-report/           # HTML test reports
│   ├── test-results/               # Test execution artifacts
│   ├── playwright.config.js        # Playwright configuration
│   └── package.json                # Node.js dependencies
├── PlaywrightTests/                # C# reference implementation
│   ├── Pages/                      # Page Object Model classes
│   ├── Test Steps/                 # Test documentation
│   ├── DashboardTests.cs           # C# dashboard tests
│   └── LoginAndDashboardTests.cs   # C# authentication tests
├── Scripts/                        # PowerShell automation scripts
│   ├── run-tests-with-reports.ps1  # C# test runner with reporting
│   ├── run-bdd-tests.ps1           # BDD test runner
│   ├── open-latest-report.ps1      # Quick report access
│   └── README.md                    # Scripts documentation
├── .vscode/                        # VS Code configuration
│   └── tasks.json                  # Task definitions
├── QUICK-ACCESS-GUIDE.md           # User guide
└── README.md                       # This file
```

## 🎯 Test Features

### JavaScript Test Features (Primary Suite)

- **📊 Dashboard Testing**: Navigation validation, menu items, authentication flows
- **👤 Person Management**: CRUD operations, form validation, duplicate prevention
- **📤 Export Testing**: Multiple format support (CSV, Excel), download verification
- **📥 Import Testing**: Excel file upload, data validation, error handling (6 comprehensive scenarios)
- **📞 Customer Contact Management**: Contact creation, form validation, dashboard metrics integration (8 creation tests)
- **📋 Contact Records Management**: Record navigation, editing workflows, status management, dashboard updates (4 management tests)
- **🔄 Sequential Execution**: Tests run in order to maintain data consistency
- **📱 Responsive Design**: Tests validate functionality across different viewport sizes

### JavaScript Test Configuration

- **Browser**: Chromium (headed mode for debugging)
- **Viewport**: 1920x1080 for consistent rendering
- **Tracing**: Enabled for debugging failed tests
- **Screenshots**: Captured on test failures
- **Video**: Recorded for failed tests
- **Reports**: Auto-generated HTML reports with detailed step information

### C# Reference Implementation

- **Complete Workflows**: End-to-end user journey validation
- **BDD Scenarios**: Business-readable Gherkin syntax for stakeholder clarity
- **Page Object Model**: Maintainable test structure with reusable components
- **Session Management**: Login/logout and authentication flows

## 📊 Reports & Analysis

### JavaScript Test Reports

#### Playwright HTML Reports
Test reports are automatically generated in `playwright-js/playwright-report/`:

- **Interactive HTML Reports**: Visual test results with expandable steps
- **Screenshots**: Captured on failures for debugging
- **Video Recordings**: Full test execution videos for failed tests
- **Trace Files**: Detailed execution traces for step-by-step analysis
- **Execution Timeline**: Performance metrics and timing information

#### Allure Reports
Enhanced test reporting with Allure framework:

- **Rich Visualizations**: Detailed graphs, charts, and trend analysis
- **Test History**: Track test execution over time
- **Screenshots & Attachments**: Embedded test artifacts
- **Step-by-Step Details**: Granular test step breakdown
- **Categorization**: Organize tests by suite, severity, and features

**Generate Allure Report:**
```bash
# Automated (runs tests and generates report)
.\Scripts\run-playwright-allure.ps1

# Manual generation
cd playwright-js
npx playwright test
npx allure generate allure-results --clean -o allure-report
npx allure open allure-report

# Open existing report
.\Scripts\open-allure-report.ps1
```

### Report Access

- **Playwright HTML**: Run `npx playwright show-report` (served at `http://localhost:9323`)
- **Allure Report**: Run `.\Scripts\run-playwright-allure.ps1` or `.\Scripts\open-allure-report.ps1`
- **C# Reports**: Automatically opened after running `.\Scripts\run-tests-with-reports.ps1`

### C# Test Reports

C# test reports are generated in `PlaywrightTests/TestResults/`:

- **HTML Reports**: Visual test results with screenshots and videos  
- **TRX Reports**: Machine-readable format for CI/CD integration

## 🚀 Getting Started

### Prerequisites

- **Node.js** (v16 or higher) for JavaScript tests
- **.NET 6.0** or higher for C# tests
- **PowerShell** for script execution
- **Visual Studio Code** (recommended)

### Installation

1. Clone the repository
2. Install JavaScript dependencies:
   ```bash
   cd playwright-js
   npm install
   npx playwright install
   ```
3. For C# tests, ensure .NET SDK is installed

### First Test Run

```bash
# Run a single test file to verify setup
cd playwright-js
npx playwright test tests/00-dashboard.spec.js --headed

# Run the comprehensive import test suite
npx playwright test tests/05-import-testing.spec.js --headed

# View the generated report
npx playwright show-report
```
