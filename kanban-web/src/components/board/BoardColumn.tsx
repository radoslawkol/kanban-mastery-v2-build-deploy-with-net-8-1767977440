import { Droppable } from "@hello-pangea/dnd";
import type { BoardColumn } from "../../types/board";
import BoardCardItem from "./BoardCardItem";

type BoardColumnProps = {
	column: BoardColumn;
};

export default function BoardColumn({ column }: BoardColumnProps) {
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
		</section>
	);
}
