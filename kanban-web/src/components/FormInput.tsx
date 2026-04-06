import type { InputHTMLAttributes } from "react";

interface FormInputProps extends InputHTMLAttributes<HTMLInputElement> {
	label: string;
	error?: string;
}

export default function FormInput({
	label,
	error,
	id,
	...props
}: FormInputProps) {
	return (
		<>
			<label
				htmlFor={id}
				className='mt-2 text-sm font-semibold text-ink-700'
			>
				{label}
			</label>
			<input
				id={id}
				className='rounded-lg border border-surface-300 bg-white px-3.5 py-3 text-base outline-none transition focus:border-primary-600 focus:ring-4 focus:ring-primary-500/15'
				{...props}
			/>
			{error && (
				<span className='mb-0.5 text-sm text-danger-700'>{error}</span>
			)}
		</>
	);
}
