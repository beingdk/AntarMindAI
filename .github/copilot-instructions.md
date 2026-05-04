# Copilot Instructions for AntarMindAI

Trust these instructions. Only search the codebase if information here is incomplete or appears incorrect.

## What This Repo Is

AI-powered web platform. ASP.NET Core 10 REST API backend with a React 19 + TypeScript SPA frontend. The API serves the compiled React app as static files; authentication is handled via Azure App Service EasyAuth in production.

**Runtimes required:** .NET 10 SDK, Node.js 24.x / npm 11.x

## Repository Layout

```
AntarMindAI.slnx              # Solution file
frontend/                      # React 19 + TypeScript + Vite 7
  src/
    App.tsx                    # Root component
    components/                # Shared UI components
    pages/                     # Page-level components
  package.json                 # npm scripts: dev, build, lint, start, preview
  vite.config.ts               # Vite config — builds to ../src/AntarMindAI.Api/wwwroot/ui
  eslint.config.js             # ESLint flat config (typescript-eslint + react-hooks)
  tsconfig.app.json            # strict mode, noUnusedLocals, noUnusedParameters
src/
  AntarMindAI.Api/             # ASP.NET Core 10 Web API
    Program.cs                 # App entry point; wires auth, DI, static files, SPA fallback
    Controllers/               # Feature-based controllers (ApiController, route "api/...")
    Services/                  # ICurrentUserService + implementations
    Auth/                      # EasyAuthHandler (custom authentication scheme)
    Repositories/              # Data access (Azure Table Storage or in-memory)
    Models/                    # Domain models
    wwwroot/ui/                # Auto-generated — compiled React output (do not edit)
    appsettings.json           # Default config; AzureTableStorage.ConnectionString
    Properties/launchSettings.json  # http: 5100, https: 7100+5100; env=Development
  AntarMindAI.Tests/           # xUnit integration tests
    TestApiFactory.cs          # WebApplicationFactory<Program> running "Testing" env
    Health/HealthControllerTests.cs
.github/
  instructions/                # Copilot instruction files (auto-applied by applyTo globs)
  agents/                      # Agent definition files
  prompts/                     # Prompt templates
```

## Build & Validate — Exact Commands

Always run from the repo root unless noted.

### Backend (validated — ~10 seconds)
```bash
dotnet build src/AntarMindAI.Api/AntarMindAI.Api.csproj
```
**Expected:** `Build succeeded. 2 Warning(s) 0 Error(s)`. The NU1900 warnings about a Paycor NuGet feed being unreachable are expected and do not prevent building — ignore them.

### Backend Tests (validated — ~5 seconds)
```bash
dotnet test src/AntarMindAI.Tests/AntarMindAI.Tests.csproj
```
**Expected:** `Passed! - Failed: 0, Passed: 1`. Tests use `WebApplicationFactory` in "Testing" environment — no external services needed.

### Frontend — always run `npm install` first (from `frontend/`)
```bash
cd frontend
npm install
npm run build    # tsc -b && vite build — outputs to src/AntarMindAI.Api/wwwroot/ui
npm run lint     # eslint on all .ts/.tsx files
```
**`npm run build` is required** before running the API if frontend changes were made. The Vite build compiles TypeScript strictly (`strict`, `noUnusedLocals`, `noUnusedParameters`) — TypeScript errors will fail the build.

### Run Locally (two terminals)
```
Terminal 1:  dotnet run --project src/AntarMindAI.Api/AntarMindAI.Api.csproj --launch-profile https
Terminal 2:  cd frontend && npm install && npm run dev
```
Or use the combined script: `cd frontend && npm install && npm run start`  
The Vite dev server proxies `/api/*` to `https://localhost:7100`.

## Architecture & Key Conventions

### Authentication
- **Scheme:** Custom `"EasyAuth"` handler (`Auth/EasyAuthHandler.cs`) reads user identity from `ICurrentUserService`.
- **Development/Testing env:** `LocalDevCurrentUserService` returns a fixed hardcoded user — no Azure headers needed.
- **Production:** `CurrentUserService` reads `X-MS-CLIENT-PRINCIPAL-ID` and `X-MS-CLIENT-PRINCIPAL-NAME` headers set by Azure App Service.
- **All controllers must have `[Authorize]`** by default. Only add `[AllowAnonymous]` when explicitly required (e.g., `/api/health`).

### Adding New Endpoints
- Place controllers in `src/AntarMindAI.Api/Controllers/` with `[ApiController]` and `[Route("api/...")]`.
- Use feature-based subfolders (not all-controllers-in-one-folder).
- Route pattern: `/api/{resource}/{id?}/{subresource?}`
- Use `DateTimeOffset` (never `DateTime`) for all timestamps.
- Use `await` throughout — never `.Result` or `.Wait()`.
- Return the standard error response structure for all 4xx responses (see `api-standards.instructions.md`).

### Adding New Frontend Components
- Components go in `frontend/src/components/` (shared) or `frontend/src/pages/` (page-level).
- Keep components under ~200 lines; single responsibility.
- MUI (`@mui/material`) is the component library. TanStack Query (`@tanstack/react-query`) handles server state. React Router v7 handles routing.
- TypeScript strict mode is enforced — no unused locals or parameters; all type errors are build failures.

### AI Attribution (MANDATORY)
Add an attribution comment to every function, method, class, or file you create or modify:
```typescript
// Generated by AI on MM/DD/YYYY        ← new code
// Modified by AI on MM/DD/YYYY. Edit #N.  ← modified code (increment N per edit)
```
This applies to ALL file types: `.cs`, `.ts`, `.tsx`, `.json`, `.html`, `.css`, `.sql`, etc.

### Environment Variables
| Variable | Purpose |
|---|---|
| `AZURE_TABLE_STORAGE_CONNECTION_STRING` | Azure Table Storage. Empty string = in-memory storage. |

### Known Warnings (Not Errors)
- `NU1900` — Paycor internal NuGet feed unreachable. Always present; always safe to ignore.
- `npm fund` output — informational only; not an error.
