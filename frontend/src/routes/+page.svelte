<script lang="ts">
	import ImageCard from '$lib/components/custom/image-card.svelte';
	import Button from '$lib/components/ui/button/button.svelte';
	import type { ImageRecord, LayoutType } from '$lib/types';
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
	import { PUBLIC_API_BASE_URL } from '$env/static/public';

	let columns = $derived(appState.settings.noOfColumns || 5);
	const cardWidth = 247.5;
	const containerPadding = 16 * 2;
	let gap = $derived(16 * (columns - 1));

	let isLoading = $state(true);
	onMount(async () => {
		appState.headerTitle = 'Home';

		if (appState.images.length > 0) {
			isLoading = false;
			return;
		}
		const res = await fetch(`${PUBLIC_API_BASE_URL}/api/v1/image`);
		appState.images = await res.json();
		isLoading = false;
	});

	onMount(() => {
		const targetElement = document.querySelector('#layout-container');

		const resizeObserver = new ResizeObserver((entries) => {
			for (let entry of entries) {
				const newWidth = entry.contentRect.width;
				const availableWidth = newWidth - containerPadding - gap;
				const newColumns = Math.max(1, Math.floor(availableWidth / cardWidth));
				appState.settings.noOfColumns = newColumns;
			}
		});

		// Start observing the element
		resizeObserver.observe(targetElement!);

		return () => {
			resizeObserver.disconnect();
		};
	});

	let orders = $state<'newest' | 'oldest'>('newest');

	let categories = $derived(
		appState.images
			.map((image) => image.category)
			.filter((value, index, self) => self.indexOf(value) === index)
			.filter((category) => !!category)
	);

	let activeCategory = $state<string>('All');

	let filteredImages = $derived(
		activeCategory === 'All'
			? appState.images.filter((image) => !image.isSoftDeleted)
			: appState.images.filter((image) => !image.isSoftDeleted && image.category === activeCategory)
	);

	let chunkedRecords = $derived(
		(() => {
			let images: ImageRecord[][] = [];
			let i = 0;
			for (; i < columns; i++) {
				images.push([]);
			}
			i = 0;

			let j = 0,
				k = filteredImages.length - 1;
			while (i < filteredImages.length) {
				images[i++ % columns].push(filteredImages[orders === 'newest' ? k-- : j++]);
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

	let layoutType = $state<LayoutType>('masonry');
</script>

<div class="flex justify-between">
	<div class="flex gap-2">
		<Button
			size="sm"
			variant={activeCategory === 'All' ? 'default' : 'outline'}
			onclick={() => (activeCategory = 'All')}
			class="cursor-pointer"
		>
			All
		</Button>

		{#each categories as category}
			<Button
				size="sm"
				variant={activeCategory === category ? 'default' : 'outline'}
				onclick={() => (activeCategory = category!)}
				class="cursor-pointer"
			>
				{category}
			</Button>
		{/each}
	</div>
	<div>
		<ButtonGroup.Root>
			<Button
				variant={layoutType === 'masonry' ? 'default' : 'outline'}
				size="icon-sm"
				onclick={() => (layoutType = 'masonry')}
			>
				<LayoutDashboardIcon />
			</Button>
			<Button
				variant={layoutType === 'grid' ? 'default' : 'outline'}
				size="icon-sm"
				onclick={() => (layoutType = 'grid')}
			>
				<LayoutGridIcon />
			</Button>
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
	{#if filteredImages.length > 0}
		<div
			class="grid h-full gap-4"
			class:grid-cols-2={columns === 2}
			class:grid-cols-3={columns === 3}
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
							title={record.title}
							onDelete={() => onDelete(record.id)}
							isCensored={record.isCensored}
							thumbnailPath={record.thumbnailPath}
							mediumPath={record.mediumPath}
							isFavorite={record.isFavorite}
							isSoftDeleted={record.isSoftDeleted}
							{layoutType}
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
