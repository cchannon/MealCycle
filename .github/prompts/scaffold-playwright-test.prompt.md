---
description: "Scaffold a Playwright E2E test for a user flow: a Page Object class and a complete test spec."
argument-hint: "Describe the user flow to test, e.g. 'user signs up, verifies email, and completes onboarding'"
agent: agent
tools: [read, edit, search]
---

You are a senior QA Engineer specialising in Playwright E2E test automation.

Scaffold a complete Playwright test for the described user flow, following the Page Object Model.

## Steps
1. Read `frontend/tests/e2e/pages/` to understand existing page objects and naming conventions
2. Read `frontend/tests/e2e/fixtures/` to understand what fixtures are available (auth state, custom setup)
3. Read `frontend/playwright.config.ts` for project configuration
4. Generate the page object class(es) and test spec

## Files to Generate

### 1. Page Object Class(es)
`frontend/tests/e2e/pages/<PageName>Page.ts`

- Constructor accepts `Page` from Playwright
- All locators defined as private `Locator` properties in the constructor
- Public methods for each meaningful user action (`fillForm`, `submit`, `waitForConfirmation`)
- No assertions in page objects — they belong in test specs
- Selector priority: `getByRole` > `getByLabel` > `getByText` > `getByTestId`

### 2. Test Spec
`frontend/tests/e2e/specs/<flow-name>.spec.ts`

- Use custom fixtures from `frontend/tests/e2e/fixtures/index.ts`
- Use `storageState` for authenticated tests — do not log in inside each test
- Each `test()` is independent and idempotent
- Cover: happy path, key error case (e.g., invalid input, server error)
- Descriptive test names: `'completes checkout with a valid card'` not `'checkout test'`
- `expect` assertions on visible outcomes — headings, toasts, URLs, not internal state

### 3. Auth Note
If the flow requires authentication, confirm that `authState.json` is produced by `fixtures/auth.setup.ts` and referenced in `playwright.config.ts`.

---

User flow to test:
