import { useMutation, useQueryClient } from "@tanstack/react-query";
import { createBoard } from "../services/boardService";

export function useCreateBoardMutation() {
	const queryClient = useQueryClient();

	return useMutation({
		mutationFn: createBoard,
		onSuccess: () => {
			queryClient.invalidateQueries({ queryKey: ["userBoards"] });
		},
	});
}
