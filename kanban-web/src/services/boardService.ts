import apiClient from "./apiClient";
import type { UserBoard } from "../types/board";

export async function getCurrentUserBoards() {
	const response = await apiClient.get<UserBoard[]>("/api/boards/");
	return response.data;
}
