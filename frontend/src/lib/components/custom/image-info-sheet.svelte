<script lang="ts">
	import { Button } from '$lib/components/ui/button/index.js';
	import * as Sheet from '$lib/components/ui/sheet/index.js';
	import { buttonVariants } from '$lib/components/ui/button/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import { appState } from '$lib/states.svelte';
	import { toast } from 'svelte-sonner';
	import { onMount } from 'svelte';
	import Textarea from '../ui/textarea/textarea.svelte';

	type Props = {
		isOpen: boolean;
	};

	let { isOpen = $bindable(false) }: Props = $props();

	let currentImageRecord = $derived(
		(() => {
			if (!appState.infoSheetData.id) return null;
			return appState.images.find((img) => img.id === appState.infoSheetData.id);
		})()
	);

	$effect(() => {
		if (!isOpen) {
			resetForm();
		}
	});

	function resetForm() {
		if (!currentImageRecord) return;
		title = currentImageRecord.title || '';
		description = currentImageRecord.description || '';
		category = currentImageRecord.category || '';
		tags = currentImageRecord.tags.join(', ');
	}

	let title = $state('');
	let description = $state('');
	let category = $state('');
	let tags = $state('');

	onMount(() => {
		if (currentImageRecord) {
			title = currentImageRecord.title || '';
			description = currentImageRecord.description || '';
			category = currentImageRecord.category || '';
			tags = currentImageRecord.tags.join(', ');
		}
	});

	type UpdateImageDetailsRequest = {
		title: string;
		description: string;
		category: string;
		tags?: string[];
	};

	async function saveChanges() {
		if (!currentImageRecord) return;

		const body: UpdateImageDetailsRequest = {
			title,
			description,
			category,
			tags: tags
				.split(',')
				.map((t) => t.trim())
				.filter((t) => t.length > 0)
		};

		try {
			const response = await fetch(`https://localhost:7146/api/v1/image/${currentImageRecord.id}`, {
				method: 'PATCH',
				headers: {
					'Content-Type': 'application/json'
				},
				body: JSON.stringify(body)
			});

			if (!response.ok) {
				throw new Error('Failed to update image details');
			}

			appState.images = appState.images.map((img) =>
				img.id === currentImageRecord.id ? { ...img, ...body } : img
			);

			toast.success('Image details updated successfully!');
		} catch (error) {
			toast.error('An error occurred while updating image details.');
		}

		isOpen = false;
	}
</script>

<Sheet.Root bind:open={isOpen}>
	<Sheet.Content side="right" class="overflow-y-scroll">
		<Sheet.Header>
			<Sheet.Title>Image Details</Sheet.Title>
			<Sheet.Description>
				The details of the image will be shown here. You can edit the details and save the changes.
			</Sheet.Description>
		</Sheet.Header>

		{#if currentImageRecord}
			<div class="grid flex-1 auto-rows-min gap-6 px-4">
				<div class="grid gap-3">
					<Label for="id" class="text-end">Id</Label>
					<Input id="id" value={currentImageRecord.id} disabled />
				</div>
				<div class="grid gap-3">
					<Label for="title" class="text-end">Title</Label>
					<Input id="title" placeholder="Enter image title" bind:value={title} />
				</div>
				<div class="grid gap-3">
					<Label for="description" class="text-end">Description</Label>
					<Textarea
						id="description"
						placeholder="Enter image description"
						bind:value={description}
					/>
				</div>
				<div class="grid gap-3">
					<Label for="category" class="text-end">Category</Label>
					<Input id="category" placeholder="Enter image category" bind:value={category} />
				</div>
				<div class="grid gap-3">
					<Label for="tags" class="text-end">Tags</Label>
					<Input id="tags" placeholder="Enter image tags (comma-separated)" bind:value={tags} />
				</div>
				<div class="grid gap-3">
					<Label for="width" class="text-end">Width</Label>
					<Input id="width" value={currentImageRecord.width} disabled />
				</div>
				<div class="grid gap-3">
					<Label for="height" class="text-end">Height</Label>
					<Input id="height" value={currentImageRecord.height} disabled />
				</div>
				<div class="grid gap-3">
					<Label for="size" class="text-end">Size</Label>
					<Input id="size" value={currentImageRecord.size} disabled />
				</div>
				<div class="grid gap-3">
					<Label for="createdAt" class="text-end">Created At</Label>
					<Input id="createdAt" value={currentImageRecord.createdAt} disabled />
				</div>

				{#if currentImageRecord.cameraMake}
					<div class="grid gap-3">
						<Label for="cameraMake" class="text-end">Camera Make</Label>
						<Input id="cameraMake" value={currentImageRecord.cameraMake} disabled />
					</div>
				{/if}

				{#if currentImageRecord.cameraModel}
					<div class="grid gap-3">
						<Label for="cameraModel" class="text-end">Camera Model</Label>
						<Input id="cameraModel" value={currentImageRecord.cameraModel} disabled />
					</div>
				{/if}

				{#if currentImageRecord.takenAt}
					<div class="grid gap-3">
						<Label for="takenAt" class="text-end">Taken At</Label>
						<Input id="takenAt" value={currentImageRecord.takenAt} disabled />
					</div>
				{/if}

				{#if currentImageRecord.orientation}
					<div class="grid gap-3">
						<Label for="orientation" class="text-end">Orientation</Label>
						<Input id="orientation" value={currentImageRecord.orientation} disabled />
					</div>
				{/if}
			</div>
		{:else}
			<!-- Fix this -->
			<p>Image not found.</p>
		{/if}
		<Sheet.Footer>
			<Button type="submit" onclick={saveChanges}>Save changes</Button>
			<Sheet.Close class={buttonVariants({ variant: 'outline' })}>Close</Sheet.Close>
		</Sheet.Footer>
	</Sheet.Content>
</Sheet.Root>
