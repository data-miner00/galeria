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
