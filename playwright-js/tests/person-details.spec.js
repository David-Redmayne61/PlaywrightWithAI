const { test, expect } = require('@playwright/test');

test.describe('Person Details and Functionality', () => {
  test('Add Person form fields and submit button', async ({ page }, testInfo) => {
    await test.step('Login and render dashboard', async () => {
      await page.goto('https://localhost:7031');
      await page.waitForSelector('input[name="Username"]', { timeout: 20000 });
      await page.waitForSelector('input[name="Password"]', { timeout: 20000 });
      await page.fill('input[name="Username"]', 'Admin');
      await page.fill('input[name="Password"]', 'Admin123!');
      await page.click('button[type="submit"]');
      await page.waitForURL('**', { timeout: 20000 });
      await expect(page).not.toHaveURL(/Login/i);
      await expect(page.locator('h1, .dashboard-title')).toBeVisible();
    });
    let totalPeopleValue;
    await test.step('Capture Total People value from dashboard', async () => {
      const totalPeopleLocator = page.locator('body > div > main > div > div:nth-child(2) > div:nth-child(1) > div > div > div.flex-grow-1 > h5');
      await totalPeopleLocator.waitFor({ state: 'visible', timeout: 10000 });
      totalPeopleValue = await totalPeopleLocator.textContent();
      testInfo.attach('Total People Value', { body: totalPeopleValue || 'Not found', contentType: 'text/plain' });
    });
    await test.step('Click Add Person button from Quick Actions', async () => {
      await page.click('a.btn-success:has-text("Add Person")');
      await page.waitForURL(/AddPerson|Add|Person/i, { timeout: 10000 });
    });
    await test.step('Assert Add Person form fields', async () => {
      await expect(page.locator('input[name="Forename"]')).toBeVisible();
      await expect(page.locator('input[name="FamilyName"]')).toBeVisible();
      await expect(page.locator('select[name="Gender"]')).toBeVisible();
      await expect(page.locator('input[name="YearOfBirth"]')).toBeVisible();
      await expect(page.locator('button[type="submit"]:has-text("Submit")')).toBeVisible();
    });
    const forename = 'John';
    const familyName = 'Smith';
    await test.step('Enter data into Add Person form fields', async () => {
      await page.fill('input[name="Forename"]', forename);
      await page.fill('input[name="FamilyName"]', familyName);
      await page.selectOption('select[name="Gender"]', { label: 'Male' });
      await page.fill('input[name="YearOfBirth"]', '1976');
    });
    await test.step('Submit Add Person form', async () => {
      await page.click('button[type="submit"]:has-text("Submit")');
      await page.waitForSelector('.alert-success, .dashboard-message, [role="alert"]', { state: 'visible', timeout: 10000 });
    });
    await test.step('Assert success message for new person', async () => {
      const successMsg = await page.locator('.alert-success, .dashboard-message, [role="alert"]').textContent();
      expect(successMsg).toMatch(/Record for John Smith added successfully!?/i);
      const closeBtn = page.locator('.alert-dismissible.fade.show > button, .dashboard-message .close, [role="alert"] .close');
      await closeBtn.first().waitFor({ state: 'visible', timeout: 10000 });
      await closeBtn.first().click();
    });
    await test.step('Assert Total People value incremented by one', async () => {
      const newTotalPeopleLocator = page.locator('body > div > main > div > div:nth-child(2) > div:nth-child(1) > div > div > div.flex-grow-1 > h5');
      await newTotalPeopleLocator.waitFor({ state: 'visible', timeout: 10000 });
      const newTotalPeople = await newTotalPeopleLocator.textContent();
      const oldValue = parseInt(totalPeopleValue);
      const newValue = parseInt(newTotalPeople);
      expect(newValue).toBe(oldValue + 1);
    });
    await test.step('Attempt to add duplicate person', async () => {
      await page.click('a.btn-success:has-text("Add Person")');
      await page.waitForURL(/AddPerson|Add|Person/i, { timeout: 10000 });
      await page.fill('input[name="Forename"]', 'John');
      await page.fill('input[name="FamilyName"]', 'Smith');
      await page.selectOption('select[name="Gender"]', { label: 'Male' });
      await page.fill('input[name="YearOfBirth"]', '1976');
      await page.click('button[type="submit"]:has-text("Submit")');
      await page.waitForSelector('.alert-danger, .dashboard-message, [role="alert"]', { state: 'visible', timeout: 10000 });
      const warningMsg = await page.locator('.alert-danger, .dashboard-message, [role="alert"]').textContent();
      expect(warningMsg).toMatch(/A person with the name John Smith already exists/i);
      const closeBtn = page.locator('.alert-dismissible.fade.show > button, .dashboard-message .close, [role="alert"] .close');
      await closeBtn.first().waitFor({ state: 'visible', timeout: 10000 });
      await closeBtn.first().click();
      await page.click('nav a:has-text("Home"), .navbar a:has-text("Home"), .top-nav a:has-text("Home")');
      await expect(page.locator('h1, .dashboard-title')).toBeVisible();
    });
  });
});
