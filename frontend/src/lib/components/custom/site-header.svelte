<script lang="ts">
	import { ArrowLeft, PlusIcon, SearchIcon, XIcon } from '@lucide/svelte';

	import { goto } from '$app/navigation';
	import NotificationsButton from '$lib/components/custom/notifications-button.svelte';
	import ThemeButton from '$lib/components/custom/theme-button.svelte';
	import * as Breadcrumb from '$lib/components/ui/breadcrumb/index.js';
	import { Button } from '$lib/components/ui/button';
	import * as InputGroup from '$lib/components/ui/input-group/index.js';
	import { Separator } from '$lib/components/ui/separator/index.js';
	import * as Sidebar from '$lib/components/ui/sidebar/index.js';
	import { appState, toggleUploadImageDialog } from '$lib/states.svelte';

	let searchQuery = $state('');
	function handleSearch(e: KeyboardEvent) {
		if (e.key === 'Enter') {
			goto('/search?q=' + encodeURIComponent(searchQuery));
			searchQuery = '';
			isMobileSearchOpen = false;
		}
	}

	// Below the md breakpoint the search input moves into its own row, opened by an icon button.
	let isMobileSearchOpen = $state(false);
	let mobileSearchInputRef: HTMLInputElement | null = $state(null);

	$effect(() => {
		if (isMobileSearchOpen) mobileSearchInputRef?.focus();
	});

	type Props = {
		searchInputRef: HTMLInputElement | null;
	};

	let { searchInputRef = $bindable(null) }: Props = $props();
</script>

<header class="sticky top-0 right-0 left-0 z-20 flex shrink-0 flex-col bg-background">
	<div class="flex h-16 w-full items-center justify-between gap-2 px-4">
		<div class="flex min-w-0 items-center gap-2">
			<Sidebar.Trigger class="-ms-1" />
			<Separator orientation="vertical" class="data-[orientation=vertical]:h-4" />
			<Button variant="ghost" size="icon" class="shrink-0" onclick={() => history.back()}>
				<ArrowLeft />
			</Button>
			<Separator
				orientation="vertical"
				class="me-2 hidden data-[orientation=vertical]:h-4 sm:block"
			/>
			<Breadcrumb.Root class="min-w-0">
				<Breadcrumb.List class="flex-nowrap">
					<Breadcrumb.Item class="min-w-0">
						<Breadcrumb.Page class="truncate">{appState.headerTitle}</Breadcrumb.Page>
					</Breadcrumb.Item>
				</Breadcrumb.List>
			</Breadcrumb.Root>
		</div>
		<div class="flex shrink-0 items-center gap-2">
			<InputGroup.Root class="hidden md:flex">
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

			<Button
				variant="outline"
				size="icon"
				class="md:hidden"
				aria-label={isMobileSearchOpen ? 'Close search' : 'Search'}
				aria-expanded={isMobileSearchOpen}
				onclick={() => (isMobileSearchOpen = !isMobileSearchOpen)}
			>
				{#if isMobileSearchOpen}
					<XIcon />
				{:else}
					<SearchIcon />
				{/if}
			</Button>

			<NotificationsButton />
			<div class="hidden sm:contents">
				<ThemeButton />
			</div>

			<Button onclick={toggleUploadImageDialog} aria-label="Create">
				<PlusIcon /> <span class="hidden sm:inline">Create</span>
			</Button>
		</div>
	</div>

	{#if isMobileSearchOpen}
		<div class="px-4 pb-3 md:hidden">
			<InputGroup.Root>
				<InputGroup.Input
					bind:ref={mobileSearchInputRef}
					placeholder="Search..."
					bind:value={searchQuery}
					onkeyup={handleSearch}
				/>
				<InputGroup.Addon>
					<SearchIcon />
				</InputGroup.Addon>
			</InputGroup.Root>
		</div>
	{/if}
</header>
