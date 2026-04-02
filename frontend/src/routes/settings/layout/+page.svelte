<script lang="ts">
	import * as InputGroup from '$lib/components/ui/input-group/index.js';
	import * as Label from '$lib/components/ui/label/index.js';
	import * as Tooltip from '$lib/components/ui/tooltip/index.js';
	import InfoIcon from '@lucide/svelte/icons/info';
	import type { UserSettings } from '$lib/types';
	import { onMount } from 'svelte';
	import { Button } from '$lib/components/ui/button/index.js';
	import { Spinner } from '$lib/components/ui/spinner/index.js';
	import { toast } from 'svelte-sonner';

	let settings = $state<UserSettings>({ noOfColumns: 5 });

	onMount(async () => {
		const res = await fetch('https://localhost:7146/api/v1/UserSettings');
		settings = await res.json();
	});

	let isSaving = $state(false);

	async function saveSettings() {
		isSaving = true;

		const request = await fetch('https://localhost:7146/api/v1/UserSettings', {
			method: 'PATCH',
			headers: {
				'Content-Type': 'application/json'
			},
			body: JSON.stringify(settings)
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
				bind:value={settings.noOfColumns}
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
