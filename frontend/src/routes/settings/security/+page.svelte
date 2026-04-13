<script lang="ts">
	import * as InputGroup from '$lib/components/ui/input-group/index.js';
	import * as Label from '$lib/components/ui/label/index.js';
	import * as Tooltip from '$lib/components/ui/tooltip/index.js';
	import InfoIcon from '@lucide/svelte/icons/info';
	import * as Avatar from '$lib/components/ui/avatar/index.js';
	import { onMount } from 'svelte';
	import { Button, buttonVariants } from '$lib/components/ui/button/index.js';
	import { Spinner } from '$lib/components/ui/spinner/index.js';
	import { toast } from 'svelte-sonner';
	import { appState } from '$lib/states.svelte';
	import { Switch } from '$lib/components/ui/switch/index.js';

	let settings = $state({});

	let isEnablePassword = $state(false);
	let isTotpEnabled = $state(false);
	let isDialogOpen = $state(false);

	$effect(() => {
		if (isTotpEnabled) {
			isDialogOpen = true;
			requestEnableTotp();
		}
	});

	function onCheckedChange(isChecked: boolean) {
		isDialogOpen = !isDialogOpen;
		if (isChecked) {
			requestEnableTotp();
		}
	}

	onMount(async () => {
		appState.headerTitle = 'Security Settings';
	});

	let isSaving = $state(false);
	let isVerifying = $state(false);
	let otpImageBlobUrl = $state('');

	async function requestEnableTotp() {
		const response = await fetch('https://localhost:7146/api/v1/auth/totp/enable', {
			method: 'POST'
		});
		const blobImage = await response.blob();

		otpImageBlobUrl = URL.createObjectURL(blobImage);
	}

	import * as InputOTP from '$lib/components/ui/input-otp/index.js';

	let otp = $state('');

	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import { slide } from 'svelte/transition';

	async function handleNextSubmit() {
		if (!isVerifying) {
			isVerifying = true;
			return;
		}

		isSaving = true;

		const response = await fetch('https://localhost:7146/api/v1/auth/totp/validate/' + otp, {
			method: 'POST'
		});

		if (!response.ok) {
			toast.error('Invalid OTP. Please try again.');
			isSaving = false;
			return;
		}

		toast.success('Multi-Factor Authentication enabled successfully.');
		isSaving = false;
		isDialogOpen = false;
	}
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
			<Switch id="is-censored" bind:checked={isTotpEnabled} {onCheckedChange} />
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

<Dialog.Root bind:open={isDialogOpen}>
	<form>
		<Dialog.Content class="sm:max-w-[425px]">
			<Dialog.Header>
				<Dialog.Title>Setup Time-based OTP</Dialog.Title>
				<Dialog.Description>
					To enhance the security of your account, we recommend setting up Time-based One-Time
					Password (TOTP) authentication.
				</Dialog.Description>
			</Dialog.Header>

			{#if !isVerifying}
				<div class="flex aspect-square w-full items-center justify-center">
					{#if otpImageBlobUrl}
						<img class="h-full w-full" src={otpImageBlobUrl} alt="QR code for TOTP" />
					{:else}
						<Spinner />
					{/if}
				</div>
			{:else}
				<InputOTP.Root class="mx-auto" maxlength={6} bind:value={otp}>
					{#snippet children({ cells })}
						<InputOTP.Group>
							{#each cells.slice(0, 6) as cell (cell)}
								<InputOTP.Slot {cell} />
							{/each}
						</InputOTP.Group>
					{/snippet}
				</InputOTP.Root>
			{/if}

			<Dialog.Footer>
				<Dialog.Close type="button" class={buttonVariants({ variant: 'outline' })}>
					Cancel
				</Dialog.Close>
				<Button type="submit" onclick={handleNextSubmit} disabled={isSaving}>
					{#if isSaving}
						<Spinner />
					{/if}
					{!isVerifying ? 'Next' : isSaving ? 'Verifying...' : 'Verify'}
				</Button>
			</Dialog.Footer>
		</Dialog.Content>
	</form>
</Dialog.Root>
