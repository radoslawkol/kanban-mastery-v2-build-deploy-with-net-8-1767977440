import type { ReactNode } from "react";

type PageContainerProps = {
	children: ReactNode;
	outerClassName?: string;
	innerclassName?: string;
};

export default function PageContainer({
	children,
	outerClassName = "",
	innerclassName = "",
}: PageContainerProps) {
	return (
		<div
			className={`min-h-screen bg-(--app-page-bg) px-6 py-8 text-ink-900 ${outerClassName}`}
		>
			<div
				className={`mx-auto w-full max-w-7xl rounded-2xl border border-surface-300 bg-(--app-surface-bg) p-8 shadow-(--app-surface-shadow) ${innerclassName}`}
			>
				{children}
			</div>
		</div>
	);
}
