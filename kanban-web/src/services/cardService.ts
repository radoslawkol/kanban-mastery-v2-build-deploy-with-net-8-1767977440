import apiClient from "./apiClient";
import type { CardUpdateRequest } from "../types/card";

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
