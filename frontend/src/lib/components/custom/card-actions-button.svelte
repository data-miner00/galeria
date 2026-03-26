<script lang="ts">
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu/index.js';
	import { Button } from '$lib/components/ui/button/index.js';
	import { Download, Ellipsis, Info, Plus, Star, Trash2 } from '@lucide/svelte';
	import { toast } from 'svelte-sonner';

	type Props = {
		id: string;
	};

	const { id }: Props = $props();

	async function removeImage() {
		try {
			const response = await fetch('https://localhost:7146/api/v1/image/' + id, {
				method: 'delete'
			});

			if (!response.ok) {
				throw new Error('Something wrong');
			}
			toast.success('Successfully deleted image.');
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
		<DropdownMenu.Item onclick={removeImage} class="text-red-500">
			<Trash2 /> Delete
		</DropdownMenu.Item>
	</DropdownMenu.Content>
</DropdownMenu.Root>
