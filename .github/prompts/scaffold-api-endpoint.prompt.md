---
description: "Scaffold a complete C# API endpoint: controller action, application service or handler, repository method, request/response DTOs, and FluentValidation validator."
argument-hint: "Describe the resource and operation, e.g. 'POST /orders — create a new order for the authenticated user'"
agent: agent
tools: [read, edit, search]
---

You are a senior C# backend developer. Scaffold a complete, production-quality API endpoint following this team's patterns.

## Steps
1. Read the existing project structure in `backend/src/` to understand naming, namespace conventions, and any existing similar endpoints
2. Identify the feature slice name (e.g., `Orders`, `Payments`) and confirm the folder structure in `Application/Features/`
3. Generate all files listed below
4. Do NOT create the EF Core migration — note that a migration is needed and what the schema change is

## Files to Generate

### 1. Request & Response DTOs
`Application/Features/<Feature>/<Operation><Feature>Request.cs`
`Application/Features/<Feature>/<Operation><Feature>Response.cs`

### 2. FluentValidation Validator
`Application/Features/<Feature>/<Operation><Feature>Validator.cs`

Rule: validate all required fields, string lengths, formats (email, URL, etc.), and business invariants.

### 3. Application Service or Command Handler
`Application/Features/<Feature>/<Operation><Feature>Handler.cs` (or update existing service)

- Async throughout with `CancellationToken`
- Uses repository interface — no direct `DbContext`
- Returns result (typed response or throws domain exception)

### 4. Repository Method (if new data access required)
Add method to `Application/Interfaces/I<Entity>Repository.cs`
Add implementation to `Infrastructure/Repositories/<Entity>Repository.cs`

### 5. Controller Action
`Api/Controllers/<Feature>Controller.cs`

- Thin action method: validate (auto via filter), call handler, return typed result
- Correct HTTP verb and route
- `[Authorize]` if the endpoint requires authentication
- XML doc comment on the action for Swagger

### 6. Program.cs Registration
Note any new services or repositories that need to be registered and where.

---

Endpoint description:
