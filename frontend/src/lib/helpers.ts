import { PUBLIC_BLOB_BASE_URL } from '$env/static/public';

export function debounce<T extends (...args: any[]) => any>(
	func: T,
	wait: number
): (...args: Parameters<T>) => void {
	let timeout: ReturnType<typeof setTimeout> | null = null;

	return (...args: Parameters<T>): void => {
		if (timeout) clearTimeout(timeout);
		timeout = setTimeout(() => func(...args), wait);
	};
}

export function createBlobUrl(relativePath: string): string {
	return PUBLIC_BLOB_BASE_URL + '/devstoreaccount1/images/' + relativePath;
}

export const B = createBlobUrl;
