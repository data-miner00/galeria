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

	onMount(async () => {
		appState.headerTitle = 'Hidden Images';
	});

	let orders = $state<'newest' | 'oldest'>('newest');

	let columns = $derived(appState.settings.noOfColumns || 5);
	let hiddenImages = $derived(appState.images.filter((image) => image.isHidden));
	let categories = $derived(
		hiddenImages
			.map((image) => image.category)
			.filter((value, index, self) => self.indexOf(value) === index)
			.filter((category) => !!category)
	);
	let activeCategory = $state<string>('All');
	let chunkedRecords = $derived.by(() => {
		let images: ImageRecord[][] = [];
		let i = 0;
		for (; i < columns; i++) {
			images.push([]);
		}
		i = 0;

		let j = 0,
			k = hiddenImages.length - 1;
		while (i < hiddenImages.length) {
			images[i++ % columns].push(hiddenImages[orders === 'newest' ? k-- : j++]);
		}

		return images;
	});

	function onDelete(deletedId: string) {
		appState.images = appState.images.filter((record) => record.id != deletedId);
	}

	function toggleOrder() {
		orders = orders === 'newest' ? 'oldest' : 'newest';
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
	<div class="flex gap-2">
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

{#if hiddenImages.length > 0}
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
						title={record.title}
						onDelete={() => onDelete(record.id)}
						isCensored={record.isCensored}
						thumbnailPath={record.thumbnailPath}
						mediumPath={record.mediumPath}
						isFavorite={record.isFavorite}
						isSoftDeleted={record.isSoftDeleted}
						isHidden={record.isHidden}
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
			<Empty.Title>Nothing hidden here</Empty.Title>
			<Empty.Description>Your hidden images will appear here</Empty.Description>
		</Empty.Header>
		<Empty.Content>
			<div class="flex gap-2">
				<Button href="/">Home</Button>
				<Button variant="outline" href="/settings/general">Settings</Button>
			</div>
		</Empty.Content>
		<Button variant="link" class="text-muted-foreground" size="sm">
			<a href="#/">
				Learn More <ArrowUpRightIcon class="inline" />
			</a>
		</Button>
	</Empty.Root>
{/if}
