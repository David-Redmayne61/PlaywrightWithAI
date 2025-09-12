# Playwright JS Test Suite

This project contains Node.js Playwright tests that mirror the C# MSTest scenarios, including login, dashboard, navigation, and About page checks. HTML reporting and trace viewer are enabled.

## Usage
- Install dependencies: `npm install`
- Run tests: `npx playwright test`
- View HTML report: `npx playwright show-report`
- View trace: `npx playwright show-trace <trace-file>`

## Test Coverage
- Login and dashboard checks
- Navigation and About page
- Return to Home and dashboard assertion

For details, see the test files in `tests/`.
