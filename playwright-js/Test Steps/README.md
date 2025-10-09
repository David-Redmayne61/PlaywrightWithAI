# JavaScript Playwright Test Steps Documentation

## Overview

This directory contains comprehensive test step documentation for the JavaScript Playwright test suite. Each document provides detailed, step-by-step instructions for test execution, validation criteria, and expected results.

## Test Suite Structure

### 🧪 Available Test Documentation

| Test File | Documentation | Purpose |
|-----------|---------------|---------|
| `00-dashboard.spec.js` | [Dashboard Tests Steps](./00-Dashboard_Tests_Steps.md) | Dashboard functionality and navigation validation |
| `01-person-details.spec.js` | [Person Details Tests Steps](./01-Person_Details_Tests_Steps.md) | Person form validation and submission testing |
| `02-person-duplicate.spec.js` | [Person Duplicate Tests Steps](./02-Person_Duplicate_Tests_Steps.md) | Duplicate person detection and prevention |
| `03-export-final.spec.js` | [Export Final Tests Steps](./03-Export_Final_Tests_Steps.md) | Complete export functionality testing |
| `04-export-formats.spec.js` | [Export Formats Tests Steps](./04-Export_Formats_Tests_Steps.md) | Multi-format export validation |
| `05-import-testing.spec.js` | [Import Testing Steps](./05-Import_Testing_Steps.md) | Comprehensive Excel import testing (6 scenarios) |

## Test Documentation Standards

### 📋 Document Structure

Each test step document follows a consistent structure:

1. **Purpose Statement** - Clear objective of the test
2. **Prerequisites** - Required setup and conditions
3. **Detailed Test Steps** - Step-by-step execution instructions
4. **Expected Results** - Success criteria and validation points
5. **Test Data** - Specific data used during testing
6. **Cleanup Actions** - Steps to restore system state

### 🎯 Test Categories

#### **UI Validation Tests**
- Dashboard Tests: Navigation, authentication, UI elements
- Person Details Tests: Form validation, field testing
- Export/Import Pages: UI functionality, element validation

#### **Data Manipulation Tests**
- Person Creation: Form submission, data validation
- Duplicate Prevention: Business rule enforcement
- Import/Export: File handling, data integrity

#### **Business Logic Tests**
- Authentication flows
- Data validation rules
- Error handling scenarios
- Success/failure messaging

## Test Execution Guidelines

### 🔧 Prerequisites for All Tests

```javascript
// Required setup
- Application running at https://localhost:7031
- Valid admin credentials (Admin/Admin123!)
- Chromium browser (headed mode, 1920x1080)
- Clean database state for consistent results
```

### 📊 Test Data Standards

#### **Person Test Data Format**
```javascript
{
  Forename: "TestFirstName",     // Required: Non-empty string
  FamilyName: "TestLastName",    // Required: Non-empty string  
  Gender: "Male|Female|Other",   // Required: Valid selection
  YearOfBirth: 1900-2024        // Required: 4-digit number ≥1900
}
```

#### **Import File Requirements**
- **Format**: Excel (.xlsx) files only
- **Structure**: Header row with exact field names
- **Validation**: Required fields must be present and valid

### 🧹 Cleanup Standards

All tests must include proper cleanup:
- Delete test persons created during testing
- Remove temporary files (Excel imports, downloads)
- Verify system returns to clean state
- Save trace files for debugging

## Performance Benchmarks

### ⏱️ Expected Execution Times

| Test Category | Individual Test | Full Suite |
|---------------|-----------------|------------|
| Dashboard | 3-5 seconds | - |
| Person Details | 5-8 seconds | - |
| Person Duplicate | 6-10 seconds | - |
| Export Final | 8-12 seconds | - |
| Export Formats | 10-15 seconds | - |
| Import Testing | 25-30 seconds | - |
| **Total Suite** | - | **60-80 seconds** |

### 📈 Success Criteria

- ✅ All tests pass consistently
- ✅ No test data remains after execution
- ✅ System remains responsive and stable
- ✅ Error scenarios handled gracefully
- ✅ Success messages detected correctly

## Quality Assurance

### 🔍 Validation Points

#### **Data Integrity**
- Input data matches stored data
- No data corruption during operations
- Proper handling of special characters
- Correct data type preservation

#### **User Experience**
- Clear error messages for invalid operations
- Intuitive navigation and UI behavior
- Responsive design across viewport sizes
- Accessibility compliance

#### **System Stability**
- No memory leaks during test execution
- Proper cleanup of resources
- Stable performance under test load
- Consistent behavior across test runs

### 🚨 Error Handling

#### **Common Error Scenarios**
- Invalid login credentials
- Network connectivity issues  
- File upload failures
- Data validation errors
- Duplicate person detection

#### **Recovery Procedures**
- Automatic retry mechanisms where appropriate
- Graceful degradation for non-critical failures
- Clear error reporting and logging
- System state restoration after errors

## Maintenance and Updates

### 🔄 Document Maintenance

These test step documents should be updated when:
- Application UI changes affect test procedures
- New functionality is added or modified
- Business rules or validation logic changes
- Test data requirements are updated
- Performance benchmarks need adjustment

### 📝 Version Control

- All test step documents are version controlled
- Changes should be documented with clear commit messages
- Major changes should include impact assessment
- Backward compatibility considerations noted

### 🤝 Collaboration

- Test steps should be clear enough for manual execution
- Business stakeholders can understand test objectives
- Developers can implement automated tests from these steps
- QA team can validate test coverage and completeness

---

*For technical implementation details, refer to the actual test files in the `../tests/` directory.*