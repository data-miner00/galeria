import type { Board, ImageRecord, UserProfile, UserSettings } from './types';

export type AppState = {
	headerTitle: string;
	images: ImageRecord[];
	boards: Board[];
	settings: UserSettings;
	profile: UserProfile;
};

export let appState = $state<AppState>({
	headerTitle: 'Home',
	images: [],
	boards: [],
	settings: {},
	profile: {}
});
