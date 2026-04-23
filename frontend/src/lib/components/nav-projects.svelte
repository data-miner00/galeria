<script lang="ts">
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu/index.js';
	import * as Sidebar from '$lib/components/ui/sidebar/index.js';
	import { useSidebar } from '$lib/components/ui/sidebar/index.js';
	import type { Board } from '$lib/types';
	import { FrameIcon, PinIcon, PinOffIcon } from '@lucide/svelte';
	import EllipsisIcon from '@lucide/svelte/icons/ellipsis';
	import FolderIcon from '@lucide/svelte/icons/folder';
	import ShareIcon from '@lucide/svelte/icons/share';
	import Trash2Icon from '@lucide/svelte/icons/trash-2';
	import { toast } from 'svelte-sonner';
	import * as AlertDialog from '$lib/components/ui/alert-dialog/index.js';
	import { appState } from '$lib/states.svelte';
	import { PUBLIC_API_BASE_URL } from '$env/static/public';
	import { goto } from '$app/navigation';

	let {
		boards
	}: {
		boards: Board[];
	} = $props();

	let isDeleteDialogOpen = $state(false);

	const sidebar = useSidebar();

	let id = $state('');

	async function deleteBoard() {
		const res = await fetch(`${PUBLIC_API_BASE_URL}/api/v1/board/${id}`, {
			method: 'delete'
		});

		if (!res.ok) {
			toast.error('Failed to delete board.');
			return;
		}

		toast.success('Board successfully deleted.');
		appState.boards = appState.boards.filter((board) => board.id !== id);
		isDeleteDialogOpen = false;
	}

	function openDeleteDialog(selectedId: string) {
		id = selectedId;
		isDeleteDialogOpen = true;
	}

	async function togglePin(board: Board) {
		board.isPinned = !board.isPinned;

		try {
			await fetch(`${PUBLIC_API_BASE_URL}/api/v1/board/${board.id}`, {
				method: 'PATCH',
				body: JSON.stringify({ isPinned: board.isPinned }),
				headers: { 'Content-Type': 'application/json' }
			});

			toast.success(`Board ${board.isPinned ? 'pinned' : 'unpinned'} successfully.`);
		} catch (error) {
			toast.error('Failed to update board pin status.');
			board.isPinned = !board.isPinned;
		}
	}
</script>

<Sidebar.Group class="group-data-[collapsible=icon]:hidden">
	<Sidebar.GroupLabel>Boards</Sidebar.GroupLabel>
	<Sidebar.Menu>
		{#each boards as item (item.id)}
			<Sidebar.MenuItem>
				<Sidebar.MenuButton>
					{#snippet child({ props })}
						<a href={`/boards/${item.id}`} {...props}>
							{#if item.isPinned}
								<PinIcon />
							{:else}
								<FrameIcon />
							{/if}
							<span>{item.title}</span>
						</a>
					{/snippet}
				</Sidebar.MenuButton>
				<DropdownMenu.Root>
					<DropdownMenu.Trigger>
						{#snippet child({ props })}
							<Sidebar.MenuAction showOnHover {...props}>
								<EllipsisIcon />
								<span class="sr-only">More</span>
							</Sidebar.MenuAction>
						{/snippet}
					</DropdownMenu.Trigger>
					<DropdownMenu.Content
						class="w-48"
						side={sidebar.isMobile ? 'bottom' : 'right'}
						align={sidebar.isMobile ? 'end' : 'start'}
					>
						<DropdownMenu.Item onclick={togglePin.bind(null, item)}>
							{#if item.isPinned}
								<PinOffIcon /> Unpin
							{:else}
								<PinIcon /> Pin
							{/if}
						</DropdownMenu.Item>
						<DropdownMenu.Item>
							<a href={`/boards/${item.id}`} class="flex items-center gap-2">
								<FolderIcon class="text-muted-foreground" />
								<span>View Board</span>
							</a>
						</DropdownMenu.Item>
						<DropdownMenu.Item>
							<ShareIcon class="text-muted-foreground" />
							<span>Share Board</span>
						</DropdownMenu.Item>
						<DropdownMenu.Separator />
						<DropdownMenu.Item variant="destructive" onclick={() => openDeleteDialog(item.id)}>
							<Trash2Icon class="text-muted-foreground" />
							<span>Delete Board</span>
						</DropdownMenu.Item>
					</DropdownMenu.Content>
				</DropdownMenu.Root>
			</Sidebar.MenuItem>
		{/each}
		<Sidebar.MenuItem>
			<Sidebar.MenuButton onclick={() => goto('/boards')}>
				<EllipsisIcon />
				<span>More</span>
			</Sidebar.MenuButton>
		</Sidebar.MenuItem>
	</Sidebar.Menu>
</Sidebar.Group>

<AlertDialog.Root bind:open={isDeleteDialogOpen}>
	<AlertDialog.Content>
		<AlertDialog.Header>
			<AlertDialog.Title>Are you absolutely sure?</AlertDialog.Title>
			<AlertDialog.Description>
				This action cannot be undone. This will permanently delete your board and the data from the
				server.
			</AlertDialog.Description>
		</AlertDialog.Header>
		<AlertDialog.Footer>
			<AlertDialog.Cancel>Cancel</AlertDialog.Cancel>
			<AlertDialog.Action onclick={deleteBoard}>Delete</AlertDialog.Action>
		</AlertDialog.Footer>
	</AlertDialog.Content>
</AlertDialog.Root>
