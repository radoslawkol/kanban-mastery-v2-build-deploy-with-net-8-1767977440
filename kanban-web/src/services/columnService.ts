import apiClient from "./apiClient";
import type {
	CreateColumnRequest,
	CreateColumnResponse,
	UpdateColumnRequest,
	UpdateColumnResponse,
	DeleteColumnRequest,
} from "../types/board";

export async function createColumn(request: CreateColumnRequest) {
	const { boardId, name, position } = request;
	const response = await apiClient.post<CreateColumnResponse>(
		`/api/boards/${boardId}/columns`,
		{
			name,
			position,
		},
	);
	return response.data;
}

export async function updateColumn(request: UpdateColumnRequest) {
	const { boardId, columnId, name } = request;
	const response = await apiClient.put<UpdateColumnResponse>(
		`/api/boards/${boardId}/columns/${columnId}`,
		{
			name,
		},
	);
	return response.data;
}

export async function deleteColumn(request: DeleteColumnRequest) {
	const { boardId, columnId } = request;
	await apiClient.delete(`/api/boards/${boardId}/columns/${columnId}`);
}
