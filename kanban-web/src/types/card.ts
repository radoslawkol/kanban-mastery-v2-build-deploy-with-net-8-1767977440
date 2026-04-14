export type CardRequest = {
	title: string;
	description: string;
	columnId: string;
	order: number;
};

export type CardCreateRequest = {
	boardId: string;
	title: string;
	columnId: string;
	description?: string;
};

export type CardCreateResponse = {
	id: string;
	title: string;
	description: string;
	columnId: string;
	order: number;
};

export type CardUpdateRequest = CardRequest & {
	boardId: string;
	cardId: string;
};
