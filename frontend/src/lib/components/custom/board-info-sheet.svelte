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

	let currentBoardRecord = $derived(
		(() => {
			if (!appState.boardInfoSheetData.id) return null;
			return appState.boards.find((board) => board.id === appState.boardInfoSheetData.id);
		})()
	);

	$effect(() => {
		if (!isOpen) {
			resetForm();
		}
	});

	function resetForm() {
		if (!currentBoardRecord) return;
		description = currentBoardRecord.description || '';
		title = currentBoardRecord.title || '';
	}

	let description = $state('');
	let title = $state('');

	onMount(() => {
		console.log('Current Board Record:', currentBoardRecord);
		if (currentBoardRecord) {
			description = currentBoardRecord.description || '';
			title = currentBoardRecord.title || '';
		}
	});

	async function saveChanges() {
		if (!currentBoardRecord) return;

		const body = {
			description,
			title
		};

		try {
			const response = await fetch(`https://localhost:7146/api/v1/board/${currentBoardRecord.id}`, {
				method: 'PATCH',
				headers: {
					'Content-Type': 'application/json'
				},
				body: JSON.stringify(body)
			});

			if (!response.ok) {
				throw new Error('Failed to update board details');
			}

			appState.boards = appState.boards.map((board) =>
				board.id === currentBoardRecord.id ? { ...board, ...body } : board
			);

			toast.success('Board details updated successfully!');
		} catch (error) {
			toast.error('An error occurred while updating board details.');
		}

		isOpen = false;
	}
</script>

<Sheet.Root bind:open={isOpen}>
	<Sheet.Content side="right">
		<Sheet.Header>
			<Sheet.Title>Board Details</Sheet.Title>
			<Sheet.Description>
				The details of the board will be shown here. You can edit the details and save the changes.
			</Sheet.Description>
		</Sheet.Header>

		{#if currentBoardRecord}
			<div class="grid flex-1 auto-rows-min gap-6 px-4">
				<div class="grid gap-3">
					<Label for="id" class="text-end">Id</Label>
					<Input id="id" value={currentBoardRecord.id} disabled />
				</div>
				<div class="grid gap-3">
					<Label for="title" class="text-end">Title</Label>
					<Input id="title" placeholder="Enter board title" bind:value={title} />
				</div>
				<div class="grid gap-3">
					<Label for="description" class="text-end">Description</Label>
					<Textarea id="description" placeholder="Enter board description" bind:value={description}
					></Textarea>
				</div>
				<div class="grid gap-3">
					<Label for="width" class="text-end">No of Images</Label>
					<Input id="width" value={currentBoardRecord.imageIds.length} disabled />
				</div>
				<div class="grid gap-3">
					<Label for="createdAt" class="text-end">Created At</Label>
					<Input id="createdAt" value={currentBoardRecord.createdAt} disabled />
				</div>
			</div>
		{:else}
			<p>Board not found.</p>
		{/if}
		<Sheet.Footer>
			<Button type="submit" onclick={saveChanges}>Save changes</Button>
			<Sheet.Close class={buttonVariants({ variant: 'outline' })}>Close</Sheet.Close>
		</Sheet.Footer>
	</Sheet.Content>
</Sheet.Root>
