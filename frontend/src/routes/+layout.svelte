<script lang="ts">
	import AppSidebar from '$lib/components/app-sidebar.svelte';
	import * as Breadcrumb from '$lib/components/ui/breadcrumb/index.js';
	import { Separator } from '$lib/components/ui/separator/index.js';
	import * as Sidebar from '$lib/components/ui/sidebar/index.js';
	import { ArrowLeft, PlusIcon, SearchIcon } from '@lucide/svelte';

	import './layout.css';
	import favicon from '$lib/assets/favicon.svg';
	import { Button } from '$lib/components/ui/button';
	import UploadImageDialog from '$lib/components/custom/upload-image-dialog.svelte';
	import { Toaster } from '$lib/components/ui/sonner/index.js';
	import CreateBoardDialog from '$lib/components/custom/create-board-dialog.svelte';
	import * as InputGroup from '$lib/components/ui/input-group/index.js';
	import { onMount } from 'svelte';
	import { appState } from '$lib/states.svelte';
	import ToTopButton from '$lib/components/custom/to-top-button.svelte';
	import ImageInfoSheet from '$lib/components/custom/image-info-sheet.svelte';
	import { ModeWatcher } from 'mode-watcher';
	import ThemeButton from '$lib/components/custom/theme-button.svelte';
	import CommandPalette from '$lib/components/custom/command-palette.svelte';
	import BoardInfoSheet from '$lib/components/custom/board-info-sheet.svelte';
	import type { Board } from '$lib/types';
	import { goto } from '$app/navigation';
	import OtpDialog from '$lib/components/custom/otp-dialog.svelte';
	import { PUBLIC_API_BASE_URL } from '$env/static/public';
	import { fetchAll as fetchImages } from '$lib/api/images';

	let { children } = $props();

	let isUploadImageDialogOpen = $state(false);
	let isCreateBoardDialogOpen = $state(false);
	let isImageInfoSheetOpen = $state(false);
	let isBoardInfoSheetOpen = $state(false);
	let isOtpDialogOpen = $state(false);

	$effect(() => {
		if (appState.infoSheetData.isOpen) isImageInfoSheetOpen = true;
	});

	$effect(() => {
		if (!isImageInfoSheetOpen) {
			appState.infoSheetData.isOpen = false;
		}
	});

	$effect(() => {
		if (appState.boardInfoSheetData.isOpen) isBoardInfoSheetOpen = true;
	});

	$effect(() => {
		if (!isBoardInfoSheetOpen) {
			appState.boardInfoSheetData.isOpen = false;
		}
	});

	onMount(async () => {
		try {
			appState.images = await fetchImages();
		} catch (e) {
			console.error('Failed to fetch images', e);
		}

		const res = await fetch(`${PUBLIC_API_BASE_URL}/api/v1/board`);
		if (res.ok) {
			const boards = (await res.json()) as Board[];

			appState.boards = boards.sort((a, b) =>
				a.isPinned === b.isPinned ? 0 : a.isPinned ? -1 : 1
			);
		}

		const resSettings = await fetch(`${PUBLIC_API_BASE_URL}/api/v1/UserSettings`);
		if (resSettings.ok) {
			appState.settings = await resSettings.json();
		}

		const resProfile = await fetch(`${PUBLIC_API_BASE_URL}/api/v1/UserProfile`);
		if (resProfile.ok) {
			appState.profile = await resProfile.json();
		}

		const securitySettingsRes = await fetch(`${PUBLIC_API_BASE_URL}/api/v1/auth`);
		if (securitySettingsRes.ok) {
			const securitySettings = await securitySettingsRes.json();
			isOtpDialogOpen = securitySettings.isTotpEnabled;
		}

		appState.isLoading = false;
	});

	let isCommandPaletteOpen = $state(false);

	function handleKeydown(e: KeyboardEvent) {
		if ((e.key === 'j' && (e.metaKey || e.ctrlKey)) || e.key === '/') {
			e.preventDefault();
			isCommandPaletteOpen = !isCommandPaletteOpen;
		} else if (e.key === 'k' && (e.metaKey || e.ctrlKey)) {
			e.preventDefault();
			searchInputRef?.focus();
		}
	}

	let searchQuery = $state('');
	let searchInputRef: HTMLInputElement | null = $state(null);

	function handleSearch(e: KeyboardEvent) {
		if (e.key === 'Enter') {
			goto('/search?q=' + encodeURIComponent(searchQuery));
			searchQuery = '';
		}
	}
</script>

<svelte:head>
	<link rel="icon" href={favicon} />
	<title>Galeria</title>
</svelte:head>

<svelte:document onkeydown={handleKeydown} />

<Toaster position="bottom-right" duration={5000} />

<ModeWatcher />

<Sidebar.Provider>
	<AppSidebar
		onCreateClick={() => (isUploadImageDialogOpen = !isUploadImageDialogOpen)}
		onCreateBoardClick={() => (isCreateBoardDialogOpen = !isCreateBoardDialogOpen)}
	/>
	<Sidebar.Inset class="relative overflow-clip">
		<header
			class="sticky top-0 right-0 left-0 z-20 flex h-16 shrink-0 items-center gap-2 bg-background"
		>
			<div class="flex w-full items-center justify-between px-4">
				<div class="flex items-center gap-2">
					<Sidebar.Trigger class="-ms-1" />
					<Separator orientation="vertical" class="data-[orientation=vertical]:h-4" />
					<Button variant="ghost" size="icon" onclick={() => history.back()}>
						<ArrowLeft />
					</Button>
					<Separator orientation="vertical" class="me-2 data-[orientation=vertical]:h-4" />
					<Breadcrumb.Root>
						<Breadcrumb.List>
							<!-- <Breadcrumb.Item class="hidden md:block">
								<Breadcrumb.Link href="##">At The Homepage</Breadcrumb.Link>
							</Breadcrumb.Item>
							<Breadcrumb.Separator class="hidden md:block" /> -->
							<Breadcrumb.Item>
								<Breadcrumb.Page>{appState.headerTitle}</Breadcrumb.Page>
							</Breadcrumb.Item>
						</Breadcrumb.List>
					</Breadcrumb.Root>
				</div>
				<div class="flex items-center gap-2">
					<InputGroup.Root>
						<InputGroup.Input
							bind:ref={searchInputRef}
							placeholder="Search..."
							bind:value={searchQuery}
							onkeyup={handleSearch}
						/>
						<InputGroup.Addon>
							<SearchIcon />
						</InputGroup.Addon>
					</InputGroup.Root>

					<ThemeButton />

					<Button onclick={() => (isUploadImageDialogOpen = !isUploadImageDialogOpen)}>
						<PlusIcon /> Create
					</Button>
				</div>
			</div>
		</header>
		<div class="flex flex-1 flex-col gap-4 p-4 pt-0" id="layout-container">
			{@render children()}
		</div>
	</Sidebar.Inset>
</Sidebar.Provider>

<UploadImageDialog bind:isDialogOpen={isUploadImageDialogOpen} />
<CreateBoardDialog bind:isDialogOpen={isCreateBoardDialogOpen} />
<ImageInfoSheet bind:isOpen={isImageInfoSheetOpen} />
<BoardInfoSheet bind:isOpen={isBoardInfoSheetOpen} />
<OtpDialog bind:isDialogOpen={isOtpDialogOpen} />

<ToTopButton />

<CommandPalette bind:isOpen={isCommandPaletteOpen} />
