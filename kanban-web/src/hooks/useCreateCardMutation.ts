import { useMutation } from "@tanstack/react-query";
import { createCard } from "../services/cardService";

export function useCreateCardMutation() {
	return useMutation({
		mutationFn: createCard,
	});
}
