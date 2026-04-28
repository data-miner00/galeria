<script lang="ts">
	import NavMain from './nav-main.svelte';
	import NavProjects from './nav-projects.svelte';
	import NavSecondary from './nav-secondary.svelte';
	import NavUser from './nav-user.svelte';
	import * as Sidebar from '$lib/components/ui/sidebar/index.js';
	import CommandIcon from '@lucide/svelte/icons/command';
	import { type ComponentProps } from 'svelte';
	import Button from './ui/button/button.svelte';
	import { BracesIcon, DatabaseIcon, LayoutGridIcon, PlusIcon } from '@lucide/svelte';
	import type { Board } from '$lib/types';
	import { appState } from '$lib/states.svelte';

	import SquareTerminalIcon from '@lucide/svelte/icons/square-terminal';
	import { SIDEBAR, t } from '$lib/i18n/translations.svelte';
	import { PUBLIC_API_BASE_URL, PUBLIC_COSMOS_BASE_URL } from '$env/static/public';

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
						title: 'Drafts',
						url: '/drafts'
					},
					{
						title: 'Hidden',
						url: '/hidden'
					},
					{
						title: 'Experiment',
						url: '/experiment'
					}
				]
			},
			{
				title: 'Layouts',
				url: '#',
				icon: LayoutGridIcon,
				items: [
					{
						title: 'Timeline',
						url: '/timeline'
					},
					{
						title: 'Carousel',
						url: '/carousel'
					},
					{
						title: 'Transition', // full screen background transition
						url: '/transition'
					},
					{
						title: 'Memories',
						url: '/memories'
					},
					{
						title: 'People',
						url: '/people'
					}
				]
			}
		],
		navSecondary: [
			{
				title: 'Database',
				url: PUBLIC_COSMOS_BASE_URL,
				icon: DatabaseIcon,
				external: true
			},
			{
				title: 'Swagger',
				url: `${PUBLIC_API_BASE_URL}/swagger/index.html`,
				icon: BracesIcon,
				external: true
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
		<Button variant="outline" onclick={onCreateBoardClick} class="cursor-pointer">
			<PlusIcon />
			{t(SIDEBAR.CREATE_BOARD)}
		</Button>
		<Button onclick={onCreateClick} class="cursor-pointer">
			<PlusIcon />
			{t(SIDEBAR.CREATE_IMAGE)}
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
