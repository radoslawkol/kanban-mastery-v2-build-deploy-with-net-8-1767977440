import { useMutation, useQueryClient } from "@tanstack/react-query";
import { createColumn } from "../services/columnService";

export function useCreateColumnMutation() {
	const queryClient = useQueryClient();

	return useMutation({
		mutationFn: createColumn,
		onSuccess: (_, variables) => {
			queryClient.invalidateQueries({
				queryKey: ["board", variables.boardId],
			});
		},
	});
}
