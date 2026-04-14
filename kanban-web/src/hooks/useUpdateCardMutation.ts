import { useMutation } from "@tanstack/react-query";
import { updateCard } from "../services/cardService";

export function useUpdateCardMutation() {
	return useMutation({
		mutationFn: updateCard,
	});
}
