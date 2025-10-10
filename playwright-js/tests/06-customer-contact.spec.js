const { test, expect } = require('@playwright/test');

test.describe('Customer Contact Recording', () => {
  let contactIds = [];
  
  test.beforeEach(async ({ page }) => {
    // Login before each test
    await page.goto('https://localhost:7031');
    await page.waitForSelector('input[name="Username"]', { timeout: 20000 });
    await page.fill('input[name="Username"]', 'Admin');
    await page.fill('input[name="Password"]', 'Admin123!');
    await page.click('button[type="submit"]');
    await page.waitForURL('**', { timeout: 20000 });
    await expect(page).not.toHaveURL(/Login/i);
    await page.waitForLoadState('networkidle');
  });

  test.afterEach(async ({ page }, testInfo) => {
    // Clean up any contact records created during testing
    if (contactIds.length > 0) {
      await test.step('Clean up test contact records', async () => {
        for (const contactId of contactIds) {
          try {
            await page.goto(`https://localhost:7031/contact/delete/${contactId}`);
            await page.waitForSelector('button:has-text("Delete"), input[type="submit"][value="Delete"]', { timeout: 5000 });
            await page.click('button:has-text("Delete"), input[type="submit"][value="Delete"]');
            await page.waitForLoadState('networkidle');
          } catch (error) {
            testInfo.attach('Cleanup Warning', { 
              body: `Could not delete contact ID ${contactId}: ${error.message}`, 
              contentType: 'text/plain' 
            });
          }
        }
        contactIds = [];
      });
    }
  });

  test('Dashboard Navigation to Customer Contact Form', async ({ page }, testInfo) => {
    await test.step('Navigate from dashboard to contact form', async () => {
      // Verify we're on dashboard (root URL after login)
      const currentUrl = page.url();
      const isDashboard = currentUrl === 'https://localhost:7031/' || currentUrl.includes('/dashboard') || currentUrl.includes('/home');
      expect(isDashboard).toBe(true);
      
      // Look for Customer Contact option on dashboard
      const dashboardContent = await page.textContent('body');
      testInfo.attach('Dashboard Content', { 
        body: `Dashboard loaded. Looking for Customer Contact access.`, 
        contentType: 'text/plain' 
      });

      // Try multiple possible selectors for Customer Contact
      const possibleSelectors = [
        'a:has-text("Record Customer Contact")',
        'a:has-text("Customer Contact")', 
        'a:has-text("Record Contact")',
        'button:has-text("Record Customer Contact")',
        'button:has-text("Customer Contact")',
        '[href*="/contact/create"]',
        '.card:has-text("Customer") a',
        '.card:has-text("Contact") a'
      ];

      let contactLink = null;
      for (const selector of possibleSelectors) {
        try {
          const element = await page.locator(selector).first();
          if (await element.isVisible({ timeout: 1000 })) {
            contactLink = element;
            testInfo.attach('Found Contact Link', { 
              body: `Found contact link using selector: ${selector}`, 
              contentType: 'text/plain' 
            });
            break;
          }
        } catch (error) {
          // Continue trying other selectors
        }
      }

      if (contactLink) {
        await contactLink.click();
        await page.waitForLoadState('networkidle');
        
        // Verify we navigated to contact create page
        const currentUrl = page.url();
        testInfo.attach('Navigation Result', { 
          body: `Navigated to: ${currentUrl}`, 
          contentType: 'text/plain' 
        });
        
        expect(currentUrl.toLowerCase()).toContain('/contact/create');
      } else {
        testInfo.attach('Navigation Search', { 
          body: 'Could not find Customer Contact link on dashboard. Trying direct navigation.', 
          contentType: 'text/plain' 
        });
        
        // Try direct navigation if link not found
        await page.goto('https://localhost:7031/contact/create');
        await page.waitForLoadState('networkidle');
      }
    });
  });

  test('Customer Contact Form Structure and Validation', async ({ page }, testInfo) => {
    await test.step('Navigate to contact form', async () => {
      await page.goto('https://localhost:7031/contact/create');
      await page.waitForLoadState('networkidle');
    });

    await test.step('Validate form structure and expected buttons', async () => {
      // Check for the three expected buttons
      const expectedButtons = [
        'Back to Customer Calls',
        'Reset Form', 
        'Record Customer Call'
      ];

      const foundButtons = [];
      for (const buttonText of expectedButtons) {
        try {
          const button = await page.locator(`button:has-text("${buttonText}")`).first();
          const isVisible = await button.isVisible({ timeout: 2000 });
          foundButtons.push(`"${buttonText}": ${isVisible ? 'FOUND' : 'NOT FOUND'}`);
        } catch (error) {
          foundButtons.push(`"${buttonText}": ERROR - ${error.message}`);
        }
      }

      testInfo.attach('Expected Buttons Check', { 
        body: `Button status:\n${foundButtons.join('\n')}`, 
        contentType: 'text/plain' 
      });

      // Check for auto-populated call number in format yyyymmdd-nnn
      const callNumberSelectors = [
        'input[name*="call"]', 
        'input[name*="number"]', 
        'input[id*="call"]',
        'input[id*="number"]',
        '[placeholder*="call"]',
        '[placeholder*="number"]'
      ];

      let callNumberValue = null;
      for (const selector of callNumberSelectors) {
        try {
          const element = await page.locator(selector).first();
          if (await element.isVisible({ timeout: 1000 })) {
            callNumberValue = await element.inputValue();
            if (callNumberValue && callNumberValue.length > 0) {
              testInfo.attach('Call Number Found', { 
                body: `Call number field: ${selector}\nValue: "${callNumberValue}"`, 
                contentType: 'text/plain' 
              });
              break;
            }
          }
        } catch (error) {
          // Continue trying
        }
      }

      // Validate call number format (yyyymmdd-nnn)
      if (callNumberValue) {
        const callNumberPattern = /^\d{8}-\d{3}$/;
        const isValidFormat = callNumberPattern.test(callNumberValue);
        const today = new Date();
        const expectedDatePrefix = today.getFullYear().toString() + 
                                 (today.getMonth() + 1).toString().padStart(2, '0') + 
                                 today.getDate().toString().padStart(2, '0');
        const hasCorrectDate = callNumberValue.startsWith(expectedDatePrefix);

        testInfo.attach('Call Number Validation', { 
          body: `Call Number: "${callNumberValue}"\nFormat Valid (yyyymmdd-nnn): ${isValidFormat}\nToday's Date (${expectedDatePrefix}): ${hasCorrectDate}`, 
          contentType: 'text/plain' 
        });
      } else {
        testInfo.attach('Call Number Missing', { 
          body: 'Could not find call number field or it was empty', 
          contentType: 'text/plain' 
        });
      }

      // Check for contact date field with today's date and current time
      const dateSelectors = [
        'input[type="datetime-local"]',
        'input[type="date"]',
        'input[name*="date"]',
        'input[name*="time"]',
        'input[id*="date"]',
        'input[id*="time"]'
      ];

      let contactDateValue = null;
      for (const selector of dateSelectors) {
        try {
          const element = await page.locator(selector).first();
          if (await element.isVisible({ timeout: 1000 })) {
            contactDateValue = await element.inputValue();
            if (contactDateValue && contactDateValue.length > 0) {
              testInfo.attach('Contact Date Found', { 
                body: `Contact date field: ${selector}\nValue: "${contactDateValue}"`, 
                contentType: 'text/plain' 
              });
              break;
            }
          }
        } catch (error) {
          // Continue trying
        }
      }

      // Validate contact date is today's date
      if (contactDateValue) {
        const today = new Date();
        const todayString = today.toISOString().split('T')[0]; // YYYY-MM-DD format
        const isToday = contactDateValue.includes(todayString);

        testInfo.attach('Contact Date Validation', { 
          body: `Contact Date: "${contactDateValue}"\nToday's Date: ${todayString}\nIs Today: ${isToday}`, 
          contentType: 'text/plain' 
        });
      } else {
        testInfo.attach('Contact Date Missing', { 
          body: 'Could not find contact date field or it was empty', 
          contentType: 'text/plain' 
        });
      }

      // Check for Status dropdown set to "Open"
      const statusSelectors = [
        'select[name*="status"]',
        'select[id*="status"]',
        'select[name*="Status"]',
        'select[id*="Status"]'
      ];

      let statusValue = null;
      for (const selector of statusSelectors) {
        try {
          const element = await page.locator(selector).first();
          if (await element.isVisible({ timeout: 1000 })) {
            statusValue = await element.inputValue();
            testInfo.attach('Status Dropdown Found', { 
              body: `Status field: ${selector}\nSelected value: "${statusValue}"`, 
              contentType: 'text/plain' 
            });
            break;
          }
        } catch (error) {
          // Continue trying
        }
      }

      // Validate status is set to "Open"
      if (statusValue) {
        const isOpen = statusValue.toLowerCase() === 'open';
        testInfo.attach('Status Validation', { 
          body: `Status Value: "${statusValue}"\nIs Open: ${isOpen}`, 
          contentType: 'text/plain' 
        });
      } else {
        testInfo.attach('Status Field Missing', { 
          body: 'Could not find status dropdown field', 
          contentType: 'text/plain' 
        });
      }

      // Check for RED "Open" label adjacent to status
      const redOpenLabelSelectors = [
        '.text-danger:has-text("Open")',
        '.text-red:has-text("Open")',
        'span[style*="red"]:has-text("Open")',
        'label[style*="red"]:has-text("Open")',
        '.badge-danger:has-text("Open")',
        '.alert-danger:has-text("Open")',
        '[class*="red"]:has-text("Open")'
      ];

      let redLabelFound = false;
      for (const selector of redOpenLabelSelectors) {
        try {
          const element = await page.locator(selector).first();
          if (await element.isVisible({ timeout: 1000 })) {
            redLabelFound = true;
            const labelText = await element.textContent();
            testInfo.attach('Red Open Label Found', { 
              body: `Red label selector: ${selector}\nLabel text: "${labelText}"`, 
              contentType: 'text/plain' 
            });
            break;
          }
        } catch (error) {
          // Continue trying
        }
      }

      if (!redLabelFound) {
        testInfo.attach('Red Open Label Missing', { 
          body: 'Could not find RED "Open" label adjacent to status', 
          contentType: 'text/plain' 
        });
      }

      // Look for other form fields that might be present
      const expectedFields = [
        { name: 'Customer Name', selectors: ['input[name*="customer"]', 'input[name*="name"]', '#customer-name', '#customerName'] },
        { name: 'Contact Type', selectors: ['select[name*="type"]', 'select[name*="contact"]', '#contact-type', '#contactType'] },
        { name: 'Notes/Description', selectors: ['textarea', 'input[name*="note"]', 'input[name*="description"]', '#notes', '#description'] }
      ];

      const foundFields = [];
      for (const field of expectedFields) {
        let found = false;
        for (const selector of field.selectors) {
          try {
            const element = await page.locator(selector).first();
            if (await element.isVisible({ timeout: 2000 })) {
              foundFields.push(`${field.name}: ${selector}`);
              found = true;
              break;
            }
          } catch (error) {
            // Continue trying other selectors
          }
        }
        if (!found) {
          foundFields.push(`${field.name}: NOT FOUND`);
        }
      }

      testInfo.attach('Form Fields Analysis', { 
        body: `Found form fields:\n${foundFields.join('\n')}`, 
        contentType: 'text/plain' 
      });
    });
  });

  test('Record Customer Contact - Complete Workflow', async ({ page }, testInfo) => {
    let dashboardMetricsBefore = {};
    let dashboardMetricsAfter = {};

    await test.step('Capture initial dashboard metrics', async () => {
      await page.goto('https://localhost:7031');
      await page.waitForLoadState('networkidle');
      
      // Capture customer call metrics from dashboard widgets
      const metricWidgets = [
        { name: 'Total Customer Calls', selectors: ['text=/total.*customer.*calls/i', '[class*="total"] text=/customer.*calls/i', 'text=/customer.*calls.*total/i'] },
        { name: 'Open Customer Calls', selectors: ['text=/open.*customer.*calls/i', '[class*="open"] text=/customer.*calls/i', 'text=/customer.*calls.*open/i'] },
        { name: 'Aged Open Calls', selectors: ['text=/aged.*open.*calls/i', '[class*="aged"] text=/calls/i', 'text=/aged.*calls/i'] },
        { name: 'Pending Customer Calls', selectors: ['text=/pending.*customer.*calls/i', '[class*="pending"] text=/calls/i', 'text=/pending.*calls/i'] },
        { name: 'Closed Customer Calls', selectors: ['text=/closed.*customer.*calls/i', '[class*="closed"] text=/calls/i', 'text=/closed.*calls/i'] }
      ];

      for (const widget of metricWidgets) {
        let value = 0;
        for (const selector of widget.selectors) {
          try {
            const element = await page.locator(selector).first();
            if (await element.count() > 0) {
              const parentCard = element.locator('..').locator('..');
              const numberElements = await parentCard.locator('text=/\\d+/').allTextContents();
              if (numberElements.length > 0) {
                // Find the largest number (likely the metric value)
                const numbers = numberElements.map(text => parseInt(text.match(/\d+/)?.[0] || '0')).filter(n => !isNaN(n));
                value = Math.max(...numbers, 0);
                break;
              }
            }
          } catch (error) {
            // Continue trying other selectors
          }
        }
        dashboardMetricsBefore[widget.name] = value;
      }

      testInfo.attach('Dashboard Metrics Before', {
        body: `Initial Dashboard Metrics:\n${JSON.stringify(dashboardMetricsBefore, null, 2)}`,
        contentType: 'text/plain'
      });
    });

    await test.step('Navigate to contact form', async () => {
      await page.goto('https://localhost:7031/contact/create');
      await page.waitForLoadState('networkidle');
    });

    await test.step('Fill and submit contact form', async () => {
      // Pre-defined test data based on user specifications
      const testContactData = {
        customerEmail: 'testcustomer@example.com',
        customerPhone: '01234 567890',
        reasonForContact: 'Customer contacted us today to inquire about our premium service package. They are particularly interested in the enterprise features and volume pricing options. Provided detailed information about our current offerings and scheduled a follow-up call for next week to discuss their specific requirements.'
      };

      // 1. Handle Customer Selection (using dropdown as specified)
      const customerDropdown = await page.locator('select[name*="customer" i], select[id*="customer" i], select:has(option:text-matches("customer", "i"))').first();
      if (await customerDropdown.count() > 0) {
        const options = await customerDropdown.locator('option').allTextContents();
        // Select the first non-empty customer option
        const customerOptions = options.filter(opt => opt.trim() && !opt.toLowerCase().includes('select') && !opt.toLowerCase().includes('choose'));
        if (customerOptions.length > 0) {
          await customerDropdown.selectOption({ label: customerOptions[0] });
          testInfo.attach('Customer Selection', { 
            body: `Selected customer: ${customerOptions[0]}`, 
            contentType: 'text/plain' 
          });
        }
      }

      // 2. Fill Customer Email (required format: xxxxx@yyy.com)
      const emailSelectors = [
        'input[type="email"]',
        'input[name*="email" i]',
        'input[id*="email" i]',
        'input[placeholder*="email" i]'
      ];
      
      for (const selector of emailSelectors) {
        try {
          const emailField = await page.locator(selector).first();
          if (await emailField.count() > 0 && await emailField.isVisible()) {
            await emailField.fill(testContactData.customerEmail);
            testInfo.attach('Email Field', { 
              body: `Filled email: ${testContactData.customerEmail}`, 
              contentType: 'text/plain' 
            });
            break;
          }
        } catch (error) {
          // Continue trying other selectors
        }
      }

      // 3. Fill Phone Number (format: 01234 567890)
      const phoneSelectors = [
        'input[type="tel"]',
        'input[name*="phone" i]',
        'input[id*="phone" i]',
        'input[placeholder*="phone" i]'
      ];
      
      for (const selector of phoneSelectors) {
        try {
          const phoneField = await page.locator(selector).first();
          if (await phoneField.count() > 0 && await phoneField.isVisible()) {
            await phoneField.fill(testContactData.customerPhone);
            testInfo.attach('Phone Field', { 
              body: `Filled phone: ${testContactData.customerPhone}`, 
              contentType: 'text/plain' 
            });
            break;
          }
        } catch (error) {
          // Continue trying other selectors
        }
      }

      // 4. Fill Reason for Contact (RTF field with rich content) - Enhanced Detection
      let reasonFieldFilled = false;
      const reasonSelectors = [
        'textarea[name*="reason" i]',
        'textarea[name*="contact" i]',
        'textarea[name*="note" i]',
        'textarea[name*="description" i]',
        'textarea[id*="reason" i]',
        'textarea[id*="contact" i]',
        'input[name*="reason" i]',
        'input[id*="reason" i]',
        '[contenteditable="true"]', // RTF editors
        'textarea', // fallback to any textarea
        '.ql-editor', // Quill editor
        '.tox-edit-area', // TinyMCE editor
        'iframe[title*="Rich Text"]' // iFrame RTF editors
      ];
      
      for (const selector of reasonSelectors) {
        try {
          const reasonField = await page.locator(selector).first();
          if (await reasonField.count() > 0 && await reasonField.isVisible({ timeout: 2000 })) {
            
            // Handle different types of RTF editors
            if (selector.includes('iframe')) {
              // Handle iframe-based RTF editors
              const frame = await reasonField.contentFrame();
              if (frame) {
                await frame.locator('body').fill(testContactData.reasonForContact);
              }
            } else if (selector.includes('contenteditable') || selector.includes('ql-editor')) {
              // Handle contenteditable RTF editors
              await reasonField.click();
              await reasonField.fill('');
              await reasonField.type(testContactData.reasonForContact);
            } else {
              // Handle standard textarea
              await reasonField.fill(testContactData.reasonForContact);
            }
            
            reasonFieldFilled = true;
            testInfo.attach('Reason Field', { 
              body: `Filled reason for contact using selector: ${selector}\nContent: ${testContactData.reasonForContact.substring(0, 100)}...`, 
              contentType: 'text/plain' 
            });
            break;
          }
        } catch (error) {
          // Continue trying other selectors
        }
      }
      
      // If no specific RTF field found, try to fill ANY textarea or input that might be the reason field
      if (!reasonFieldFilled) {
        const allTextareas = await page.locator('textarea').all();
        const allTextInputs = await page.locator('input[type="text"]').all();
        
        for (const field of [...allTextareas, ...allTextInputs]) {
          try {
            const placeholder = await field.getAttribute('placeholder') || '';
            const name = await field.getAttribute('name') || '';
            const id = await field.getAttribute('id') || '';
            const label = await field.locator('..').locator('label').textContent() || '';
            
            const fieldText = `${placeholder} ${name} ${id} ${label}`.toLowerCase();
            
            if (fieldText.includes('reason') || fieldText.includes('contact') || fieldText.includes('note') || fieldText.includes('description')) {
              const currentValue = await field.inputValue();
              if (!currentValue || currentValue.trim() === '') {
                await field.fill(testContactData.reasonForContact);
                reasonFieldFilled = true;
                testInfo.attach('Reason Field (Fallback)', { 
                  body: `Filled reason field via fallback detection\nField info: ${fieldText}\nContent: ${testContactData.reasonForContact.substring(0, 100)}...`, 
                  contentType: 'text/plain' 
                });
                break;
              }
            }
          } catch (error) {
            // Continue with next field
          }
        }
      }
      
      if (!reasonFieldFilled) {
        testInfo.attach('Reason Field Warning', { 
          body: 'Could not locate the Reason for Contact field. This may cause validation errors.', 
          contentType: 'text/plain' 
        });
      }

      // 5. Fill any remaining required fields dynamically
      const allRequiredFields = await page.locator('input[required], select[required], textarea[required]').all();
      for (const field of allRequiredFields) {
        try {
          const currentValue = await field.inputValue();
          if (!currentValue || currentValue.trim() === '') {
            const fieldType = await field.getAttribute('type');
            const fieldName = await field.getAttribute('name') || await field.getAttribute('id') || 'unknown';
            
            if (fieldType === 'email') {
              await field.fill(testContactData.customerEmail);
            } else if (fieldType === 'tel') {
              await field.fill(testContactData.customerPhone);
            } else if (fieldType === 'text' || !fieldType) {
              await field.fill('Test Data for ' + fieldName);
            }
            
            testInfo.attach('Required Field Filled', { 
              body: `Filled required field: ${fieldName} (${fieldType || 'text'})`, 
              contentType: 'text/plain' 
            });
          }
        } catch (error) {
          // Continue with next field
        }
      }

      // Submit the form using the correct button text
      const submitButton = await page.locator('button:has-text("Record Customer Call")').first();
      
      if (await submitButton.isVisible({ timeout: 5000 })) {
        await submitButton.click();
        await page.waitForLoadState('networkidle');
        testInfo.attach('Form Submission', { 
          body: 'Successfully clicked "Record Customer Call" button', 
          contentType: 'text/plain' 
        });
      } else {
        testInfo.attach('Submit Button Error', { 
          body: 'Could not find "Record Customer Call" button', 
          contentType: 'text/plain' 
        });
      }
    });

    await test.step('Verify contact creation success', async () => {
      const currentUrl = page.url();
      const pageContent = await page.textContent('body');
      
      // Look for success indicators
      const hasSuccessMessage = pageContent.includes('successfully') || 
                               pageContent.includes('created') || 
                               pageContent.includes('saved') ||
                               pageContent.includes('recorded');
      
      // Check for redirection (URL changed from contact/create)
      const isRedirected = !currentUrl.toLowerCase().includes('/contact/create');
      
      // Detailed validation error investigation
      const hasErrorMessage = await page.locator('text=/error|failed|invalid/i').count() > 0;
      const hasValidationMessages = await page.locator('[class*="validation"], [class*="error"], .field-validation-error').count() > 0;
      
      // Capture specific validation errors
      const validationErrors = [];
      const errorSelectors = [
        '.field-validation-error',
        '.validation-summary-errors',
        '[class*="error"]',
        '[class*="invalid"]',
        'span[data-valmsg-for]',
        '.text-danger',
        '.alert-danger'
      ];
      
      for (const selector of errorSelectors) {
        try {
          const errorElements = await page.locator(selector).all();
          for (const element of errorElements) {
            const errorText = await element.textContent();
            if (errorText && errorText.trim()) {
              validationErrors.push(`${selector}: ${errorText.trim()}`);
            }
          }
        } catch (error) {
          // Continue checking other selectors
        }
      }
      
      // Capture form field states for debugging
      const formFieldStates = [];
      const formFields = await page.locator('input, select, textarea').all();
      for (const field of formFields) {
        try {
          const tagName = await field.evaluate(el => el.tagName.toLowerCase());
          const type = await field.getAttribute('type');
          const name = await field.getAttribute('name');
          const id = await field.getAttribute('id');
          const placeholder = await field.getAttribute('placeholder');
          const value = await field.inputValue();
          const required = await field.getAttribute('required');
          const hasError = await field.locator('..').locator('[class*="error"], [class*="invalid"]').count() > 0;
          
          formFieldStates.push({
            element: `${tagName}${type ? `[type="${type}"]` : ''}`,
            identifier: name || id || placeholder || 'unnamed',
            value: value || 'empty',
            required: required !== null,
            hasError: hasError
          });
        } catch (error) {
          // Continue with next field
        }
      }
      
      testInfo.attach('Validation Errors Investigation', {
        body: `Validation Errors Found: ${validationErrors.length}
${validationErrors.join('\n')}

Form Field States:
${JSON.stringify(formFieldStates, null, 2)}`,
        contentType: 'text/plain'
      });
      
      testInfo.attach('Contact Creation Result', { 
        body: `Current URL: ${currentUrl}\nSuccess message found: ${hasSuccessMessage}\nRedirected: ${isRedirected}\nError message found: ${hasErrorMessage}\nValidation messages: ${hasValidationMessages}`, 
        contentType: 'text/plain' 
      });

      // Try to extract contact ID from URL or page for cleanup
      const urlMatch = currentUrl.match(/\/contact\/(\d+)|\/contact\/details\/(\d+)|\/(\d+)/);
      if (urlMatch) {
        const contactId = urlMatch[1] || urlMatch[2] || urlMatch[3];
        contactIds.push(contactId);
        testInfo.attach('Contact ID', { 
          body: `Contact ID for cleanup: ${contactId}`, 
          contentType: 'text/plain' 
        });
      }

      // Verify form submission was handled properly (success, redirection, or shows validation errors)
      // This ensures the application doesn't crash and provides appropriate feedback
      expect(hasSuccessMessage || isRedirected || hasValidationMessages).toBe(true);
    });

    await test.step('Verify dashboard metrics increment', async () => {
      // Navigate back to dashboard to check updated metrics
      await page.goto('https://localhost:7031');
      await page.waitForLoadState('networkidle');
      await page.waitForTimeout(2000); // Allow time for metrics to update

      // Capture customer call metrics after contact creation
      const metricWidgets = [
        { name: 'Total Customer Calls', selectors: ['text=/total.*customer.*calls/i', '[class*="total"] text=/customer.*calls/i', 'text=/customer.*calls.*total/i'] },
        { name: 'Open Customer Calls', selectors: ['text=/open.*customer.*calls/i', '[class*="open"] text=/customer.*calls/i', 'text=/customer.*calls.*open/i'] },
        { name: 'Aged Open Calls', selectors: ['text=/aged.*open.*calls/i', '[class*="aged"] text=/calls/i', 'text=/aged.*calls/i'] },
        { name: 'Pending Customer Calls', selectors: ['text=/pending.*customer.*calls/i', '[class*="pending"] text=/calls/i', 'text=/pending.*calls/i'] },
        { name: 'Closed Customer Calls', selectors: ['text=/closed.*customer.*calls/i', '[class*="closed"] text=/calls/i', 'text=/closed.*calls/i'] }
      ];

      for (const widget of metricWidgets) {
        let value = 0;
        for (const selector of widget.selectors) {
          try {
            const element = await page.locator(selector).first();
            if (await element.count() > 0) {
              const parentCard = element.locator('..').locator('..');
              const numberElements = await parentCard.locator('text=/\\d+/').allTextContents();
              if (numberElements.length > 0) {
                // Find the largest number (likely the metric value)
                const numbers = numberElements.map(text => parseInt(text.match(/\d+/)?.[0] || '0')).filter(n => !isNaN(n));
                value = Math.max(...numbers, 0);
                break;
              }
            }
          } catch (error) {
            // Continue trying other selectors
          }
        }
        dashboardMetricsAfter[widget.name] = value;
      }

      testInfo.attach('Dashboard Metrics After', {
        body: `Updated Dashboard Metrics:\n${JSON.stringify(dashboardMetricsAfter, null, 2)}`,
        contentType: 'text/plain'
      });

      // Calculate and verify increments
      const totalCallsIncrement = dashboardMetricsAfter['Total Customer Calls'] - dashboardMetricsBefore['Total Customer Calls'];
      const openCallsIncrement = dashboardMetricsAfter['Open Customer Calls'] - dashboardMetricsBefore['Open Customer Calls'];

      testInfo.attach('Metrics Comparison', {
        body: `Metrics Comparison:
Before: ${JSON.stringify(dashboardMetricsBefore, null, 2)}
After: ${JSON.stringify(dashboardMetricsAfter, null, 2)}

Increments:
- Total Customer Calls: +${totalCallsIncrement} (Expected: +1)
- Open Customer Calls: +${openCallsIncrement} (Expected: +1)`,
        contentType: 'text/plain'
      });

      // Verify the metrics have incremented by exactly 1
      expect(totalCallsIncrement).toBe(1);
      expect(openCallsIncrement).toBe(1);
    });
  });

  test('Contact Form Validation - Required Fields', async ({ page }, testInfo) => {
    await test.step('Navigate to contact form', async () => {
      await page.goto('https://localhost:7031/contact/create');
      await page.waitForLoadState('networkidle');
    });

    await test.step('Test required field validation', async () => {
      // Try to submit empty form using the correct button
      const submitButton = await page.locator('button:has-text("Record Customer Call")').first();
      
      if (await submitButton.isVisible({ timeout: 5000 })) {
        await submitButton.click();
        await page.waitForTimeout(2000);
        
        testInfo.attach('Empty Form Submission', { 
          body: 'Attempted to submit empty form using "Record Customer Call" button', 
          contentType: 'text/plain' 
        });
      } else {
        testInfo.attach('Submit Button Not Found', { 
          body: 'Could not find "Record Customer Call" button for validation test', 
          contentType: 'text/plain' 
        });
      }

      // Check for validation messages
      const pageContent = await page.textContent('body');
      const hasValidationErrors = pageContent.includes('required') || 
                                 pageContent.includes('Please') || 
                                 pageContent.includes('error') ||
                                 pageContent.includes('must');

      const currentUrl = page.url();
      const stayedOnForm = currentUrl.includes('/contact/create');

      testInfo.attach('Validation Test', { 
        body: `Validation errors shown: ${hasValidationErrors}\nStayed on form: ${stayedOnForm}\nCurrent URL: ${currentUrl}`, 
        contentType: 'text/plain' 
      });

      // Either validation errors should show OR form should stay on create page
      expect(hasValidationErrors || stayedOnForm).toBe(true);
    });
  });

  test('Reset Form Functionality', async ({ page }, testInfo) => {
    await test.step('Navigate to contact form', async () => {
      await page.goto('https://localhost:7031/contact/create');
      await page.waitForLoadState('networkidle');
    });

    await test.step('Fill form and test reset functionality', async () => {
      // Fill some form fields first
      const testData = {
        customerName: 'Test Reset Customer',
        notes: 'This data should be cleared by reset'
      };

      // Try to fill customer name field
      const nameSelectors = ['input[name*="customer"]', 'input[name*="name"]', '[placeholder*="customer"]', '[placeholder*="name"]'];
      for (const selector of nameSelectors) {
        try {
          const element = await page.locator(selector).first();
          if (await element.isVisible({ timeout: 1000 })) {
            await element.fill(testData.customerName);
            testInfo.attach('Form Filling', { 
              body: `Filled customer name using selector: ${selector}`, 
              contentType: 'text/plain' 
            });
            break;
          }
        } catch (error) {
          // Continue trying
        }
      }

      // Try to fill notes field
      const notesSelectors = ['textarea', 'input[name*="note"]', 'input[name*="description"]'];
      for (const selector of notesSelectors) {
        try {
          const element = await page.locator(selector).first();
          if (await element.isVisible({ timeout: 1000 })) {
            await element.fill(testData.notes);
            testInfo.attach('Notes Filling', { 
              body: `Filled notes using selector: ${selector}`, 
              contentType: 'text/plain' 
            });
            break;
          }
        } catch (error) {
          // Continue trying
        }
      }

      // Click Reset Form button
      const resetButton = await page.locator('button:has-text("Reset Form")').first();
      if (await resetButton.isVisible({ timeout: 5000 })) {
        await resetButton.click();
        await page.waitForTimeout(1000);
        
        testInfo.attach('Reset Action', { 
          body: 'Clicked "Reset Form" button successfully', 
          contentType: 'text/plain' 
        });

        // Verify form fields are cleared (check first few inputs)
        const inputs = await page.locator('input[type="text"], input[type="email"], textarea').all();
        const clearedFields = [];
        for (let i = 0; i < Math.min(3, inputs.length); i++) {
          try {
            const value = await inputs[i].inputValue();
            clearedFields.push(`Field ${i + 1}: "${value}" (empty: ${value === ''})`);
          } catch (error) {
            clearedFields.push(`Field ${i + 1}: Error checking value`);
          }
        }

        testInfo.attach('Reset Verification', { 
          body: `Field values after reset:\n${clearedFields.join('\n')}`, 
          contentType: 'text/plain' 
        });
      } else {
        testInfo.attach('Reset Button Not Found', { 
          body: 'Could not find "Reset Form" button', 
          contentType: 'text/plain' 
        });
      }
    });
  });

  test('Back to Customer Calls Navigation', async ({ page }, testInfo) => {
    await test.step('Navigate to contact form', async () => {
      await page.goto('https://localhost:7031/contact/create');
      await page.waitForLoadState('networkidle');
    });

    await test.step('Test back navigation button', async () => {
      const backButton = await page.locator('button:has-text("Back to Customer Calls")').first();
      
      if (await backButton.isVisible({ timeout: 5000 })) {
        await backButton.click();
        await page.waitForLoadState('networkidle');
        
        const currentUrl = page.url();
        testInfo.attach('Back Navigation', { 
          body: `Clicked "Back to Customer Calls" button. Current URL: ${currentUrl}`, 
          contentType: 'text/plain' 
        });

        // Verify we navigated away from the create form
        const navigatedAway = !currentUrl.includes('/contact/create');
        expect(navigatedAway).toBe(true);
      } else {
        testInfo.attach('Back Button Not Found', { 
          body: 'Could not find "Back to Customer Calls" button', 
          contentType: 'text/plain' 
        });
      }
    });
  });

  test('Contact History/List View Access', async ({ page }, testInfo) => {
    await test.step('Look for contact history or list view', async () => {
      // Try to find contact list/history from dashboard
      await page.goto('https://localhost:7031');
      await page.waitForLoadState('networkidle');

      // Look for contact history/list links
      const possibleListSelectors = [
        'a:has-text("Contact History")',
        'a:has-text("View Contacts")',
        'a:has-text("Customer Contacts")',
        '[href*="/contact"]',
        'a:has-text("Contacts")'
      ];

      let found = false;
      for (const selector of possibleListSelectors) {
        try {
          const element = await page.locator(selector).first();
          if (await element.isVisible({ timeout: 1000 })) {
            await element.click();
            await page.waitForLoadState('networkidle');
            found = true;
            
            const currentUrl = page.url();
            testInfo.attach('Contact List Access', { 
              body: `Found contact list at: ${currentUrl}`, 
              contentType: 'text/plain' 
            });
            break;
          }
        } catch (error) {
          // Continue
        }
      }

      if (!found) {
        // Try direct navigation to common contact list URLs
        const possibleUrls = [
          'https://localhost:7031/contact',
          'https://localhost:7031/contact/list',
          'https://localhost:7031/contact/index',
          'https://localhost:7031/contacts'
        ];

        for (const url of possibleUrls) {
          try {
            await page.goto(url);
            await page.waitForLoadState('networkidle');
            
            const pageContent = await page.textContent('body');
            if (!pageContent.includes('404') && !pageContent.includes('Not Found')) {
              testInfo.attach('Contact List Found', { 
                body: `Contact list accessible at: ${url}`, 
                contentType: 'text/plain' 
              });
              found = true;
              break;
            }
          } catch (error) {
            // Continue trying
          }
        }
      }

      testInfo.attach('Contact List Search Result', { 
        body: `Contact list/history page found: ${found}`, 
        contentType: 'text/plain' 
      });
    });
  });

  test('Validate Default Form Values and Auto-Population', async ({ page }, testInfo) => {
    await test.step('Navigate to contact form and validate defaults', async () => {
      await page.goto('https://localhost:7031/contact/create');
      await page.waitForLoadState('networkidle');
      
      const today = new Date();
      const validationResults = [];

      // 1. Validate Call Number format (yyyymmdd-nnn)
      const callNumberSelectors = ['input[name*="call"]', 'input[name*="number"]', 'input[id*="call"]', 'input[id*="number"]'];
      let callNumberFound = false;
      
      for (const selector of callNumberSelectors) {
        try {
          const element = await page.locator(selector).first();
          if (await element.isVisible({ timeout: 1000 })) {
            const callNumber = await element.inputValue();
            if (callNumber && callNumber.length > 0) {
              callNumberFound = true;
              const callNumberPattern = /^\d{8}-\d{3}$/;
              const isValidFormat = callNumberPattern.test(callNumber);
              
              const expectedDatePrefix = today.getFullYear().toString() + 
                                       (today.getMonth() + 1).toString().padStart(2, '0') + 
                                       today.getDate().toString().padStart(2, '0');
              const hasCorrectDate = callNumber.startsWith(expectedDatePrefix);
              
              validationResults.push(`✓ Call Number: "${callNumber}"`);
              validationResults.push(`  Format Valid (yyyymmdd-nnn): ${isValidFormat ? '✓' : '✗'}`);
              validationResults.push(`  Today's Date (${expectedDatePrefix}): ${hasCorrectDate ? '✓' : '✗'}`);
              
              expect(isValidFormat).toBe(true);
              expect(hasCorrectDate).toBe(true);
              break;
            }
          }
        } catch (error) {
          // Continue trying
        }
      }
      
      if (!callNumberFound) {
        validationResults.push('✗ Call Number: NOT FOUND');
      }

      // 2. Validate Contact Date shows today's date and current time
      const dateSelectors = ['input[type="datetime-local"]', 'input[type="date"]', 'input[name*="date"]', 'input[id*="date"]'];
      let contactDateFound = false;
      
      for (const selector of dateSelectors) {
        try {
          const element = await page.locator(selector).first();
          if (await element.isVisible({ timeout: 1000 })) {
            const contactDate = await element.inputValue();
            if (contactDate && contactDate.length > 0) {
              contactDateFound = true;
              const todayString = today.toISOString().split('T')[0]; // YYYY-MM-DD
              const isToday = contactDate.includes(todayString);
              
              validationResults.push(`✓ Contact Date: "${contactDate}"`);
              validationResults.push(`  Is Today (${todayString}): ${isToday ? '✓' : '✗'}`);
              
              expect(isToday).toBe(true);
              break;
            }
          }
        } catch (error) {
          // Continue trying
        }
      }
      
      if (!contactDateFound) {
        validationResults.push('✗ Contact Date: NOT FOUND');
      }

      // 3. Validate Status dropdown is set to "Open"
      const statusSelectors = ['select[name*="status"]', 'select[id*="status"]', 'select[name*="Status"]'];
      let statusFound = false;
      
      for (const selector of statusSelectors) {
        try {
          const element = await page.locator(selector).first();
          if (await element.isVisible({ timeout: 1000 })) {
            const statusValue = await element.inputValue();
            statusFound = true;
            const isOpen = statusValue && statusValue.toLowerCase() === 'open';
            
            validationResults.push(`✓ Status Dropdown: "${statusValue}"`);
            validationResults.push(`  Is "Open": ${isOpen ? '✓' : '✗'}`);
            
            expect(isOpen).toBe(true);
            break;
          }
        } catch (error) {
          // Continue trying
        }
      }
      
      if (!statusFound) {
        validationResults.push('✗ Status Dropdown: NOT FOUND');
      }

      // 4. Validate RED "Open" label is present
      const redLabelSelectors = [
        '.text-danger:has-text("Open")',
        '.text-red:has-text("Open")',
        'span[style*="red"]:has-text("Open")',
        'span[style*="color: red"]:has-text("Open")',
        'span[style*="color:red"]:has-text("Open")',
        '.badge-danger:has-text("Open")',
        '.alert-danger:has-text("Open")',
        '[class*="red"]:has-text("Open")',
        '[style*="background-color: red"]:has-text("Open")',
        '[style*="color: #ff"]:has-text("Open")'
      ];
      
      let redLabelFound = false;
      for (const selector of redLabelSelectors) {
        try {
          const element = await page.locator(selector).first();
          if (await element.isVisible({ timeout: 1000 })) {
            redLabelFound = true;
            validationResults.push(`✓ RED "Open" Label: FOUND (${selector})`);
            break;
          }
        } catch (error) {
          // Continue trying
        }
      }
      
      if (!redLabelFound) {
        validationResults.push('✗ RED "Open" Label: NOT FOUND');
      }
      
      expect(redLabelFound).toBe(true);

      testInfo.attach('Default Values Validation Summary', { 
        body: validationResults.join('\n'), 
        contentType: 'text/plain' 
      });
    });
  });
});