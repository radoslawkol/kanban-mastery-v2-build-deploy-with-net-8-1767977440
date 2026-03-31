import { useMutation } from "@tanstack/react-query";
import { loginUser } from "../services/authService";

export function useLoginUser() {
	return useMutation({
		mutationFn: loginUser,
		onSuccess: (data) => {
			localStorage.setItem("token", data.accessToken);
		},
	});
}
