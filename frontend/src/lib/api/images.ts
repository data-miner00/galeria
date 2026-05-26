import { PUBLIC_API_BASE_URL } from '$env/static/public';

const BASE = `${PUBLIC_API_BASE_URL}/api/v1/image`;

export async function fetchAll(): Promise<any> {
	const res = await fetch(BASE);
	if (!res.ok) throw new Error(await res.text());
	return res.json();
}

export async function upload(formData: FormData): Promise<any> {
	const res = await fetch(BASE, { method: 'POST', body: formData });
	if (!res.ok) throw new Error(await res.text());
	return res.json();
}

export async function getByIds(imageIds: string[]): Promise<any> {
	const res = await fetch(`${BASE}/getbyids`, {
		method: 'POST',
		headers: { 'Content-Type': 'application/json' },
		body: JSON.stringify({ imageIds })
	});
	if (!res.ok) throw new Error(await res.text());
	return res.json();
}

export async function downloadAll(): Promise<Response> {
	return fetch(`${BASE}/blob/download`);
}

export async function downloadMultiple(requestedIds: string[]): Promise<Response> {
	return fetch(`${BASE}/blob/download/multiple`, {
		method: 'POST',
		headers: { 'Content-Type': 'application/json' },
		body: JSON.stringify({ requestedIds })
	});
}

export async function deleteByIds(requestedIds: string[], isSoftDelete = true): Promise<any> {
	const res = await fetch(BASE, {
		method: 'DELETE',
		headers: { 'Content-Type': 'application/json' },
		body: JSON.stringify({ requestedIds, isSoftDelete })
	});
	if (!res.ok) throw new Error(await res.text());
	return res.json().catch(() => null);
}

export async function deleteById(id: string): Promise<void> {
	const res = await fetch(`${BASE}/${id}`, { method: 'DELETE' });
	if (!res.ok) throw new Error(await res.text());
}

export async function patchImage(id: string, payload: Record<string, any>): Promise<any> {
	const res = await fetch(`${BASE}/${id}`, {
		method: 'PATCH',
		headers: { 'Content-Type': 'application/json' },
		body: JSON.stringify(payload)
	});
	if (!res.ok) throw new Error(await res.text());
	return res.json().catch(() => null);
}

export async function clearRecycleBin(): Promise<void> {
	const res = await fetch(`${BASE}/recyclebin/clear`, { method: 'DELETE' });
	if (!res.ok) throw new Error(await res.text());
}

export async function search(q: string): Promise<any> {
	const res = await fetch(`${BASE}/search?q=${encodeURIComponent(q)}`);
	if (!res.ok) throw new Error(await res.text());
	return res.json();
}

export default {
	fetchAll,
	upload,
	getByIds,
	downloadAll,
	downloadMultiple,
	deleteByIds,
	deleteById,
	patchImage,
	clearRecycleBin,
	search
};
