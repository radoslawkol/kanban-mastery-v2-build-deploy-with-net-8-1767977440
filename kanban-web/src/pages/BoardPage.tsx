import { useParams } from "react-router-dom";

export default function BoardPage() {
	const { boardId } = useParams();

	return (
		<div className='min-h-screen bg-(--auth-page-bg) px-6 py-8 text-ink-900'>
			<div className='mx-auto w-full max-w-5xl rounded-2xl border border-surface-300 bg-(--auth-card-bg) p-8 shadow-(--auth-card-shadow)'>
				<h1 className='text-2xl font-bold'>Board view</h1>
				<p className='mt-2 text-ink-600'>
					Opened board: {boardId}
				</p>
			</div>
		</div>
	);
}
