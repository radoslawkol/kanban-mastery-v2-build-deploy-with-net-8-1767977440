import { Link, useParams } from "react-router-dom";
import { useBoardByIdQuery } from "../hooks/useBoardByIdQuery";
import ErrorMessage from "../components/ErrorMessage";
import { extractApiErrorMessage } from "../lib/extractApiErrorMessage";
import BoardColumn from "../components/board/BoardColumn";
import PageContainer from "../components/ui/PageContainer";
import { DragDropContext, type DropResult } from "@hello-pangea/dnd";
import { useState, useEffect } from "react";

export default function BoardPage() {
	const { boardId } = useParams();
	const boardQuery = useBoardByIdQuery(boardId);
	const boardLoadError = extractApiErrorMessage(
		boardQuery.error,
		"We could not load this board. Please try again.",
	);
	const [columns, setColumns] = useState(boardQuery.data?.columns);

	useEffect(() => {
		if (boardQuery.data?.columns) {
			setColumns(boardQuery.data.columns);
		}
	}, [boardQuery.data]);

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

	const handleOnDragEnd = (result: DropResult<string>) => {
		const { destination, source, draggableId } = result;

		if (!destination) {
			return;
		}

		if (
			destination.droppableId === source.droppableId &&
			destination.index === source.index
		) {
			return;
		}

		// Create a deep copy of columns to avoid mutating state
		const newColumns =
			columns?.map((col) => ({
				...col,
				cards: [...col.cards],
			})) ?? [];

		// Find source and destination columns
		const sourceColumnIndex = newColumns.findIndex(
			(col) => col.id === source.droppableId,
		);
		const destColumnIndex = newColumns.findIndex(
			(col) => col.id === destination.droppableId,
		);

		if (sourceColumnIndex === -1 || destColumnIndex === -1) {
			return;
		}

		const sourceColumn = newColumns[sourceColumnIndex];
		const destColumn = newColumns[destColumnIndex];

		// Find the card being dragged
		const cardIndex = sourceColumn.cards.findIndex(
			(card) => card.id === draggableId,
		);
		if (cardIndex === -1) {
			return;
		}

		const [movedCard] = sourceColumn.cards.splice(cardIndex, 1);

		// Insert card into destination column at the new index
		destColumn.cards.splice(destination.index, 0, movedCard);

		// Update order for all cards in source column
		sourceColumn.cards.forEach((card, idx) => {
			card.order = idx;
		});

		// Update order for all cards in destination column
		destColumn.cards.forEach((card, idx) => {
			card.order = idx;
		});

		// Update state with new columns
		setColumns(newColumns);

		// TODO: Call endpoint to update card order in the backend
	};

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

			<DragDropContext onDragEnd={(result) => handleOnDragEnd(result)}>
				{board.columns.length === 0 ? (
					<div className='rounded-xl border border-dashed border-surface-300 p-5 text-sm text-ink-700'>
						This board has no columns yet.
					</div>
				) : (
					<div className='flex gap-5 overflow-x-auto pb-2'>
						{columns?.map((column) => (
							<BoardColumn key={column.id} column={column} />
						))}
					</div>
				)}
			</DragDropContext>
		</PageContainer>
	);
}
