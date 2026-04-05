import type { Board, ImageRecord, InfoSheetData, UserProfile, UserSettings } from './types';

export type AppState = {
	headerTitle: string;
	images: ImageRecord[];
	boards: Board[];
	settings: UserSettings;
	profile: UserProfile;
	infoSheetData: InfoSheetData;
};

export let appState = $state<AppState>({
	headerTitle: 'Home',
	images: [],
	boards: [],
	settings: {},
	profile: {},
	infoSheetData: {}
});
