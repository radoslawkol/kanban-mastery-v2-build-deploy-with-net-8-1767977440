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

export type CreateColumnRequest = {
	boardId: string;
	name: string;
	position?: number;
};

export type CreateColumnResponse = {
	id: string;
	name: string;
	order: number;
	boardId: string;
};

export type UpdateColumnRequest = {
	boardId: string;
	columnId: string;
	name: string;
};

export type UpdateColumnResponse = {
	id: string;
	name: string;
	order: number;
	boardId: string;
};

export type DeleteColumnRequest = {
	boardId: string;
	columnId: string;
};

export type InviteBoardMemberRequest = {
	boardId: string;
	email: string;
};

export type InviteBoardMemberResponse = {
	message: string;
};
