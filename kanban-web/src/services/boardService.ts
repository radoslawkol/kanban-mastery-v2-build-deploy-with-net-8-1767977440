import apiClient from "./apiClient";
import type { BoardDetail, UserBoard } from "../types/board";

export async function getCurrentUserBoards() {
	const response = await apiClient.get<UserBoard[]>("/api/boards/");
	return response.data;
}

export async function getBoardById(boardId: string) {
	const response = await apiClient.get<BoardDetail>(`/api/boards/${boardId}`);
	return response.data;
}
