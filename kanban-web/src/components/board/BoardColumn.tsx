import { Droppable } from "@hello-pangea/dnd";
import { useState } from "react";
import type { SubmitEvent } from "react";
import type { BoardColumn } from "../../types/board";
import { extractApiErrorMessage } from "../../lib/extractApiErrorMessage";
import BoardCardItem from "./BoardCardItem";

type BoardColumnProps = {
	column: BoardColumn;
	onCreateCard: (title: string) => Promise<void>;
	isCreatingCard?: boolean;
};

export default function BoardColumn({
	column,
	onCreateCard,
	isCreatingCard = false,
}: BoardColumnProps) {
	const [isFormVisible, setIsFormVisible] = useState(false);
	const [newCardTitle, setNewCardTitle] = useState("");
	const [createCardError, setCreateCardError] = useState<string | null>(null);

	const handleCreateCardSubmit = async (
		event: SubmitEvent<HTMLFormElement>,
	) => {
		event.preventDefault();
		const trimmedTitle = newCardTitle.trim();

		if (!trimmedTitle) {
			return;
		}

		setCreateCardError(null);

		try {
			await onCreateCard(trimmedTitle);
			setNewCardTitle("");
			setIsFormVisible(false);
		} catch (error) {
			setCreateCardError(
				extractApiErrorMessage(
					error,
					"We could not create this card. Please try again.",
				),
			);
		}
	};

	return (
		<section className='flex flex-col w-72 shrink-0 rounded-xl border border-surface-300 bg-surface-50 p-4'>
			<header className='mb-4 flex items-center justify-between'>
				<h2 className='text-lg font-semibold text-ink-900'>
					{column.name}
				</h2>
				<span className='rounded-full bg-white px-2 py-1 text-xs text-ink-600'>
					{column.cards.length}
				</span>
			</header>
			{column.cards.length === 0 ? (
				<p className='mt-3 rounded-lg border border-dashed border-surface-300 p-3 text-sm text-ink-600'>
					No cards in this column yet.
				</p>
			) : null}

			<Droppable droppableId={column.id}>
				{(provided, snapshot) => (
					<div
						className={`grow space-y-3 transition-colors delay-200 ${snapshot.isDraggingOver ? "bg-gray-100" : ""}`}
						ref={provided.innerRef}
						{...provided.droppableProps}
					>
						{column.cards.map((card) => (
							<BoardCardItem key={card.id} card={card} />
						))}
						{provided.placeholder}
					</div>
				)}
			</Droppable>

			{isFormVisible ? (
				<form
					onSubmit={handleCreateCardSubmit}
					className='mt-4 flex flex-col gap-2'
				>
					<input
						type='text'
						value={newCardTitle}
						onChange={(event) => {
							setNewCardTitle(event.target.value);
							if (createCardError) {
								setCreateCardError(null);
							}
						}}
						placeholder='Card title'
						className='w-full rounded-lg border border-surface-300 bg-white px-3 py-2 text-sm text-ink-900 placeholder:text-ink-500 focus:border-primary focus:outline-none focus:ring-2 focus:ring-primary-200'
						disabled={isCreatingCard}
						required
						autoFocus
					/>
					<button
						type='submit'
						disabled={isCreatingCard || !newCardTitle.trim()}
						className='rounded-lg border border-primary bg-primary px-3 py-2 text-sm font-medium text-white transition hover:bg-primary-700 hover:cursor-pointer disabled:cursor-not-allowed disabled:opacity-60'
					>
						{isCreatingCard ? "Creating..." : "Create Card"}
					</button>
					{createCardError ? (
						<p className='text-sm text-danger'>{createCardError}</p>
					) : null}
				</form>
			) : (
				<button
					type='button'
					onClick={() => setIsFormVisible(true)}
					className='mt-4 rounded-lg border border-dashed border-surface-400 bg-white px-3 py-2 text-sm font-medium text-ink-700 transition hover:border-primary-600 hover:text-primary hover:cursor-pointer'
				>
					Create Card
				</button>
			)}
		</section>
	);
}
