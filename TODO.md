# Specification

A self-hosted authenticationless image board used for managing personal images.

## Core

- [x] CRUD image
- [x] CRUD boards
- [x] Add/Remove image from boards
- [x] Display in Masonry layout
- [x] Lightbox
- [x] Tags/Category
- [x] Favorites
- [x] Recycle Bin
- [x] Download
- [x] Fuzzy search (filename, desc, metadata, tags)

## Presentation

- [x] Sorting
  - [x] Based on created date time
- [x] Skeleton loading
- [x] Dark/light mode
- [x] Internationalization (en,ja,ko,es,pt,fr,zh,ms)
- [x] Censor/blur option
- [x] Filter base on tags / category
- [x] Mobile responsive
- [ ] 404 page
- [x] Swap between masonry to squared layout

## Extra

- [ ] Batch upload from folder
- [ ] Batch download and metadata
- [ ] Password-protected board (need to rethink this)
- [ ] Image forking to create another modified version of it
- [ ] Image manipulation
  - [ ] Filters
  - [ ] Drawings
  - [ ] Crop
  - [ ] Invert
  - [ ] Flip
  - [ ] Resize + download
  - [ ] Compress + download
- [ ] Identify similar (vector database)
- [ ] Identify duplicates (vector database)
- [ ] Usage analytics (to think what to analyse)
- [ ] Add Log aggregator like logseq or loki to monitor API calls
- [ ] Optional Authentication
- [ ] Optional TOTP MFA
- [ ] Craft image from text and emoji icons and backgrounds
- [ ] Optional watermarks for downloaded image
- [x] Add image from URL
- [ ] Bulk select for actions

## Performance

- [ ] Lazy loading
- [ ] Deconstruct image when going out of view?
- [ ] Refactorings (TBD)
- [ ] Benchmark memory usage
- [ ] Fix issues based on benchmark
- [ ] Client-side caching for frequent images

## Enhancement

- [ ] Move in-memory upload of different size image to a scheduled job instead.
- [ ] Make search indexing in the background. NotIndexed, Indexed, NeedsReindex

## Deployment/Build

- [ ] Docker image for frontend/backend
- [ ] Docker compose for dev environment & prod environment

~~Should be simple and quick. Aim to get MVP by 22 March 2026.~~

## Image Store

1. File storage repository.
2. Azure Storage Account (Emulator)
3. Azure Cosmos DB

## References

- [Pinterest](https://www.pinterest.com/)
- [Midjourney](https://www.midjourney.com/explore?tab=video_top)
- [Mobbin Gallery](https://mobbin.com/glossary/gallery)
- [Masonry Layout](https://piccalil.li/blog/a-simple-masonry-like-composable-layout/)
- [Behance](https://www.behance.net/#)
- [Tailwind Aspect Ratio](https://tailwindcss.com/docs/aspect-ratio)
- [Shadcn/Svelte](https://shadcn-svelte.com/)
- [Svelte](https://svelte.dev/)
