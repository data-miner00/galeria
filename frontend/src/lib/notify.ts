import { type ExternalToast, toast as sonnerToast } from 'svelte-sonner';

import { addNotification } from './states.svelte';
import type { NotificationType } from './types';

// Factory method that notifies and keeps track of the notification in app state.
function notify(type: NotificationType) {
	return (message: string, options?: ExternalToast) => {
		addNotification({
			id: crypto.randomUUID(),
			type,
			title: message,
			description: typeof options?.description === 'string' ? options.description : undefined,
			createdAt: new Date().toISOString(),
			isRead: false
		});
		return sonnerToast[type](message, options);
	};
}

/** Drop-in replacement for svelte-sonner's `toast` that also records the notification in app state. */
export const toast = {
	success: notify('success'),
	error: notify('error'),
	info: notify('info'),
	warning: notify('warning')
};
