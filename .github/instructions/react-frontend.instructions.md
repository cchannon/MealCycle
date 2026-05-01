---
description: "Use when writing or reviewing React components, custom hooks, TypeScript types, Zustand stores, React Query hooks, or Tailwind styling in the frontend. Covers component structure, data fetching, state, forms, and TypeScript strict mode."
applyTo: "frontend/**/*.tsx, frontend/src/**/*.ts"
---

## Stack

- **Framework**: Vite + React 18 + TypeScript (strict mode)
- **Styling**: Tailwind CSS
- **State**: Zustand for global state; React Query (TanStack Query) for server state
- **Routing**: React Router v6
- **Forms**: React Hook Form + Zod for validation
- **Testing**: Vitest + React Testing Library for unit/integration; Playwright for E2E
- **Auth**: Microsoft Entra ID (OIDC/OAuth 2.0)

## Project Structure

```
frontend/
├── src/
│   ├── api/           # React Query hooks wrapping backend API calls
│   ├── components/    # Shared, reusable UI components
│   ├── features/      # Feature-scoped modules (components + hooks + types)
│   ├── hooks/         # Shared custom hooks
│   ├── lib/           # Third-party client setup (queryClient, msal/appInsights)
│   ├── pages/         # Route-level page components
│   ├── stores/        # Zustand stores
│   ├── types/         # Shared TypeScript types and interfaces
│   └── utils/         # Pure utility functions
├── tests/
│   └── e2e/           # Playwright tests (page objects + specs)
└── vite.config.ts
```

## React / TypeScript / Vite Standards

### TypeScript
- Strict mode is always on — zero tolerance for `any`; use `unknown` and narrow explicitly
- Prefer `interface` for object shapes; `type` for unions, intersections, and utility types
- Explicitly type function parameters and return types where inference isn't obvious
- No `@ts-ignore` without an explanatory comment

### Component Structure
- Functional components only — no class components
- One component per file; filename and export match in PascalCase
- Define props interface in the same file: `interface ButtonProps { ... }`
- Avoid `React.FC` — use bare function syntax: `function Button({ label }: ButtonProps) { ... }`
- Extract into a custom hook when a component exceeds ~150 lines or has multi-step state

### Custom Hooks
- Prefix with `use` — `useMealCycleSummary`, `useMealCycleFilters`
- Return an object (not a tuple) unless the hook is a simple value/setter pair
- Hooks that call the backend live in `src/api/` and use React Query

### Data Fetching
- All API calls go through typed React Query hooks in `src/api/` — never raw `fetch` in components
- Define query keys as constants in the same file as the hook
- Use `useMutation` for writes; invalidate related queries on success
- Handle `isLoading`, `isError`, and empty states explicitly — no silent failures

### State Management
- Server state: React Query (don't duplicate in Zustand)
- Global client state: Zustand stores in `src/stores/`
- Local UI state: `useState` / `useReducer` — keep it in the component
- No Redux; no MobX

### Forms
- React Hook Form + Zod schema validation — no manual `onChange` state for forms
- Define Zod schemas in a separate `schema.ts` alongside the form component
- `resolver: zodResolver(schema)` wired into `useForm`

### Styling
- Tailwind utility classes only — no component-scoped CSS files unless overriding a third-party library
- Compose conditional classes with `cn()` (clsx + tailwind-merge): `cn('base-class', condition && 'extra-class')`
- No inline `style={{}}` except for truly dynamic values unavailable in Tailwind (e.g., CSS custom properties)

### Imports
- Path aliases via `@/` mapping to `src/` — use `@/components/Button` not `../../components/Button`
- Group imports: external libraries → internal modules → relative imports, separated by blank lines

### Error Boundaries
- Wrap page-level routes in an `ErrorBoundary` — don't let a page crash the whole app
- Use React Query's `onError` callback or `useErrorBoundary` option for query failures

## Application Insights

- Frontend JS SDK (`@microsoft/applicationinsights-web`) initialised in `src/lib/appInsights.ts`
- Track page views automatically via React Router integration
- Track custom events for significant user actions (form submissions, entity updates, errors)
- Pass the shared correlation ID from backend responses as a custom property on all tracked events
