import type { UserBoard } from "../types/board";
import BoardCard from "./BoardCard";

type BoardCardListProps = {
	boards: UserBoard[];
};

export default function BoardCardList({ boards }: BoardCardListProps) {
	return (
		<div className='mt-6 grid gap-4 sm:grid-cols-2 lg:grid-cols-3'>
			{boards.map((board) => (
				<BoardCard key={board.id} board={board} />
			))}
		</div>
	);
}
