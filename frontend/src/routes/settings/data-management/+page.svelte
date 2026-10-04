<script lang="ts">
	import { Trash2Icon } from '@lucide/svelte';
	import { onMount } from 'svelte';

	import { resolve } from '$app/paths';
	import * as AlertDialog from '$lib/components/ui/alert-dialog/index.js';
	import { Button } from '$lib/components/ui/button/index.js';
	import { Spinner } from '$lib/components/ui/spinner/index.js';
	import { toast } from '$lib/notify';
	import { clearRecycleBin } from '$lib/services/imageService';
	import { appState } from '$lib/states.svelte';

	onMount(() => {
		appState.headerTitle = 'Data Management';
	});

	let isDeleteDialogOpen = $state(false);
	let isEmptying = $state(false);

	let recycledCount = $derived(appState.images.filter((image) => image.isSoftDeleted).length);

	async function emptyRecycleBin() {
		isDeleteDialogOpen = false;
		isEmptying = true;

		try {
			await clearRecycleBin();

			appState.images = appState.images.filter((record) => !record.isSoftDeleted);

			toast.success('Recycle bin emptied successfully.');
		} catch {
			toast.error('An error has occurred while emptying the recycle bin.');
		}

		isEmptying = false;
	}
</script>

<h1 class="text-2xl font-bold">Data Management</h1>

<p>Manage the images and data stored by this application.</p>

<section class="mt-6">
	<h2 class="mb-1 text-lg font-semibold">Recycle Bin</h2>
	<p class="mb-4 max-w-sm text-sm text-muted-foreground">
		{#if appState.isLoading}
			Checking the recycle bin...
		{:else}
			{recycledCount}
			{recycledCount === 1 ? 'image is' : 'images are'} in the
			<a href={resolve('/recycle')} class="underline">recycle bin</a>.
		{/if}
	</p>

	<div class="grid w-full max-w-sm gap-4">
		<Button
			size="sm"
			variant="destructive"
			disabled={appState.isLoading || isEmptying || recycledCount === 0}
			onclick={() => (isDeleteDialogOpen = true)}
		>
			{#if isEmptying}
				<Spinner />
			{:else}
				<Trash2Icon />
			{/if}
			{isEmptying ? 'Emptying...' : 'Empty Recycle Bin'}
		</Button>
	</div>
</section>

<AlertDialog.Root bind:open={isDeleteDialogOpen}>
	<AlertDialog.Content>
		<AlertDialog.Header>
			<AlertDialog.Title>Empty the recycle bin?</AlertDialog.Title>
			<AlertDialog.Description>
				This action cannot be undone. This will permanently delete {recycledCount}
				{recycledCount === 1 ? 'image' : 'images'} and the data from the server.
			</AlertDialog.Description>
		</AlertDialog.Header>
		<AlertDialog.Footer>
			<AlertDialog.Cancel>Cancel</AlertDialog.Cancel>
			<AlertDialog.Action onclick={emptyRecycleBin}>Delete</AlertDialog.Action>
		</AlertDialog.Footer>
	</AlertDialog.Content>
</AlertDialog.Root>
