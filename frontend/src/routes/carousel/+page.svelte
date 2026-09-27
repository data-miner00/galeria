<script lang="ts">
	import {
		CalendarDaysIcon,
		ChevronLeftIcon,
		ChevronRightIcon,
		ImagesIcon,
		Maximize2Icon,
		Minimize2Icon,
		PauseIcon,
		PlayIcon,
		StarIcon
	} from '@lucide/svelte';
	import { onDestroy, onMount, tick } from 'svelte';
	import { cubicOut } from 'svelte/easing';
	import { fade, fly } from 'svelte/transition';

	import CardActionsButton from '$lib/components/custom/card-actions-button.svelte';
	import { Button } from '$lib/components/ui/button/index.js';
	import * as Empty from '$lib/components/ui/empty/index.js';
	import { Skeleton } from '$lib/components/ui/skeleton/index.js';
	import { B } from '$lib/helpers';
	import { toast } from '$lib/notify';
	import { patchImage } from '$lib/services/imageService';
	import { appState } from '$lib/states.svelte';
	import { cn } from '$lib/utils.js';

	const AUTOPLAY_INTERVAL_MS = 5000;
	const SWIPE_THRESHOLD_PX = 50;

	onMount(() => {
		appState.headerTitle = 'Carousel';
	});

	let isLoading = $derived(appState.isLoading);
	let images = $derived(appState.images.filter((image) => !image.isSoftDeleted && !image.isHidden));

	let selectedIndex = $state(0);
	let direction = $state<1 | -1>(1);
	let currentImage = $derived(images[selectedIndex]);

	let isPlaying = $state(false);
	let autoplayTimer: ReturnType<typeof setInterval> | null = null;

	let isFullscreen = $state(false);
	let stageRef = $state<HTMLDivElement | null>(null);
	let filmstripRef = $state<HTMLDivElement | null>(null);

	let pointerStartX: number | null = null;

	let dateTime = $derived(formatDateTime(currentImage?.takenAt ?? currentImage?.createdAt));

	function goTo(index: number) {
		const count = images.length;
		if (count === 0) return;
		const next = ((index % count) + count) % count;
		if (next === selectedIndex) return;
		direction = index > selectedIndex ? 1 : -1;
		selectedIndex = next;
	}

	function next() {
		goTo(selectedIndex + 1);
	}

	function prev() {
		goTo(selectedIndex - 1);
	}

	function startAutoplay() {
		if (autoplayTimer || images.length < 2) return;
		isPlaying = true;
		autoplayTimer = setInterval(next, AUTOPLAY_INTERVAL_MS);
	}

	function stopAutoplay() {
		isPlaying = false;
		if (autoplayTimer) {
			clearInterval(autoplayTimer);
			autoplayTimer = null;
		}
	}

	function toggleAutoplay() {
		if (isPlaying) {
			stopAutoplay();
		} else {
			startAutoplay();
		}
	}

	function navigate(action: () => void) {
		stopAutoplay();
		action();
	}

	async function toggleFullscreen() {
		if (!stageRef) return;
		try {
			if (!document.fullscreenElement) {
				await stageRef.requestFullscreen();
			} else {
				await document.exitFullscreen();
			}
		} catch {
			toast.error('Fullscreen is not available.');
		}
	}

	function onFullscreenChange() {
		isFullscreen = document.fullscreenElement === stageRef;
	}

	async function toggleFavorite() {
		if (!currentImage) return;
		const image = currentImage;
		const nextValue = !image.isFavorite;

		try {
			await patchImage(image.id, { isFavorite: nextValue });
			appState.images = appState.images.map((img) =>
				img.id === image.id ? { ...img, isFavorite: nextValue } : img
			);
			toast.success(
				nextValue ? 'Successfully added to favorites.' : 'Successfully removed from favorites.'
			);
		} catch {
			toast.error('An error has occurred.');
		}
	}

	function handleDeleted(id: string) {
		appState.images = appState.images.filter((image) => image.id !== id);
	}

	function isTypingTarget(target: EventTarget | null) {
		if (!(target instanceof HTMLElement)) return false;
		return (
			target.isContentEditable ||
			target.tagName === 'INPUT' ||
			target.tagName === 'TEXTAREA' ||
			target.tagName === 'SELECT'
		);
	}

	function handleKeydown(event: KeyboardEvent) {
		if (images.length === 0 || event.defaultPrevented || isTypingTarget(event.target)) return;
		if (event.ctrlKey || event.metaKey || event.altKey) return;

		switch (event.key) {
			case 'ArrowLeft':
				event.preventDefault();
				navigate(prev);
				break;
			case 'ArrowRight':
				event.preventDefault();
				navigate(next);
				break;
			case 'Home':
				event.preventDefault();
				navigate(() => goTo(0));
				break;
			case 'End':
				event.preventDefault();
				navigate(() => goTo(images.length - 1));
				break;
			case ' ':
				event.preventDefault();
				toggleAutoplay();
				break;
			case 'f':
			case 'F':
				event.preventDefault();
				toggleFullscreen();
				break;
		}
	}

	function handlePointerDown(event: PointerEvent) {
		if (event.pointerType === 'mouse') return;
		pointerStartX = event.clientX;
	}

	function handlePointerUp(event: PointerEvent) {
		if (pointerStartX === null) return;
		const deltaX = event.clientX - pointerStartX;
		pointerStartX = null;
		if (Math.abs(deltaX) < SWIPE_THRESHOLD_PX) return;
		navigate(deltaX < 0 ? next : prev);
	}

	function formatDateTime(value?: string) {
		if (!value) return null;
		const date = new Date(value);
		if (Number.isNaN(date.getTime())) return null;
		return {
			iso: date.toISOString(),
			date: date.toLocaleDateString(undefined, {
				weekday: 'short',
				year: 'numeric',
				month: 'long',
				day: 'numeric'
			}),
			time: date.toLocaleTimeString(undefined, { hour: '2-digit', minute: '2-digit' })
		};
	}

	$effect(() => {
		if (selectedIndex > images.length - 1) {
			selectedIndex = Math.max(0, images.length - 1);
		}
		if (images.length < 2) stopAutoplay();
	});

	// Warm the cache for the neighbours so arrowing through feels instant.
	$effect(() => {
		const count = images.length;
		if (count < 2) return;
		for (const offset of [1, -1]) {
			const neighbour = images[(selectedIndex + offset + count) % count];
			const preload = new Image();
			preload.src = B(neighbour.mediumPath);
		}
	});

	$effect(() => {
		const activeIndex = selectedIndex;
		if (!filmstripRef) return;
		tick().then(() => {
			const activeThumb = filmstripRef?.querySelector<HTMLElement>(`[data-index="${activeIndex}"]`);
			activeThumb?.scrollIntoView({ behavior: 'smooth', inline: 'center', block: 'nearest' });
		});
	});

	onDestroy(stopAutoplay);
