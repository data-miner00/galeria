<script lang="ts">
	import * as Card from '$lib/components/ui/card/index.js';
	import * as Carousel from '$lib/components/ui/carousel/index.js';
	import { B } from '$lib/helpers';
	import { appState } from '$lib/states.svelte';
	import { onMount } from 'svelte';

	onMount(() => {
		appState.headerTitle = 'Carousel';
	});
</script>

<div class="flex h-full w-full items-center justify-center">
	<Carousel.Root class="max-w-xs">
		<Carousel.Content>
			{#each appState.images as image (image.id)}
				<Carousel.Item>
					<div class="p-1">
						<Card.Root>
							<Card.Content class="flex items-center justify-center p-6">
								<img src={B(image.mediumPath)} alt={image.title || image.description} />
							</Card.Content>
						</Card.Root>
					</div>
				</Carousel.Item>
			{/each}
		</Carousel.Content>
		<Carousel.Previous />
		<Carousel.Next />
	</Carousel.Root>
</div>
