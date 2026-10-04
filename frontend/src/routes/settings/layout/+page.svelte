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
	import type { LayoutType } from '$lib/types';

	let isSaving = $state(false);
	let noOfColumnsInput = $derived(appState.settings.noOfColumns || 5);
	let layoutTypeInput = $derived<LayoutType>(appState.settings.layoutType ?? 'masonry');

	const layoutTypes: { value: LayoutType; label: string }[] = [
		{ value: 'masonry', label: 'Masonry' },
		{ value: 'grid', label: 'Grid' }
	];

	const triggerLayoutTypeContent = $derived(
		layoutTypes.find((f) => f.value === layoutTypeInput)?.label ?? 'Select layout'
	);

	onMount(() => {
		appState.headerTitle = 'Layout Settings';
	});

	async function saveSettings() {
		isSaving = true;

		appState.settings.noOfColumns = noOfColumnsInput;
		appState.settings.layoutType = layoutTypeInput;

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

		<Button size="sm" variant="outline" disabled={isSaving} onclick={saveSettings}>
			{#if isSaving}
				<Spinner />
			{/if}
			{isSaving ? 'Submitting...' : 'Submit'}
		</Button>
	</div>
</section>
