<script lang="ts">
	import { page } from '$app/state';
	import ImageCard from '$lib/components/custom/image-card.svelte';
	import type { Board, ImageRecord } from '$lib/types';

	import * as Empty from '$lib/components/ui/empty/index.js';
	import { Button } from '$lib/components/ui/button/index.js';

	import ArrowUpRightIcon from '@lucide/svelte/icons/arrow-up-right';
	import { ImageIcon } from '@lucide/svelte';

	import type { PageProps } from './$types';
	import { appState } from '$lib/states.svelte';

	let { data }: PageProps = $props();
	// svelte-ignore state_referenced_locally
	let board: Board = $state(data.board);
	// svelte-ignore state_referenced_locally
	let records: ImageRecord[] = $state(data.images);

	let columns = $derived(appState.settings.noOfColumns || 5);

	let chunkedRecords = $derived(
		(() => {
			const images: ImageRecord[][] = [];
			let i = 0;
			for (; i < columns; i++) {
				images.push([]);
			}
			i = 0;

			let j = 0;
			while (i < records.length) {
				images[i++ % columns].push(records[j++]);
			}

			return images;
		})()
	);
	function onDelete(deletedId: string) {
		records = records.filter((record) => record.id != deletedId);
	}
</script>

{#key page.params.id}
	{#if records.length > 0}
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
{/key}
