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
};
