<script lang="ts">
	import type { LayoutType } from '$lib/types';

	import { appState } from '$lib/states.svelte';
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

	function getPathFromId(imageId: string): string {
		return appState.images.find((image) => image.id === imageId)?.thumbnailPath || '';
	}
</script>

<div class="flex flex-wrap items-center gap-4">
	{#each appState.boards as board (board.id)}
		<a href={`/boards/${board.id}`}>
			<div
				class="mb-2 grid h-42.5 w-62.5 grid-cols-3 grid-rows-2 gap-px overflow-hidden rounded-lg"
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
