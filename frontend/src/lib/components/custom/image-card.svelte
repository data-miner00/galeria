<script lang="ts">
	import Ellipsis from '@lucide/svelte/icons/ellipsis';
	import Button from '../ui/button/button.svelte';

	type Props = {
		id: string;
		path: string;
		description?: string;
	};

	const { id, path, description }: Props = $props();
</script>

<div>
	<div class="mb-1 w-70 overflow-hidden rounded">
		<img alt={description} src={`http://127.0.0.1:10003/devstoreaccount1/images/${path}`} />
	</div>
	<div class="flex items-center justify-between">
		<div>
			<p class="max-w-40 text-sm text-ellipsis">{description}</p>
		</div>
		<Button variant="ghost" size="icon">
			<Ellipsis />
		</Button>
	</div>
</div>
