<script lang="ts">
	import * as InputGroup from '$lib/components/ui/input-group/index.js';
	import * as Label from '$lib/components/ui/label/index.js';
	import * as Tooltip from '$lib/components/ui/tooltip/index.js';
	import InfoIcon from '@lucide/svelte/icons/info';
	import * as Avatar from '$lib/components/ui/avatar/index.js';
	import type { UserProfile } from '$lib/types';
	import { onMount } from 'svelte';
	import { Button } from '$lib/components/ui/button/index.js';
	import { Spinner } from '$lib/components/ui/spinner/index.js';
	import { toast } from 'svelte-sonner';
	import { appState } from '$lib/states.svelte';
	import { PUBLIC_API_BASE_URL } from '$env/static/public';

	let profile = $state<UserProfile>({});

	onMount(async () => {
		appState.headerTitle = 'Profile Settings';

		const res = await fetch(`${PUBLIC_API_BASE_URL}/api/v1/UserProfile`);
		profile = await res.json();
	});

	let isSaving = $state(false);

	async function saveProfile() {
		isSaving = true;

		const request = await fetch(`${PUBLIC_API_BASE_URL}/api/v1/UserProfile`, {
			method: 'PATCH',
			headers: {
				'Content-Type': 'application/json'
			},
			body: JSON.stringify(profile)
		});

		if (request.ok) {
			toast.success('Profile updated successfully!');
		} else {
			const error = await request.json();
			toast.error(`Failed to update profile: ${error.errorMessage}`);
		}

		isSaving = false;
	}
</script>

<h1 class="text-2xl font-bold">Profile Settings</h1>

<p>Manage your profile information and preferences that will be visible to yourself.</p>

<section class="mt-6">
	<div class="grid w-full max-w-sm gap-4">
		<Avatar.Root size="xl">
			<Avatar.Image src={profile.avatarImage} alt={`@${profile.username}`} />
			<Avatar.Fallback
				>{profile.firstName?.charAt(0) || 'C'}{profile.lastName?.charAt(0) || 'N'}</Avatar.Fallback
			>
		</Avatar.Root>

		<InputGroup.Root>
			<InputGroup.Input id="username" placeholder="shadcn" bind:value={profile.username} />
			<InputGroup.Addon>
				<Label.Root for="username">@</Label.Root>
			</InputGroup.Addon>
		</InputGroup.Root>
		<InputGroup.Root>
			<InputGroup.Input id="firstName" placeholder="Shaun" bind:value={profile.firstName} />
			<InputGroup.Addon align="block-start">
				<Label.Root for="firstName" class="text-foreground">First Name</Label.Root>
			</InputGroup.Addon>
		</InputGroup.Root>
		<InputGroup.Root>
			<InputGroup.Input id="lastName" placeholder="Smith" bind:value={profile.lastName} />
			<InputGroup.Addon align="block-start">
				<Label.Root for="lastName" class="text-foreground">Last Name</Label.Root>
			</InputGroup.Addon>
		</InputGroup.Root>
		<InputGroup.Root>
			<InputGroup.Input id="email" placeholder="shadcn@vercel.com" bind:value={profile.email} />
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
			<InputGroup.Textarea
				id="bio"
				placeholder="Tell us about yourself..."
				bind:value={profile.bio}
			/>
			<InputGroup.Addon align="block-start">
				<Label.Root for="bio" class="text-foreground">Bio</Label.Root>
			</InputGroup.Addon>
		</InputGroup.Root>
		<InputGroup.Root>
			<InputGroup.Input id="website" placeholder="https://" bind:value={profile.websiteUrl} />
			<InputGroup.Addon align="block-start">
				<Label.Root for="website" class="text-foreground">Website</Label.Root>
			</InputGroup.Addon>
		</InputGroup.Root>

		<Button size="sm" variant="outline" disabled={isSaving} onclick={saveProfile}>
			{#if isSaving}
				<Spinner />
			{/if}
			{isSaving ? 'Submitting...' : 'Submit'}
		</Button>
	</div>
</section>
