# Environment Configuration Guide

## Overview

This project now uses environment variables to manage sensitive data like URLs, usernames, and passwords. This keeps credentials secure and out of version control.

## Setup

1. **Environment variables are already configured** in `playwright-js/env/.env`
2. The `.env` file is automatically ignored by git (listed in `.gitignore`)
3. A template file `.env.example` is provided for reference

## Files

- **`playwright-js/env/.env`** - Contains actual credentials (git ignored)
- **`playwright-js/env/.env.example`** - Template showing required variables
- **`playwright-js/config.js`** - Helper module for accessing environment variables

## Usage in Tests

### Import the config module:

```javascript
const config = require('../config');
```

### Use config values in your tests:

```javascript
// Instead of hardcoded values:
await page.goto('https://localhost:7031');
await page.fill('input[name="Username"]', 'Admin');
await page.fill('input[name="Password"]', 'Admin123!');

// Use config values:
await page.goto(config.baseUrl);
await page.fill('input[name="Username"]', config.username);
await page.fill('input[name="Password"]', config.password);
```

### Available config properties:

- `config.baseUrl` - Base application URL
- `config.username` - Test user username
- `config.password` - Test user password
- `config.getUrl(path)` - Helper to build URLs
- `config.urls.home` - Home page URL
- `config.urls.dashboard` - Dashboard URL
- `config.urls.contactCreate` - Contact creation URL
- `config.urls.contactList` - Contact list URL

## Updating Tests

To update your existing tests:

1. Add the config import at the top:
   ```javascript
   const config = require('../config');
   ```

2. Replace hardcoded URLs with `config.baseUrl` or `config.urls.*`
3. Replace hardcoded credentials with `config.username` and `config.password`

## For New Team Members

1. Copy `playwright-js/env/.env.example` to `playwright-js/env/.env`
2. Update the values with actual credentials
3. The `.env` file will never be committed to git

## Security Notes

- ✅ The `.env` file is in `.gitignore` and will not be committed
- ✅ Use `.env.example` to share the template with the team
- ✅ Never commit actual credentials to version control
- ✅ Each team member maintains their own `.env` file

## Example: Updated Test File

See `tests/00-dashboard.spec.js` for an example of using the config module.
