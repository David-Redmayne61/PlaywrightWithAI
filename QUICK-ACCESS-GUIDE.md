# Quick Access to Playwright Test Reports

This guide shows you multiple easy ways to run tests and access your Playwright test reports without manually navigating through Windows Explorer.

## 🎯 VS Code Tasks (Recommended - Easiest Method)

The fastest way to run tests and access reports directly from VS Code:

### How to Use

1. Press `Ctrl+Shift+P` to open the Command Palette
2. Type "Tasks: Run Task" and press Enter
3.

- **Run Playwright Tests** - Runs all tests and automatically opens the HTML report
- - **Open Latest Test Report** - Just opens the most recent report (no testing)

### Pro Tip

You can also access tasks via the Terminal menu: `Terminal` → `Run Task...`

## 🚀 PowerShell Scripts (Alternative Method)

If you prefer using the terminal or need more control:

### Run Tests and Open Report

```powershell
.\Scripts\run-tests-with-reports.ps1
```text

### Run Tests Without Auto-Opening Report

```powershell
.\Scripts\run-tests-with-reports.ps1 -NoOpen
```text

### Just Open the Latest Report

```powershell
.\Scripts\open-latest-report.ps1
```text

### Show Help

```powershell
.\Scripts\run-tests-with-reports.ps1 -Help
.\Scripts\open-latest-report.ps1 -Help
```text

## 📍 Report Locations

Your test reports are saved in:

- **HTML Report**: `PlaywrightTests\TestResults\PlaywrightReport.html`
- **TRX Report**: `PlaywrightTests\TestResults\PlaywrightResults.trx`

## 💡 Pro Tips for Report Access
### 1. Pin Report to Browser

After opening the report once:
-

- This gives you one-click access for future viewing

### 2. Refresh After New Tests
-

- No need to re-open the file

### 3. Open in VS Code

The scripts provide a `code` command you can copy/paste:

```powershell
code 'C:\Users\David.Redmayne\VSC\PlaywrightWithAI\PlaywrightTests\TestResults\PlaywrightReport.html'
```text

### 4. Keyboard Shortcuts

Set up VS Code keyboard shortcuts for even faster access:
1. Go to `File` → `Preferences` → `Keyboard Shortcuts`
2. Search for "Tasks: Run Task"
3. Assign a shortcut like `Ctrl+Shift+T`

## 🔧 Troubleshooting
### "No reports found"

- - Check that tests completed successfully

### "Tasks not found in VS Code"

- - The `.vscode\tasks.json` file should be present in your project root

### "PowerShell execution policy errors"

Run this command in PowerShell as Administrator:

```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```text

## 📊 What's in the Reports
### HTML Report Contents
-

- 🎬 Videos of failed test runs (if enabled)
- - ⏱️ Execution times and performance data

### TRX Report

- - Can be imported into Azure DevOps, GitHub Actions, etc.

## 🚀 Quick Reference

| What You Want To Do | Method |
|---------------------|--------|
| Run tests + open report | `Ctrl+Shift+P` → "Run Playwright Tests" |
| Just open latest report | `Ctrl+Shift+P` → "Open Latest Test Report" |
| Run tests via terminal | `.\Scripts\run-tests-with-reports.ps1` |
| Open report via terminal | `.\Scripts\open-latest-report.ps1` |
---
**Remember**: The goal is to make accessing your test results as friction-free as possible. Choose the method that works best for your workflow! 🎉
