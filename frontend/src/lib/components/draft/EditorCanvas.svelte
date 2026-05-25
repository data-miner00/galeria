<script lang="ts">
	import { onMount, tick } from 'svelte';
	import { saveImage, saveDraft } from '$lib/api/drafts';
	import type { Draft } from '$lib/types';

	type ObjType = 'text' | 'image' | 'rect' | 'line';

	type CanvasObject = {
		id: string;
		type: ObjType;
		x: number;
		y: number;
		width: number;
		height: number;
		rotation: number;
		scale: number;
		color?: string;
		text?: string;
		fontSize?: number;
		src?: string; // for image
	};

	let bgColor = '#ffffff';
	let aspect = '1:1';
	let canvasWidth = 800;
	let canvasHeight = 800;

	let objects: CanvasObject[] = [];
	let selectedId: string | null = null;

	let dragState: { id?: string; offsetX?: number; offsetY?: number } = {};

	function setAspect(a: string) {
		aspect = a;
		if (a === '1:1') {
			canvasWidth = 800;
			canvasHeight = 800;
		} else if (a === '16:9') {
			canvasWidth = 1280;
			canvasHeight = 720;
		} else if (a === '3:4') {
			canvasWidth = 720;
			canvasHeight = 960;
		} else if (a === '4:3') {
			canvasWidth = 960;
			canvasHeight = 720;
		}
	}

	 async function addText() {
		const id = String(Date.now());
		objects = [
			...objects,
			{
				id,
				type: 'text',
				x: canvasWidth / 2 - 100,
				y: canvasHeight / 2 - 20,
				width: 200,
				height: 40,
				rotation: 0,
				scale: 1,
				color: '#000000',
				text: 'Double-click to edit',
				fontSize: 24
			}
		];
		selectedId = id;
		await tick();
		focusEditable(id);
	}

	function addRect() {
		const id = String(Date.now());
		objects = [
			...objects,
			{
				id,
				type: 'rect',
				x: 50,
				y: 50,
				width: 200,
				height: 120,
				rotation: 0,
				scale: 1,
				color: '#FF7A7A'
			}
		];
		selectedId = id;
	}

	function onFileChange(e: Event) {
		const input = e.target as HTMLInputElement;
		const file = input.files && input.files[0];
		if (!file) return;
		const reader = new FileReader();
		reader.onload = () => {
			const src = String(reader.result);
			const id = String(Date.now());
			const img = new Image();
			img.onload = () => {
				objects = [
					...objects,
					{
						id,
						type: 'image',
						x: 20,
						y: 20,
						width: img.width,
						height: img.height,
						rotation: 0,
						scale: Math.min(300 / img.width, 300 / img.height, 1),
						src
					}
				];
				selectedId = id;
			};
			img.src = src;
		};
		reader.readAsDataURL(file);
		input.value = '';
	}

	function pointerDown(e: PointerEvent, obj: CanvasObject) {
		// if user clicked into an editable element, only prevent drag when it's already focused
		const target = e.target as HTMLElement;
		const editable = target.closest && (target.closest('[contenteditable]') as HTMLElement | null);
		if (editable) {
			// if the editable element is focused, we want to edit instead of dragging
			if (document.activeElement === editable) {
				selectedId = obj.id;
				return;
			}
			// otherwise allow single-click drag and select the object
		}

		(e.target as Element).setPointerCapture(e.pointerId);
		dragState = { id: obj.id, offsetX: e.clientX - obj.x, offsetY: e.clientY - obj.y };
		selectedId = obj.id;
	}

	function focusEditable(id: string) {
		// focus the contenteditable element for the given object id and place caret at end
		const sel = document.querySelector(`[data-objid="${id}"]`) as HTMLElement | null;
		if (!sel) return;
		sel.focus();
		try {
			const range = document.createRange();
			range.selectNodeContents(sel);
			range.collapse(false);
			const s = window.getSelection();
			s && (s.removeAllRanges(), s.addRange(range));
		} catch (e) {
			// ignore
		}
	}

	function pointerMove(e: PointerEvent) {
		if (!dragState.id) return;
		const id = dragState.id;
		const o = objects.find((x) => x.id === id);
		if (!o) return;
		o.x = e.clientX - (dragState.offsetX ?? 0);
		o.y = e.clientY - (dragState.offsetY ?? 0);
		objects = objects.map((x) => (x.id === id ? o : x));
	}

	function pointerUp(e: PointerEvent) {
		dragState = {};
	}

	function updateSelected(partial: Partial<CanvasObject>) {
		if (!selectedId) return;
		objects = objects.map((o) => (o.id === selectedId ? { ...o, ...partial } : o));
	}

	async function exportToBlob(): Promise<Blob> {
		const scale = 1; // export at canvas size
		const c = document.createElement('canvas');
		c.width = Math.round(canvasWidth * scale);
		c.height = Math.round(canvasHeight * scale);
		const ctx = c.getContext('2d');
		if (!ctx) throw new Error('No canvas context');
		ctx.fillStyle = bgColor;
		ctx.fillRect(0, 0, c.width, c.height);

		for (const o of objects) {
			ctx.save();
			ctx.translate(o.x + o.width / 2, o.y + o.height / 2);
			ctx.rotate(((o.rotation || 0) * Math.PI) / 180);
			ctx.scale(o.scale || 1, o.scale || 1);
			if (o.type === 'image' && o.src) {
				const img = await loadImage(o.src);
				ctx.drawImage(img, -o.width / 2, -o.height / 2, o.width, o.height);
			} else if (o.type === 'text') {
				ctx.fillStyle = o.color || '#000';
				ctx.font = `${o.fontSize || 24}px sans-serif`;
				ctx.textAlign = 'center';
				ctx.textBaseline = 'middle';
				ctx.fillText(o.text || '', 0, 0);
			} else if (o.type === 'rect') {
				ctx.fillStyle = o.color || '#000';
				ctx.fillRect(-o.width / 2, -o.height / 2, o.width, o.height);
			}
			ctx.restore();
		}

		return await new Promise<Blob>((res) => c.toBlob((b) => b && res(b), 'image/png'));
	}

	function loadImage(src: string): Promise<HTMLImageElement> {
		return new Promise((res, rej) => {
			const img = new Image();
			img.crossOrigin = 'anonymous';
			img.onload = () => res(img);
			img.onerror = rej;
			img.src = src;
		});
	}

	async function saveAsImage() {
		try {
			const blob = await exportToBlob();
			await saveImage(blob, 'draft-export.png');
			alert('Image saved to server');
		} catch (err) {
			console.error(err);
			alert('Save failed');
		}
	}

	async function saveAsDraft() {
		try {
			const blob = await exportToBlob();
			const project = { bgColor, aspect, canvasWidth, canvasHeight, objects };
			await saveDraft(project, blob, 'Draft ' + new Date().toISOString());
			alert('Draft saved');
		} catch (err) {
			console.error(err);
			alert('Save draft failed');
		}
	}
