<script lang="ts">
	import ImageCard from '$lib/components/custom/image-card.svelte';
	import { onMount } from 'svelte';

	type ImageRecord = {
		id: string;
		path: string;
		originalFileName: string;
		contentType: string;
		description?: string;
		status: number;
		createdAt: string;
	};

	let records = $state<ImageRecord[]>([]);

	onMount(async () => {
		const res = await fetch('https://localhost:7146/api/v1/image');
		records = await res.json();
	});
</script>

<div class="grid h-full grid-cols-5 gap-4">
	<div class="flex w-70 flex-col gap-4">
		{#each records as record}
			<ImageCard id={record.id} path={record.path} description={record.description} />
		{/each}
	</div>
	<div class="flex w-70 flex-col gap-4">
		{#each records as record}
			<ImageCard id={record.id} path={record.path} description={record.description} />
		{/each}
	</div>
	<div class="flex w-70 flex-col gap-4">
		{#each records as record}
			<ImageCard id={record.id} path={record.path} description={record.description} />
		{/each}
	</div>
	<div class="flex w-70 flex-col gap-4">
		{#each records as record}
			<ImageCard id={record.id} path={record.path} description={record.description} />
		{/each}
	</div>
	<div class="flex w-70 flex-col gap-4">
		{#each records as record}
			<ImageCard id={record.id} path={record.path} description={record.description} />
		{/each}
	</div>
</div>
