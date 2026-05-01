---
name: playwright-test-authoring
description: "Playwright E2E test authoring patterns for this stack. Use when writing Playwright tests, creating page object classes, setting up fixtures, configuring auth state, or debugging E2E test failures. Triggers: Playwright test, E2E test, page object, end-to-end, UI test, browser test, test fixture, playwright fixture, storageState, Playwright config."
---

# Playwright Test Authoring

## When to Use
- Writing a new E2E test for a user flow
- Creating a page object class for a new page or feature
- Setting up a Playwright fixture (auth state, API context, custom setup)
- Debugging a failing Playwright test
- Reviewing Playwright configuration

## Structure

```
frontend/tests/e2e/
├── pages/                    # Page Object Model classes
│   ├── LoginPage.ts
│   ├── MealCyclePage.ts
│   └── ...
├── fixtures/
│   ├── auth.setup.ts         # Global auth state setup (runs once)
│   └── index.ts              # Custom test fixtures extending base test
├── specs/
│   ├── auth.spec.ts
│   ├── meal-cycle.spec.ts
│   └── ...
└── playwright.config.ts
```

## Page Object Pattern

```ts
import { type Page, type Locator } from '@playwright/test';

export class MealCyclePage {
  private readonly nameInput: Locator;
  private readonly submitButton: Locator;

  constructor(private readonly page: Page) {
    this.nameInput = page.getByLabel('Meal cycle name');
    this.submitButton = page.getByRole('button', { name: /save/i });
  }

  async goto() {
    await this.page.goto('/meal-cycles/new');
  }

  async fillForm(name: string) {
    await this.nameInput.fill(name);
  }

  async submit() {
    await this.submitButton.click();
  }

  async waitForConfirmation() {
    await this.page.getByText(/meal cycle saved/i).waitFor();
  }
}
```

## Auth State Setup

Run authentication once per suite, not per test:

```ts
// fixtures/auth.setup.ts
import { test as setup } from '@playwright/test';

setup('authenticate', async ({ page }) => {
  await page.goto('/login');
  await page.getByLabel('Email').fill(process.env.TEST_USER_EMAIL!);
  await page.getByLabel('Password').fill(process.env.TEST_USER_PASSWORD!);
  await page.getByRole('button', { name: /sign in/i }).click();
  await page.waitForURL('/dashboard');
  await page.context().storageState({ path: 'authState.json' });
});
```

Tests reference it via `playwright.config.ts`:
```ts
use: { storageState: 'authState.json' }
```

## Selector Priority
1. `getByRole('button', { name: /submit/i })` — semantic, resilient
2. `getByLabel('Email')` — tied to accessible label
3. `getByText('Confirm order')` — visible text
4. `getByTestId('meal-cycle-save')` — last resort; use `data-testid` conventions

## CI Configuration

```ts
// playwright.config.ts excerpt
export default defineConfig({
  testDir: './tests/e2e/specs',
  timeout: 30_000,
  retries: process.env.CI ? 2 : 0,
  reporter: process.env.CI ? 'github' : 'html',
  use: {
    baseURL: process.env.PLAYWRIGHT_BASE_URL ?? 'http://localhost:5173',
    trace: 'on-first-retry',
    screenshot: 'only-on-failure',
  },
});
```

## References
- [Playwright config template](./references/playwright-config.md)
- [Custom fixtures template](./references/fixtures-template.md)
- [API context for test data seeding](./references/api-context-seeding.md)
