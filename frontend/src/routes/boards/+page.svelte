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
	import { PUBLIC_API_BASE_URL } from '$env/static/public';
	import { onMount } from 'svelte';
	import { B } from '$lib/helpers';

	let orders = '';

	onMount(() => {
		appState.headerTitle = 'Boards';
	});

	function toggleOrder() {
		orders = orders === 'newest' ? 'oldest' : 'newest';
	}

	let layoutType = $state<LayoutType>('masonry');

	function onInfoClick() {
		appState.boardInfoSheetData.id = page.params.id;
		appState.boardInfoSheetData.isOpen = true;
	}

	function getPathFromId(imageId: string): string {
		return appState.images.find((image) => image.id === imageId)?.thumbnailPath || '';
	}
</script>

<div class="flex flex-wrap items-center gap-4">
	{#each appState.boards as board (board.id)}
		<a href={`/boards/${board.id}`}>
			<div
				class="mb-2 grid h-[170px] w-[250px] grid-cols-3 grid-rows-2 gap-px overflow-hidden rounded-lg"
			>
				<div class="col-span-2 row-span-2 bg-muted">
					{#if board.imageIds[0]}
						<img
							class="h-full w-full object-cover"
							src={B(getPathFromId(board.imageIds[0]))}
							alt="first thumbnail"
						/>
					{/if}
				</div>
				<div class="bg-muted">
					{#if board.imageIds[1]}
						<img
							class="h-full w-full object-cover"
							src={B(getPathFromId(board.imageIds[1]))}
							alt="second thumbnail"
						/>
					{/if}
				</div>
				<div class="bg-muted">
					{#if board.imageIds[2]}
						<img
							class="h-full w-full object-cover"
							src={B(getPathFromId(board.imageIds[2]))}
							alt="third thumbnail"
						/>
					{/if}
				</div>
			</div>
			<div>
				<p class="text-xl font-bold">{board.title}</p>
				<p class="text-sm">{board.imageIds.length} images</p>
			</div>
		</a>
	{/each}
</div>
<!-- <div class="flex justify-between">
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
</div> -->
