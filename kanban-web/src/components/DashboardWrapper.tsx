import type { ReactNode } from "react";

interface DashboardWrapperProps {
	title?: string;
	subtitle?: string;
	children: ReactNode;
}

export default function DashboardWrapper({
	title,
	subtitle,
	children,
}: DashboardWrapperProps) {
	return (
		<div className='min-h-screen bg-(--app-page-bg) px-6 py-8 text-ink-900'>
			<div className='mx-auto w-full max-w-5xl rounded-2xl border border-surface-300 bg-(--app-surface-bg) p-8 shadow-(--app-surface-shadow)'>
				{title ? <h1 className='text-3xl font-bold'>{title}</h1> : null}
				{subtitle ? (
					<p className='mt-2 text-ink-600'>{subtitle}</p>
				) : null}
				{children}
			</div>
		</div>
	);
}
