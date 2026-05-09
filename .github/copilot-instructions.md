# MoneySaver.SPA agent instructions

## What this project does

MoneySaver.SPA is a **Blazor WebAssembly** frontend for a personal finance app. It lets authenticated users:

- manage transactions
- manage spending categories
- view the current budget in use and browse other budgets
- inspect spending reports with pie and line charts
- register, log in, and log out with JWT-based authentication

This repository is the **SPA only**. Business data comes from external backend APIs configured in `wwwroot\appsettings*.json`.

## High-level architecture

- **Framework:** .NET 9 Blazor WebAssembly
- **UI library:** Radzen.Blazor
- **Client storage:** Blazored.LocalStorage
- **Charts:** custom JS modules under `wwwroot\js`
- **HTTP access:** service layer in `Services\`
- **Authentication:** JWT token stored in local storage under `authToken`

## Runtime flow

- `Program.cs` registers the SPA services and configures `SpaSettings`.
- `App.razor` wraps routing in `CascadingAuthenticationState` and redirects unauthenticated users to `/login` unless they are already on `/login` or `/register`.
- `AuthenticationService` talks to the identity API at `SpaSettings.AuthenticationAddress`.
- `ApiCallsService` talks to the data API at `SpaSettings.DataApiAddress`.
- `CustomeAuthorization` adds the bearer token to outgoing API requests.
- `AuthStateProvider` rebuilds the authentication state from the JWT in local storage.

## Main user-facing areas

- `/` - dashboard/home page showing the active budget, recent transactions, and category spend summaries
- `/transactions` - paged transaction list with search and CRUD dialogs
- `/budget` - current budget in use
- `/budgets` - paged list of budgets and budget details
- `/report` - spending visualizations by category and period
- `/configuration` - category management
- `/login`, `/register`, `/logout` - authentication flow

## Important folders

- `Pages\` - routed pages, usually split into `.razor` and `.razor.cs`
- `Components\` - reusable dialogs, charts, and budget UI pieces
- `Services\` - API-facing application services
- `Models\` - DTOs, view models, filters, request/response models
- `AuthProviders\` - auth state management
- `Features\` - utility logic such as JWT parsing
- `Shared\` - layouts and shared navigation
- `wwwroot\` - static assets, app settings, JS chart helpers, CSS

## Working conventions for agents

1. Treat this repository as a **thin client over backend APIs**. Prefer changing page logic, components, or services here rather than inventing backend behavior.
2. Reuse the existing service layer (`AuthenticationService`, `TransactionService`, `CategoryService`, `BudgetService`, `ReportsDataService`, `ApiCallsService`) instead of making ad hoc `HttpClient` calls from pages.
3. Preserve the current category presentation pattern: child categories are often displayed through `AlternativeName` in the format `Parent, Child`.
4. Keep auth behavior consistent with the existing flow: token in local storage, route protection in `App.razor`, and API auth via the custom delegating handler.
5. When adding UI behavior, follow the existing partial-class pattern used by pages and components.
6. Do not edit `bin\`, `obj\`, or bundled vendor assets unless the task explicitly requires it.

## Configuration notes

- Development API endpoints are defined in `wwwroot\appsettings.Development.json`.
- Default settings live in `wwwroot\appsettings.json`.
- Production endpoints are defined in `wwwroot\appsettings.Production.json`.

## Useful command

Build the SPA with:

```powershell
dotnet build .\MoneySaver.SPA.csproj -nologo
```