</script>

<div class="editor-wrap">
	<div class="toolbar">
		<div style="display:flex;gap:8px;flex-direction:column">
			<label>Background</label>
			<input type="color" bind:value={bgColor} />
			<label>Aspect</label>
			<select bind:value={aspect} on:change={(e) => setAspect(aspect)}>
				<option>1:1</option>
				<option>16:9</option>
				<option>3:4</option>
				<option>4:3</option>
			</select>
			<button on:click={addText}>Add Text</button>
			<button on:click={addRect}>Add Shape</button>
			<label>Attach Image</label>
			<input type="file" accept="image/*" on:change={onFileChange} />
			<hr />
			<div>
				<button on:click={saveAsImage}>Save As Image</button>
				<button on:click={saveAsDraft}>Save Draft</button>
			</div>
		</div>
	</div>

	<div style="flex:1">
		<div
			class="canvas"
			style="width:{canvasWidth}px;height:{canvasHeight}px;background:{bgColor};"
			on:pointermove={pointerMove}
			on:pointerup={pointerUp}
			on:contextmenu|preventDefault
		>
			{#each objects as o (o.id)}
				<div
					class="obj {selectedId === o.id ? 'selected' : ''}"
					on:pointerdown|preventDefault={(e) => pointerDown(e, o)}
					style="left:{o.x}px;top:{o.y}px;width:{o.width}px;height:{o.height}px;transform:rotate({o.rotation}deg) scale({o.scale});"
				>
					{#if o.type === 'text'}
						<div
							contenteditable
							data-objid={o.id}
							tabindex="0"
							role="textbox"
							on:dblclick={() => focusEditable(o.id)}
							on:input={(e) => updateSelected({ text: (e.target as HTMLDivElement).innerText })}
							on:blur={(e) => updateSelected({ text: (e.target as HTMLDivElement).innerText })}
							style="font-size:{o.fontSize}px;color:{o.color};min-width:20px;min-height:20px;padding:2px"
						>
							{o.text}
						</div>
					{:else if o.type === 'image'}
						<img src={o.src} alt="" style="max-width:100%;max-height:100%;pointer-events:none;" />
					{:else if o.type === 'rect'}
						<div style="width:100%;height:100%;background:{o.color}"></div>
					{/if}
				</div>
			{/each}
		</div>

		{#if selectedId}
			{@const sel = objects.find((x) => x.id === selectedId)}
			{#if sel}
				<div style="margin-top:8px;display:flex;gap:12px;align-items:center;flex-wrap:wrap">
					<div style="display:flex;align-items:center;gap:8px">
						<label style="font-weight:600">Rotate</label>
						<input
							type="range"
							min="0"
							max="360"
							bind:value={sel.rotation}
							on:input={(e) =>
								updateSelected({ rotation: Number((e.target as HTMLInputElement).value) })}
							style="width:160px"
						/>
					</div>
					<div style="display:flex;align-items:center;gap:8px">
						<label style="font-weight:600">Scale</label>
						<input
							type="range"
							min="0.1"
							max="3"
							step="0.1"
							bind:value={sel.scale}
							on:input={(e) =>
								updateSelected({ scale: Number((e.target as HTMLInputElement).value) })}
							style="width:160px"
						/>
					</div>
					{#if sel.type === 'text'}
						<div style="display:flex;align-items:center;gap:8px">
							<label style="font-weight:600">Text color</label>
							<input
								type="color"
								value={sel.color}
								on:input={(e) => updateSelected({ color: (e.target as HTMLInputElement).value })}
							/>
						</div>
						<div style="display:flex;align-items:center;gap:8px">
							<label style="font-weight:600">Font size</label>
							<input
								type="number"
								min="8"
								max="200"
								value={sel.fontSize}
								on:input={(e) =>
									updateSelected({ fontSize: Number((e.target as HTMLInputElement).value) })}
								style="width:72px"
							/>
						</div>
					{/if}
				</div>
			{/if}
		{/if}
	</div>
</div>

<style>
	.editor-wrap {
		display: flex;
		gap: 12px;
	}
	.toolbar {
		width: 240px;
		background: #fbfbff;
		border: 1px solid #eef2ff;
		padding: 12px;
		border-radius: 8px;
		box-shadow: 0 1px 2px rgba(16, 24, 40, 0.04);
	}
	.canvas {
		position: relative;
		background: var(--bg, #fff);
		border: 1px solid #ddd;
	}

	button {
		background: #2563eb;
		color: white;
		border: none;
		padding: 8px 10px;
		border-radius: 6px;
		cursor: pointer;
	}
	button:hover {
		opacity: 0.95;
	}

	input[type='file'] {
		padding: 6px 0;
	}
	.obj {
		position: absolute;
		touch-action: none;
		user-select: none;
	}
	.selected {
		outline: 2px dashed #3b82f6;
	}
</style>
