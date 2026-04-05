<script lang="ts">
	import * as InputGroup from '$lib/components/ui/input-group/index.js';
	import * as Label from '$lib/components/ui/label/index.js';
	import * as Tooltip from '$lib/components/ui/tooltip/index.js';
	import InfoIcon from '@lucide/svelte/icons/info';
	import * as Avatar from '$lib/components/ui/avatar/index.js';
	import { onMount } from 'svelte';
	import { Button } from '$lib/components/ui/button/index.js';
	import { Spinner } from '$lib/components/ui/spinner/index.js';
	import { toast } from 'svelte-sonner';
	import { appState } from '$lib/states.svelte';
	import { Switch } from '$lib/components/ui/switch/index.js';

	let settings = $state({});

	let isEnablePassword = $state(false);

	onMount(async () => {
		appState.headerTitle = 'Security Settings';
	});

	let isSaving = $state(false);
</script>

<h1 class="text-2xl font-bold">Security Settings</h1>

<p>Manage your security preferences and settings for using this application.</p>

<section class="mt-6">
	<div class="grid w-full max-w-sm gap-4">
		<div class="flex items-center gap-3">
			<Switch id="is-censored" bind:checked={isEnablePassword} />
			<Label.Root for="is-censored">Enable Password On Login</Label.Root>
		</div>

		<InputGroup.Root>
			<InputGroup.Input id="username" placeholder="e.g john_doe" />
			<InputGroup.Addon align="block-start">
				<Label.Root for="username" class="text-foreground">Username</Label.Root>
			</InputGroup.Addon>
		</InputGroup.Root>
		<InputGroup.Root>
			<InputGroup.Input id="email" placeholder="doe@email.com" />
			<InputGroup.Addon align="block-start">
				<Label.Root for="email" class="text-foreground">Email</Label.Root>
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
						<p>We'll use this to send you notifications</p>
					</Tooltip.Content>
				</Tooltip.Root>
			</InputGroup.Addon>
		</InputGroup.Root>
		<InputGroup.Root>
			<InputGroup.Input id="password" placeholder="••••••••" type="password" />
			<InputGroup.Addon align="block-start">
				<Label.Root for="password" class="text-foreground">Password</Label.Root>
			</InputGroup.Addon>
		</InputGroup.Root>

		<div class="flex items-center gap-3">
			<Switch id="is-censored" bind:checked={isEnablePassword} />
			<Label.Root for="is-censored">Enable Multi-Factor Authentication</Label.Root>
		</div>

		<Button size="sm" variant="outline" disabled={isSaving}>Revert Changes</Button>

		<Button size="sm" disabled={isSaving}>
			{#if isSaving}
				<Spinner />
			{/if}
			{isSaving ? 'Submitting...' : 'Submit'}
		</Button>
	</div>
</section>
