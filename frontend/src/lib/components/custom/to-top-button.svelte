<script lang="ts">
	import ArrowUpIcon from '@lucide/svelte/icons/arrow-up';
	import { Button } from '$lib/components/ui/button/index.js';
</script>

<Button
	class="fixed right-6 bottom-4"
	variant="outline"
	size="icon"
	aria-label="Scroll to top"
	onclick={() => window.scrollTo({ top: 0, behavior: 'smooth' })}
>
	<ArrowUpIcon />
</Button>
