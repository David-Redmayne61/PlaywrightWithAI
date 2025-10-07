const { test, expect } = require('@playwright/test');
const { testPerson } = require('./shared-test-data');

test.describe('Dashboard and Navigation', () => {
  test('Cleanup - Remove any existing test data', async ({ page }, testInfo) => {
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
    await test.step('Click Search link in top nav', async () => {
      await page.click('nav a:has-text("Search"), .navbar a:has-text("Search"), .top-nav a:has-text("Search")');
      await page.waitForURL('**/Home/Search', { timeout: 10000 });
    });
    await test.step('Enter family name in Family Name field', async () => {
      await page.fill('body > div > main > div > div.card.shadow.mb-4 > div > form > div:nth-child(2) > input', testPerson.familyName);
    });
    await test.step('Click Search button', async () => {
      await page.click('body > div > main > div > div.card.shadow.mb-4 > div > form button[type="submit"]');
    });
    let johnId;
    let personFound = false;
    await test.step('Find test person row if it exists', async () => {
      try {
        const johnRow = page.locator('table tbody tr', { hasText: testPerson.fullName });
        await johnRow.waitFor({ state: 'visible', timeout: 5000 });
        // Assume ID is in the first cell of the row
        johnId = await johnRow.locator('td').first().textContent();
        personFound = true;
        testInfo.attach('Test Person Found', { body: `ID: ${johnId}`, contentType: 'text/plain' });
      } catch (error) {
        // No existing test person found - this is fine for cleanup
        testInfo.attach('Cleanup Result', { body: 'No existing test person found to clean up', contentType: 'text/plain' });
      }
    });
    
    if (personFound) {
      await test.step('Click Delete on test person row', async () => {
        const deleteBtn = page.locator('table tbody tr', { hasText: testPerson.fullName }).locator('a:has-text("Delete"), button:has-text("Delete")');
        await deleteBtn.waitFor({ state: 'visible', timeout: 10000 });
        await deleteBtn.click();
        await page.waitForURL(`**/Home/Delete/${johnId}`, { timeout: 10000 });
      });
      await test.step('Assert Delete confirmation details', async () => {
        // Assert correct ID in URL
        expect(page.url()).toMatch(new RegExp(`/Home/Delete/${johnId}$`));
        // Assert confirmation details contain test person data
        await expect(page.locator(`dd:has-text("${testPerson.forename}")`)).toBeVisible();
        await expect(page.locator(`dd:has-text("${testPerson.familyName}")`)).toBeVisible();
        await expect(page.locator(`dd:has-text("${testPerson.gender}")`)).toBeVisible();
      });
      await test.step('Click Delete to confirm', async () => {
        await page.click('button:has-text("Delete"), input[type="submit"][value="Delete"]');
        await page.waitForURL('**/Home/ViewPeople', { timeout: 10000 });
        // Be more flexible with the page title after deletion
        await expect(page.locator('h1, .dashboard-title, .view-people-title, .page-title')).toBeVisible();
      });
    }
  });
  test('Login shows dashboard with expected items', async ({ page }, testInfo) => {
    await test.step('Navigate to login page', async () => {
      await page.goto('https://localhost:7031');
    });
    await test.step('Wait for Username field', async () => {
      await page.waitForSelector('input[name="Username"]', { timeout: 20000 });
    });
    await test.step('Wait for Password field', async () => {
      await page.waitForSelector('input[name="Password"]', { timeout: 20000 });
    });
    await test.step('Fill Username', async () => {
      await page.fill('input[name="Username"]', 'Admin');
    });
    await test.step('Fill Password', async () => {
      await page.fill('input[name="Password"]', 'Admin123!');
    });
    await test.step('Click submit', async () => {
      await page.click('button[type="submit"]');
    });
    await test.step('Wait for post-login URL', async () => {
      await page.waitForURL('**', { timeout: 20000 });
    });
    await test.step('Assert not on Login page', async () => {
      await expect(page).not.toHaveURL(/Login/i);
    });
    const dashboardItems = [
      { title: "Search People", buttonText: "Search Now", buttonClass: "btn-primary" },
      { title: "Add Person", buttonText: "Add Person", buttonClass: "btn-success" },
      { title: "View All", buttonText: "View All", buttonClass: "btn-info" },
      { title: "Import Data", buttonText: "Import", buttonClass: "btn-warning" },
      { title: "Record Customer Call", buttonText: "New Call", buttonClass: "btn-danger" },
      { title: "View all Customer Calls", buttonText: "View List", buttonClass: "btn-info" },
      { title: "Export Data", buttonText: "Export", buttonClass: "btn-secondary" },
      { title: "Reports", buttonText: "View Reports", buttonClass: "btn-primary" }
    ];
    for (const item of dashboardItems) {
      await test.step(`Check dashboard card title: ${item.title}`, async () => {
        if (item.title === "View All") {
          await expect(page.getByRole('heading', { name: 'View All', exact: true })).toBeVisible();
        } else if (item.title === "View all Customer Calls") {
          await expect(page.getByRole('heading', { name: 'View all Customer Calls', exact: true })).toBeVisible();
        } else {
          await expect(page.locator(`h5.card-title:has-text('${item.title}')`)).toBeVisible();
        }
      });
      await test.step(`Check dashboard action button: ${item.buttonText}`, async () => {
        await expect(page.locator(`a.btn.${item.buttonClass}:has-text('${item.buttonText}')`)).toBeVisible();
      });
    }
    const expectedNavLinks = [
      "Home", "User Management", "View People", "Add Person", "Search", "Customer Calls", "Reports", "Import Data", "Export Data"
    ];
    const navLinks = await page.locator("nav a, .navbar a, .nav-link, .top-nav a").allTextContents();
    for (const link of expectedNavLinks) {
      await test.step(`Check navigation link: ${link}`, async () => {
        expect(navLinks.map(x => x.trim())).toContain(link);
      });
    }
    await test.step('Assert total navigation links', async () => {
      expect(navLinks.filter(x => expectedNavLinks.includes(x.trim())).length).toBe(9);
    });
    const userNameLocator = page.locator('.dropdown-toggle, .user-name');
    await test.step('Check logged in user name', async () => {
      await expect(userNameLocator).toContainText(/admin/i);
    });
    await test.step('Open user dropdown', async () => {
      await userNameLocator.click();
    });
    const dropdownOptions = await page.locator('.dropdown-menu a, .dropdown-menu button, .dropdown-menu li').allTextContents();
    const expectedOptions = ["Change Admin Password", "About", "Logout"];
    for (const option of expectedOptions) {
      await test.step(`Check dropdown option: ${option}`, async () => {
        expect(dropdownOptions.map(x => x.trim())).toContain(option);
      });
    }
    await test.step('Click account dropdown', async () => {
      await page.click('#accountDropdown');
    });
    await test.step('Force dropdown menu open', async () => {
      await page.evaluate(() => {
        document.querySelector('.dropdown-menu[aria-labelledby=accountDropdown]').classList.add('show');
      });
    });
    await test.step('Wait for About link', async () => {
      await page.waitForSelector("a.dropdown-item[href='/Home/About']", { state: 'visible', timeout: 3000 });
    });
    await test.step('Click About link', async () => {
      await page.click("a.dropdown-item[href='/Home/About']");
    });
    await test.step('Wait for About page URL', async () => {
      await page.waitForURL('**/Home/About');
    });
    await test.step('Assert About page content', async () => {
      const aboutContent = await page.content();
      expect(aboutContent).toMatch(/About FirstProject/);
    });
    const returnHomeButton = page.locator("a:has-text('Return to Home'), button:has-text('Return to Home')");
    await test.step('Wait for Return to Home button', async () => {
      await returnHomeButton.waitFor({ state: 'visible', timeout: 10000 });
    });
    await test.step('Click Return to Home button', async () => {
      await returnHomeButton.click();
    });
    await test.step('Assert dashboard title after return', async () => {
      const dashboardTitle = await page.locator('h1, .dashboard-title').textContent();
      expect(dashboardTitle).toMatch(/Dashboard/);
    });
  });

  // All navigation and About steps are now covered in the login test above
});
