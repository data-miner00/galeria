<script lang="ts">
	import ImageCard from '$lib/components/custom/image-card.svelte';
	import Button from '$lib/components/ui/button/button.svelte';
	import type { ImageRecord } from '$lib/types';
	import { onMount } from 'svelte';
	import * as ButtonGroup from '$lib/components/ui/button-group/index.js';
	import {
		ArrowDown01Icon,
		ArrowUp01Icon,
		ArrowUpRightIcon,
		ImageIcon,
		LayoutDashboardIcon,
		LayoutGridIcon
	} from '@lucide/svelte';
	import { appState } from '$lib/states.svelte';
	import * as Empty from '$lib/components/ui/empty/index.js';

	let isLoading = $state(true);
	onMount(async () => {
		appState.headerTitle = 'Home';

		if (appState.images.length > 0) {
			isLoading = false;
			return;
		}
		const res = await fetch('https://localhost:7146/api/v1/image');
		appState.images = await res.json();
		isLoading = false;
	});

	let orders = $state<'newest' | 'oldest'>('newest');

	let columns = $derived(appState.settings.noOfColumns || 5);

	let nonSoftDeletedImages = $derived(appState.images.filter((image) => !image.isSoftDeleted));

	let chunkedRecords = $derived(
		(() => {
			let images: ImageRecord[][] = [];
			let i = 0;
			for (; i < columns; i++) {
				images.push([]);
			}
			i = 0;

			let j = 0,
				k = nonSoftDeletedImages.length - 1;
			while (i < nonSoftDeletedImages.length) {
				images[i++ % columns].push(nonSoftDeletedImages[orders === 'newest' ? k-- : j++]);
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

	import LoadingImagesSkeleton from '$lib/components/custom/loading-images-skeleton.svelte';
</script>

<div class="flex justify-between">
	<div class="flex gap-2">
		<Button size="sm">All</Button>
		<Button size="sm" variant="ghost">Photography</Button>
		<Button size="sm" variant="ghost">Gadgets</Button>
		<Button size="sm" variant="ghost">Books</Button>
		<Button size="sm" variant="ghost">Cars</Button>
	</div>
	<div>
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
	</div>
</div>

{#if !isLoading}
	{#if nonSoftDeletedImages.length > 0}
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
				<Empty.Title>No Images Yet</Empty.Title>
				<Empty.Description>Get started by adding your first image to show here!</Empty.Description>
			</Empty.Header>
			<Empty.Content>
				<div class="flex gap-2">
					<Button>Upload Image</Button>
					<Button variant="outline">Import Images</Button>
				</div>
			</Empty.Content>
			<Button variant="link" class="text-muted-foreground" size="sm">
				<a href="#/">
					Learn More <ArrowUpRightIcon class="inline" />
				</a>
			</Button>
		</Empty.Root>
	{/if}
{:else}
	<LoadingImagesSkeleton />
{/if}
