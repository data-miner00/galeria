import type { Board, ImageRecord, UserSettings } from './types';

export type AppState = {
	images: ImageRecord[];
	boards: Board[];
	settings: UserSettings;
};

export let appState = $state<AppState>({ images: [], boards: [], settings: {} });
