<script lang="ts">
	import { onDestroy, tick } from 'svelte';
	import CardActionsButton from './card-actions-button.svelte';
	import { EyeOffIcon } from '@lucide/svelte';
	import type { LayoutType } from '$lib/types';

	type Props = {
		id: string;
		path: string;
		thumbnailPath: string;
		mediumPath: string;
		description?: string;
		onDelete: () => void;
		isCensored: boolean;
		isFavorite: boolean;
		isSoftDeleted: boolean;
		layoutType?: LayoutType;
	};

	let revealCensoredImage = $state(false);

	const {
		id,
		path,
		description,
		onDelete,
		thumbnailPath,
		mediumPath,
		isCensored = false,
		isFavorite = false,
		isSoftDeleted = false,
		layoutType = 'masonry'
	}: Props = $props();

	let isLightboxOpen = $state(false);
	let closeButton: HTMLButtonElement | null = $state(null);
	let previousActiveElement: HTMLElement | null = $state(null);

	function openLightbox() {
		previousActiveElement = document.activeElement as HTMLElement | null;
		isLightboxOpen = true;
	}

	function closeLightbox() {
		isLightboxOpen = false;
		previousActiveElement?.focus();
	}

	function handleLightboxKeyDown(event: KeyboardEvent) {
		if (!isLightboxOpen) return;

		if (event.key === 'Escape') {
			event.preventDefault();
			closeLightbox();
		}
	}

	$effect(() => {
		if (isLightboxOpen) {
			// Move focus to the close button when the lightbox opens.
			tick().then(() => closeButton?.focus());
		}
	});

	onDestroy(() => {
		if (isLightboxOpen) {
			closeLightbox();
		}
	});
</script>

<div>
	<button
		class="relative w-full cursor-pointer overflow-hidden rounded focus-visible:ring-2 focus-visible:ring-primary focus-visible:outline-none"
		class:aspect-square={layoutType === 'grid'}
		type="button"
		onclick={!isCensored || revealCensoredImage ? openLightbox : () => (revealCensoredImage = true)}
		aria-label="Open image preview"
	>
		<img
			class="h-full w-full object-cover"
			alt={description ?? 'Gallery image'}
			src={`http://127.0.0.1:10003/devstoreaccount1/images/${path}`}
		/>

		{#if isCensored && !revealCensoredImage}
			<div class="absolute inset-0 flex items-center justify-center bg-black/30 backdrop-blur-xl">
				<span class="text-sm font-medium text-white">
					<EyeOffIcon class="me-1 inline-block" />
				</span>
			</div>
		{/if}
	</button>

	<div class="flex items-center justify-between">
		<div>
			<p class="max-w-40 truncate text-sm">{description}</p>
		</div>
		<CardActionsButton
			{id}
			{path}
			{thumbnailPath}
			{mediumPath}
			{onDelete}
			{isFavorite}
			{isSoftDeleted}
			{isCensored}
		/>
	</div>
</div>

{#if isLightboxOpen}
	<div
		class="fixed inset-0 z-50 flex items-center justify-center bg-black/80 p-4"
		role="dialog"
		aria-modal="true"
		aria-label="Image preview dialog"
		onclick={closeLightbox}
		onkeydown={handleLightboxKeyDown}
		tabindex="-1"
	>
		<!-- svelte-ignore a11y_click_events_have_key_events -->
		<!-- svelte-ignore a11y_no_static_element_interactions -->
		<div class="relative max-h-[90vh] max-w-[90vw]" onclick={(e) => e.stopPropagation()}>
			<button
				class="absolute top-2 right-2 z-20 rounded bg-white/90 px-3 py-1 text-sm font-medium text-slate-900 hover:bg-white"
				type="button"
				onclick={closeLightbox}
				aria-label="Close image preview"
				bind:this={closeButton}
			>
				Close
			</button>

			<img
				class="max-h-[85vh] max-w-[85vw] object-contain"
				alt={description ?? 'Gallery image preview'}
				src={`http://127.0.0.1:10003/devstoreaccount1/images/${path}`}
			/>
		</div>
	</div>
{/if}
