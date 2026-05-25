<script lang="ts">
	import EditorCanvas from '$lib/components/draft/EditorCanvas.svelte';
	import { appState } from '$lib/states.svelte';
	import { onMount } from 'svelte';

	onMount(() => {
		appState.headerTitle = 'Drafts';
	});
</script>

<h1 style="margin-bottom:12px">Draft Editor</h1>
<EditorCanvas />
