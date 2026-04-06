export type Board = {
	id: string;
	title: string;
	description: string;
	imageIds: string[];
	createdAt: string;
};

export type ImageRecord = {
	id: string;
	path: string;
	originalFileName: string;
	contentType: string;
	description?: string;
	status: number;
	createdAt: string;
	isCensored: boolean;
	thumbnailPath: string;
	mediumPath: string;
	isFavorite: boolean;
	isSoftDeleted: boolean;
	width: number;
	height: number;
	size: number;
	category?: string;
	tags: string[];
};

export type UserProfile = {
	username?: string;
	avatarImage?: string;
	firstName?: string;
	lastName?: string;
	websiteUrl?: string;
	bio?: string;
	email?: string;
};

export type UserSettings = {
	noOfColumns?: number;
};

export type InfoSheetData = {
	id?: string;
	isOpen?: boolean;
};

export type GeneralSettings = {
	theme?: 'light' | 'dark' | 'system';
	language?: string;
};

export type SecuritySettings = {
	enablePassword?: boolean;
	enable2FA?: boolean;
	rememberMeDuration?: number;
};
