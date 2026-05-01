---
name: nextauth-setup
description: "Project auth standard for this stack. NextAuth is not used; use Microsoft Entra ID (OIDC/OAuth 2.0) for frontend sign-in and backend JWT validation. Triggers: Entra ID, OAuth, OIDC, JWT validation, bearer token, auth setup, login."
---

# Entra Authentication Setup (Project Standard)

## When to Use
- Configuring authentication for this internal app
- Implementing protected routes on the frontend
- Setting up JWT validation on the C# backend API
- Debugging sign-in, token, or authorization issues

## Architecture

```
User                  Frontend (MSAL)               Backend API (C#)
  |-- sign in -------> |                               |
  |                    |-- OAuth/OIDC flow -> Entra    |
  |                    | <-- access token ------------  |
  |-- API call ------> |-- Authorization: Bearer <jwt> |
  |                    |                               |-- validate issuer/audience/signature
  |                    |                               |-- authorize request (roles/scopes)
```

## Frontend Configuration

```ts
// src/lib/authConfig.ts
export const authConfig = {
  authority: `https://login.microsoftonline.com/${import.meta.env.VITE_ENTRA_TENANT_ID}`,
  clientId: import.meta.env.VITE_ENTRA_CLIENT_ID,
  redirectUri: import.meta.env.VITE_ENTRA_REDIRECT_URI,
  scopes: ['api://<backend-app-id>/access_as_user'],
};
```

### Protected Routes
```ts
// Wrap protected pages with auth check
import { useIsAuthenticated } from '@azure/msal-react';

export function RequireAuth({ children }: { children: React.ReactNode }) {
  const isAuthenticated = useIsAuthenticated();
  if (!isAuthenticated) {
    return null;
  }
  return <>{children}</>;
}
```

### Sending the Access Token to the Backend
```ts
const accessToken = await acquireToken();
const res = await fetch('/api/backend-endpoint', {
  headers: { Authorization: `Bearer ${accessToken}` },
});
```

## Backend JWT Validation (C#)

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Auth:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Auth:Audience"],
            ValidateIssuerSigningKey = true
        };
    });
```

- All protected API controllers use `[Authorize]`; extract the user object ID claim as needed
- Authorization policies should validate required scopes/roles for internal app operations

## Security Notes
- Client IDs, tenant IDs, and app URIs are environment-configured and stored securely
- Token validation settings (`Auth:Issuer`, `Auth:Audience`) come from Key Vault-backed config
- NextAuth, Google, and Meta providers are out of scope for this project
