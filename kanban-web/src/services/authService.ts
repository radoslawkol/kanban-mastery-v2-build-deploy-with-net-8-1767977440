import apiClient from "./apiClient";
import type { AuthResponse, RegisterRequest } from "../types/auth";

export async function registerUser(
	payload: RegisterRequest,
): Promise<AuthResponse> {
	const response = await apiClient.post<AuthResponse>("/register", payload);
	return response.data;
}
