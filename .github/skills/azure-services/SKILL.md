---
name: azure-services
description: "Configuration and integration patterns for Azure Blob Storage, Azure Service Bus, Azure Key Vault, Azure Functions (isolated worker), and Logic Apps on this stack. Use when wiring up an Azure service in C# or configuring infrastructure. Triggers: Azure Blob Storage, Azure Service Bus, Key Vault, Azure Functions, Logic App, azure service integration, managed identity wiring, service bus queue, blob container."
---

# Azure Services Integration

## When to Use
- Registering Azure service clients in `Program.cs`
- Implementing a Blob Storage upload/download operation
- Publishing a message to a Service Bus queue
- Writing an Azure Function trigger handler (ServiceBusTrigger, HttpTrigger, TimerTrigger)
- Configuring Key Vault references in app settings
- Setting up managed identity role assignments

## Service Registration (Program.cs)

All Azure SDK clients are registered as singletons using `DefaultAzureCredential`:

```csharp
// Blob Storage
builder.Services.AddSingleton(sp =>
    new BlobServiceClient(
        new Uri($"https://{builder.Configuration["Storage:AccountName"]}.blob.core.windows.net"),
        new DefaultAzureCredential()));

// Service Bus
builder.Services.AddSingleton(sp =>
    new ServiceBusClient(
        $"{builder.Configuration["ServiceBus:Namespace"]}.servicebus.windows.net",
        new DefaultAzureCredential()));

// Key Vault (secret client — for runtime secret resolution)
builder.Services.AddSingleton(sp =>
    new SecretClient(
        new Uri($"https://{builder.Configuration["KeyVault:Name"]}.vault.azure.net"),
        new DefaultAzureCredential()));
```

## Key Patterns

### Azure Blob Storage
- Container names: `kebab-case` — `user-uploads`, `processed-exports`
- Never return a `BlobClient` directly from a service — return typed results (URLs, streams, metadata)
- Generate short-lived SAS tokens for client-side download links when needed

### Azure Service Bus
- Inject `ServiceBusClient` as singleton; create `ServiceBusSender` / `ServiceBusProcessor` per use
- Dispose senders and processors via `IAsyncDisposable` (register as hosted service or scoped resource)
- Always process messages idempotently — Service Bus guarantees at-least-once delivery
- Set `MaxDeliveryCount` and dead-letter queue strategy before going to production

### Azure Functions (Isolated Worker)
- Register the same DI, Options, and logging setup as the API in `Functions/Program.cs`
- `ServiceBusTrigger` for queue processing; `TimerTrigger` for scheduled jobs
- Functions reference Key Vault secrets via `@Microsoft.KeyVault(...)` in the Function App settings — same as API

### Key Vault
- Never read secrets at startup into memory — use Key Vault references in app settings for managed rotation
- If runtime secret reading is required, inject `SecretClient` and call `.GetSecretAsync()` lazily

## References
- [Service Bus consumer Function template](./references/servicebus-function-template.md)
- [Blob Storage service template](./references/blob-service-template.md)
- [Bicep resource templates](./references/bicep-templates.md)
