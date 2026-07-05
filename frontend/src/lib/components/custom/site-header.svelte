<script lang="ts">
	import { ArrowLeft, PlusIcon, SearchIcon } from '@lucide/svelte';

	import { goto } from '$app/navigation';
	import ThemeButton from '$lib/components/custom/theme-button.svelte';
	import ToTopButton from '$lib/components/custom/to-top-button.svelte';
	import UploadImageDialog from '$lib/components/custom/upload-image-dialog.svelte';
	import * as Breadcrumb from '$lib/components/ui/breadcrumb/index.js';
	import { Button } from '$lib/components/ui/button';
	import * as InputGroup from '$lib/components/ui/input-group/index.js';
	import { Separator } from '$lib/components/ui/separator/index.js';
	import * as Sidebar from '$lib/components/ui/sidebar/index.js';
	import { appState } from '$lib/states.svelte';

	let searchQuery = $state('');
	function handleSearch(e: KeyboardEvent) {
		if (e.key === 'Enter') {
			goto('/search?q=' + encodeURIComponent(searchQuery));
			searchQuery = '';
		}
	}

	type Props = {
		searchInputRef: HTMLInputElement | null;
	};

	let { searchInputRef = $bindable(null) }: Props = $props();
</script>

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

			<Button
				onclick={() =>
					(appState.openState.isUploadImageDialogOpen =
						!appState.openState.isUploadImageDialogOpen)}
			>
				<PlusIcon /> Create
			</Button>
		</div>
	</div>
</header>
