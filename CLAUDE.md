# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

Galeria — a self-hosted gallery web app. SvelteKit frontend + ASP.NET Core backend, backed by
Cosmos DB (documents), Azure Blob Storage / Azurite (images), Meilisearch (search indexing), and
Gemini (image auto-tagging).

## Commands

### Local infra (Cosmos DB emulator, Azurite, Meilisearch)

```
docker-compose up -d              # or: just dev
docker-compose -f docker-compose-prod.yml up -d   # prod compose, or: just prod
```

### Backend (`backend/`, solution: `Gallery.slnx`)

```
cd backend
dotnet run --project Provisioning/Provisioning.csproj   # sets up containers/indexes — run once after infra is up (also required in prod, since ports stay exposed for it)
dotnet run --project WebApi/WebApi.csproj                # API, https://localhost:5173-adjacent (see below)
dotnet build backend/Gallery.slnx                         # build everything
```

There are no test projects in the solution currently.

### Frontend (`frontend/`)

```
cd frontend
npm install
npm run dev              # https://localhost:5173
npm run build
npm run check             # svelte-kit sync + svelte-check (type checking)
npm run lint              # prettier --check . && eslint .
npm run format             # prettier --write .
```

### Docker images (built individually)

```
docker build -t galeria-api .
docker build -t galeria-web --build-arg PUBLIC_API_BASE_URL=http://localhost:3000 .
```

Note: the backend `Dockerfile` lives at `backend/Dockerfile` and its build `context` must be
`./backend` (not `./backend/WebApi`) — it was hoisted out of `WebApi/` to support building the
multi-project solution (`Core`, `WebApi`, `WebJobs`) in one image.

## Architecture

### Backend: three .NET projects sharing one `Core`

- **`Core`** — shared library: `Models` (Cosmos documents like `ImageDocument`, `BoardDocument`,
  `UserProfileDocument`, plus their domain-facing counterparts), `Repositories` (one repo per
  document type, all built via `RepositoryFactory` from a single `CosmosClient`), `Clients`
  (`IGenAiClient`/`GeminiClient` for Gemini auto-tagging, `IImageClient`/`BlobStorageImageClient`
  for blob storage), `Services` (`ImageService`).
- **`WebApi`** — the HTTP API. `Program.cs` wires everything up itself via a chain of
  `WebApplicationBuilder` extension methods (`ConfigureRepositories`, `ConfigureClients`,
  `ConfigureCors`, `ConfigureMeilisearch`, `ConfigureGeminiClient`) instead of a DI container
  module system — read `Program.cs` top to bottom to see what's registered as a singleton and
  where its config section comes from. Controllers live under `Controllers/V1/` (Auth, Board,
  Image, UserProfile, UserSettings). In production, requests to the Cosmos emulator's fixed
  `127.0.0.1:8081` address are rewritten to `cosmos-emulator:8081` by a custom
  `CosmosRedirectHandler` — the emulator/SDK combo hardcodes that host:port pair.
- **`WebJobs`** — background job runner (Quartz-based scheduling via `JobScheduler`/`JobFactory`,
  jobs under `Jobs/`, e.g. `SoftDeleteCleanupJob`). Configured independently from `WebApi`.
- **`Provisioning`** — a one-shot CLI (targets both `net8.0` and `net10.0`) that creates the
  Cosmos containers, blob containers, and Meilisearch indexes the other projects expect to
  already exist. Must be run after bringing up infra, in both dev and prod.
- Package versions are centrally managed in `backend/Directory.Packages.props`
  (`ManagePackageVersionsCentrally`) — add new package versions there, not in individual
  `.csproj` files.

### Frontend: SvelteKit (Svelte 5) with adapter-node

- `src/lib/api/*.ts` — thin fetch wrappers per backend resource (`images.ts`, `boards.ts`,
  `drafts.ts`), all reading `PUBLIC_API_BASE_URL` from `$env/static/public` and hitting
  `/api/v1/<resource>`. Follow this pattern (base URL constant + one exported async function per
  endpoint) when adding new API calls.
- `src/lib/states.svelte.ts` — shared reactive app state (Svelte 5 runes) referenced across
  routes/components (e.g. `appState.settings.watermark`).
- `src/lib/components/ui/` — shadcn-svelte generated primitives; `src/lib/components/custom/` and
  other subfolders hold app-specific components.
- `src/routes/` — file-based routing; most feature areas (`boards`, `drafts`, `favorites`,
  `hidden`, `recycle`, `search`, `settings/*`, `timeline`, `carousel`) are flat `+page.svelte`
  routes under their own folder.
- `src/lib/i18n/` — translations setup (`translations.svelte.ts`, `languages.ts`).
- Styling: Tailwind v4 (`@tailwindcss/vite`) + shadcn-svelte (`components.json`). Import order is
  enforced by `@trivago/prettier-plugin-sort-imports` — run `npm run format` rather than
  hand-ordering imports.

### Cross-cutting

- CORS, Meilisearch host/key, Gemini API key, Cosmos/Storage connection strings are all read from
  `WebApi` config sections (`Cors`, `Meilisearch`, `Gemini`, `BlobStorage`, connection strings
  `CosmosDb`/`StorageAccount`) — see `appsettings.json` / `appsettings.Development.json` and the
  matching `Options` classes in `Core/Options` and `WebApi/Options`.
- Root `.env` / `.env.example` supplies `MEILI_MASTER_KEY` and `GEMINI_API_KEY` to
  docker-compose.
