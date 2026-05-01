---
description: "Use when writing or reviewing C# backend code, ASP.NET Core controllers, services, repositories, domain entities, Application layer features, or Azure Functions. Covers async, DI, validation, logging, and error handling patterns."
applyTo: "backend/**/*.cs"
---

## Stack

- **Language**: C# (.NET 8+)
- **API**: ASP.NET Core Web API
- **Serverless**: Azure Functions (isolated worker model)
- **Data Access**: Azure.Data.Tables (Table Storage)
- **Database**: Azure Storage Account (Table Storage)
- **Validation**: FluentValidation
- **Mapping**: Mapster (or AutoMapper where already established)
- **Testing**: xUnit + Moq + FluentAssertions

## Project Structure

```
backend/
├── src/
│   ├── Api/                  # ASP.NET Core Web API project
│   │   ├── Controllers/
│   │   ├── Middleware/
│   │   └── Program.cs
│   ├── Application/          # Business logic layer
│   │   ├── Features/         # Vertical slices (Commands + Queries + Handlers)
│   │   ├── Interfaces/       # Repository and service contracts
│   │   └── DTOs/
│   ├── Domain/               # Domain entities, value objects, domain events
│   ├── Infrastructure/       # Repositories and external service clients
│   │   ├── Data/
│   │   │   ├── Tables/
│   │   │   └── TableClientFactory.cs
│   │   ├── Services/         # Azure service clients (Blob, Service Bus, Key Vault)
│   │   └── Repositories/
│   └── Functions/            # Azure Functions project (isolated worker)
│       ├── Triggers/
│       └── Program.cs
└── tests/
    ├── Unit/
    └── Integration/
```

## C# / ASP.NET Core Standards

### Async
- All I/O operations must be async — `await` every `Task`, pass `CancellationToken` from the outermost caller down
- Never `.Result`, `.Wait()`, or `.GetAwaiter().GetResult()` — these cause deadlocks in ASP.NET contexts
- Method signatures: `async Task<T>` not `async void` (except event handlers)

### Dependency Injection
- Constructor injection only — no service locator, no `IServiceProvider` injected into application services
- Register lifetimes deliberately: `Singleton` for stateless clients (`BlobServiceClient`, `ServiceBusClient`, `TableServiceClient`), `Scoped` for repositories, `Transient` for lightweight stateless services

### Controllers
- Thin — no business logic; controllers call a handler/service and return the result
- Always return typed `IActionResult` or `Results<>` with correct HTTP status codes
- Use `[ApiController]` + route attributes; no conventional routing in API projects

### Validation
- FluentValidation — register validators via `AddFluentValidationAutoValidation()`
- Return 400 ProblemDetails automatically on validation failure; no manual `if (!ModelState.IsValid)` checks
- Validators live alongside their request DTOs in `Application/Features/<Feature>/`

### Error Handling
- Global exception middleware returns RFC 7807 `ProblemDetails` — do not catch-and-swallow in controllers
- Domain/application exceptions: use typed exception classes (`NotFoundException`, `ConflictException`) that the middleware maps to status codes
- Always log before rethrowing; never log after — avoid double-logging

### Logging
- `ILogger<T>` injected via constructor — never `LogManager.GetLogger()` or static loggers
- Structured logging with named placeholders: `_logger.LogInformation("Order {OrderId} processed", orderId)` not string interpolation
- Log levels: `Debug` for trace-level dev info, `Information` for business events, `Warning` for handled degradations, `Error` for exceptions

### Nullability
- Nullable reference types are enabled — all null paths must be handled explicitly
- Prefer `ArgumentNullException.ThrowIfNull()` at public boundaries
- Do not use `!` (null-forgiving) without an inline comment explaining why it is safe

### Records vs Classes
- DTOs, command/query objects, value objects: `record` with positional or init-only properties
- Domain entities, services, repositories: `class`
- API response models: `record` for immutability

### Azure Key Vault / Secrets
- Never hardcode secrets — all configuration resolves from Key Vault via `DefaultAzureCredential`
- Use the Options pattern: `services.Configure<StorageOptions>(config.GetSection("Storage"))` not raw `IConfiguration` in services

### Global Usings
- Declare global usings in `GlobalUsings.cs` at the root of each project — minimise per-file `using` clutter
- Only add global usings for namespaces used in the majority of files in that project

## Application Insights

- `TelemetryClient` and `ILogger<T>` both route to App Insights in production via the .NET SDK
- Inject a correlation ID into every incoming request via middleware and propagate it on all outbound calls and structured log entries
- Azure SDK and `HttpClient` dependency tracking is enabled automatically via the SDK — no manual instrumentation needed
- Never swallow exceptions — always log with full context before handling or rethrowing
