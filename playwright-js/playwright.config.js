// @ts-check
/** @type {import('@playwright/test').PlaywrightTestConfig} */
const config = {
  testDir: './tests',
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
