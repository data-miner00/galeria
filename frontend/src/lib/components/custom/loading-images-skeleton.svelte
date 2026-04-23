<script lang="ts">
	import { Skeleton } from '$lib/components/ui/skeleton/index.js';

	type Props = {
		layout?: 'masonry' | 'timeline';
	};

	const { layout = 'masonry' }: Props = $props();
</script>

{#if layout === 'masonry'}
	<div class="flex space-x-4">
		<div class="flex flex-1 flex-col gap-4">
			<Skeleton class="h-75 w-full" />
			<Skeleton class="h-70 w-full" />
			<Skeleton class="h-100 w-full" />
			<Skeleton class="h-75 w-full" />
		</div>
		<div class="flex flex-1 flex-col gap-4">
			<Skeleton class="h-100 w-full" />
			<Skeleton class="h-70 w-full" />
			<Skeleton class="h-50 w-full" />
			<Skeleton class="h-75 w-full" />
		</div>
		<div class="flex flex-1 flex-col gap-4">
			<Skeleton class="h-45 w-full" />
			<Skeleton class="h-70 w-full" />
			<Skeleton class="h-37.5 w-full" />
			<Skeleton class="h-75 w-full" />
		</div>
		<div class="flex flex-1 flex-col gap-4">
			<Skeleton class="h-75 w-full" />
			<Skeleton class="h-40 w-full" />
			<Skeleton class="h-55 w-full" />
			<Skeleton class="h-75 w-full" />
		</div>
		<div class="flex flex-1 flex-col gap-4">
			<Skeleton class="h-75 w-full" />
			<Skeleton class="h-70 w-full" />
			<Skeleton class="h-100 w-full" />
			<Skeleton class="h-75 w-full" />
		</div>
	</div>
{:else}
	<div class="flex flex-wrap gap-1">
		{#each Array(20)}
			<Skeleton class="h-25 w-25" />
		{/each}
	</div>
{/if}
