# PlaywrightWithAI

Automated testing project using Playwright for .NET with comprehensive reporting and easy-to-use automation scripts.

## 🚀 Quick Start

### Running Tests

- **Via Terminal**: `.\Scripts\run-tests-with-reports.ps1`

### Accessing Reports

📖 **For detailed instructions, see [QUICK-ACCESS-GUIDE.md](QUICK-ACCESS-GUIDE.md)**

## 📁 Project Structure

```text
PlaywrightWithAI/
├── Scripts/                    # PowerShell automation scripts
│   ├── run-tests-with-reports.ps1  # Main test runner with reporting
│   ├── open-latest-report.ps1      # Quick report access
│   └── README.md                    # Scripts documentation
├── PlaywrightTests/            # Test project
│   ├── Pages/                  # Page Object Model classes
│   ├── TestResults/           # Generated reports (HTML, TRX)
│   └── *.cs                   # Test files
├── Test Steps/                # Test documentation
└── .vscode/                   # VS Code tasks and settings
```text

## 🧪 Test Features

- **Comprehensive Reporting**: HTML and TRX reports with screenshots/videos
- **CI/CD Ready**: TRX reports for integration with build pipelines

## 📊 Reports

Test reports are automatically generated in `PlaywrightTests/TestResults/`:

- **HTML Report**: Visual test results with screenshots and videos
- **TRX Report**: Machine-readable format for CI/CD integration
