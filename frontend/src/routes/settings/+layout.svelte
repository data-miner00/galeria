<script lang="ts">
	let { children } = $props();
	import { page } from '$app/state';
</script>

<div class="mt-10 flex px-4">
	<aside class="flex basis-72 flex-col gap-5">
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

	<div class="flex flex-1 flex-col gap-4 p-4 pt-0">
		{@render children()}
	</div>
</div>
