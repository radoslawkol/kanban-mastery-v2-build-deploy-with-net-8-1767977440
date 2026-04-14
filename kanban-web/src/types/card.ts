export type CardRequest = {
	title: string;
	description: string;
	columnId: string;
	order: number;
};

export type CardUpdateRequest = CardRequest & {
	boardId: string;
	cardId: string;
};
