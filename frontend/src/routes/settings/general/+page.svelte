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
	import { setMode, mode } from 'mode-watcher';

	let settings = $state({});

	let isEnableLoggings = $state(false);

	onMount(async () => {
		appState.headerTitle = 'Security Settings';
		value = mode.current;
	});

	let isSaving = $state(false);

	import * as Select from '$lib/components/ui/select/index.js';
	import Separator from '$lib/components/ui/separator/separator.svelte';
	import { availableLanguages } from '$lib/i18n/languages';
	import { locale } from '$lib/i18n/translations.svelte';

	const themes = [
		{ value: 'light', label: 'Light' },
		{ value: 'dark', label: 'Dark' },
		{ value: 'system', label: 'System' }
	];

	let value = $state<'light' | 'dark'>();

	const triggerContent = $derived(
		themes.find((f) => f.value === mode.current)?.label ?? 'Select theme'
	);
	const triggerLanguageContent = $derived(
		availableLanguages.find((f) => f.value === locale.current)?.label ?? 'Select language'
	);
</script>

<h1 class="text-2xl font-bold">Application Generals</h1>

<p>Explore application preferences and settings suited for your use case.</p>

<section class="mt-6">
	<div class="grid w-full max-w-sm gap-4">
		<div>
			<Label.Root for="theme" class="mb-3 text-foreground">Theme</Label.Root>
			<Select.Root
				type="single"
				name="theme"
				bind:value
				onValueChange={(value) => setMode(value as 'light' | 'dark' | 'system')}
			>
				<Select.Trigger class="w-full">
					{triggerContent}
				</Select.Trigger>
				<Select.Content>
					<Select.Group>
						{#each themes as theme (theme.value)}
							<Select.Item value={theme.value} label={theme.label}>
								{theme.label}
							</Select.Item>
						{/each}
					</Select.Group>
				</Select.Content>
			</Select.Root>
		</div>

		<div>
			<Label.Root for="language" class="mb-3 text-foreground">Language</Label.Root>
			<Select.Root type="single" name="language" bind:value={locale.current}>
				<Select.Trigger class="w-full">
					{triggerLanguageContent}
				</Select.Trigger>
				<Select.Content>
					<Select.Group>
						{#each availableLanguages as lang (lang.value)}
							<Select.Item value={lang.value} label={lang.label}>
								{lang.label}
							</Select.Item>
						{/each}
					</Select.Group>
				</Select.Content>
			</Select.Root>
		</div>

		<!-- <Button size="sm" disabled={isSaving}>
			{#if isSaving}
				<Spinner />
			{/if}
			{isSaving ? 'Submitting...' : 'Submit'}
		</Button> -->
	</div>

	<Separator class="my-6 max-w-sm" />

	<h2 class="mb-1 text-lg font-semibold">Monitoring</h2>
	<p class="mb-4 max-w-sm text-sm text-muted-foreground">
		Manage your application's logging and monitoring preferences.
	</p>

	<div class="flex items-center gap-3">
		<Switch id="is-censored" bind:checked={isEnableLoggings} />
		<Label.Root for="is-censored">Enable Loggings</Label.Root>
	</div>

	<Separator class="my-6 max-w-sm" />

	<h2 class="mb-1 text-lg font-semibold">Data Management</h2>
	<p class="mb-4 max-w-sm text-sm text-muted-foreground">
		Manage your application's data storage and backup preferences.
	</p>

	<div class="grid w-full max-w-sm gap-4">
		<Button size="sm" variant="outline">Download all as Zip</Button>
	</div>
</section>
