<script lang="ts">
	import AppSidebar from '$lib/components/app-sidebar.svelte';
	import * as Breadcrumb from '$lib/components/ui/breadcrumb/index.js';
	import { Separator } from '$lib/components/ui/separator/index.js';
	import * as Sidebar from '$lib/components/ui/sidebar/index.js';
	import { ArrowLeft, SearchIcon } from '@lucide/svelte';

	import './layout.css';
	import favicon from '$lib/assets/favicon.svg';
	import { Button } from '$lib/components/ui/button';

	let { children } = $props();
</script>

<svelte:head>
	<link rel="icon" href={favicon} />
	<title>Galeria</title>
</svelte:head>

<Sidebar.Provider>
	<AppSidebar />
	<Sidebar.Inset class="relative overflow-clip">
		<header class="sticky top-0 right-0 left-0 flex h-16 shrink-0 items-center gap-2 bg-white">
			<div class="flex w-full items-center justify-between px-4">
				<div class="flex items-center gap-2">
					<Sidebar.Trigger class="-ms-1" />
					<Separator orientation="vertical" class="data-[orientation=vertical]:h-4" />
					<Button variant="ghost" size="icon">
						<ArrowLeft />
					</Button>
					<Separator orientation="vertical" class="me-2 data-[orientation=vertical]:h-4" />
					<Breadcrumb.Root>
						<Breadcrumb.List>
							<!-- <Breadcrumb.Item class="hidden md:block">
								<Breadcrumb.Link href="##">At The Homepage</Breadcrumb.Link>
							</Breadcrumb.Item>
							<Breadcrumb.Separator class="hidden md:block" /> -->
							<Breadcrumb.Item>
								<Breadcrumb.Page>Home</Breadcrumb.Page>
							</Breadcrumb.Item>
						</Breadcrumb.List>
					</Breadcrumb.Root>
				</div>
				<div>
					<Button variant="ghost" size="icon">
						<SearchIcon />
					</Button>
				</div>
			</div>
		</header>
		<div class="flex flex-1 flex-col gap-4 p-4 pt-0">
			{@render children()}
		</div>
	</Sidebar.Inset>
</Sidebar.Provider>
