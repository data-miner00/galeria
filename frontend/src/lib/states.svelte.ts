import type { Board, ImageRecord, InfoSheetData, UserProfile, UserSettings } from './types';

export type AppState = {
	isLoading: boolean;
	headerTitle: string;
	images: ImageRecord[];
	boards: Board[];
	settings: UserSettings;
	profile: UserProfile;
	infoSheetData: InfoSheetData;
	boardInfoSheetData: InfoSheetData;
};

export let appState = $state<AppState>({
	isLoading: true,
	headerTitle: 'Home',
	images: [],
	boards: [],
	settings: {},
	profile: {},
	infoSheetData: {},
	boardInfoSheetData: {}
});
