<script lang="ts">
	import { PinIcon, PlusIcon } from '@lucide/svelte';
	import { onMount } from 'svelte';

	import { B } from '$lib/helpers';
	import { appState, toggleCreateBoardDialog } from '$lib/states.svelte';
	import type { LayoutType } from '$lib/types';

	let orders = '';

	onMount(() => {
		appState.headerTitle = 'Boards';
	});

	function toggleOrder() {
		orders = orders === 'newest' ? 'oldest' : 'newest';
	}

	let layoutType = $state<LayoutType>('masonry');

	function getPathFromId(imageId: string): string {
		return appState.images.find((image) => image.id === imageId)?.thumbnailPath || '';
	}
</script>

<p class="mb-4 text-sm text-muted-foreground">
	{appState.boards.length}
	{appState.boards.length === 1 ? 'board' : 'boards'}
</p>

<div class="flex flex-wrap items-center gap-4">
	{#each appState.boards as board (board.id)}
		<a href={`/boards/${board.id}`}>
			<div
				class="relative mb-2 grid h-42.5 w-62.5 grid-cols-3 grid-rows-2 gap-px overflow-hidden rounded-lg"
			>
				{#if board.isPinned}
					<div class="absolute top-1.5 right-1.5 z-10 rounded-full bg-background/80 p-1">
						<PinIcon class="size-3.5" />
					</div>
				{/if}
				<div class="col-span-2 row-span-2 bg-muted">
					{#if board.imageIds[0]}
						<img
							class="h-full w-full object-cover"
							src={B(getPathFromId(board.imageIds[0]))}
							alt="first thumbnail"
							loading="lazy"
							decoding="async"
						/>
					{/if}
				</div>
				<div class="bg-muted">
					{#if board.imageIds[1]}
						<img
							class="h-full w-full object-cover"
							src={B(getPathFromId(board.imageIds[1]))}
							alt="second thumbnail"
							loading="lazy"
							decoding="async"
						/>
					{/if}
				</div>
				<div class="bg-muted">
					{#if board.imageIds[2]}
						<img
							class="h-full w-full object-cover"
							src={B(getPathFromId(board.imageIds[2]))}
							alt="third thumbnail"
							loading="lazy"
							decoding="async"
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
	<button class="cursor-pointer text-left" onclick={toggleCreateBoardDialog}>
		<div
			class="mb-2 flex h-42.5 w-62.5 items-center justify-center rounded-lg border-2 border-dashed border-muted-foreground/30 text-muted-foreground transition-colors hover:border-muted-foreground/60 hover:bg-muted/50 hover:text-foreground"
		>
			<PlusIcon class="size-8" />
		</div>
		<div>
			<p class="text-xl font-bold">New board</p>
			<p class="text-sm text-muted-foreground">Create a board</p>
		</div>
	</button>
</div>
