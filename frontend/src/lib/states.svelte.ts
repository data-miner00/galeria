import type { Board } from './types';

export type AppState = {
	boards: Board[];
};

export let appState = $state<AppState>({ boards: [] });