</script>

<svelte:document onfullscreenchange={onFullscreenChange} />
<svelte:window onkeydown={handleKeydown} />

<div
	class="flex h-[calc(100svh-5rem)] min-h-[28rem] w-full min-w-0 flex-col gap-4 contain-inline-size"
>
	{#if isLoading}
		<Skeleton class="h-7 w-24" />
		<Skeleton class="flex-1 rounded-2xl" />
		<div class="flex flex-col gap-2">
			<Skeleton class="h-6 w-64" />
			<Skeleton class="h-4 w-48" />
		</div>
		<div class="flex gap-2">
			{#each Array(10)}
				<Skeleton class="h-14 w-14 shrink-0 rounded-lg" />
			{/each}
		</div>
	{:else if images.length === 0}
		<Empty.Root class="flex-1">
			<Empty.Header>
				<Empty.Media variant="icon">
					<ImagesIcon />
				</Empty.Media>
				<Empty.Title>No Images to Show</Empty.Title>
				<Empty.Description
					>Upload some images to start browsing them in the carousel.</Empty.Description
				>
			</Empty.Header>
		</Empty.Root>
	{:else if currentImage}
		<!-- Index + controls -->
		<div class="flex items-center justify-between gap-4">
			<div class="flex min-w-0 items-baseline gap-1.5 tabular-nums" aria-live="polite">
				<span class="text-2xl font-semibold tracking-tight">{selectedIndex + 1}</span>
				<span class="text-sm text-muted-foreground">/ {images.length}</span>
			</div>

			<div class="flex items-center gap-1">
				<Button
					variant="ghost"
					size="icon"
					onclick={toggleFavorite}
					aria-label={currentImage.isFavorite ? 'Remove from favorites' : 'Add to favorites'}
				>
					<StarIcon class={cn(currentImage.isFavorite && 'fill-current text-yellow-500')} />
				</Button>
				<Button
					variant="ghost"
					size="icon"
					onclick={toggleAutoplay}
					disabled={images.length < 2}
					aria-label={isPlaying ? 'Pause slideshow' : 'Play slideshow'}
				>
					{#if isPlaying}
						<PauseIcon />
					{:else}
						<PlayIcon />
					{/if}
				</Button>
				<Button
					variant="ghost"
					size="icon"
					onclick={toggleFullscreen}
					aria-label={isFullscreen ? 'Exit fullscreen' : 'Enter fullscreen'}
				>
					{#if isFullscreen}
						<Minimize2Icon />
					{:else}
						<Maximize2Icon />
					{/if}
				</Button>
				<CardActionsButton
					id={currentImage.id}
					path={currentImage.path}
					thumbnailPath={currentImage.thumbnailPath}
					mediumPath={currentImage.mediumPath}
					isFavorite={currentImage.isFavorite}
					isSoftDeleted={currentImage.isSoftDeleted}
					isCensored={currentImage.isCensored}
					isHidden={currentImage.isHidden}
					onDelete={() => handleDeleted(currentImage.id)}
				/>
			</div>
		</div>

		<!-- Stage -->
		<div
			bind:this={stageRef}
			role="region"
			aria-roledescription="carousel"
			aria-label="Image carousel"
			class={cn(
				'group relative isolate min-h-0 flex-1 touch-pan-y overflow-hidden select-none',
				isFullscreen ? 'bg-black' : 'rounded-2xl border bg-muted/40'
			)}
			onpointerdown={handlePointerDown}
			onpointerup={handlePointerUp}
			onpointercancel={() => (pointerStartX = null)}
		>
			<!-- Ambient backdrop tinted by the current image -->
			<div class="pointer-events-none absolute inset-0 -z-10 overflow-hidden" aria-hidden="true">
				{#key currentImage.id}
					<img
						src={B(currentImage.thumbnailPath)}
						alt=""
						transition:fade={{ duration: 600 }}
						class="absolute inset-0 h-full w-full scale-125 object-cover opacity-50 blur-3xl saturate-150 dark:opacity-35"
					/>
				{/key}
				<div
					class="absolute inset-0 bg-gradient-to-b from-transparent via-transparent to-background/40"
				></div>
			</div>

			<div
				class="absolute inset-0 grid grid-cols-[minmax(0,1fr)] grid-rows-[minmax(0,1fr)] place-items-center p-4 sm:p-8"
			>
				{#key currentImage.id}
					<img
						src={B(currentImage.mediumPath)}
						alt={currentImage.title || currentImage.description || currentImage.originalFileName}
						decoding="async"
						draggable="false"
						in:fly={{ x: 48 * direction, duration: 450, easing: cubicOut, opacity: 0 }}
						out:fade={{ duration: 200 }}
						class={cn(
							'max-h-full max-w-full object-contain [grid-area:1/1]',
							!isFullscreen && 'rounded-lg shadow-2xl ring-1 shadow-black/30 ring-black/10'
						)}
					/>
				{/key}
			</div>

			{#if images.length > 1}
				<button
					type="button"
					onclick={() => navigate(prev)}
					aria-label="Previous image"
					class="absolute inset-y-0 left-0 flex w-1/5 max-w-40 cursor-w-resize items-center justify-start pl-3 outline-none"
				>
					<span
						class="grid size-10 place-items-center rounded-full border border-white/10 bg-background/60 text-foreground opacity-0 shadow-lg backdrop-blur-md transition group-focus-within:opacity-100 group-hover:opacity-100 hover:bg-background/90 focus-visible:opacity-100"
					>
						<ChevronLeftIcon class="size-5" />
					</span>
				</button>
				<button
					type="button"
					onclick={() => navigate(next)}
					aria-label="Next image"
					class="absolute inset-y-0 right-0 flex w-1/5 max-w-40 cursor-e-resize items-center justify-end pr-3 outline-none"
				>
					<span
						class="grid size-10 place-items-center rounded-full border border-white/10 bg-background/60 text-foreground opacity-0 shadow-lg backdrop-blur-md transition group-focus-within:opacity-100 group-hover:opacity-100 hover:bg-background/90"
					>
						<ChevronRightIcon class="size-5" />
					</span>
				</button>
			{/if}

			<!-- Autoplay progress -->
			{#if isPlaying}
				{#key selectedIndex}
					<div class="absolute inset-x-0 bottom-0 h-0.5 bg-foreground/10">
						<div
							class="autoplay-progress h-full origin-left bg-foreground/70"
							style:animation-duration="{AUTOPLAY_INTERVAL_MS}ms"
						></div>
					</div>
				{/key}
			{/if}
		</div>

		<!-- Title + date time -->
		<div class="flex min-w-0 flex-col gap-1">
			{#key currentImage.id}
				<h2
					in:fade={{ duration: 250 }}
					class="truncate text-lg leading-tight font-semibold tracking-tight"
					title={currentImage.title || currentImage.originalFileName}
				>
					{currentImage.title || currentImage.originalFileName}
				</h2>
			{/key}
			{#if dateTime}
				<time
					datetime={dateTime.iso}
					class="flex items-center gap-1.5 text-sm text-muted-foreground tabular-nums"
				>
					<CalendarDaysIcon class="size-3.5" />
					<span>{dateTime.date}</span>
					<span aria-hidden="true" class="text-muted-foreground/50">·</span>
					<span>{dateTime.time}</span>
				</time>
			{/if}
		</div>

		<!-- Filmstrip -->
		{#if images.length > 1}
			<div
				bind:this={filmstripRef}
				class="filmstrip flex shrink-0 items-center gap-2 overflow-x-auto px-8 py-2"
			>
				{#each images as image, i (image.id)}
					<button
						type="button"
						data-index={i}
						onclick={() => navigate(() => goTo(i))}
						aria-label={`Go to image ${i + 1}`}
						aria-current={i === selectedIndex}
						class={cn(
							'relative shrink-0 overflow-hidden rounded-lg transition-all duration-300 ease-out outline-none focus-visible:ring-2 focus-visible:ring-ring',
							i === selectedIndex
								? 'size-16 opacity-100 ring-2 ring-foreground ring-offset-2 ring-offset-background'
								: 'size-12 opacity-45 grayscale-[40%] hover:opacity-90 hover:grayscale-0'
						)}
					>
						<img
							src={B(image.thumbnailPath)}
							alt=""
							loading="lazy"
							decoding="async"
							draggable="false"
							class="h-full w-full object-cover"
						/>
					</button>
				{/each}
			</div>
		{/if}
	{/if}
</div>

<style>
	.autoplay-progress {
		animation-name: autoplay-progress;
		animation-timing-function: linear;
		animation-fill-mode: forwards;
	}

	@keyframes autoplay-progress {
		from {
			transform: scaleX(0);
		}
		to {
			transform: scaleX(1);
		}
	}

	.filmstrip {
		scrollbar-width: none;
		mask-image: linear-gradient(
			to right,
			transparent,
			black 2rem,
			black calc(100% - 2rem),
			transparent
		);
	}

	.filmstrip::-webkit-scrollbar {
		display: none;
	}
</style>
