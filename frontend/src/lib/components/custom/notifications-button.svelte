<script lang="ts">
	import BellIcon from '@lucide/svelte/icons/bell';
	import CircleCheckIcon from '@lucide/svelte/icons/circle-check';
	import InfoIcon from '@lucide/svelte/icons/info';
	import OctagonXIcon from '@lucide/svelte/icons/octagon-x';
	import TriangleAlertIcon from '@lucide/svelte/icons/triangle-alert';

	import { Button, buttonVariants } from '$lib/components/ui/button/index.js';
	import * as Popover from '$lib/components/ui/popover/index.js';
	import {
		appState,
		clearNotifications,
		markAllNotificationsRead,
		markNotificationRead,
		notificationCounts
	} from '$lib/states.svelte';
	import type { NotificationType } from '$lib/types';
	import { cn } from '$lib/utils';

	const icons = {
		success: { icon: CircleCheckIcon, class: 'text-green-600 dark:text-green-500' },
		error: { icon: OctagonXIcon, class: 'text-destructive' },
		info: { icon: InfoIcon, class: 'text-blue-600 dark:text-blue-500' },
		warning: { icon: TriangleAlertIcon, class: 'text-amber-600 dark:text-amber-500' }
	} satisfies Record<NotificationType, unknown>;
</script>

<Popover.Root>
	<Popover.Trigger class={buttonVariants({ variant: 'outline', size: 'icon' }) + ' relative'}>
		<BellIcon class="h-[1.2rem] w-[1.2rem]" />
		{#if notificationCounts.unread > 0}
			<span
				class="absolute -top-1 -right-1 flex h-4 min-w-4 items-center justify-center rounded-full bg-destructive px-1 text-[10px] leading-none font-medium text-white"
			>
				{notificationCounts.unread > 9 ? '9+' : notificationCounts.unread}
			</span>
		{/if}
		<span class="sr-only">Notifications</span>
	</Popover.Trigger>
	<Popover.Content align="end" class="w-80 p-0">
		<div class="flex items-center justify-between border-b px-4 py-2">
			<span class="text-sm font-medium">Notifications</span>
			<div class="flex items-center">
				<Button
					variant="ghost"
					size="sm"
					disabled={notificationCounts.unread === 0}
					onclick={markAllNotificationsRead}
				>
					Read all
				</Button>
				<Button
					variant="ghost"
					size="sm"
					disabled={appState.notifications.length === 0}
					onclick={clearNotifications}
				>
					Clear
				</Button>
			</div>
		</div>
		{#if appState.notifications.length === 0}
			<p class="px-4 py-8 text-center text-sm text-muted-foreground">No notifications yet</p>
		{:else}
			<ul class="max-h-96 divide-y overflow-y-auto">
				{#each appState.notifications as notification (notification.id)}
					{@const { icon: Icon, class: iconClass } = icons[notification.type]}
					<li>
						<button
							type="button"
							class={cn(
								'flex w-full gap-3 px-4 py-3 text-left transition-colors hover:bg-accent',
								notification.isRead ? 'opacity-60' : 'bg-muted/60'
							)}
							onclick={() => markNotificationRead(notification.id)}
						>
							<Icon class="mt-0.5 size-4 shrink-0 {iconClass}" />
							<div class="flex min-w-0 flex-1 flex-col gap-0.5">
								<span
									class={cn('text-sm wrap-break-word', !notification.isRead && 'font-semibold')}
								>
									{notification.title}
								</span>
								{#if notification.description}
									<span class="text-xs wrap-break-word text-muted-foreground">
										{notification.description}
									</span>
								{/if}
								<time class="text-xs text-muted-foreground" datetime={notification.createdAt}>
									{new Date(notification.createdAt).toLocaleTimeString()}
								</time>
							</div>
							{#if !notification.isRead}
								<span class="mt-1.5 size-2 shrink-0 rounded-full bg-primary">
									<span class="sr-only">Unread</span>
								</span>
							{/if}
						</button>
					</li>
				{/each}
			</ul>
		{/if}
	</Popover.Content>
</Popover.Root>
