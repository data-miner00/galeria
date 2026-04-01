<script lang="ts">
	let { children } = $props();
</script>

<div class="mt-10 flex">
	<aside class="flex basis-72 flex-col gap-5 text-lg">
		<a href="/settings/profile" class="underline">Profile</a>
		<a href="/settings/layout">Layout</a>
		<a href="/settings/boards">Boards</a>
		<a href="/settings/privacy">Privacy</a>
	</aside>

	<div class="flex flex-1 flex-col gap-4 p-4 pt-0">
		{@render children()}
	</div>
</div>
