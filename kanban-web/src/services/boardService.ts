import apiClient from "./apiClient";
import type {
	BoardDetail,
	InviteBoardMemberRequest,
	InviteBoardMemberResponse,
	UserBoard,
} from "../types/board";

export async function getCurrentUserBoards() {
	const response = await apiClient.get<UserBoard[]>("/api/boards/");
	return response.data;
}

export async function getBoardById(boardId: string) {
	const response = await apiClient.get<BoardDetail>(`/api/boards/${boardId}`);
	return response.data;
}

export async function inviteBoardMember(request: InviteBoardMemberRequest) {
	const { boardId, email } = request;
	const response = await apiClient.post<InviteBoardMemberResponse>(
		`/api/boards/${boardId}/members`,
		{ email },
	);

	return response.data;
}
