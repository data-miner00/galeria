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
- [x] 404 page
- [x] Swap between masonry to squared layout
- [ ] Album view
- [ ] Timeline view
- [ ] Select mode

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
- [ ] Add Log aggregator like loki to monitor API calls
- [ ] Optional Authentication
- [x] Optional TOTP MFA
- [ ] Craft image from text and emoji icons and backgrounds
- [ ] Optional watermarks for downloaded image
- [ ] Add image from URL (not working for most becoz CORS block, need implement backend)
- [ ] Bulk select for actions
- [ ] AI Prompts
- [x] AI Auto tagging
- [ ] Auditing
- [ ] Video
- [ ] Live image

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

- [x] Docker image for frontend/backend
- [x] Docker compose for dev environment & prod environment

~~Should be simple and quick. Aim to get MVP by 22 March 2026.~~
