import { Link } from "react-router-dom";
import type { UserBoard } from "../types/board";

type BoardCardProps = {
	board: UserBoard;
};

export default function BoardCard({ board }: BoardCardProps) {
	return (
		<Link
			to={`/board/${board.id}`}
			className='rounded-xl border border-surface-300 bg-white p-5 text-left transition hover:border-primary-600 hover:shadow-md'
		>
			<h2 className='text-lg font-semibold text-ink-900'>{board.name}</h2>
			<p className='mt-2 text-sm text-primary'>Open board details</p>
		</Link>
	);
}
