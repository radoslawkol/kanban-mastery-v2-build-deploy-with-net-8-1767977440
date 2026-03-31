import type { ButtonHTMLAttributes } from "react";

interface FormButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
	isLoading?: boolean;
	loadingText?: string;
}

export default function FormButton({
	children,
	isLoading = false,
	loadingText = "Loading...",
	disabled,
	...props
}: FormButtonProps) {
	return (
		<button
			type='submit'
			disabled={isLoading || disabled}
			className='mt-4 rounded-lg bg-linear-to-r from-primary-600 to-accent-500 px-4 py-3 text-base font-bold text-white transition hover:-translate-y-0.5 disabled:cursor-not-allowed disabled:opacity-75 disabled:hover:translate-y-0 hover:cursor-pointer'
			{...props}
		>
			{isLoading ? loadingText : children}
		</button>
	);
}
