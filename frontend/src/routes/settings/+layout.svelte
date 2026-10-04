<script lang="ts">
	import { page } from '$app/state';

	let { children } = $props();
</script>

<div class="mt-4 flex flex-col gap-6 md:mt-10 md:flex-row md:gap-0 md:px-4">
	<aside
		class="flex shrink-0 gap-5 overflow-x-auto border-b pb-3 whitespace-nowrap md:basis-72 md:flex-col md:overflow-visible md:border-b-0 md:pb-0 md:whitespace-normal"
	>
		<a href="/settings" class:underline={page.url.pathname === '/settings'}>General</a>
		<a href="/settings/profile" class:underline={page.url.pathname === '/settings/profile'}>
			Profile Related
		</a>
		<a href="/settings/layout" class:underline={page.url.pathname === '/settings/layout'}>
			Layout Management
		</a>
		<a href="/settings/security" class:underline={page.url.pathname === '/settings/security'}>
			Security Settings
		</a>
		<a
			href="/settings/data-management"
			class:underline={page.url.pathname === '/settings/data-management'}
		>
			Data Management
		</a>
		<a
			href="/settings/notification"
			class:underline={page.url.pathname === '/settings/notification'}
		>
			Notification
		</a>
	</aside>

	<div class="flex min-w-0 flex-1 flex-col gap-4 pb-4 md:px-4">
		{@render children()}
	</div>
</div>
