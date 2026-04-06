import { useQuery } from "@tanstack/react-query";
import { getCurrentUserBoards } from "../services/boardService";

export function useUserBoardsQuery() {
	return useQuery({
		queryKey: ["boards"],
		queryFn: getCurrentUserBoards,
	});
}
