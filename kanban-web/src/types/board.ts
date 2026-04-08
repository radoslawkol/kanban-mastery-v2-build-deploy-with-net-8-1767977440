export type UserBoard = {
	id: string;
	name: string;
};

export type CardAssignee = {
	id?: string;
	username?: string;
	email?: string;
};

export type BoardCard = {
	id: string;
	title: string;
	description: string;
	order: number;
	assignedToUserId?: string;
	assignedToUser?: CardAssignee;
};

export type BoardColumn = {
	id: string;
	name: string;
	order: number;
	cards: BoardCard[];
};

export type BoardDetail = {
	id: string;
	name: string;
	columns: BoardColumn[];
};
