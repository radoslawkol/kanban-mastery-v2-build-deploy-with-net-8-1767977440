import { useQuery } from "@tanstack/react-query";
import { getBoardById } from "../services/boardService";

export function useBoardByIdQuery(boardId?: string) {
	return useQuery({
		queryKey: ["board", boardId],
		queryFn: () => getBoardById(boardId!),
		staleTime: 1 * 60 * 1000,
	});
}
