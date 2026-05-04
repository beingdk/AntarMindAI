# AntarMindAI

AI-powered platform built with ASP.NET Core 10 and React 19.

## Structure

```
AntarMindAI.sln
frontend/                        # React 19 + TypeScript + Vite
src/
  AntarMindAI.Api/               # ASP.NET Core 10 Web API
    Auth/                        # EasyAuth handler
    Controllers/                 # API controllers (feature-based)
    Models/                      # Domain models
    Repositories/                # Data access (in-memory & Azure Table Storage)
    Services/                    # Business logic & user identity
    wwwroot/ui/                  # Compiled React output (auto-generated)
  AntarMindAI.Tests/             # xUnit integration tests
```

## Getting Started

### Prerequisites
- .NET 10 SDK
- Node.js 24.x / npm 11.x

### Run locally

```bash
# Terminal 1 — API
dotnet run --project src/AntarMindAI.Api/AntarMindAI.Api.csproj --launch-profile https

# Terminal 2 — Frontend dev server
cd frontend
npm install
npm run dev
```

Or use the combined start script:

```bash
cd frontend
npm install
npm run start
```

### Build

```bash
# Backend
dotnet build src/AntarMindAI.Api/AntarMindAI.Api.csproj

# Frontend
cd frontend
npm run build
```

### Test

```bash
dotnet test src/AntarMindAI.Tests/AntarMindAI.Tests.csproj
```

## API Endpoints

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/api/health` | Anonymous | Liveness check |

## Environment Variables

| Variable | Description |
|---|---|
| `AZURE_TABLE_STORAGE_CONNECTION_STRING` | Azure Table Storage connection string (empty = in-memory) |
