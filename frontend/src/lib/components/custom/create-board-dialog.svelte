<script lang="ts">
	import { Button, buttonVariants } from '$lib/components/ui/button/index.js';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import { Input } from '$lib/components/ui/input/index.js';
	import { Label } from '$lib/components/ui/label/index.js';
	import { toast } from 'svelte-sonner';

	type Props = {
		isDialogOpen: boolean;
	};

	let { isDialogOpen = $bindable(false) }: Props = $props();

	let title = $state('');
	let description = $state('');

	function clearInput() {
		title = '';
		description = '';
	}

	async function createBoard() {
		const res = await fetch('https://localhost:7146/api/v1/board', {
			method: 'post',
			headers: {
				'Content-Type': 'application/json'
			},
			body: JSON.stringify({
				title,
				description
			})
		});

		if (!res.ok) {
			toast.error('Board could not be created.');
			return;
		}

		toast.success('Board created successfully.');

		clearInput();
		isDialogOpen = false;
	}
</script>

<Dialog.Root bind:open={isDialogOpen}>
	<form>
		<Dialog.Content class="sm:max-w-[425px]">
			<Dialog.Header>
				<Dialog.Title>Create Board</Dialog.Title>
				<Dialog.Description
					>Add a new image board to organize the collections nicely.</Dialog.Description
				>
			</Dialog.Header>
			<div class="grid gap-4">
				<div class="grid gap-3">
					<Label for="description">Title</Label>
					<Input bind:value={title} id="title" name="title" placeholder="e.g. My orange cat" />
				</div>
				<div class="grid gap-3">
					<Label for="description">Description</Label>
					<Input
						bind:value={description}
						id="description"
						name="description"
						placeholder="e.g. A collection of my naughty orange kids"
					/>
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
				<Button type="submit" onclick={createBoard}>Save changes</Button>
			</Dialog.Footer>
		</Dialog.Content>
	</form>
</Dialog.Root>
