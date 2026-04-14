import apiClient from "./apiClient";
import type {
	CardCreateRequest,
	CardCreateResponse,
	CardUpdateRequest,
} from "../types/card";

export async function createCard(request: CardCreateRequest) {
	const { boardId, title, columnId, description } = request;
	const response = await apiClient.post<CardCreateResponse>(
		`/api/boards/${boardId}/cards`,
		{
			title,
			description: description ?? "",
			columnId,
		},
	);
	return response.data;
}

export async function updateCard(request: CardUpdateRequest) {
	const { boardId, cardId, title, description, columnId, order } = request;
	const response = await apiClient.put(
		`/api/boards/${boardId}/cards/${cardId}`,
		{
			title,
			description,
			columnId,
			order,
		},
	);
	return response.data;
}
