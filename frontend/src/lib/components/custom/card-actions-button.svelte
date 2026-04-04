<script lang="ts">
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu/index.js';
	import { Button } from '$lib/components/ui/button/index.js';
	import {
		Download,
		Ellipsis,
		ExternalLink,
		GitForkIcon,
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

	let isDeleteDialogOpen = $state(false);

	type Props = {
		id: string;
		onDelete: () => void;
		path: string;
		thumbnailPath: string;
		mediumPath: string;
		isFavorite: boolean;
		isSoftDeleted: boolean;
	};

	const { id, path, thumbnailPath, mediumPath, onDelete, isFavorite, isSoftDeleted }: Props =
		$props();

	let isAddToBoardDialogOpen = $state(false);

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
			console.log('The image has been downloaded');
		} catch (error) {
			console.error('Error downloading image: ', error);
		}
	}

	async function setAsProfilePicture(imageSrc: string) {
		try {
			const response = await fetch('https://localhost:7146/api/v1/UserProfile', {
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
		window.open(`http://127.0.0.1:10003/devstoreaccount1/images/${path}`, '_blank');
	}

	function openMedium() {
		window.open(`http://127.0.0.1:10003/devstoreaccount1/images/${mediumPath}`, '_blank');
	}

	function openThumbnail() {
		window.open(`http://127.0.0.1:10003/devstoreaccount1/images/${thumbnailPath}`, '_blank');
	}

	async function toggleFavorite() {
		try {
			const response = await fetch(`https://localhost:7146/api/v1/image/${id}`, {
				method: 'PATCH',
				headers: {
					'Content-Type': 'application/json'
				},
				body: JSON.stringify({ isFavorite: !isFavorite })
			});

			if (!response.ok) {
				throw new Error('Something wrong');
			}

			appState.images = appState.images.map((image) =>
				image.id === id ? { ...image, isFavorite: !isFavorite } : image
			);
			toast.success(!isFavorite ? 'Added to favorites.' : 'Removed from favorites.');
		} catch {
			toast.error('An error has occurred.');
		}
	}

	async function toggleSoftDeleted() {
		try {
			const response = await fetch(`https://localhost:7146/api/v1/image/${id}`, {
				method: 'PATCH',
				headers: {
					'Content-Type': 'application/json'
				},
				body: JSON.stringify({ isSoftDeleted: !isSoftDeleted })
			});

			if (!response.ok) {
				throw new Error('Something wrong');
			}

			appState.images = appState.images.map((image) =>
				image.id === id ? { ...image, isSoftDeleted: !isSoftDeleted } : image
			);

			toast.success(!isSoftDeleted ? 'Image moved to trash.' : 'Image restored.');
		} catch {
			toast.error('An error has occurred.');
		}
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
		<DropdownMenu.Item><Info /> View</DropdownMenu.Item>
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
		<DropdownMenu.Item
			onclick={() => downloadImage(`http://127.0.0.1:10003/devstoreaccount1/images/${path}`, path)}
		>
			<Download /> Download
		</DropdownMenu.Item>
		<DropdownMenu.Item
			onclick={() =>
				setAsProfilePicture(`http://127.0.0.1:10003/devstoreaccount1/images/${thumbnailPath}`)}
		>
			<ExternalLink /> Set as profile picture
		</DropdownMenu.Item>
		<DropdownMenu.Separator />
		<DropdownMenu.Item onclick={toggleSoftDeleted}>
			<RecycleIcon /> Move to Trash
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
