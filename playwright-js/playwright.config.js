// @ts-check
/** @type {import('@playwright/test').PlaywrightTestConfig} */
const config = {
  testDir: './tests',
  workers: 1, // Force sequential execution
  fullyParallel: false, // Disable parallel execution
  reporter: [['html', { open: 'never' }]],
  use: {
  headless: false,
  viewport: { width: 1920, height: 1080 },
  trace: 'on',
  screenshot: 'only-on-failure',
  video: 'retain-on-failure',
  ignoreHTTPSErrors: true
  }
};
module.exports = config;
