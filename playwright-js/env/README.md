# Environment Configuration

This folder contains environment-specific configuration files for test automation.

## Setup

1. Copy `.env.example` to `.env`
2. Update the values in `.env` with your actual credentials
3. The `.env` file is ignored by git and will not be committed

## Files

- **`.env`** - Your actual environment variables (git ignored)
- **`.env.example`** - Template file showing required variables (committed to git)
- **`README.md`** - This file

## Usage

The test suite automatically loads variables from `.env` file. You can access them in your tests via `process.env.BASE_URL`, `process.env.TEST_USERNAME`, etc.
