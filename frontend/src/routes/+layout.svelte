<script lang="ts">
	import { ModeWatcher } from 'mode-watcher';
	import { onMount } from 'svelte';

	import { PUBLIC_API_BASE_URL } from '$env/static/public';
	import { fetchAll as fetchImages } from '$lib/api/images';
	import favicon from '$lib/assets/favicon.svg';
	import AppSidebar from '$lib/components/app-sidebar.svelte';
	import BoardInfoSheet from '$lib/components/custom/board-info-sheet.svelte';
	import CommandPalette from '$lib/components/custom/command-palette.svelte';
	import CreateBoardDialog from '$lib/components/custom/create-board-dialog.svelte';
	import ImageInfoSheet from '$lib/components/custom/image-info-sheet.svelte';
	import OtpDialog from '$lib/components/custom/otp-dialog.svelte';
	import SiteHeader from '$lib/components/custom/site-header.svelte';
	import ToTopButton from '$lib/components/custom/to-top-button.svelte';
	import UploadImageDialog from '$lib/components/custom/upload-image-dialog.svelte';
	import * as Sidebar from '$lib/components/ui/sidebar/index.js';
	import { Toaster } from '$lib/components/ui/sonner/index.js';
	import { appState } from '$lib/states.svelte';
	import type { Board } from '$lib/types';

	import './layout.css';

	let { children } = $props();

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

	function handleKeydown(e: KeyboardEvent) {
		if ((e.key === 'j' && (e.metaKey || e.ctrlKey)) || e.key === '/') {
			e.preventDefault();
			appState.openState.isCommandPaletteOpen = !appState.openState.isCommandPaletteOpen;
		} else if (e.key === 'k' && (e.metaKey || e.ctrlKey)) {
			e.preventDefault();
			searchInputRef?.focus();
		}
	}

	let searchInputRef: HTMLInputElement | null = $state(null);
</script>

<svelte:head>
	<link rel="icon" href={favicon} />
	<title>Galeria</title>
</svelte:head>

<svelte:document onkeydown={handleKeydown} />

<Toaster position="bottom-right" duration={5000} />

<ModeWatcher />

<Sidebar.Provider>
	<AppSidebar />
	<Sidebar.Inset class="relative overflow-clip">
		<SiteHeader bind:searchInputRef />
		<div class="flex flex-1 flex-col gap-4 p-4 pt-0" id="layout-container">
			{@render children()}
		</div>
	</Sidebar.Inset>
</Sidebar.Provider>

<UploadImageDialog bind:isDialogOpen={appState.openState.isUploadImageDialogOpen} />
<CreateBoardDialog bind:isDialogOpen={appState.openState.isCreateBoardDialogOpen} />
<ImageInfoSheet bind:isOpen={isImageInfoSheetOpen} />
<BoardInfoSheet bind:isOpen={isBoardInfoSheetOpen} />
<OtpDialog bind:isDialogOpen={isOtpDialogOpen} />

<ToTopButton />

<CommandPalette bind:isOpen={appState.openState.isCommandPaletteOpen} />
