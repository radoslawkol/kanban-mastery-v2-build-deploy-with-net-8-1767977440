import { useMutation, useQueryClient } from "@tanstack/react-query";
import { updateColumn } from "../services/columnService";

export function useUpdateColumnMutation() {
	const queryClient = useQueryClient();

	return useMutation({
		mutationFn: updateColumn,
		onSuccess: (_, variables) => {
			queryClient.invalidateQueries({
				queryKey: ["board", variables.boardId],
			});
		},
	});
}
