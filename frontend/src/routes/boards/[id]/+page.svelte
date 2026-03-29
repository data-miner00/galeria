<script lang="ts">
	import { page } from '$app/state';
	import ImageCard from '$lib/components/custom/image-card.svelte';
	import type { Board, ImageRecord } from '$lib/types';
	import { onMount } from 'svelte';
	import { toast } from 'svelte-sonner';

	let board = $state<Board | null>(null);
	let records = $state<ImageRecord[]>([]);

	onMount(async () => {
		const res = await fetch(`https://localhost:7146/api/v1/board/${page.params.id}`);
		if (!res.ok) {
			toast.error('Failed to fetch board details');
			return;
		}
		board = await res.json();

		if (!board) {
			toast.error('Board not found');
			return;
		}

		// TODO: Optimize this by creating an endpoint that returns all images for a board in one request
		if (board.imageIds.length > 0) {
			for (const imageId of board.imageIds) {
				const imageRes = await fetch(`https://localhost:7146/api/v1/image/${imageId}`);
				if (!imageRes.ok) {
					toast.error(`Failed to fetch image with ID: ${imageId}`);
					continue;
				}
				const imageData = await imageRes.json();
				records = [...records, imageData];
			}
		}
	});

	function onDelete(deletedId: string) {
		records = records.filter((record) => record.id != deletedId);
	}
</script>

<div class="flex w-70 flex-col gap-4">
	{#each records as record}
		<ImageCard
			id={record.id}
			path={record.path}
			description={record.description}
			onDelete={() => onDelete(record.id)}
		/>
	{/each}
</div>
