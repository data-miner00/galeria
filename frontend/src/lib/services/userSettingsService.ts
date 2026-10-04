import { toast } from '$lib/notify';
import { appState } from '$lib/states.svelte';
import type { LayoutType, UserSettings } from '$lib/types';

import { apiPatch } from './api';

export async function updateSettings(payload: Partial<UserSettings>): Promise<UserSettings> {
	return apiPatch<Partial<UserSettings>, UserSettings>('/UserSettings', payload);
}

export async function saveLayoutType(layoutType: LayoutType): Promise<void> {
	const previous = appState.settings.layoutType;
	if (previous === layoutType) return;

	appState.settings.layoutType = layoutType;

	try {
		await updateSettings({ layoutType });
	} catch (e) {
		console.error('Failed to save layout type', e);
		appState.settings.layoutType = previous;
		toast.error('Failed to save layout preference');
	}
}
