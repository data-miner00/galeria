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
	import { page } from '$app/state';
	import { onMount } from 'svelte';
	import { appState } from '$lib/states.svelte';
	import ToTopButton from '$lib/components/custom/to-top-button.svelte';

	let { children } = $props();

	let isUploadImageDialogOpen = $state(false);
	let isCreateBoardDialogOpen = $state(false);

	onMount(async () => {
		const res = await fetch('https://localhost:7146/api/v1/board');
		if (res.ok) {
			appState.boards = await res.json();
		}

		const resSettings = await fetch('https://localhost:7146/api/v1/UserSettings');
		if (resSettings.ok) {
			appState.settings = await resSettings.json();
		}

		const resProfile = await fetch('https://localhost:7146/api/v1/UserProfile');
		if (resProfile.ok) {
			appState.profile = await resProfile.json();
		}
	});
</script>

<svelte:head>
	<link rel="icon" href={favicon} />
	<title>Galeria</title>
</svelte:head>

<Toaster position="bottom-right" duration={5000} />

<Sidebar.Provider>
	<AppSidebar
		onCreateClick={() => (isUploadImageDialogOpen = !isUploadImageDialogOpen)}
		onCreateBoardClick={() => (isCreateBoardDialogOpen = !isCreateBoardDialogOpen)}
	/>
	<Sidebar.Inset class="relative overflow-clip">
		<header class="sticky top-0 right-0 left-0 flex h-16 shrink-0 items-center gap-2 bg-white">
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
					<Button variant="ghost" size="icon">
						<SearchIcon />
					</Button>

					<Button onclick={() => (isUploadImageDialogOpen = !isUploadImageDialogOpen)}>
						<PlusIcon /> Create
					</Button>
				</div>
			</div>
		</header>
		<div class="flex flex-1 flex-col gap-4 p-4 pt-0">
			{@render children()}
		</div>
	</Sidebar.Inset>
</Sidebar.Provider>

<UploadImageDialog bind:isDialogOpen={isUploadImageDialogOpen} />
<CreateBoardDialog bind:isDialogOpen={isCreateBoardDialogOpen} />

<ToTopButton />
