# Galeria

Self-hosted gallery web app.

## Tech Stacks

Frontend:

- SvelteKit
- Shadcn/svelte
- Tailwind

Backend:

- ASP.NET Core API
- Azure Cosmos Db Emulator - Database
- Azurite (Storage Account Emulator) - Image store
- Meilisearch - Search indexing
- Gemini - Image auto-tagging

## Getting Started

### Development

1. Spin up the infra
   ```
   docker-compose up -d
   ```
2. Run the provisioning tool. This will setup containers and indexes for the services.
   ```
   cd backend
   dotnet run Provisioning/Provisioning.csproj
   ```
3. Run the API with CLI or Visual Studio.
   ```
   dotnet run WebApi/WebApi.csproj
   ```
4. Run the frontend
   ```
   cd ../frontend
   npm install
   npm run dev
   ```
5. Visit app at https://localhost:5173

### Production (Self-host)

1. Copy and update the env
1. Spin up the infra
   ```
   docker-compose -f docker-compose-prod.yml up -d
   ```
1. Run the provisioning tool. This is why we still need to expose the ports in prod.
   ```
   cd backend
   dotnet run Provisioning/Provisioning.csproj
   ```
1. Visit app at https://localhost:3000

### Build Image Individually

1. For API
   ```
   docker build -t galeria-api .
   ```
2. For frontend
   ```
   docker build -t galeria-web --build-arg PUBLIC_API_BASE_URL=http://localhost:3000 .
   ```

## References

- [Pinterest](https://www.pinterest.com/)
- [Midjourney](https://www.midjourney.com/explore?tab=video_top)
- [Mobbin Gallery](https://mobbin.com/glossary/gallery)
- [Masonry Layout](https://piccalil.li/blog/a-simple-masonry-like-composable-layout/)
- [Behance](https://www.behance.net/#)
- [Tailwind Aspect Ratio](https://tailwindcss.com/docs/aspect-ratio)
- [Shadcn/Svelte](https://shadcn-svelte.com/)
- [Svelte](https://svelte.dev/)
- [Meilisearch](https://www.meilisearch.com/)
