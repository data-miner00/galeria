<script lang="ts">
	import { Button, buttonVariants } from '$lib/components/ui/button/index.js';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import type { Board } from '$lib/types';
	import { onMount } from 'svelte';
	import { toast } from 'svelte-sonner';
	import * as Select from '$lib/components/ui/select/index.js';

	type Props = {
		isDialogOpen: boolean;
		imageId: string;
	};

	let { isDialogOpen = $bindable(false), imageId }: Props = $props();

	let boardId = $state('');

	function clearInput() {
		boardId = '';
	}

	let boards = $state<Board[]>([]);

	onMount(async () => {
		const res = await fetch('https://localhost:7146/api/v1/board');
		if (!res.ok) {
			toast.error('Failed to fetch boards.');
			return;
		}
		boards = await res.json();
	});

	async function addImageToBoard() {
		if (!boardId) {
			toast.error('Please select a board.');
			return;
		}

		const res = await fetch(`https://localhost:7146/api/v1/board/${boardId}/${imageId}`, {
			method: 'post'
		});

		if (!res.ok) {
			toast.error('Image could not be added.');
			return;
		}

		toast.success('Image added to board successfully.');

		clearInput();
		isDialogOpen = false;
	}

	const triggerContent = $derived(boards.find((f) => f.id === boardId)?.title ?? 'Select a board');
</script>

<Dialog.Root bind:open={isDialogOpen}>
	<form>
		<Dialog.Content class="sm:max-w-106.25">
			<Dialog.Header>
				<Dialog.Title>Add to Board</Dialog.Title>
				<Dialog.Description>Add this image to the selected board.</Dialog.Description>
			</Dialog.Header>
			<div class="grid gap-4">
				<div class="grid gap-3">
					<Label for="board">Board</Label>
					<Select.Root type="single" bind:value={boardId}>
						<Select.Trigger class="w-full">
							{triggerContent}
						</Select.Trigger>
						<Select.Content>
							<Select.Group>
								{#each boards as board}
									<Select.Item value={board.id}>
										{board.title}
									</Select.Item>
								{/each}
							</Select.Group>
						</Select.Content>
					</Select.Root>
				</div>
			</div>
			<Dialog.Footer>
				<Dialog.Close
					type="button"
					class={buttonVariants({ variant: 'outline' })}
					onclick={clearInput}
				>
					Cancel
				</Dialog.Close>
				<Button disabled={!boardId} type="submit" onclick={addImageToBoard}>Add Image</Button>
			</Dialog.Footer>
		</Dialog.Content>
	</form>
</Dialog.Root>
