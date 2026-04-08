import { Link, useParams } from "react-router-dom";
import { useBoardByIdQuery } from "../hooks/useBoardByIdQuery";
import ErrorMessage from "../components/ErrorMessage";
import { extractApiErrorMessage } from "../lib/extractApiErrorMessage";
import BoardColumn from "../components/board/BoardColumn";
import PageContainer from "../components/ui/PageContainer";
import { DragDropContext } from "@hello-pangea/dnd";

export default function BoardPage() {
	const { boardId } = useParams();
	const boardQuery = useBoardByIdQuery(boardId);
	const boardLoadError = extractApiErrorMessage(
		boardQuery.error,
		"We could not load this board. Please try again.",
	);

	if (boardQuery.isLoading) {
		return (
			<PageContainer>
				<p className='text-ink-700'>Loading board...</p>
			</PageContainer>
		);
	}

	if (boardQuery.isError) {
		return (
			<PageContainer>
				<ErrorMessage message={boardLoadError} />
			</PageContainer>
		);
	}

	const board = boardQuery.data;
	if (!board) {
		return (
			<PageContainer>
				<ErrorMessage message='Board not found.' />
			</PageContainer>
		);
	}

	const handleOnDragEnd = () => {};

	return (
		<PageContainer>
			<div className='mb-6 flex items-center justify-between gap-4'>
				<div>
					<h1 className='text-3xl font-bold'>{board.name}</h1>
					<p className='mt-1 text-sm text-ink-600'>
						Board id: {board.id}
					</p>
				</div>
				<Link
					to='/dashboard'
					className='rounded-lg border border-surface-300 bg-white px-3 py-2 text-sm text-ink-700 transition hover:border-primary-600 hover:text-primary'
				>
					Back to dashboard
				</Link>
			</div>

			<DragDropContext onDragEnd={handleOnDragEnd}>
				{board.columns.length === 0 ? (
					<div className='rounded-xl border border-dashed border-surface-300 p-5 text-sm text-ink-700'>
						This board has no columns yet.
					</div>
				) : (
					<div className='flex gap-5 overflow-x-auto pb-2'>
						{board.columns.map((column) => (
							<BoardColumn key={column.id} column={column} />
						))}
					</div>
				)}
			</DragDropContext>
		</PageContainer>
	);
}
