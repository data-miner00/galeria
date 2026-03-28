<script lang="ts">
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu/index.js';
	import { Button } from '$lib/components/ui/button/index.js';
	import { Download, Ellipsis, Info, Plus, Star, Trash2 } from '@lucide/svelte';
	import { toast } from 'svelte-sonner';
	import * as AlertDialog from '$lib/components/ui/alert-dialog/index.js';

	let isDeleteDialogOpen = $state(false);

	type Props = {
		id: string;
		onDelete: () => void;
	};

	const { id, onDelete }: Props = $props();

	async function removeImage() {
		try {
			const response = await fetch('https://localhost:7146/api/v1/image/' + id, {
				method: 'delete'
			});

			if (!response.ok) {
				throw new Error('Something wrong');
			}
			toast.success('Successfully deleted image.');

			isDeleteDialogOpen = false;

			onDelete();
		} catch {
			toast.error('An error has occurred.');
		}
	}
</script>

<DropdownMenu.Root>
	<DropdownMenu.Trigger>
		{#snippet child({ props })}
			<Button variant="ghost" size="icon" {...props}>
				<Ellipsis />
			</Button>
		{/snippet}
	</DropdownMenu.Trigger>
	<DropdownMenu.Content class="w-56" align="start">
		<DropdownMenu.Item><Info /> View</DropdownMenu.Item>
		<DropdownMenu.Item><Star /> Add to Favorite</DropdownMenu.Item>
		<DropdownMenu.Item><Plus /> Create Board</DropdownMenu.Item>
		<DropdownMenu.Item><Download /> Download</DropdownMenu.Item>
		<DropdownMenu.Separator />
		<DropdownMenu.Item
			class="text-red-500"
			onclick={() => (isDeleteDialogOpen = !isDeleteDialogOpen)}
		>
			<Trash2 /> Delete
		</DropdownMenu.Item>
	</DropdownMenu.Content>
</DropdownMenu.Root>

<AlertDialog.Root bind:open={isDeleteDialogOpen}>
	<AlertDialog.Content>
		<AlertDialog.Header>
			<AlertDialog.Title>Are you absolutely sure?</AlertDialog.Title>
			<AlertDialog.Description>
				This action cannot be undone. This will permanently delete your image and the data from the
				server.
			</AlertDialog.Description>
		</AlertDialog.Header>
		<AlertDialog.Footer>
			<AlertDialog.Cancel>Cancel</AlertDialog.Cancel>
			<AlertDialog.Action onclick={removeImage}>Delete</AlertDialog.Action>
		</AlertDialog.Footer>
	</AlertDialog.Content>
</AlertDialog.Root>
