---
name: react-component-patterns
description: "Patterns and templates for React TypeScript component development on this stack. Use when scaffolding page components, shared UI components, custom hooks, React Query API hooks, Zustand stores, or React Hook Form + Zod forms. Triggers: React component scaffold, page component, custom hook, React Query hook, Zustand store, form component, feature module."
---

# React Component Patterns

## When to Use
- Scaffolding a new page or feature module
- Creating a shared UI component in `src/components/`
- Building a data-fetching hook in `src/api/`
- Creating a Zustand store in `src/stores/`
- Building a form with React Hook Form + Zod

## Procedure

### New Page
1. Create `src/pages/<PageName>.tsx` — page component, handles layout and composition
2. Create `src/api/use<Resource>.ts` — React Query hook(s) for data needed by the page
3. Add route in the router config
4. Add an error boundary wrapper at the route level if the page has complex data dependencies

### New Feature Module
1. Create `src/features/<feature-name>/` with:
   - `components/` — feature-scoped components
   - `hooks/` — feature-scoped custom hooks
   - `types.ts` — feature-scoped TypeScript types
   - `index.ts` — barrel export
2. Shared components that emerge across features move to `src/components/`

### Shared Component
1. Create `src/components/<ComponentName>/index.tsx` (or flat if simple: `<ComponentName>.tsx`)
2. Define `<ComponentName>Props` interface in the same file
3. Export as named export: `export function ComponentName(...)`
4. Add a basic Vitest smoke test alongside the component

## Templates

See references for annotated code templates:
- [Page component template](./references/page-template.md)
- [React Query hook template](./references/query-hook-template.md)
- [Zustand store template](./references/store-template.md)
- [Form component template](./references/form-template.md)
