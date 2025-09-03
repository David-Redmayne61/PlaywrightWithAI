# PlaywrightWithAI

Comprehensive automated testing project featuring both traditional C# Playwright tests and modern BDD/SpecFlow scenarios with advanced reporting and browser optimization.

## 🚀 Quick Start

### Running All Tests

- **Traditional C# Tests**: `.\Scripts\run-tests-with-reports.ps1`
- **BDD/SpecFlow Tests**: `.\Scripts\run-bdd-tests.ps1`
- **All Tests Combined**: `.\Scripts\run-bdd-tests.ps1 -AllTests`

### Accessing Reports

📖 **For detailed instructions, see [QUICK-ACCESS-GUIDE.md](QUICK-ACCESS-GUIDE.md)**

## 🧪 Test Architecture

### Traditional C# Tests

- **Test1.cs**: User registration and duplicate prevention
- **Test2.cs**: Complete CRUD operations and data management
- **Test3.cs**: Multiple record operations and bulk actions

### BDD/SpecFlow Tests

- **SearchFunctionality.feature**: 6 comprehensive search scenarios
  - Exact record lookup
  - Gender filtering (45 Female records)
  - Wildcard patterns: A*, Al*, D*+I* combinations
  - Year-based search with specific ID validation

## 📁 Project Structure

```text
PlaywrightWithAI/
├── Scripts/                         # PowerShell automation scripts
│   ├── run-tests-with-reports.ps1   # Traditional test runner with reporting
│   ├── run-bdd-tests.ps1            # BDD test runner with optimization
│   ├── open-latest-report.ps1       # Quick report access
│   └── README.md                     # Scripts documentation
├── PlaywrightTests/                 # Test project
│   ├── Features/                    # BDD/Gherkin feature files
│   ├── StepDefinitions/             # SpecFlow step implementations
│   ├── Pages/                       # Page Object Model classes
│   ├── TestResults/                 # Generated reports (HTML, TRX)
│   └── *.cs                         # Traditional test files
├── Test Steps/                      # Comprehensive test documentation
│   ├── Word/                        # .docx versions of test documentation
│   ├── PDF/                         # .pdf versions of test documentation
│   ├── SearchFunctionality_Steps.md # BDD scenarios documentation
│   ├── Test1_Steps.md               # Traditional test flow
│   ├── Test2_Steps.md               # CRUD operations flow
│   ├── Test3_Steps.md               # Multiple operations flow
│   └── README.md                    # Test suite overview
└── .vscode/                         # VS Code tasks and settings
```

## 🎯 Test Features

### BDD/SpecFlow Features

- **6 Search Scenarios**: Comprehensive wildcard and filtering tests
- **Browser Optimization**: Single browser instance reused across scenarios
- **Step Tracing**: Detailed execution timing and logging
- **Business Readable**: Gherkin syntax for stakeholder clarity

### Traditional Test Features

- **Complete Workflows**: End-to-end user journey validation
- **CRUD Operations**: Full record lifecycle testing
- **UI Validation**: Comprehensive element and navigation testing
- **Session Management**: Login/logout and authentication flows

### Reporting & Performance

- **HTML Reports**: Visual test results with screenshots and videos
- **TRX Reports**: Machine-readable format for CI/CD integration
- **Execution Time**: BDD suite ~40 seconds with browser optimization
- **Detailed Logging**: Step-by-step execution with timing information

## 📊 Reports

Test reports are automatically generated in `PlaywrightTests/TestResults/`:

- **BDD Reports**: BDDTestsReport.html with scenario details
- **Traditional Reports**: TraditionalTestsReport.html with test results
- **Combined Reports**: AllTestsReport.html for complete suite
- **TRX Reports**: Machine-readable format for CI/CD integration
