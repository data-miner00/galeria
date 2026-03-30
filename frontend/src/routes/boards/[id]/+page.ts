import { error } from '@sveltejs/kit';
import type { PageLoad } from './$types';
import type { Board, ImageRecord } from '$lib/types';

export const load: PageLoad = async ({ params, fetch, depends }) => {
	depends(`board:${params.id}`);
	const res = await fetch(`https://localhost:7146/api/v1/board/${params.id}`);
	const board: Board = await res.json();

	let images: ImageRecord[] = [];
	// Question: Should fetch by passing params or post body
	if (board.imageIds.length > 0) {
		const response = await fetch(`https://localhost:7146/api/v1/image/getbyids`, {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json'
			},
			body: JSON.stringify({ imageIds: board.imageIds })
		});
		const imageRecords = await response.json();
		images = imageRecords;
	}

	if (res.ok) {
		return {
			board,
			images
		};
	}

	error(404, 'Not found');
};
