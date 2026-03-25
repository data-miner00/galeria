<script lang="ts">
	import { Button, buttonVariants } from '$lib/components/ui/button/index.js';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';

	type Props = {
		isDialogOpen: boolean;
	};

	let { isDialogOpen = $bindable(false) }: Props = $props();

	let description = $state('');
	let files: FileList;
	let image: HTMLImageElement;
	let showImage = $state(false);

	function onImageChange() {
		const file = files[0];
		if (file) {
			showImage = true;

			const reader = new FileReader();

			reader.addEventListener('load', function () {
				if (reader.result) {
					image.setAttribute('src', reader.result.toString());
				}
			});
			reader.readAsDataURL(file);
		}
	}

	async function uploadImage() {
		const file = files[0];
		if (!file) return;

		const formData = new FormData();
		formData.append('File', file);
		formData.append('Description', description);

		try {
			const response = await fetch('https://localhost:7146/api/v1/image', {
				method: 'POST',
				body: formData
			});

			if (!response.ok) {
				throw new Error(`Upload failed: ${response.statusText}`);
			}

			console.log('Upload successful');

			isDialogOpen = false;
		} catch (error) {
			console.error('Upload error:', error);
		}
	}
</script>

<Dialog.Root bind:open={isDialogOpen}>
	<form>
		<Dialog.Content class="sm:max-w-[425px]">
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
				<!-- svelte-ignore a11y_missing_attribute -->
				<img bind:this={image} />
			</div>
			<Dialog.Footer>
				<Dialog.Close type="button" class={buttonVariants({ variant: 'outline' })}>
					Cancel
				</Dialog.Close>
				<Button type="submit" onclick={uploadImage}>Save changes</Button>
			</Dialog.Footer>
		</Dialog.Content>
	</form>
</Dialog.Root>
