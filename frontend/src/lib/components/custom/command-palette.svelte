<script lang="ts">
	import {
		BanIcon,
		FolderIcon,
		HouseIcon,
		ImageIcon,
		RecycleIcon,
		StarIcon,
		TimelineIcon,
		Undo2Icon
	} from '@lucide/svelte';
	import SettingsIcon from '@lucide/svelte/icons/settings';
	import UserIcon from '@lucide/svelte/icons/user';

	import { goto } from '$app/navigation';
	import * as Command from '$lib/components/ui/command/index.js';
	import { appState } from '$lib/states.svelte';

	type Props = {
		isOpen: boolean;
	};

	let { isOpen = $bindable(false) }: Props = $props();

	function gotoPage(path: string) {
		goto(path);
		isOpen = false;
	}

	function onUploadImage() {
		appState.openState.isUploadImageDialogOpen = true;
		isOpen = false;
	}

	function onCreateBoard() {
		appState.openState.isCreateBoardDialogOpen = true;
		isOpen = false;
	}

	function goBack() {
		history.back();
		isOpen = false;
	}
</script>

<Command.Dialog class="rounded-lg border shadow-md md:min-w-112.5" bind:open={isOpen}>
	<Command.Input placeholder="Type a command or search..." />
	<Command.List>
		<Command.Empty>No results found.</Command.Empty>
		<Command.Group heading="Suggestions">
			<Command.Item onSelect={onUploadImage}>
				<ImageIcon />
				<span>Upload Image</span>
			</Command.Item>
			<Command.Item onSelect={onCreateBoard}>
				<FolderIcon />
				<span>Create Board</span>
			</Command.Item>
			<Command.Item onSelect={goBack}>
				<Undo2Icon />
				<span>Go Back</span>
			</Command.Item>
		</Command.Group>
		<Command.Separator />
		<Command.Group heading="Pages">
			<Command.Item onSelect={() => gotoPage('/')}>
				<HouseIcon />
				<span>Home</span>
			</Command.Item>
			<Command.Item onSelect={() => gotoPage('/boards')}>
				<FolderIcon />
				<span>Boards</span>
			</Command.Item>
			<Command.Item onSelect={() => gotoPage('/recycle')}>
				<RecycleIcon />
				<span>Recycle Bin</span>
			</Command.Item>
			<Command.Item onSelect={() => gotoPage('/timeline')}>
				<TimelineIcon />
				<span>Timeline</span>
			</Command.Item>
			<Command.Item onSelect={() => gotoPage('/favorites')}>
				<StarIcon />
				<span>Favorites</span>
			</Command.Item>
			<Command.Item onSelect={() => gotoPage('/hidden')}>
				<BanIcon />
				<span>Hidden</span>
			</Command.Item>
		</Command.Group>

		<Command.Separator />
		<Command.Group heading="Settings">
			<Command.Item onSelect={() => gotoPage('/settings/profile')}>
				<UserIcon />
				<span>Profile</span>
				<Command.Shortcut>⌘P</Command.Shortcut>
			</Command.Item>
			<Command.Item onSelect={() => gotoPage('/settings')}>
				<SettingsIcon />
				<span>Settings</span>
				<Command.Shortcut>⌘B</Command.Shortcut>
			</Command.Item>
		</Command.Group>
	</Command.List>
</Command.Dialog>
