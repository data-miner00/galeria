<script lang="ts">
	import ImageCard from '$lib/components/custom/image-card.svelte';
	import Button from '$lib/components/ui/button/button.svelte';
	import type { ImageRecord } from '$lib/types';
	import { onMount } from 'svelte';
	import * as ButtonGroup from '$lib/components/ui/button-group/index.js';
	import {
		ArchiveRestoreIcon,
		ArrowDown01Icon,
		ArrowUp01Icon,
		ArrowUpRightIcon,
		ImageIcon,
		LayoutDashboardIcon,
		LayoutGridIcon,
		Trash2Icon
	} from '@lucide/svelte';
	import { appState } from '$lib/states.svelte';
	import * as Empty from '$lib/components/ui/empty/index.js';
	import * as AlertDialog from '$lib/components/ui/alert-dialog/index.js';
	import { toast } from 'svelte-sonner';

	onMount(async () => {
		appState.headerTitle = 'Recycle Bin';
	});

	let isDeleteDialogOpen = $state(false);

	let orders = $state<'newest' | 'oldest'>('newest');

	let columns = $derived(appState.settings.noOfColumns || 5);
	let softDeletedImages = $derived(appState.images.filter((image) => image.isSoftDeleted));

	let chunkedRecords = $derived(
		(() => {
			let images: ImageRecord[][] = [];
			let i = 0;
			for (; i < columns; i++) {
				images.push([]);
			}
			i = 0;

			let j = 0,
				k = softDeletedImages.length - 1;
			while (i < softDeletedImages.length) {
				images[i++ % columns].push(softDeletedImages[orders === 'newest' ? k-- : j++]);
			}

			return images;
		})()
	);

	function onDelete(deletedId: string) {
		appState.images = appState.images.filter((record) => record.id != deletedId);
	}

	function toggleOrder() {
		orders = orders === 'newest' ? 'oldest' : 'newest';
	}

	async function deleteAll() {
		try {
			await fetch('https://localhost:7146/api/v1/image/recyclebin/clear', {
				method: 'DELETE'
			});

			appState.images = appState.images.filter((record) => !record.isSoftDeleted);

			toast.success('Recycle bin cleared successfully.');
		} catch {
			toast.error('An error has occurred while clearing recycle bin.');
		}

		isDeleteDialogOpen = false;
	}
</script>

<div class="flex justify-between">
	<div class="flex gap-2">
		<Button size="sm">All</Button>
		<Button size="sm" variant="ghost">Photography</Button>
		<Button size="sm" variant="ghost">Gadgets</Button>
		<Button size="sm" variant="ghost">Books</Button>
		<Button size="sm" variant="ghost">Cars</Button>
	</div>
	<div class="flex gap-2">
		<ButtonGroup.Root>
			<Button variant="outline" size="icon-sm"><LayoutDashboardIcon /></Button>
			<Button variant="outline" size="icon-sm"><LayoutGridIcon /></Button>
			<Button variant="outline" size="icon-sm" onclick={toggleOrder}>
				{#if orders === 'newest'}
					<ArrowDown01Icon />
				{:else}
					<ArrowUp01Icon />
				{/if}
			</Button>
		</ButtonGroup.Root>

		<Button variant="outline" size="icon-sm"><ArchiveRestoreIcon /></Button>

		<Button variant="destructive" size="sm" onclick={() => (isDeleteDialogOpen = true)}>
			<Trash2Icon /> Clear All
		</Button>
	</div>
</div>

{#if softDeletedImages.length > 0}
	<div
		class="grid h-full gap-4"
		class:grid-cols-5={columns === 5}
		class:grid-cols-4={columns === 4}
		class:grid-cols-6={columns === 6}
	>
		{#each chunkedRecords as chunk}
			<div class="flex flex-col gap-4">
				{#each chunk as record}
					<ImageCard
						id={record.id}
						path={record.path}
						description={record.description}
						onDelete={() => onDelete(record.id)}
						isCensored={record.isCensored}
						thumbnailPath={record.thumbnailPath}
						mediumPath={record.mediumPath}
						isFavorite={record.isFavorite}
						isSoftDeleted={record.isSoftDeleted}
					/>
				{/each}
			</div>
		{/each}
	</div>
{:else}
	<Empty.Root>
		<Empty.Header>
			<Empty.Media variant="icon">
				<ImageIcon />
			</Empty.Media>
			<Empty.Title>Nothing in the Recycle Bin</Empty.Title>
			<Empty.Description>Your recycled images will appear here</Empty.Description>
		</Empty.Header>
		<Empty.Content>
			<div class="flex gap-2">
				<Button href="/">Home</Button>
				<Button variant="outline" href="/settings/general">Settings</Button>
			</div>
		</Empty.Content>
		<Button variant="link" class="text-muted-foreground" size="sm">
			<a href="#/">
				Learn More <ArrowUpRightIcon class="inline" />
			</a>
		</Button>
	</Empty.Root>
{/if}

<AlertDialog.Root bind:open={isDeleteDialogOpen}>
	<AlertDialog.Content>
		<AlertDialog.Header>
			<AlertDialog.Title>Are you absolutely sure?</AlertDialog.Title>
			<AlertDialog.Description>
				This action cannot be undone. This will permanently delete your image and the data from the
				server.
			</AlertDialog.Description>
		</AlertDialog.Header>
		<AlertDialog.Footer>
			<AlertDialog.Cancel>Cancel</AlertDialog.Cancel>
			<AlertDialog.Action onclick={deleteAll}>Delete</AlertDialog.Action>
		</AlertDialog.Footer>
	</AlertDialog.Content>
</AlertDialog.Root>
