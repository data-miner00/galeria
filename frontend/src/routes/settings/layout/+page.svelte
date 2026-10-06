<script lang="ts">
	import InfoIcon from '@lucide/svelte/icons/info';
	import { onMount } from 'svelte';

	import { PUBLIC_API_BASE_URL } from '$env/static/public';
	import { Button } from '$lib/components/ui/button/index.js';
	import * as InputGroup from '$lib/components/ui/input-group/index.js';
	import * as Label from '$lib/components/ui/label/index.js';
	import * as Select from '$lib/components/ui/select/index.js';
	import { Spinner } from '$lib/components/ui/spinner/index.js';
	import * as Tooltip from '$lib/components/ui/tooltip/index.js';
	import { toast } from '$lib/notify';
	import { appState } from '$lib/states.svelte';
	import type { LayoutType, ThumbnailCornerRadius, ThumbnailGap } from '$lib/types';

	let isSaving = $state(false);
	let noOfColumnsInput = $derived(appState.settings.noOfColumns || 5);
	let layoutTypeInput = $derived<LayoutType>(appState.settings.layoutType ?? 'masonry');
	let thumbnailGapInput = $derived<ThumbnailGap>(appState.settings.thumbnailGap ?? 'comfortable');
	let thumbnailCornerRadiusInput = $derived<ThumbnailCornerRadius>(
		appState.settings.thumbnailCornerRadius ?? 'small'
	);

	const layoutTypes: { value: LayoutType; label: string }[] = [
		{ value: 'masonry', label: 'Masonry' },
		{ value: 'grid', label: 'Grid' }
	];

	const thumbnailGaps: { value: ThumbnailGap; label: string }[] = [
		{ value: 'compact', label: 'Compact' },
		{ value: 'comfortable', label: 'Comfortable' },
		{ value: 'spacious', label: 'Spacious' }
	];

	const thumbnailCornerRadii: { value: ThumbnailCornerRadius; label: string }[] = [
		{ value: 'none', label: 'None' },
		{ value: 'small', label: 'Small' },
		{ value: 'medium', label: 'Medium' },
		{ value: 'large', label: 'Large' }
	];

	const triggerLayoutTypeContent = $derived(
		layoutTypes.find((f) => f.value === layoutTypeInput)?.label ?? 'Select layout'
	);
	const triggerThumbnailGapContent = $derived(
		thumbnailGaps.find((f) => f.value === thumbnailGapInput)?.label ?? 'Select spacing'
	);
	const triggerThumbnailCornerRadiusContent = $derived(
		thumbnailCornerRadii.find((f) => f.value === thumbnailCornerRadiusInput)?.label ??
			'Select corner radius'
	);

	onMount(() => {
		appState.headerTitle = 'Layout Settings';
	});

	async function saveSettings() {
		isSaving = true;

		appState.settings.noOfColumns = noOfColumnsInput;
		appState.settings.layoutType = layoutTypeInput;
		appState.settings.thumbnailGap = thumbnailGapInput;
		appState.settings.thumbnailCornerRadius = thumbnailCornerRadiusInput;

		const request = await fetch(`${PUBLIC_API_BASE_URL}/api/v1/UserSettings`, {
			method: 'PATCH',
			headers: {
				'Content-Type': 'application/json'
			},
			body: JSON.stringify(appState.settings)
		});

		if (request.ok) {
			toast.success('Settings updated successfully!');
		} else {
			const error = await request.json();
			toast.error(`Failed to update settings: ${error.errorMessage}`);
		}

		isSaving = false;
	}
</script>

<h1 class="text-2xl font-bold">User Settings</h1>

<p>Manage your user preferences and settings for the layout representation.</p>

<section class="mt-6">
	<div class="grid w-full max-w-sm gap-4">
		<InputGroup.Root>
			<InputGroup.Input
				id="noOfColumns"
				type="number"
				placeholder="5"
				max="6"
				min="4"
				bind:value={noOfColumnsInput}
			/>
			<InputGroup.Addon align="block-start">
				<Label.Root for="noOfColumns" class="text-foreground">Number of Columns</Label.Root>
				<Tooltip.Root>
					<Tooltip.Trigger>
						{#snippet child({ props })}
							<InputGroup.Button
								{...props}
								variant="ghost"
								aria-label="Help"
								class="ms-auto rounded-full"
								size="icon-xs"
							>
								<InfoIcon />
							</InputGroup.Button>
						{/snippet}
					</Tooltip.Trigger>
					<Tooltip.Content>
						<p>We'll use this to display your content in the desired number of columns</p>
					</Tooltip.Content>
				</Tooltip.Root>
			</InputGroup.Addon>
		</InputGroup.Root>

		<div>
			<Label.Root for="layoutType" class="mb-3 text-foreground">Default Layout</Label.Root>
			<Select.Root type="single" name="layoutType" bind:value={layoutTypeInput}>
				<Select.Trigger id="layoutType" class="w-full">
					{triggerLayoutTypeContent}
				</Select.Trigger>
				<Select.Content>
					<Select.Group>
						{#each layoutTypes as layoutType (layoutType.value)}
							<Select.Item value={layoutType.value} label={layoutType.label}>
								{layoutType.label}
							</Select.Item>
						{/each}
					</Select.Group>
				</Select.Content>
			</Select.Root>
		</div>

		<div>
			<Label.Root for="thumbnailGap" class="mb-3 text-foreground">Thumbnail Spacing</Label.Root>
			<Select.Root type="single" name="thumbnailGap" bind:value={thumbnailGapInput}>
				<Select.Trigger id="thumbnailGap" class="w-full">
					{triggerThumbnailGapContent}
				</Select.Trigger>
				<Select.Content>
					<Select.Group>
						{#each thumbnailGaps as thumbnailGap (thumbnailGap.value)}
							<Select.Item value={thumbnailGap.value} label={thumbnailGap.label}>
								{thumbnailGap.label}
							</Select.Item>
						{/each}
					</Select.Group>
				</Select.Content>
			</Select.Root>
		</div>

		<div>
			<Label.Root for="thumbnailCornerRadius" class="mb-3 text-foreground">
				Thumbnail Corner Radius
			</Label.Root>
			<Select.Root
				type="single"
				name="thumbnailCornerRadius"
				bind:value={thumbnailCornerRadiusInput}
			>
				<Select.Trigger id="thumbnailCornerRadius" class="w-full">
					{triggerThumbnailCornerRadiusContent}
				</Select.Trigger>
				<Select.Content>
					<Select.Group>
						{#each thumbnailCornerRadii as cornerRadius (cornerRadius.value)}
							<Select.Item value={cornerRadius.value} label={cornerRadius.label}>
								{cornerRadius.label}
							</Select.Item>
						{/each}
					</Select.Group>
				</Select.Content>
			</Select.Root>
		</div>

		<Button size="sm" variant="outline" disabled={isSaving} onclick={saveSettings}>
			{#if isSaving}
				<Spinner />
			{/if}
			{isSaving ? 'Submitting...' : 'Submit'}
		</Button>
	</div>
</section>
