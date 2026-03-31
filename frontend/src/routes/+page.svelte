<script lang="ts">
	import ImageCard from '$lib/components/custom/image-card.svelte';
	import Button from '$lib/components/ui/button/button.svelte';
	import type { ImageRecord } from '$lib/types';
	import { onMount } from 'svelte';
	import * as ButtonGroup from '$lib/components/ui/button-group/index.js';
	import { LayoutDashboardIcon, LayoutGridIcon } from '@lucide/svelte';
	import { appState } from '$lib/states.svelte';

	onMount(async () => {
		if (appState.images.length > 0) {
			return;
		}
		const res = await fetch('https://localhost:7146/api/v1/image');
		appState.images = await res.json();
	});

	let chunkedRecords = $derived(
		(() => {
			const images: ImageRecord[][] = [];
			let i = 0;
			for (; i < 5; i++) {
				images.push([]);
			}
			i = 0;

			let j = 0;
			while (i < appState.images.length) {
				images[i++ % 5].push(appState.images[j++]);
			}

			return images;
		})()
	);

	function onDelete(deletedId: string) {
		appState.images = appState.images.filter((record) => record.id != deletedId);
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
	<div>
		<ButtonGroup.Root>
			<Button variant="outline" size="icon-sm"><LayoutDashboardIcon /></Button>
			<Button variant="outline" size="icon-sm"><LayoutGridIcon /></Button>
		</ButtonGroup.Root>
	</div>
</div>

<div class="grid h-full grid-cols-5 gap-4">
	{#each chunkedRecords as chunk}
		<div class="flex flex-col gap-4">
			{#each chunk as record}
				<ImageCard
					id={record.id}
					path={record.path}
					description={record.description}
					onDelete={() => onDelete(record.id)}
				/>
			{/each}
		</div>
	{/each}
</div>
