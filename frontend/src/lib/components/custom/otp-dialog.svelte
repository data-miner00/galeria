<script lang="ts">
	import { Button } from '$lib/components/ui/button/index.js';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import { toast } from 'svelte-sonner';
	import * as InputOTP from '$lib/components/ui/input-otp/index.js';
	import { PUBLIC_API_BASE_URL } from '$env/static/public';

	type Props = {
		isDialogOpen: boolean;
	};

	let { isDialogOpen = $bindable(false) }: Props = $props();

	let otp = $state('');

	async function verifyOtp() {
		const res = await fetch(`${PUBLIC_API_BASE_URL}/api/v1/auth/totp/validate/${otp}`, {
			method: 'POST'
		});

		if (!res.ok) {
			toast.error('OTP verification failed.');
			return;
		}

		toast.success('OTP verified successfully.');
		isDialogOpen = false;
	}
</script>

<Dialog.Root bind:open={isDialogOpen}>
	<form>
		<Dialog.Content class="sm:max-w-106.25">
			<Dialog.Header>
				<Dialog.Title>OTP Verification</Dialog.Title>
				<Dialog.Description>Please enter the OTP from the validator.</Dialog.Description>
			</Dialog.Header>
			<InputOTP.Root class="mx-auto" maxlength={6} bind:value={otp}>
				{#snippet children({ cells })}
					<InputOTP.Group>
						{#each cells.slice(0, 6) as cell (cell)}
							<InputOTP.Slot {cell} />
						{/each}
					</InputOTP.Group>
				{/snippet}
			</InputOTP.Root>
			<Dialog.Footer>
				<Button type="submit" onclick={verifyOtp}>Verify OTP</Button>
			</Dialog.Footer>
		</Dialog.Content>
	</form>
</Dialog.Root>
