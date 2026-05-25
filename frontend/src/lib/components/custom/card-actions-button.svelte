<script lang="ts">
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu/index.js';
	import { Button } from '$lib/components/ui/button/index.js';
	import {
		CircleMinus,
		Download,
		Ellipsis,
		ExternalLink,
		Eye,
		EyeOff,
		GitForkIcon,
		ImageIcon,
		ImageOffIcon,
		Info,
		Plus,
		RecycleIcon,
		Star,
		Trash2
	} from '@lucide/svelte';
	import { toast } from 'svelte-sonner';
	import * as AlertDialog from '$lib/components/ui/alert-dialog/index.js';
	import AddToBoardDialog from './add-to-board-dialog.svelte';
	import { appState } from '$lib/states.svelte';
	import { page } from '$app/state';
	import { PUBLIC_API_BASE_URL } from '$env/static/public';

	let isDeleteDialogOpen = $state(false);

	type Props = {
		id: string;
		onDelete: () => void;
		path: string;
		thumbnailPath: string;
		mediumPath: string;
		isFavorite: boolean;
		isSoftDeleted: boolean;
		isCensored: boolean;
		isHidden: boolean;
	};

	const {
		id,
		path,
		thumbnailPath,
		mediumPath,
		onDelete,
		isFavorite,
		isSoftDeleted,
		isCensored,
		isHidden
	}: Props = $props();

	import { B } from '$lib/helpers';
	import type { ImageRecord } from '$lib/types';
	let isAddToBoardDialogOpen = $state(false);

	async function removeImage() {
		try {
			const response = await fetch(`${PUBLIC_API_BASE_URL}/api/v1/image/${id}`, {
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

	async function removeImageFromBoard() {
		try {
			const response = await fetch(`${PUBLIC_API_BASE_URL}/api/v1/board/${page.params.id}/${id}`, {
				method: 'delete'
			});

			if (!response.ok) {
				throw new Error('Something wrong');
			}

			appState.boards = appState.boards.map((board) =>
				board.id === page.params.id
					? { ...board, imageIds: board.imageIds.filter((imageId) => imageId !== id) }
					: board
			);

			onDelete();

			toast.success('Successfully removed image from board.');
		} catch {
			toast.error('An error has occurred.');
		}
	}

	async function downloadImage(imageSrc: string, nameOfDownload = 'my-image.jpeg') {
		try {
			// 1. Fetch the image data
			const response = await fetch(imageSrc);
			const blobImage = await response.blob();

			// 2. Create a temporary URL for the blob
			const href = URL.createObjectURL(blobImage);

			// 3. Create a temporary anchor element
			const anchorElement = document.createElement('a');
			anchorElement.href = href;
			anchorElement.download = nameOfDownload; // Set the desired file name

			// 4. Append anchor to body, click it to initiate download, and remove
			document.body.appendChild(anchorElement);
			anchorElement.click();
			document.body.removeChild(anchorElement);

			// 5. Revoke the temporary URL to free up memory
			window.URL.revokeObjectURL(href);
			toast.success('The image has been downloaded');
		} catch (error) {
			toast.error('Error downloading image.');
		}
	}

	async function setAsProfilePicture(imageSrc: string) {
		try {
			const response = await fetch(`${PUBLIC_API_BASE_URL}/api/v1/UserProfile`, {
				method: 'PATCH',
				headers: {
					'Content-Type': 'application/json'
				},
				body: JSON.stringify({ avatarImage: imageSrc })
			});

			appState.profile.avatarImage = imageSrc;

			if (!response.ok) {
				throw new Error('Something wrong');
			}
			toast.success('Successfully set profile picture.');
		} catch {
			toast.error('An error has occurred.');
		}
	}

	function openOriginal() {
		window.open(B(path), '_blank');
	}

	function openMedium() {
		window.open(B(mediumPath), '_blank');
	}

	function openThumbnail() {
		window.open(B(thumbnailPath), '_blank');
	}

	function toggleFavorite() {
		return patchImageProperty(
			'isFavorite',
			!isFavorite,
			'Successfully added to favorites.',
			'Successfully removed from favorites.'
		);
	}

	function toggleSoftDeleted() {
		return patchImageProperty(
			'isSoftDeleted',
			!isSoftDeleted,
			'Successfully moved to trash.',
			'Successfully restored image.'
		);
	}

	function toggleCensored() {
		return patchImageProperty(
			'isCensored',
			!isCensored,
			'Successfully censored image.',
			'Successfully uncensored image.'
		);
	}

	function toggleHidden() {
		return patchImageProperty(
			'isHidden',
			!isHidden,
			'Successfully hide image.',
			'Successfully unhide image.'
		);
	}

	async function patchImageProperty<K extends keyof ImageRecord>(
		property: K,
		value: ImageRecord[K],
		toastMessage: string = 'Successfully updated image.',
		toastMessageOpposite: string = 'Successfully updated image.'
	) {
		try {
			const response = await fetch(`${PUBLIC_API_BASE_URL}/api/v1/image/${id}`, {
				method: 'PATCH',
				headers: {
					'Content-Type': 'application/json'
				},
				body: JSON.stringify({ [property]: value })
			});

			if (!response.ok) {
				throw new Error('Something wrong');
			}

			const image = appState.images.find((image) => image.id === id);
			if (!image) return;
			image[property] = value;

			toast.success(value ? toastMessage : toastMessageOpposite);
		} catch {
			toast.error('An error has occurred.');
		}
	}

	function toggleCensoredForThisSession() {
		appState.images = appState.images.map((image) =>
			image.id === id ? { ...image, isCensored: !image.isCensored } : image
		);
	}

	function onInfoClick() {
		appState.infoSheetData.id = id;
		appState.infoSheetData.isOpen = true;
	}
</script>

<AddToBoardDialog imageId={id} bind:isDialogOpen={isAddToBoardDialogOpen} />

<DropdownMenu.Root>
	<DropdownMenu.Trigger>
		{#snippet child({ props })}
			<Button variant="ghost" size="icon" {...props}>
				<Ellipsis />
			</Button>
		{/snippet}
	</DropdownMenu.Trigger>
	<DropdownMenu.Content class="w-56" align="start">
		<DropdownMenu.Item onclick={onInfoClick}>
			<Info /> Details
		</DropdownMenu.Item>
		<DropdownMenu.Sub>
			<DropdownMenu.SubTrigger>
				<ExternalLink /> Open in new tab
			</DropdownMenu.SubTrigger>
			<DropdownMenu.SubContent>
				<DropdownMenu.Item onclick={openOriginal}>Original</DropdownMenu.Item>
				<DropdownMenu.Item onclick={openMedium}>Medium</DropdownMenu.Item>
				<DropdownMenu.Item onclick={openThumbnail}>Thumbnail</DropdownMenu.Item>
			</DropdownMenu.SubContent>
		</DropdownMenu.Sub>
		<DropdownMenu.Item onclick={toggleFavorite}>
			{#if !isFavorite}
				<Star /> Add to Favorite
			{:else}
				<Star fill="currentColor" /> Remove from Favorite
			{/if}
		</DropdownMenu.Item>
		<DropdownMenu.Item><GitForkIcon /> Fork this image</DropdownMenu.Item>
		<DropdownMenu.Item onclick={() => (isAddToBoardDialogOpen = !isAddToBoardDialogOpen)}>
			<Plus /> Add to Board
		</DropdownMenu.Item>
		{#if page.params.id}
			<DropdownMenu.Item onclick={removeImageFromBoard}>
				<CircleMinus /> Remove from this Board
			</DropdownMenu.Item>
		{/if}
		<DropdownMenu.Item onclick={() => downloadImage(B(path), path)}>
			<Download /> Download
		</DropdownMenu.Item>
		<DropdownMenu.Item onclick={() => setAsProfilePicture(B(thumbnailPath))}>
			<ExternalLink /> Set as profile picture
		</DropdownMenu.Item>
		<DropdownMenu.Separator />
		<DropdownMenu.Item onclick={toggleCensoredForThisSession}>
			{#if isCensored}
				<Eye /> Reveal Image temporarily
			{:else}
				<EyeOff /> Blur Image temporarily
			{/if}
		</DropdownMenu.Item>
		<DropdownMenu.Item onclick={toggleCensored}>
			{#if isCensored}
				<Eye /> Reveal Image
			{:else}
				<EyeOff /> Blur Image
			{/if}
		</DropdownMenu.Item>
		<DropdownMenu.Item onclick={toggleHidden}>
			{#if isHidden}
				<ImageIcon /> Unhide Image
			{:else}
				<ImageOffIcon /> Hide Image
			{/if}
		</DropdownMenu.Item>
		<DropdownMenu.Separator />
		<DropdownMenu.Item onclick={toggleSoftDeleted}>
			<RecycleIcon />
			{#if !isSoftDeleted}Move to Trash{:else}Restore from Trash{/if}
		</DropdownMenu.Item>
		<DropdownMenu.Item
			variant="destructive"
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
