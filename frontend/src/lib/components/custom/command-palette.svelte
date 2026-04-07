<script lang="ts">
	import CalculatorIcon from '@lucide/svelte/icons/calculator';
	import CalendarIcon from '@lucide/svelte/icons/calendar';
	import CreditCardIcon from '@lucide/svelte/icons/credit-card';
	import SettingsIcon from '@lucide/svelte/icons/settings';
	import SmileIcon from '@lucide/svelte/icons/smile';
	import UserIcon from '@lucide/svelte/icons/user';
	import * as Command from '$lib/components/ui/command/index.js';
	import { goto } from '$app/navigation';

	type Props = {
		isOpen: boolean;
	};

	let { isOpen = $bindable(false) }: Props = $props();

	function gotoPage(path: string) {
		goto(path);
		isOpen = false;
	}
</script>

<Command.Dialog class="rounded-lg border shadow-md md:min-w-112.5" bind:open={isOpen}>
	<Command.Input placeholder="Type a command or search..." />
	<Command.List>
		<Command.Empty>No results found.</Command.Empty>
		<Command.Group heading="Suggestions">
			<Command.Item>
				<CalendarIcon />
				<span>Calendar</span>
			</Command.Item>
			<Command.Item>
				<SmileIcon />
				<span>Search Emoji</span>
			</Command.Item>
			<Command.Item disabled>
				<CalculatorIcon />
				<span>Calculator</span>
			</Command.Item>
		</Command.Group>
		<Command.Separator />
		<Command.Group heading="Settings">
			<Command.Item onSelect={() => gotoPage('/settings/profile')}>
				<UserIcon />
				<span>Profile</span>
				<Command.Shortcut>⌘P</Command.Shortcut>
			</Command.Item>
			<Command.Item onSelect={() => gotoPage('/settings/general')}>
				<CreditCardIcon />
				<span>General</span>
				<Command.Shortcut>⌘B</Command.Shortcut>
			</Command.Item>
			<Command.Item>
				<SettingsIcon />
				<span>Settings</span>
				<Command.Shortcut>⌘S</Command.Shortcut>
			</Command.Item>
		</Command.Group>
	</Command.List>
</Command.Dialog>
