import type { BoardCard } from "../../types/board";
import BoardAssigneeBadge from "./BoardAssigneeBadge";
import { Draggable } from "@hello-pangea/dnd";

type BoardCardItemProps = {
	card: BoardCard;
};

export default function BoardDetailCard({ card }: BoardCardItemProps) {
	return (
		<Draggable draggableId={card.id} index={card.order}>
			{(provided) => (
				<article
					className='rounded-lg border border-surface-300 bg-white p-3 shadow-sm'
					{...provided.draggableProps}
					{...provided.dragHandleProps}
					ref={provided.innerRef}
				>
					<h3 className='font-medium text-ink-900'>{card.title}</h3>
					{card.description ? (
						<p className='mt-1 text-sm text-ink-600'>
							{card.description}
						</p>
					) : null}

					<div className='mt-3 flex items-center justify-end'>
						<BoardAssigneeBadge card={card} />
					</div>
				</article>
			)}
		</Draggable>
	);
}
