---
description: "Use when writing Vitest unit tests, React Testing Library tests, or Playwright E2E test specs and page objects for the frontend."
applyTo: "frontend/**/*.test.ts, frontend/**/*.test.tsx, frontend/**/*.spec.ts, frontend/tests/**"
---

## Frontend Testing Standards

### Vitest + React Testing Library

#### Naming
- Test files co-located with the component: `Button.test.tsx` alongside `Button.tsx`
- Test description mirrors user intent: `'shows error message when email is invalid'` not `'validation works'`
- Use `describe` blocks to group by component or behaviour; `it` for individual cases

#### What to Test
- Component behaviour from the user's perspective — what they see, not implementation details
- Query by accessible roles and labels: `getByRole('button', { name: /submit/i })`, `getByLabelText('Email')`
- Do NOT query by class names or test IDs unless there is no accessible alternative
- Test state transitions: initial render → interaction → expected DOM change

#### Mocking
- Mock React Query hooks with `vi.mock('@tanstack/react-query')` or via `QueryClient` with a test wrapper
- Mock `fetch` with `msw` (Mock Service Worker) for API calls — do not mock `fetch` directly
- Mock Zustand stores by re-exporting a test version or resetting state in `beforeEach`

#### Async
- Use `await user.click()` (via `@testing-library/user-event`) for interactions — not `fireEvent`
- `waitFor()` for async DOM updates; `findBy*` queries for elements that appear asynchronously

---

### Playwright E2E

#### Structure
- All E2E tests in `frontend/tests/e2e/`
- Page Object Model: each page/major feature has a class in `frontend/tests/e2e/pages/`
- Page object encapsulates all locators and actions; tests only call page object methods and assert

#### Page Object Pattern
```ts
export class MealCyclePage {
  constructor(private readonly page: Page) {}

  async fillName(name: string) {
    await this.page.getByLabel('Meal cycle name').fill(name);
  }

  async submit() {
    await this.page.getByRole('button', { name: /save/i }).click();
  }
}
```

#### Test Design
- Cover critical user flows only — login, signup, purchase, key CRUD operations
- Do not duplicate what unit tests cover (form validation, error states)
- Tests are independent and idempotent — each test cleans up after itself or uses isolated test data
- Use Playwright fixtures (`test.extend`) for shared setup (auth state, API context)

#### Auth
- Persist authenticated browser state via `storageState` — log in once per test suite run, not per test
- Use `test.use({ storageState: 'authState.json' })` for authenticated test groups

#### Selectors
- Prefer `getByRole`, `getByLabel`, `getByText` in that order
- Use `data-testid` as a last resort, with the convention `data-testid="<feature>-<element>"`

#### CI
- Tests run headless in CI; timeouts set to accommodate slower CI runners (default 30s)
- Capture screenshots and traces on failure — configured in `playwright.config.ts`
