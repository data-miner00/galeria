<script lang="ts">
	import { Button, buttonVariants } from '$lib/components/ui/button/index.js';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import { appState } from '$lib/states.svelte';
	import { X } from '@lucide/svelte';
	import { toast } from 'svelte-sonner';
	import { Switch } from '$lib/components/ui/switch/index.js';
	import * as Tabs from '$lib/components/ui/tabs/index.js';

	type Props = {
		isDialogOpen: boolean;
	};

	let { isDialogOpen = $bindable(false) }: Props = $props();

	let title = $state('');
	let isCensored = $state(false);
	let files: FileList | undefined = $state();
	let image: HTMLImageElement | undefined = $state();
	let imageUrl = $state('');

	type UploadMode = 'file' | 'url';
	let uploadMode = $state<UploadMode>('file');

	function onImageChange() {
		if (files?.[0]) {
			const file = files[0];

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
		const formData = new FormData();

		if (uploadMode === 'file') {
			if (!files?.[0]) return;
			const file = files[0];
			formData.append('File', file);
		} else {
			if (!imageUrl) return;
			const response = await fetch(imageUrl);
			const fileName = imageUrl.split('/').pop() || 'image';

			const blob = await response.blob();

			formData.append('File', new File([blob], fileName, { type: blob.type }));
		}

		formData.append('Title', title);
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
		title = '';
		imageUrl = '';
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

			<Tabs.Root bind:value={uploadMode} class="w-full">
				<Tabs.List class="mb-4">
					<Tabs.Trigger value="file">Upload File</Tabs.Trigger>
					<Tabs.Trigger value="url">URL</Tabs.Trigger>
				</Tabs.List>
				<Tabs.Content value="file">
					<div class="grid gap-4">
						<div class="grid gap-3">
							<Label for="title">Title</Label>
							<Input
								bind:value={title}
								id="title"
								name="title"
								placeholder="e.g. Icelandic volcano"
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
								<Button
									class="absolute top-0 right-0"
									variant="ghost"
									onclick={clearInput}
									size="icon"
								>
									<X />
								</Button>
								<!-- svelte-ignore a11y_missing_attribute -->
								<img bind:this={image} />
							</div>
						{/if}
					</div>
				</Tabs.Content>
				<Tabs.Content value="url">
					<div class="grid gap-4">
						<div class="grid gap-3">
							<Label for="title">Title</Label>
							<Input
								bind:value={title}
								id="title"
								name="title"
								placeholder="e.g. Icelandic volcano"
							/>
						</div>
						<div class="grid gap-3">
							<Label for="imageUrl">Image URL</Label>
							<Input
								bind:value={imageUrl}
								id="imageUrl"
								name="imageUrl"
								placeholder="e.g. https://github.com/data-miner00.png"
							/>
						</div>

						<div>
							<!-- svelte-ignore a11y_missing_attribute -->
							<img src={imageUrl} />
						</div>
						<div class="flex items-center gap-3">
							<Switch id="is-censored" bind:checked={isCensored} />
							<Label for="is-censored">Censor Image</Label>
						</div>
					</div>
				</Tabs.Content>
			</Tabs.Root>

			<Dialog.Footer>
				<Dialog.Close
					type="button"
					class={buttonVariants({ variant: 'outline' })}
					onclick={clearInput}
				>
					Cancel
				</Dialog.Close>
				<Button type="submit" onclick={uploadImage}>Upload</Button>
			</Dialog.Footer>
		</Dialog.Content>
	</form>
</Dialog.Root>
