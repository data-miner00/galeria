<script lang="ts">
	import Button from '$lib/components/ui/button/button.svelte';
	import { onMount } from 'svelte';
	import {
		ArrowUpRightIcon,
		DownloadIcon,
		ImageIcon,
		RecycleIcon,
		Trash2Icon
	} from '@lucide/svelte';
	import { appState } from '$lib/states.svelte';
	import * as Empty from '$lib/components/ui/empty/index.js';

	let isLoading = $derived(appState.isLoading);
	onMount(async () => {
		appState.headerTitle = 'Timeline';
	});

	type Groupings = 'year' | 'month' | 'none';
	type Gap = 'medium' | 'small' | 'none';

	const allGroups = [
		{
			label: 'Year',
			value: 'year'
		},
		{
			label: 'Month',
			value: 'month'
		},
		{
			label: 'None',
			value: 'none'
		}
	];

	const allGaps = [
		{
			label: 'Medium',
			value: 'medium'
		},
		{
			label: 'Small',
			value: 'small'
		},
		{
			label: 'None',
			value: 'none'
		}
	];

	let groupings = $state<Groupings>('year');
	let gap = $state<Gap>('small');

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

	let mappedImages = $derived(
		filteredImages.map((image) => ({ date: new Date(image.takenAt ?? image.createdAt), ...image }))
	);
	let selectedImageIds = $state<string[]>([]);
	let selectedImageSize = $state(0);

	let groupedImages = $derived.by(() => {
		switch (groupings) {
			case 'year':
				return Map.groupBy(mappedImages, ({ date }) => date.getFullYear());
			case 'month':
				return Map.groupBy(mappedImages, ({ date }) => date.getMonth());
			default:
				return Map.groupBy(mappedImages, () => true);
		}
	});

	function toggleOrder() {
		orders = orders === 'newest' ? 'oldest' : 'newest';
	}

	import LoadingImagesSkeleton from '$lib/components/custom/loading-images-skeleton.svelte';
	import { B } from '$lib/helpers';
	import * as Select from '$lib/components/ui/select/index.js';

	function capitalize(str: string): string {
		return str[0].toUpperCase() + str.slice(1);
	}

	import { Label } from '$lib/components/ui/label/index.js';
	import { Switch } from '$lib/components/ui/switch/index.js';
	import { Checkbox } from '$lib/components/ui/checkbox/index.js';
	import Separator from '$lib/components/ui/separator/separator.svelte';
	import { PUBLIC_API_BASE_URL } from '$env/static/public';

	let isSelectMode = $state(false);

	function imageCheckChange(isChecked: boolean, id: string, sizeInBytes: number) {
		if (isChecked) {
			selectedImageIds.push(id);
			selectedImageSize += sizeInBytes;
		} else {
			selectedImageIds = selectedImageIds.filter((imageId) => imageId !== id);
			selectedImageSize -= sizeInBytes;
		}
	}

	function deleteSelectedImages(isSoftDelete: boolean = true) {
		try {
			fetch(`${PUBLIC_API_BASE_URL}/api/v1/image`, {
				method: 'DELETE',
				body: JSON.stringify({
					isSoftDelete,
					requestedIds: selectedImageIds
				})
			});

			if (isSoftDelete) {
				for (var id of selectedImageIds) {
					const image = appState.images.find((image) => image.id == id);
					image!.isSoftDeleted = true;
				}
				toast.success(`Successfully moved ${selectedImageIds.length} images to recycle bin.`);
			} else {
				appState.images = appState.images.filter((image) => !selectedImageIds.includes(image.id));
				isDeleteDialogOpen = false;
				toast.success(`Successfully deleted ${selectedImageIds.length} images.`);
			}

			isSelectMode = false;
			selectedImageIds = [];
			selectedImageSize = 0;
		} catch (error) {
			toast.error(`Something wrong happened. ${error}`);
		}
	}

	let isDeleteDialogOpen = $state(false);

	import * as AlertDialog from '$lib/components/ui/alert-dialog/index.js';
	import { toast } from 'svelte-sonner';
</script>

<div class="flex justify-between">
	{#if !isSelectMode}
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
	{:else}
		<div class="flex items-center gap-2">
			<div class="mr-3 rounded-xl bg-red-50 px-2 py-1 text-sm text-red-700">
				{selectedImageIds.length} selected • {selectedImageSize} bytes
			</div>
			<Separator orientation="vertical" />
			<Button variant="ghost" onclick={() => (isDeleteDialogOpen = !isDeleteDialogOpen)}
				><Trash2Icon /></Button
			>
			<Button variant="ghost" onclick={() => deleteSelectedImages(true)}><RecycleIcon /></Button>
			<Button variant="ghost"><DownloadIcon /></Button>
		</div>
	{/if}
	<div class="flex items-center gap-2">
		<div class="flex items-center gap-3">
			<Switch id="is-selectmode" bind:checked={isSelectMode} />
			<Label for="is-selectmode">Select Mode</Label>
		</div>

		<Select.Root type="single" name="groupings" bind:value={groupings}>
			<Select.Trigger class="w-full">
				{capitalize(groupings)}
			</Select.Trigger>
			<Select.Content>
				<Select.Group>
					{#each allGroups as grouping (grouping.value)}
						<Select.Item value={grouping.value} label={grouping.label}>
							{grouping.label}
						</Select.Item>
					{/each}
				</Select.Group>
			</Select.Content>
		</Select.Root>
		<Select.Root type="single" name="gaps" bind:value={gap}>
			<Select.Trigger class="w-full">
				{capitalize(gap)}
			</Select.Trigger>
			<Select.Content>
				<Select.Group>
					{#each allGaps as gap (gap.value)}
						<Select.Item value={gap.value} label={gap.label}>
							{gap.label}
						</Select.Item>
					{/each}
				</Select.Group>
			</Select.Content>
		</Select.Root>
	</div>
</div>

{#if !isLoading}
	{#if filteredImages.length > 0}
		{#each groupedImages as group}
			{#if groupings !== 'none'}
				<h1 class="my-4 text-lg font-bold">{group[0]}</h1>
			{/if}
			<div class="mb-8 flex flex-wrap" class:gap-1={gap === 'small'} class:gap-2={gap === 'medium'}>
				{#each group[1] as image}
					<div class="relative h-25 w-25">
						{#if isSelectMode}
							<Checkbox
								id={image.id}
								onCheckedChange={(isChecked) => imageCheckChange(isChecked, image.id, image.size)}
								class="absolute -top-0.5 -right-0.5 bg-background"
							/>
						{/if}
						<img
							src={B(image.thumbnailPath)}
							class="h-full w-full object-cover"
							alt={image.title || ''}
						/>
					</div>
				{/each}
			</div>
		{/each}
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
	<LoadingImagesSkeleton layout="timeline" />
{/if}

<AlertDialog.Root bind:open={isDeleteDialogOpen}>
	<AlertDialog.Content>
		<AlertDialog.Header>
			<AlertDialog.Title>Are you absolutely sure?</AlertDialog.Title>
			<AlertDialog.Description>
				This action cannot be undone. This will permanently delete your board and the data from the
				server.
			</AlertDialog.Description>
		</AlertDialog.Header>
		<AlertDialog.Footer>
			<AlertDialog.Cancel>Cancel</AlertDialog.Cancel>
			<AlertDialog.Action onclick={() => deleteSelectedImages(false)}>Delete</AlertDialog.Action>
		</AlertDialog.Footer>
	</AlertDialog.Content>
</AlertDialog.Root>
