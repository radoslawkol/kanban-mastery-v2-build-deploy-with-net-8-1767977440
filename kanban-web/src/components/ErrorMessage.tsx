interface ErrorMessageProps {
	message: string;
	className?: string;
}

export default function ErrorMessage({
	message,
	className = "mt-2 text-danger-700",
}: ErrorMessageProps) {
	return <p className={className}>{message}</p>;
}
