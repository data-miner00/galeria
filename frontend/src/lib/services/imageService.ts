import { appState } from '$lib/states.svelte';
import type { ImageRecord } from '$lib/types';

import {
	apiDelete,
	apiDeleteForJson,
	apiGet,
	apiPatch,
	apiPost,
	apiRawFetch,
	apiUploadFileForJson
} from './api';

export async function fetchAll(): Promise<ImageRecord[]> {
	return apiGet<ImageRecord[]>('/image');
}

export async function upload(formData: FormData): Promise<ImageRecord> {
	return apiUploadFileForJson<ImageRecord>('/image', formData);
}

export async function getByIds(imageIds: string[]): Promise<ImageRecord[]> {
	return apiPost<{ imageIds: string[] }, ImageRecord[]>('/image/getbyids', { imageIds });
}

export async function downloadAll(): Promise<Response> {
	return apiRawFetch('/image/blob/download');
}

export async function downloadWithWatermark(id: string): Promise<Response> {
	return apiRawFetch(
		`/image/blob/${id}/download?watermark=${encodeURIComponent(appState.settings.watermark || '')}`
	);
}

export async function downloadMultiple(requestedIds: string[]): Promise<Response> {
	return apiRawFetch('/image/blob/download/multiple', {
		method: 'POST',
		headers: { 'Content-Type': 'application/json' },
		body: JSON.stringify({ requestedIds })
	});
}

export async function deleteByIds(
	requestedIds: string[],
	isSoftDelete = true
): Promise<ImageRecord[] | null> {
	return apiDeleteForJson<ImageRecord[] | null, { requestedIds: string[]; isSoftDelete: boolean }>(
		'/image',
		{ requestedIds, isSoftDelete }
	);
}

export async function deleteById(id: string): Promise<void> {
	return apiDelete(`/image/${id}`);
}

export async function patchImage(
	id: string,
	payload: Partial<ImageRecord>
): Promise<ImageRecord | null> {
	return apiPatch<Partial<ImageRecord>, ImageRecord | null>(`/image/${id}`, payload);
}

export async function clearRecycleBin(): Promise<void> {
	return apiDelete('/image/recyclebin/clear');
}

export async function restoreRecycleBin(): Promise<{ restoredCount: number }> {
	return apiPost<undefined, { restoredCount: number }>('/image/recyclebin/restore', undefined);
}

export async function purgeOrphanedIndexDocuments(): Promise<{ purgedCount: number }> {
	return apiDeleteForJson<{ purgedCount: number }>('/image/index/orphans');
}

export async function search(q: string): Promise<ImageRecord[]> {
	return apiGet<ImageRecord[]>(`/image/search?q=${encodeURIComponent(q)}`);
}
