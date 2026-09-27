import type {
	AppNotification,
	Board,
	ImageRecord,
	InfoSheetData,
	UserProfile,
	UserSettings
} from './types';

const MAX_NOTIFICATIONS = 100;

export type AppState = {
	isLoading: boolean;
	headerTitle: string;
	images: ImageRecord[];
	boards: Board[];
	settings: UserSettings;
	profile: UserProfile;
	infoSheetData: InfoSheetData;
	boardInfoSheetData: InfoSheetData;
	notifications: AppNotification[];
	openState: {
		isCommandPaletteOpen: boolean;
		isUploadImageDialogOpen: boolean;
		isCreateBoardDialogOpen: boolean;
		isOtpDialogOpen: boolean;
	};
};

export let appState = $state<AppState>({
	isLoading: true,
	headerTitle: 'Home',
	images: [],
	boards: [],
	settings: {},
	profile: {},
	infoSheetData: {},
	boardInfoSheetData: {},
	notifications: [],
	openState: {
		isCommandPaletteOpen: false,
		isUploadImageDialogOpen: false,
		isCreateBoardDialogOpen: false,
		isOtpDialogOpen: false
	}
});

export function toggleUploadImageDialog() {
	appState.openState.isUploadImageDialogOpen = !appState.openState.isUploadImageDialogOpen;
}

export function toggleCreateBoardDialog() {
	appState.openState.isCreateBoardDialogOpen = !appState.openState.isCreateBoardDialogOpen;
}

export function addNotification(notification: AppNotification) {
	appState.notifications = [notification, ...appState.notifications].slice(0, MAX_NOTIFICATIONS);
}

export function markNotificationsRead() {
	for (const notification of appState.notifications) {
		notification.isRead = true;
	}
}

export function clearNotifications() {
	appState.notifications = [];
}

const unreadNotificationCount = $derived(appState.notifications.filter((n) => !n.isRead).length);

// Derived state can't be exported directly from a module, so expose it through getters.
export const notificationCounts = {
	get unread() {
		return unreadNotificationCount;
	},
	get read() {
		return appState.notifications.length - unreadNotificationCount;
	}
};
