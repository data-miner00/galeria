<script lang="ts">
	import { Button, buttonVariants } from '$lib/components/ui/button/index.js';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import { appState } from '$lib/states.svelte';
	import { X } from '@lucide/svelte';
	import { toast } from 'svelte-sonner';
	import { Switch } from '$lib/components/ui/switch/index.js';

	type Props = {
		isDialogOpen: boolean;
	};

	let { isDialogOpen = $bindable(false) }: Props = $props();

	let description = $state('');
	let isCensored = $state(false);
	let files: FileList | undefined = $state();
	let image: HTMLImageElement | undefined = $state();
	let showImage = $state(false);

	function onImageChange() {
		if (files?.[0]) {
			const file = files[0];
			showImage = true;

			const reader = new FileReader();

			reader.addEventListener('load', function () {
				if (reader.result) {
					image?.setAttribute('src', reader.result.toString());
				}
			});
			reader.readAsDataURL(file);
		}
	}

	async function uploadImage() {
		if (!files?.[0]) return;
		const file = files[0];

		const formData = new FormData();
		formData.append('File', file);
		formData.append('Description', description);
		formData.append('IsCensored', isCensored.toString());

		try {
			const response = await fetch('https://localhost:7146/api/v1/image', {
				method: 'POST',
				body: formData
			});

			const result = await response.json();

			if (!response.ok) {
				throw new Error(result.errorMessage || 'Something went wrong.');
			}

			appState.images.push(result);
			toast.success('Image uploaded successfully.');
			isDialogOpen = false;

			clearInput();
		} catch (error) {
			toast.error('Image upload failed. ' + (error as Error).message);
		}
	}

	function clearInput() {
		description = '';
		isCensored = false;
		files = undefined;
		image = undefined;
	}
</script>

<Dialog.Root bind:open={isDialogOpen}>
	<form>
		<Dialog.Content class="max-h-[90dvh] overflow-y-auto sm:max-w-106.25">
			<Dialog.Header>
				<Dialog.Title>Upload Image</Dialog.Title>
				<Dialog.Description>Upload and create a new entry in the gallery.</Dialog.Description>
			</Dialog.Header>
			<div class="grid gap-4">
				<div class="grid gap-3">
					<Label for="description">Description</Label>
					<Input
						bind:value={description}
						id="description"
						name="description"
						placeholder="e.g. A very cool photograph of volcano"
					/>
				</div>
				<div class="grid gap-3">
					<Label for="image">Image</Label>
					<Input bind:files onchange={onImageChange} id="image" name="image" type="file" />
				</div>

				<div class="flex items-center gap-3">
					<Switch id="is-censored" bind:checked={isCensored} />
					<Label for="is-censored">Censor Image</Label>
				</div>

				{#if files}
					<div class="relative">
						<Button class="absolute top-0 right-0" variant="ghost" onclick={clearInput} size="icon">
							<X />
						</Button>
						<!-- svelte-ignore a11y_missing_attribute -->
						<img bind:this={image} />
					</div>
				{/if}
			</div>
			<Dialog.Footer>
				<Dialog.Close
					type="button"
					class={buttonVariants({ variant: 'outline' })}
					onclick={clearInput}
				>
					Cancel
				</Dialog.Close>
				<Button type="submit" onclick={uploadImage}>Save changes</Button>
			</Dialog.Footer>
		</Dialog.Content>
	</form>
</Dialog.Root>
