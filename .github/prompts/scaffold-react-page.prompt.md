---
description: "Scaffold a complete React page component including the page file, React Query data hook, route registration, and loading/error states."
argument-hint: "Describe the page — its purpose, what data it displays, and any user interactions"
agent: agent
tools: [read, edit, search]
---

You are a senior React/TypeScript frontend developer. Scaffold a complete, production-quality page following this team's patterns.

## Steps
1. Read `frontend/src/pages/` and `frontend/src/api/` to understand existing patterns and naming
2. Check the router config (likely `src/main.tsx` or `src/routes.tsx`) to understand how routes are registered
3. Generate all files listed below

## Files to Generate

### 1. React Query API Hook
`frontend/src/api/use<Resource>.ts`

- Typed query key constant
- `useQuery` or `useMutation` with appropriate options
- Typed response using Zod schema validated at the boundary
- Example:
```ts
const QUERY_KEYS = {
  list: (filters: OrderFilters) => ['orders', filters] as const,
  detail: (id: string) => ['orders', id] as const,
};

export function useOrders(filters: OrderFilters) {
  return useQuery({
    queryKey: QUERY_KEYS.list(filters),
    queryFn: () => fetchOrders(filters),
  });
}
```

### 2. Page Component
`frontend/src/pages/<PageName>.tsx`

- Handles layout and composition only — no data fetching logic inline
- Explicit loading state (skeleton or spinner)
- Explicit error state (user-friendly error message, retry option)
- Explicit empty state where applicable
- Uses sub-components from `src/features/<feature>/components/` for complex sections

### 3. Route Registration
Show the route entry to add to the router config, including:
- Path
- Element
- Any lazy loading wrapper if the page is heavy
- Auth guard if the route requires authentication

### 4. Page-level Error Boundary (if needed)
If the page has complex data dependencies, wrap it in an `ErrorBoundary`.

---

Page description:
