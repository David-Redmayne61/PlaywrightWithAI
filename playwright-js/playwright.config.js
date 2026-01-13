// @ts-check
require('dotenv').config({ path: './env/.env' });

/** @type {import('@playwright/test').PlaywrightTestConfig} */
const config = {
  testDir: './tests',
  workers: 1, // Force sequential execution
  fullyParallel: false, // Disable parallel execution
  reporter: [
    ['html', { open: 'never' }],
    ['allure-playwright', { 
      outputFolder: 'allure-results',
      detail: true,
      suiteTitle: false
    }]
  ],
  use: {
  headless: false,
  viewport: { width: 1920, height: 1080 },
  trace: 'on',
  screenshot: 'on',
  video: 'retain-on-failure',
  ignoreHTTPSErrors: true
  }
};
module.exports = config;
