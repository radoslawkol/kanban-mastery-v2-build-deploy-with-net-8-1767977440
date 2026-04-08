import type { BoardCard } from "../../types/board";

type BoardAssigneeBadgeProps = {
	card: BoardCard;
};

function createInitials(value: string): string {
	const normalized = value.trim();
	if (!normalized) {
		return "?";
	}

	const chunks = normalized.split(/\s+/).filter(Boolean);
	if (chunks.length === 1) {
		return chunks[0]!.slice(0, 2).toUpperCase();
	}

	return `${chunks[0]?.[0] ?? ""}${chunks[1]?.[0] ?? ""}`.toUpperCase();
}

function getAssigneeLabel(card: BoardCard): string | null {
	if (!card.assignedToUser && !card.assignedToUserId) {
		return null;
	}

	if (card.assignedToUser) {
		const label =
			card.assignedToUser.username ??
			card.assignedToUser.email ??
			card.assignedToUser.id ??
			card.assignedToUserId;
		return label ?? null;
	}

	return card.assignedToUserId ?? null;
}

export default function BoardAssigneeBadge({ card }: BoardAssigneeBadgeProps) {
	const assigneeLabel = getAssigneeLabel(card);

	if (!assigneeLabel) {
		return (
			<span className='rounded-full border border-dashed border-surface-300 px-2 py-1 text-xs text-ink-600'>
				Unassigned
			</span>
		);
	}

	return (
		<div
			title={assigneeLabel}
			className='inline-flex h-8 w-8 items-center justify-center overflow-hidden rounded-full border border-surface-300 bg-primary-600 text-xs font-semibold text-white'
		>
			{createInitials(assigneeLabel)}
		</div>
	);
}
