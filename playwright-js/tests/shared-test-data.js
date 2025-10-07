// Shared test data for cross-test communication
const timestamp = Date.now();

module.exports = {
  testPerson: {
    forename: 'John',
    familyName: `Smith${timestamp}`,
    fullName: `John Smith${timestamp}`,
    gender: 'Male',
    yearOfBirth: '1976'
  }
};