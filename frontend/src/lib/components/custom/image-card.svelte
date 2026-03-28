<script lang="ts">
	import CardActionsButton from './card-actions-button.svelte';

	type Props = {
		id: string;
		path: string;
		description?: string;
		onDelete: () => void;
	};

	const { id, path, description, onDelete }: Props = $props();
</script>

<div>
	<div class="mb-1 w-70 overflow-hidden rounded">
		<img alt={description} src={`http://127.0.0.1:10003/devstoreaccount1/images/${path}`} />
	</div>
	<div class="flex items-center justify-between">
		<div>
			<p class="max-w-40 text-sm text-ellipsis">{description}</p>
		</div>
		<CardActionsButton {id} {onDelete} />
	</div>
</div>
