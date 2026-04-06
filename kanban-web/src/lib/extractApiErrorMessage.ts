import { isAxiosError } from "axios";

type ApiErrorPayload = {
	message?: string;
	title?: string;
	errors?: Record<string, string[] | string>;
};

export function extractApiErrorMessage(
	error: unknown,
	fallback: string,
): string {
	if (!isAxiosError(error)) {
		return fallback;
	}

	const payload = error.response?.data as
		| ApiErrorPayload
		| string
		| undefined;

	if (typeof payload === "string" && payload.trim()) {
		return payload;
	}

	if (payload && typeof payload === "object") {
		if (payload.errors && typeof payload.errors === "object") {
			for (const value of Object.values(payload.errors)) {
				if (Array.isArray(value)) {
					const firstMessage = value.find(
						(message) =>
							typeof message === "string" && message.trim(),
					);
					if (firstMessage) {
						return firstMessage;
					}
				}

				if (typeof value === "string" && value.trim()) {
					return value;
				}
			}
		}

		if (typeof payload.message === "string" && payload.message.trim()) {
			return payload.message;
		}

		if (typeof payload.title === "string" && payload.title.trim()) {
			return payload.title;
		}
	}

	if (typeof error.message === "string" && error.message.trim()) {
		return error.message;
	}

	return fallback;
}
