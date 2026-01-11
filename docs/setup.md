# Setup Guide

## Prerequisites
- .NET 10 SDK (preview)
- SQL Server (local or container)
- Azure subscription with Microsoft Entra ID

## Entra ID App Registrations
### API App Registration
1. Create an app registration for the API.
2. Expose an API scope (e.g., `api://<api-client-id>/config.read`).
3. Set **Supported account types** to **Accounts in any organizational directory (Multi-tenant)**.
4. Under **Authentication** enable **Access tokens**.

### Desktop App Registration (Public Client)
1. Create a second app registration for the WinUI desktop app.
2. Set **Supported account types** to **Accounts in any organizational directory (Multi-tenant)**.
3. Add a **Public client** redirect URI (e.g., `msal<client-id>://auth`).
4. Add API permissions for the API scope and grant admin consent.

### Graph App Permissions (Groups)
1. In the API registration, add **Microsoft Graph** application permissions:
   - `Group.Read.All`
   - `User.Read.All`
2. Grant admin consent for the tenant(s).

## Configuration
### API
Set these values in `src/Tinterra.Api.Test/appsettings.json` or environment variables:
- `AzureAd:ClientId` (API app ID)
- `AzureAd:Audience` (api://<api-client-id>)
- `ConnectionStrings:SqlServer` or `TINTERRA_SQL_CONNECTION`
- `BootstrapAdminObjectId` **or** `BootstrapAdminGroupId` for the first bootstrap admin

Bootstrap behavior:
- On first start (when security tables are empty), the API creates a `Security.Admin` bundle and maps the bootstrap object/group ID.
- After that, all management should happen through the admin UI or the API endpoints.

### Desktop
Set the MSAL settings in the desktop app (see `Tinterra.Desktop.Test/Services/MsalAuthService.cs`) for:
- `clientId`
- `redirectUri`
- `scopes`

## Database Migrations
From repo root:
```bash
# Create/update database
DOTNET_ENVIRONMENT=Development \
  dotnet ef database update \
  --project src/Tinterra.Infrastructure.Persistence.DesignTime \
  --startup-project src/Tinterra.Api.Test
```

## Seed Data
Run the idempotent SQL seed script:
```bash
sqlcmd -S localhost -d Tinterra.Test -i scripts/sql/seed.sql
```

## Run the API
```bash
dotnet run --project src/Tinterra.Api.Test
```

## Run the Desktop App
```bash
dotnet run --project src/Tinterra.Desktop.Test
```

## Troubleshooting
- **401 Unauthorized**: verify the access token audience matches `AzureAd:Audience` and the API scope is granted.
- **403 Forbidden**: ensure the tenant is in `AllowedTenants` and `IsEnabled = 1`.
- **Groups overage**: if `_claim_names` is present, make sure Graph permissions are granted and admin consent is completed.
- **Issuer issues**: use `organizations` and v2.0 endpoints in the API registration.
