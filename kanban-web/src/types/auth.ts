export type RegisterRequest = {
	username: string;
	email: string;
	password: string;
};

export type LoginRequest = {
	email: string;
	password: string;
};

export type AuthResponse = {
	accessToken: string;
	tokenType: string;
	expiresIn: number;
	refreshToken?: string;
};

export type ApiErrorResponse = {
	message: string;
	statusCode?: number;
	retryAfterSeconds?: number;
};
