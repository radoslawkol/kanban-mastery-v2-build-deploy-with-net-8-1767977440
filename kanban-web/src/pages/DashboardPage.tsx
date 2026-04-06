import { useUserBoardsQuery } from "../hooks/useUserBoardsQuery";
import BoardCardList from "../components/BoardCardList";
import { extractApiErrorMessage } from "../lib/extractApiErrorMessage";

export default function DashboardPage() {
	const boardsQuery = useUserBoardsQuery();
	const boards = boardsQuery.data ?? [];
	const boardsLoadError = extractApiErrorMessage(
		boardsQuery.error,
		"We could not load your boards. Please refresh the page.",
	);

	if (boardsQuery.isLoading) {
		return (
			<div className='min-h-screen bg-(--auth-page-bg) px-6 py-8 text-ink-900'>
				<div className='mx-auto w-full max-w-5xl rounded-2xl border border-surface-300 bg-(--auth-card-bg) p-8 shadow-(--auth-card-shadow)'>
					<p className='text-ink-700'>Loading your boards...</p>
				</div>
			</div>
		);
	}

	if (boardsQuery.isError) {
		return (
			<div className='min-h-screen bg-(--auth-page-bg) px-6 py-8 text-ink-900'>
				<div className='mx-auto w-full max-w-5xl rounded-2xl border border-surface-300 bg-(--auth-card-bg) p-8 shadow-(--auth-card-shadow)'>
					<h1 className='text-3xl font-bold'>Dashboard</h1>
					<p className='mt-2 text-danger-700'>{boardsLoadError}</p>
				</div>
			</div>
		);
	}

	return (
		<div className='min-h-screen bg-(--auth-page-bg) px-6 py-8 text-ink-900'>
			<div className='mx-auto w-full max-w-5xl rounded-2xl border border-surface-300 bg-(--auth-card-bg) p-8 shadow-(--auth-card-shadow)'>
				<h1 className='text-3xl font-bold'>Dashboard</h1>
				<p className='mt-2 text-ink-600'>
					Your home base for all boards.
				</p>

				{boards.length === 0 ? (
					<div className='mt-6 rounded-xl border border-dashed border-surface-300 bg-surface-50 p-6'>
						<p className='text-ink-700'>
							No boards yet. Create your first board to get
							started.
						</p>
					</div>
				) : (
					BoardCardList({ boards })
				)}
			</div>
		</div>
	);
}
