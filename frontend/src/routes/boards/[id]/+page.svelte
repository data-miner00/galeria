<script lang="ts">
	import { page } from '$app/state';
	import ImageCard from '$lib/components/custom/image-card.svelte';
	import type { Board, ImageRecord, LayoutType } from '$lib/types';

	import * as Empty from '$lib/components/ui/empty/index.js';
	import { Button } from '$lib/components/ui/button/index.js';

	import ArrowUpRightIcon from '@lucide/svelte/icons/arrow-up-right';
	import {
		ArrowDown01Icon,
		ArrowUp01Icon,
		ImageIcon,
		InfoIcon,
		LayoutDashboardIcon,
		LayoutGridIcon,
		PinIcon,
		PinOffIcon
	} from '@lucide/svelte';

	import { appState } from '$lib/states.svelte';
	import LoadingImagesSkeleton from '$lib/components/custom/loading-images-skeleton.svelte';
	import * as ButtonGroup from '$lib/components/ui/button-group/index.js';
	import { toast } from 'svelte-sonner';

	let isLoading = $state(true);
	let isPinned = $state(false);

	async function loadBoard(boardId: string) {
		const res = await fetch(`https://localhost:7146/api/v1/board/${boardId}`);
		const board = (await res.json()) as Board;

		isPinned = board.isPinned;

		appState.headerTitle = board.title || 'Unnamed Board';

		// Question: Should fetch by passing params or post body
		if (board.imageIds.length > 0) {
			const response = await fetch(`https://localhost:7146/api/v1/image/getbyids`, {
				method: 'POST',
				headers: {
					'Content-Type': 'application/json'
				},
				body: JSON.stringify({ imageIds: board.imageIds })
			});
			records = await response.json();
		} else {
			records = [];
		}
	}

	// Workaround for onMount not working when navigating from one board to another. This is because Svelte cache the array for some reason.
	$effect(() => {
		if (page.params.id) {
			loadBoard(page.params.id).finally(() => (isLoading = false));
		}
	});

	let records: ImageRecord[] = $state([]);

	let categories = $derived(
		records
			.map((image) => image.category)
			.filter((value, index, self) => self.indexOf(value) === index)
			.filter((category) => !!category)
	);

	let activeCategory = $state<string>('All');
	let filteredImages = $derived(
		activeCategory === 'All'
			? records.filter((image) => !image.isSoftDeleted)
			: records.filter((image) => !image.isSoftDeleted && image.category === activeCategory)
	);
	let columns = $derived(appState.settings.noOfColumns || 5);
	let orders = $state<'newest' | 'oldest'>('newest');

	let chunkedRecords = $derived(
		(() => {
			const images: ImageRecord[][] = [];
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
		records = records.filter((record) => record.id != deletedId);
	}

	function toggleOrder() {
		orders = orders === 'newest' ? 'oldest' : 'newest';
	}

	let layoutType = $state<LayoutType>('masonry');

	function onInfoClick() {
		appState.boardInfoSheetData.id = page.params.id;
		appState.boardInfoSheetData.isOpen = true;
	}

	async function onPinToggle() {
		try {
			const res = await fetch(`https://localhost:7146/api/v1/board/${page.params.id}`, {
				method: 'PATCH',
				headers: {
					'Content-Type': 'application/json'
				},
				body: JSON.stringify({ isPinned: !isPinned })
			});

			if (!res.ok) {
				throw new Error('Failed to update pin status');
			}

			isPinned = !isPinned;
			toast.success(isPinned ? 'Board pinned successfully!' : 'Board unpinned successfully!');
		} catch (error) {
			toast.error('Failed to update pin status');
		}
	}
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
	<div class="flex items-center gap-1">
		<ButtonGroup.Root>
			<Button variant="outline" size="icon-sm" onclick={() => (layoutType = 'masonry')}>
				<LayoutDashboardIcon />
			</Button>
			<Button variant="outline" size="icon-sm" onclick={() => (layoutType = 'grid')}>
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

		<Button variant="outline" size="icon-sm" onclick={onPinToggle} class="ms-2">
			{#if isPinned}
				<PinOffIcon />
			{:else}
				<PinIcon />
			{/if}
		</Button>

		<Button variant="outline" size="icon-sm" onclick={onInfoClick} class="ms-2">
			<InfoIcon />
		</Button>
	</div>
</div>

{#if !isLoading}
	{#if filteredImages.length > 0}
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
				<Empty.Description>Get started by adding your first image to this board.</Empty.Description>
			</Empty.Header>
			<Empty.Content>
				<div class="flex gap-2">
					<Button>Add Image</Button>
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
