<script lang="ts">
	import { page } from '$app/state';
	import ImageCard from '$lib/components/custom/image-card.svelte';
	import type { Board, ImageRecord } from '$lib/types';

	import * as Empty from '$lib/components/ui/empty/index.js';
	import { Button } from '$lib/components/ui/button/index.js';

	import ArrowUpRightIcon from '@lucide/svelte/icons/arrow-up-right';
	import { ImageIcon } from '@lucide/svelte';

	import type { PageProps } from './$types';

	let { data }: PageProps = $props();
	let board: Board = data.board;
	let records: ImageRecord[] = data.images;

	function onDelete(deletedId: string) {
		records = records.filter((record) => record.id != deletedId);
	}
</script>

{#key page.params.id}
	{#if records.length > 0}
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
