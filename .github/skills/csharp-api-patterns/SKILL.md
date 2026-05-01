---
name: csharp-api-patterns
description: "Patterns and templates for C# ASP.NET Core Web API development on this stack. Use when scaffolding controllers, application services, repository implementations, FluentValidation validators, middleware, or dependency injection registrations. Triggers: scaffold API, create controller, service pattern, repository pattern, DI registration, middleware, validator, C# patterns."
---

# C# API Patterns

## When to Use
- Scaffolding a new API feature (controller + service + repository)
- Implementing a new command/query handler in `Application/Features/`
- Writing a FluentValidation validator
- Registering services in `Program.cs`
- Creating middleware (exception handling, correlation ID, auth)

## Procedure
1. Determine the feature slice: `Application/Features/<Feature>/` contains Commands, Queries, Handlers, Validators, and DTOs
2. Create the domain entity in `Domain/` if new
3. Add the Table Storage entity mapping in `Infrastructure/Data/Tables/`
4. Scaffold the repository interface in `Application/Interfaces/` and implementation in `Infrastructure/Repositories/` using `TableClient`
5. Scaffold the application service or CQRS handlers
6. Create the controller in `Api/Controllers/`
7. Register all new services in `Program.cs`

## Templates

See references for detailed annotated code templates:
- [Controller template](./references/controller-template.md)
- [Application service and handler template](./references/service-handler-template.md)
- [Repository template](./references/repository-template.md)
- [Validator template](./references/validator-template.md)
- [Program.cs registration patterns](./references/registration-patterns.md)
