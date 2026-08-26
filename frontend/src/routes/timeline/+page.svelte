<script lang="ts">
	import {
		ArrowUpRightIcon,
		ChevronLeftIcon,
		ChevronRightIcon,
		DownloadIcon,
		EyeOffIcon,
		ImageIcon,
		ImagesIcon,
		RecycleIcon,
		Trash2Icon
	} from '@lucide/svelte';
	import { onDestroy, onMount, tick } from 'svelte';
	import { toast } from 'svelte-sonner';

	import { PUBLIC_API_BASE_URL } from '$env/static/public';
	import { deleteByIds, downloadMultiple } from '$lib/api/images';
	import LoadingImagesSkeleton from '$lib/components/custom/loading-images-skeleton.svelte';
	import * as AlertDialog from '$lib/components/ui/alert-dialog/index.js';
	import Button from '$lib/components/ui/button/button.svelte';
	import { Checkbox } from '$lib/components/ui/checkbox/index.js';
	import * as Empty from '$lib/components/ui/empty/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import * as Select from '$lib/components/ui/select/index.js';
	import Separator from '$lib/components/ui/separator/separator.svelte';
	import { Switch } from '$lib/components/ui/switch/index.js';
	import { B } from '$lib/helpers';
	import { appState } from '$lib/states.svelte';

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
				return Map.groupBy(mappedImages, ({ date }) => `${date.getFullYear()}-${date.getMonth()}`);
			default:
				return Map.groupBy(mappedImages, () => true);
		}
	});

	const monthNames = [
		'January',
		'February',
		'March',
		'April',
		'May',
		'June',
		'July',
		'August',
		'September',
		'October',
		'November',
		'December'
	];

	function formatGroupHeader(key: string | number | boolean): string {
		if (groupings === 'month') {
			const [year, month] = String(key).split('-').map(Number);
			return `${monthNames[month]} ${year}`;
		}
		return String(key);
	}

	function toggleOrder() {
		orders = orders === 'newest' ? 'oldest' : 'newest';
	}

	function capitalize(str: string): string {
		return str[0].toUpperCase() + str.slice(1);
	}

	let isSelectMode = $state(false);

	let revealedCensoredIds = $state<Set<string>>(new Set());
	let lightboxIndex = $state<number | null>(null);
	let lightboxCloseButton: HTMLButtonElement | null = $state(null);
	let previousActiveElement: HTMLElement | null = $state(null);

	let lightboxImage = $derived(lightboxIndex !== null ? mappedImages[lightboxIndex] : null);

	function handleImageClick(image: (typeof mappedImages)[number]) {
		if (isSelectMode) {
			return;
		}

		if (image.isCensored && !revealedCensoredIds.has(image.id)) {
			revealedCensoredIds = new Set(revealedCensoredIds).add(image.id);
			return;
		}

		const index = mappedImages.findIndex((i) => i.id === image.id);
		if (index === -1) {
			return;
		}

		previousActiveElement = document.activeElement as HTMLElement | null;
		lightboxIndex = index;
	}

	function closeLightbox() {
		lightboxIndex = null;
		previousActiveElement?.focus();
	}

	function showPrevImage() {
		if (lightboxIndex === null) return;
		lightboxIndex = (lightboxIndex - 1 + mappedImages.length) % mappedImages.length;
	}

	function showNextImage() {
		if (lightboxIndex === null) return;
		lightboxIndex = (lightboxIndex + 1) % mappedImages.length;
	}

	function handleLightboxKeyDown(event: KeyboardEvent) {
		if (lightboxIndex === null) return;

		if (event.key === 'Escape') {
			event.preventDefault();
			closeLightbox();
		} else if (event.key === 'ArrowLeft') {
			event.preventDefault();
			showPrevImage();
		} else if (event.key === 'ArrowRight') {
			event.preventDefault();
			showNextImage();
		}
	}

	$effect(() => {
		if (lightboxIndex !== null) {
			tick().then(() => lightboxCloseButton?.focus());
		}
	});

	onDestroy(() => {
		if (lightboxIndex !== null) {
			closeLightbox();
		}
	});

	function imageCheckChange(isChecked: boolean, id: string, sizeInBytes: number) {
		if (isChecked) {
			selectedImageIds.push(id);
			selectedImageSize += sizeInBytes;
		} else {
			selectedImageIds = selectedImageIds.filter((imageId) => imageId !== id);
			selectedImageSize -= sizeInBytes;
		}
	}

	async function deleteSelectedImages(isSoftDelete: boolean = true) {
		try {
			await deleteByIds(selectedImageIds, isSoftDelete);

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

	async function downloadSelectedImages() {
		try {
			const response = await downloadMultiple(selectedImageIds);
			// Read filename from header: Content-Disposition: attachment; filename="archive.zip"
			const disposition = response.headers.get('Content-Disposition');
			const filename = disposition?.match(/filename="?([^"]+)"?/)?.[1] ?? 'download.zip';

			const blob = await response.blob();
			const url = URL.createObjectURL(blob);
			const a = document.createElement('a');
			a.href = url;
			a.download = filename;
			a.click();

			URL.revokeObjectURL(url);

			toast.success('Download started...');
		} catch (error) {
			toast.error(`Download failed. ${error}`);
		}
	}

	let isDeleteDialogOpen = $state(false);
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
			<Button variant="ghost" onclick={downloadSelectedImages}><DownloadIcon /></Button>
			<Button variant="ghost" onclick={() => {}}><ImagesIcon /></Button>
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
				<h1 class="my-4 text-lg font-bold">{formatGroupHeader(group[0])}</h1>
			{/if}
			<div class="mb-8 flex flex-wrap" class:gap-1={gap === 'small'} class:gap-2={gap === 'medium'}>
				{#each group[1] as image}
					<div class="relative h-25 w-25">
						{#if isSelectMode}
							<Checkbox
								id={image.id}
								onCheckedChange={(isChecked) => imageCheckChange(isChecked, image.id, image.size)}
								class="absolute -top-0.5 -right-0.5 z-10 bg-background"
							/>
						{/if}
						<button
							type="button"
							class="relative h-full w-full cursor-pointer overflow-hidden focus-visible:ring-2 focus-visible:ring-primary focus-visible:outline-none"
							onclick={() => handleImageClick(image)}
							aria-label="Open image preview"
						>
							<img
								src={B(image.thumbnailPath)}
								class="h-full w-full object-cover"
								alt={image.title || ''}
							/>
							{#if image.isCensored && !revealedCensoredIds.has(image.id)}
								<div
									class="absolute inset-0 flex items-center justify-center bg-black/30 backdrop-blur-xl"
								>
									<EyeOffIcon class="text-white" />
								</div>
							{/if}
						</button>
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

{#if lightboxImage}
	<div
		class="fixed inset-0 z-50 flex items-center justify-center bg-black/80 p-4"
		role="dialog"
		aria-modal="true"
		aria-label="Image preview dialog"
		onclick={closeLightbox}
		onkeydown={handleLightboxKeyDown}
		tabindex="-1"
	>
		<!-- svelte-ignore a11y_click_events_have_key_events -->
		<!-- svelte-ignore a11y_no_static_element_interactions -->
		<div class="relative max-h-[90vh] max-w-[90vw]" onclick={(e) => e.stopPropagation()}>
			<button
				class="absolute top-2 right-2 z-20 rounded bg-white/90 px-3 py-1 text-sm font-medium text-slate-900 hover:bg-white"
				type="button"
				onclick={closeLightbox}
				aria-label="Close image preview"
				bind:this={lightboxCloseButton}
			>
				Close
			</button>

			{#if mappedImages.length > 1}
				<button
					class="absolute top-1/2 left-2 z-20 -translate-y-1/2 rounded-full bg-white/90 p-2 text-slate-900 hover:bg-white"
					type="button"
					onclick={showPrevImage}
					aria-label="Previous image"
				>
					<ChevronLeftIcon />
				</button>
				<button
					class="absolute top-1/2 right-2 z-20 -translate-y-1/2 rounded-full bg-white/90 p-2 text-slate-900 hover:bg-white"
					type="button"
					onclick={showNextImage}
					aria-label="Next image"
				>
					<ChevronRightIcon />
				</button>
			{/if}

			<img
				class="max-h-[85vh] max-w-[85vw] object-contain"
				alt={lightboxImage.title ?? 'Gallery image preview'}
				src={B(lightboxImage.path)}
			/>
		</div>
	</div>
{/if}
