import { useUserBoardsQuery } from "../hooks/useUserBoardsQuery";
import BoardCardList from "../components/BoardCardList";
import DashboardWrapper from "../components/DashboardWrapper";
import ErrorMessage from "../components/ErrorMessage";
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
			<DashboardWrapper>
				<p className='text-ink-700'>Loading your boards...</p>
			</DashboardWrapper>
		);
	}

	if (boardsQuery.isError) {
		return (
			<DashboardWrapper title='Dashboard'>
				<ErrorMessage message={boardsLoadError} />
			</DashboardWrapper>
		);
	}

	return (
		<DashboardWrapper
			title='Dashboard'
			subtitle='Your home base for all boards.'
		>
			{boards.length === 0 ? (
				<div className='mt-6 rounded-xl border border-dashed border-surface-300 bg-surface-50 p-6'>
					<p className='text-ink-700'>
						No boards yet. Create your first board to get started.
					</p>
				</div>
			) : (
				<BoardCardList boards={boards} />
			)}
		</DashboardWrapper>
	);
}
