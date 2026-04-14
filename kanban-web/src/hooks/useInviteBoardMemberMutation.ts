import { useMutation } from "@tanstack/react-query";
import { inviteBoardMember } from "../services/boardService";

export function useInviteBoardMemberMutation() {
	return useMutation({
		mutationFn: inviteBoardMember,
	});
}
