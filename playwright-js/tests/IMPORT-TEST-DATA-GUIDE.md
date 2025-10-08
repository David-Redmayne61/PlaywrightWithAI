# Excel Import Test Data Guide

## 📋 **Recommended Excel File Structure**

Create an Excel file named `test-import-data.xlsx` with the following structure:

### **Column Headers (Row 1):**
```
Forename | FamilyName | Gender | YearOfBirth
```

### **Test Data Rows:**

#### **✅ Valid Data (Should Import Successfully):**
```
Row 2: John        | TestImport1  | Male   | 1980
Row 3: Jane        | TestImport2  | Female | 1985  
Row 4: Michael     | TestImport3  | Male   | 1975
Row 5: Sarah       | TestImport4  | Female | 1990
Row 6: David       | TestImport5  | Male   | 1988
```

#### **❌ Invalid Data (Should Be Rejected):**
```
Row 7: [empty]     | TestBad1     | Male   | 1985    (Missing forename)
Row 8: BadTest1    | [empty]      | Female | 1990    (Missing family name)
Row 9: BadTest2    | TestBad2     | Unknown| 1985    (Invalid gender)
Row 10: BadTest3   | TestBad3     | Male   | 1850    (Invalid year - too old)
Row 11: BadTest4   | TestBad4     | Female | 2030    (Invalid year - future)
Row 12: BadTest5   | TestBad5     | Male   | ABC     (Non-numeric year)
```

#### **🔍 Edge Cases:**
```
Row 13: Test-Name  | Test-Family  | Male   | 1900    (Boundary year)
Row 14: Test'Name  | Test'Family  | Female | 2025    (Special characters)
Row 15: VeryLongFirstNameTest | VeryLongFamilyNameTest | Male | 1995 (Long names)
```

## 📊 **Excel File Specifications:**

1. **Format**: `.xlsx` (Excel 2007+)
2. **Sheet Name**: "People" or "Sheet1"
3. **Headers**: Must be in Row 1
4. **Data**: Starting from Row 2
5. **File Size**: Keep under 5MB

## 🎯 **What the Tests Will Validate:**

### **✅ Success Scenarios:**
- Valid data imports correctly
- Proper number of records imported
- Data appears in the people list
- Success messages displayed

### **❌ Error Scenarios:**
- Invalid file formats rejected
- Missing required fields flagged
- Invalid data types caught
- Duplicate detection (if implemented)
- File size limits enforced

### **🧹 Automatic Cleanup:**
- All imported test records automatically deleted
- No leftover data in your database
- Clean state for next test run

## 📝 **Sample Excel Content:**

| Forename | FamilyName  | Gender | YearOfBirth |
|----------|-------------|---------|-------------|
| John     | TestImport1 | Male    | 1980        |
| Jane     | TestImport2 | Female  | 1985        |
| Michael  | TestImport3 | Male    | 1975        |
| [empty]  | TestBad1    | Male    | 1985        |
| BadTest2 | TestBad2    | Unknown | 1985        |

## 🚀 **Testing Strategy:**

1. **Place the Excel file** in your test data folder
2. **Run the import tests** - they'll automatically use your file
3. **Review results** - detailed reports on what imported/failed
4. **Automatic cleanup** - no manual data removal needed

## ⚠️ **Important Notes:**

- Use unique names (e.g., "TestImport1", "TestImport2") to avoid conflicts
- Include both valid and invalid data for comprehensive testing
- The test will automatically delete imported records after completion
- Keep file size reasonable for testing purposes

Would you like me to create a sample Excel file for you, or would you prefer to create it yourself based on this guide?