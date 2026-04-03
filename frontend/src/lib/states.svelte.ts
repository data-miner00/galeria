import type { Board, ImageRecord, UserSettings } from './types';

export type AppState = {
	headerTitle: string;
	images: ImageRecord[];
	boards: Board[];
	settings: UserSettings;
};

export let appState = $state<AppState>({
	headerTitle: 'Home',
	images: [],
	boards: [],
	settings: {}
});
