<script lang="ts">
	import * as InputGroup from '$lib/components/ui/input-group/index.js';
	import * as Label from '$lib/components/ui/label/index.js';
	import * as Tooltip from '$lib/components/ui/tooltip/index.js';
	import InfoIcon from '@lucide/svelte/icons/info';
	import { Button } from '$lib/components/ui/button/index.js';
	import { Spinner } from '$lib/components/ui/spinner/index.js';
	import { toast } from 'svelte-sonner';
	import { appState } from '$lib/states.svelte';
	import { onMount } from 'svelte';
	import { PUBLIC_API_BASE_URL } from '$env/static/public';

	let isSaving = $state(false);
	let noOfColumnsInput = $state(appState.settings.noOfColumns || 5);

	onMount(() => {
		appState.headerTitle = 'Layout Settings';
	});

	async function saveSettings() {
		isSaving = true;

		const request = await fetch(`${PUBLIC_API_BASE_URL}/api/v1/UserSettings`, {
			method: 'PATCH',
			headers: {
				'Content-Type': 'application/json'
			},
			body: JSON.stringify(appState.settings)
		});

		if (request.ok) {
			appState.settings.noOfColumns = noOfColumnsInput;
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

		<Button size="sm" variant="outline" disabled={isSaving} onclick={saveSettings}>
			{#if isSaving}
				<Spinner />
			{/if}
			{isSaving ? 'Submitting...' : 'Submit'}
		</Button>
	</div>
</section>
