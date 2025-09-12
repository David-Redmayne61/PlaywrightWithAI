const { test, expect } = require('@playwright/test');

test.describe('Person Duplicate Prevention', () => {
  test('Cannot add duplicate person', async ({ page }, testInfo) => {
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
    await test.step('Click Add Person button from Quick Actions', async () => {
      await page.click('a.btn-success:has-text("Add Person")');
      await page.waitForURL(/AddPerson|Add|Person/i, { timeout: 10000 });
    });
    await test.step('Enter duplicate data into Add Person form fields', async () => {
      await page.fill('input[name="Forename"]', 'John');
      await page.fill('input[name="FamilyName"]', 'Smith');
      await page.selectOption('select[name="Gender"]', { label: 'Male' });
      await page.fill('input[name="YearOfBirth"]', '1976');
    });
    await test.step('Submit Add Person form', async () => {
      await page.click('button[type="submit"]:has-text("Submit")');
      await page.waitForSelector('.alert-danger, .dashboard-message, [role="alert"]', { state: 'visible', timeout: 10000 });
    });
    await test.step('Assert duplicate warning message', async () => {
      const warningMsg = await page.locator('.alert-danger, .dashboard-message, [role="alert"]').textContent();
      expect(warningMsg).toMatch(/A person with the name John Smith already exists/i);
    });
  });
});
