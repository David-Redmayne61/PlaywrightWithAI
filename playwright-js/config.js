/**
 * Environment configuration helper
 * Provides centralized access to environment variables
 */

const config = {
  // Base application URL
  baseUrl: process.env.BASE_URL || 'https://localhost:7031',
  
  // Test user credentials
  username: process.env.TEST_USERNAME || 'Admin',
  password: process.env.TEST_PASSWORD || 'Admin123!',
  
  // Helper methods to build URLs
  getUrl: (path = '') => {
    const base = config.baseUrl;
    return path ? `${base}${path.startsWith('/') ? path : '/' + path}` : base;
  },
  
  // Specific page URLs
  urls: {
    home: process.env.BASE_URL || 'https://localhost:7031',
    dashboard: `${process.env.BASE_URL || 'https://localhost:7031'}/dashboard`,
    contactCreate: `${process.env.BASE_URL || 'https://localhost:7031'}/contact/create`,
    contactList: `${process.env.BASE_URL || 'https://localhost:7031'}/contact`,
  }
};

module.exports = config;
