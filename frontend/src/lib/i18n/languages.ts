export type Language = {
	value: string;
	label: string;
};

export const availableLanguages: Language[] = [
	{ value: 'en', label: 'English' },
	{ value: 'zh', label: '中文' },
	{ value: 'ko', label: '한국어' },
	{ value: 'ja', label: '日本語' },
	{ value: 'ms', label: 'Bahasa Melayu' },
	{ value: 'pt', label: 'Português' },
	{ value: 'es', label: 'Español' },
	{ value: 'fr', label: 'Français' }
];
