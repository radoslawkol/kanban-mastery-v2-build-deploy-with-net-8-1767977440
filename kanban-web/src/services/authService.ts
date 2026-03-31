import apiClient from "./apiClient";
import type {
	AuthResponse,
	RegisterRequest,
	LoginRequest,
} from "../types/auth";

export async function registerUser(
	payload: RegisterRequest,
): Promise<AuthResponse> {
	const response = await apiClient.post<AuthResponse>("/register", payload);
	return response.data;
}

export async function loginUser(payload: LoginRequest): Promise<AuthResponse> {
	const response = await apiClient.post<AuthResponse>("/login", payload);
	return response.data;
}
