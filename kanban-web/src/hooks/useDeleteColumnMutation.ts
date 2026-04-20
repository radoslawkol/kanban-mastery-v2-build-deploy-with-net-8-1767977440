import { useMutation, useQueryClient } from "@tanstack/react-query";
import { deleteColumn } from "../services/columnService";

export function useDeleteColumnMutation() {
	const queryClient = useQueryClient();

	return useMutation({
		mutationFn: deleteColumn,
		onSuccess: (_, variables) => {
			queryClient.invalidateQueries({
				queryKey: ["board", variables.boardId],
			});
		},
	});
}
