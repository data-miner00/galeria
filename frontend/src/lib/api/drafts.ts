import { PUBLIC_API_BASE_URL } from '$env/static/public';

export async function saveImage(blob: Blob, filename = 'image.png') {
	const fd = new FormData();
	fd.append('File', blob, filename);
	fd.append('Title', 'Draft Image');
	fd.append('IsCensored', false.toString());
	fd.append('IsAutoCaption', false.toString());
	const res = await fetch(`${PUBLIC_API_BASE_URL}/api/v1/image`, { method: 'POST', body: fd });
	if (!res.ok) throw new Error('Upload failed');
	return res.json();
}

export async function saveDraft(project: any, previewBlob?: Blob, title?: string) {
	const fd = new FormData();
	fd.append(
		'project',
		new Blob([JSON.stringify(project)], { type: 'application/json' }),
		'project.json'
	);
	if (previewBlob) fd.append('preview', previewBlob, 'preview.png');
	if (title) fd.append('title', title);
	const res = await fetch('/api/v1/draft', { method: 'POST', body: fd });
	if (!res.ok) throw new Error('Save draft failed');
	return res.json();
}

export async function listDrafts() {
	const res = await fetch('/api/v1/draft');
	if (!res.ok) throw new Error('List drafts failed');
	return res.json();
}
