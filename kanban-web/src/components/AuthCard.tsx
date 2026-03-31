import type { ReactNode } from "react";

interface AuthCardProps {
	title: string;
	subtitle: string;
	children: ReactNode;
}

export default function AuthCard({ title, subtitle, children }: AuthCardProps) {
	return (
		<div className='min-h-screen bg-(--auth-page-bg) px-6 py-8 text-ink-900'>
			<div className='mx-auto flex min-h-[calc(100vh-4rem)] w-full max-w-115 items-center justify-center'>
				<div className='w-full rounded-2xl border border-surface-300 bg-(--auth-card-bg) p-8 shadow-(--auth-card-shadow)'>
					<h1 className='text-3xl font-bold leading-tight'>
						{title}
					</h1>
					<p className='mb-4 mt-2 text-ink-600'>{subtitle}</p>
					{children}
				</div>
			</div>
		</div>
	);
}
