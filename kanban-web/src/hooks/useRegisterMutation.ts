import { useMutation } from "@tanstack/react-query";
import { registerUser } from "../services/authService";

export function useRegisterUser() {
	return useMutation({
		mutationFn: registerUser,
	});
}
