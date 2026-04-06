import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useUserBoardsQuery } from "../hooks/useUserBoardsQuery";
import type { UserBoard } from "../types/board";

export default function HomePage() {
	const navigate = useNavigate();
	const boardsQuery = useUserBoardsQuery();
	const [boards, setBoards] = useState<UserBoard[]>([]);

	useEffect(() => {
		if (boardsQuery.data) {
			setBoards(boardsQuery.data);
		}
	}, [boardsQuery.data]);

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
					<p className='mt-2 text-danger-700'>
						We could not load your boards. Please refresh the page.
					</p>
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
					<div className='mt-6 grid gap-4 sm:grid-cols-2 lg:grid-cols-3'>
						{boards.map((board) => (
							<button
								key={board.id}
								type='button'
								onClick={() => navigate(`/board/${board.id}`)}
								className='rounded-xl border border-surface-300 bg-white p-5 text-left transition hover:border-primary-600 hover:shadow-md'
							>
								<h2 className='text-lg font-semibold text-ink-900'>
									{board.name}
								</h2>
								<p className='mt-2 text-sm text-ink-600'>
									Open board details
								</p>
							</button>
						))}
					</div>
				)}
			</div>
		</div>
	);
}
