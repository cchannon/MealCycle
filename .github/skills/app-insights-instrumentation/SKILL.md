---
name: app-insights-instrumentation
description: "Application Insights telemetry patterns for both C# backend (.NET SDK) and React frontend (JavaScript SDK) on this stack. Use when adding telemetry, custom events, dependency tracking, structured logging with correlation IDs, or configuring App Insights for a new component. Triggers: App Insights, Application Insights, telemetry, custom events, correlation ID, distributed tracing, frontend tracking, ILogger, TelemetryClient."
---

# Application Insights Instrumentation

## When to Use
- Initialising App Insights in a new API, Function App, or frontend
- Adding custom event tracking for a significant user action or business process
- Propagating correlation IDs across service boundaries
- Configuring structured logging that flows to App Insights
- Setting up dependency tracking for HTTP clients or database calls

## Backend (.NET SDK)

### Setup in `Program.cs`
```csharp
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
});
// For Azure Functions: use AddApplicationInsightsTelemetryWorkerService()
```

### Correlation ID Middleware
Inject a shared `X-Correlation-Id` header on every request; propagate it on all outbound calls:
```csharp
app.Use(async (context, next) =>
{
    var correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault()
        ?? Activity.Current?.Id
        ?? Guid.NewGuid().ToString();
    context.Items["CorrelationId"] = correlationId;
    context.Response.Headers["X-Correlation-Id"] = correlationId;
    using (LogContext.PushProperty("CorrelationId", correlationId))
    {
        await next();
    }
});
```

### Custom Events via ILogger (preferred)
```csharp
_logger.LogInformation("Meal cycle updated. MealCycleId={MealCycleId} Status={Status}", mealCycleId, status);
```
Structured log entries flow to App Insights as traces with searchable properties.

### Custom Events via TelemetryClient (for rich business metrics)
```csharp
_telemetryClient.TrackEvent("MealCycleUpdated", new Dictionary<string, string>
{
    ["MealCycleId"] = mealCycleId.ToString(),
    ["Status"] = status
});
```

### HttpClient Dependency Tracking
Register `HttpClient` via `AddHttpClient()` — the .NET SDK instruments outbound calls automatically.

## Frontend (JavaScript SDK)

### Setup in `src/lib/appInsights.ts`
```ts
import { ApplicationInsights } from '@microsoft/applicationinsights-web';
import { ReactPlugin } from '@microsoft/applicationinsights-react-js';

export const reactPlugin = new ReactPlugin();
export const appInsights = new ApplicationInsights({
  config: {
    connectionString: import.meta.env.VITE_APPINSIGHTS_CONNECTION_STRING,
    extensions: [reactPlugin],
    enableAutoRouteTracking: true,
  },
});
appInsights.loadAppInsights();
```

### Custom Event Tracking
```ts
import { appInsights } from '@/lib/appInsights';

appInsights.trackEvent({ name: 'MealCycleFormSubmitted' }, { source: 'operations-dashboard' });
```

### Correlation ID Propagation
Pass the `X-Correlation-Id` from backend responses as a custom property on frontend events so traces can be joined in App Insights:
```ts
appInsights.trackEvent({ name: 'MealCycleSaved' }, { correlationId });
```

## References
- [Backend Program.cs App Insights setup](./references/backend-setup.md)
- [Frontend SDK initialisation and router integration](./references/frontend-setup.md)
- [Correlation ID middleware](./references/correlation-middleware.md)
