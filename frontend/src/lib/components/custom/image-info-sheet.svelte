<script lang="ts">
	import { Button } from '$lib/components/ui/button/index.js';
	import * as Sheet from '$lib/components/ui/sheet/index.js';
	import { buttonVariants } from '$lib/components/ui/button/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import { appState } from '$lib/states.svelte';

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
</script>

<Sheet.Root bind:open={isOpen}>
	<Sheet.Content side="right">
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
					<Label for="description" class="text-end">Description</Label>
					<Input
						id="description"
						placeholder="Enter image description"
						value={currentImageRecord.description}
					/>
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
			</div>
		{:else}
			<p>Image not found.</p>
		{/if}
		<Sheet.Footer>
			<Button type="submit">Save changes</Button>
			<Sheet.Close class={buttonVariants({ variant: 'outline' })}>Close</Sheet.Close>
		</Sheet.Footer>
	</Sheet.Content>
</Sheet.Root>
