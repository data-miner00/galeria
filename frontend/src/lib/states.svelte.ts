import type { Board, ImageRecord } from './types';

export type AppState = {
	images: ImageRecord[];
	boards: Board[];
};

export let appState = $state<AppState>({ images: [], boards: [] });
