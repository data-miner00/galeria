<script lang="ts">
	import NavMain from './nav-main.svelte';
	import NavProjects from './nav-projects.svelte';
	import NavSecondary from './nav-secondary.svelte';
	import NavUser from './nav-user.svelte';
	import * as Sidebar from '$lib/components/ui/sidebar/index.js';
	import CommandIcon from '@lucide/svelte/icons/command';
	import { type ComponentProps } from 'svelte';
	import Button from './ui/button/button.svelte';
	import { PlusIcon } from '@lucide/svelte';
	import type { Board } from '$lib/types';
	import { appState } from '$lib/states.svelte';

	import BookOpenIcon from '@lucide/svelte/icons/book-open';
	import BotIcon from '@lucide/svelte/icons/bot';
	import LifeBuoyIcon from '@lucide/svelte/icons/life-buoy';
	import SendIcon from '@lucide/svelte/icons/send';
	import SquareTerminalIcon from '@lucide/svelte/icons/square-terminal';

	const data = {
		navMain: [
			{
				title: 'Playground',
				url: '#',
				icon: SquareTerminalIcon,
				isActive: true,
				items: [
					{
						title: 'Recycle Bin',
						url: '/recycle'
					},
					{
						title: 'Starred',
						url: '/favorites'
					},
					{
						title: 'Experiment',
						url: '/experiment'
					}
				]
			},
			{
				title: 'Models',
				url: '#',
				icon: BotIcon,
				items: [
					{
						title: 'Genesis',
						url: '#'
					},
					{
						title: 'Explorer',
						url: '#'
					},
					{
						title: 'Quantum',
						url: '#'
					}
				]
			},
			{
				title: 'Documentation',
				url: '#',
				icon: BookOpenIcon,
				items: [
					{
						title: 'Introduction',
						url: '#'
					},
					{
						title: 'Get Started',
						url: '#'
					},
					{
						title: 'Tutorials',
						url: '#'
					},
					{
						title: 'Changelog',
						url: '#'
					}
				]
			}
		],
		navSecondary: [
			{
				title: 'Support',
				url: '#',
				icon: LifeBuoyIcon
			},
			{
				title: 'Feedback',
				url: '#',
				icon: SendIcon
			}
		]
	};

	type Props = ComponentProps<typeof Sidebar.Root> & {
		onCreateClick: () => void;
		onCreateBoardClick: () => void;
	};

	let { ref = $bindable(null), onCreateClick, onCreateBoardClick, ...restProps }: Props = $props();

	let boards = $derived<Board[]>(appState.boards);
</script>

<Sidebar.Root bind:ref variant="inset" {...restProps}>
	<Sidebar.Header>
		<Sidebar.Menu>
			<Sidebar.MenuItem>
				<Sidebar.MenuButton size="lg">
					{#snippet child({ props })}
						<a href="/" {...props}>
							<div
								class="flex aspect-square size-8 items-center justify-center rounded-lg bg-sidebar-primary text-sidebar-primary-foreground"
							>
								<CommandIcon class="size-4" />
							</div>
							<div class="grid flex-1 text-start text-sm leading-tight">
								<span class="truncate font-medium">Acme Inc</span>
								<span class="truncate text-xs">Enterprise</span>
							</div>
						</a>
					{/snippet}
				</Sidebar.MenuButton>
			</Sidebar.MenuItem>
		</Sidebar.Menu>
	</Sidebar.Header>
	<Sidebar.Content>
		<NavMain items={data.navMain} />
		<NavProjects {boards} />
		<Button variant="outline" onclick={onCreateBoardClick}>
			<PlusIcon /> Create Board
		</Button>
		<Button onclick={onCreateClick}>
			<PlusIcon /> Create New Image
		</Button>
		<NavSecondary items={data.navSecondary} class="mt-auto" />
	</Sidebar.Content>
	<Sidebar.Footer>
		<NavUser
			user={{
				name: appState.profile.username || 'User',
				email: appState.profile.email || 'user@example.com',
				avatar: appState.profile.avatarImage || ''
			}}
		/>
	</Sidebar.Footer>
</Sidebar.Root>
