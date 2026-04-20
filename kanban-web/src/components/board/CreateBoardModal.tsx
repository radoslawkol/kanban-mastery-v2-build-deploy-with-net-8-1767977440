import { useState } from "react";
import type { SubmitEvent } from "react";
import Modal from "react-modal";
import { toast } from "react-toastify";
import { useNavigate } from "react-router-dom";
import z from "zod";
import { extractApiErrorMessage } from "../../lib/extractApiErrorMessage";
import { useCreateBoardMutation } from "../../hooks/useCreateBoardMutation";
import FormInput from "../FormInput";
import FormButton from "../FormButton";

const CreateBoardSchema = z.object({
	boardName: z
		.string()
		.min(1, "Board name is required.")
		.min(3, "Board name must be at least 3 characters.")
		.max(50, "Board name must be no more than 50 characters."),
});

export default function CreateBoardModal() {
	const [isOpen, setIsOpen] = useState(false);
	const [boardName, setBoardName] = useState("");
	const [error, setError] = useState<string | null>(null);
	const createBoardMutation = useCreateBoardMutation();
	const navigate = useNavigate();
	const isSubmitting = createBoardMutation.isPending;

	const closeModal = () => {
		setIsOpen(false);
		setBoardName("");
		setError(null);
		createBoardMutation.reset();
	};

	const handleCreateSubmit = (event: SubmitEvent<HTMLFormElement>) => {
		event.preventDefault();

		const parsed = CreateBoardSchema.safeParse({ boardName });
		if (!parsed.success) {
			setError(
				parsed.error.issues[0]?.message ??
					"Please provide a valid board name.",
			);
			return;
		}

		setError(null);

		createBoardMutation.mutate(
			{ boardName: parsed.data.boardName },
			{
				onSuccess: (response) => {
					toast.success("Board created successfully!");
					closeModal();
					navigate(`/board/${response.id}`);
				},
				onError: (createError) => {
					const message = extractApiErrorMessage(
						createError,
						"We could not create this board. Please try again.",
					);
					setError(message);
					toast.error(message);
				},
			},
		);
	};

	return (
		<>
			<button
				type='button'
				onClick={() => setIsOpen(true)}
				className='rounded-lg bg-linear-to-r from-primary-600 to-accent-500 px-4 py-3 text-base font-bold text-white transition hover:-translate-y-0.5 hover:cursor-pointer'
			>
				Create Board
			</button>

			<Modal
				isOpen={isOpen}
				onRequestClose={closeModal}
				shouldCloseOnOverlayClick
				shouldCloseOnEsc
				contentLabel='Create a new board'
				className='mx-auto mt-24 w-[min(92vw,32rem)] rounded-2xl border border-surface-200 bg-white p-6 shadow-2xl outline-none'
				overlayClassName='fixed inset-0 z-50 bg-slate-950/60 px-4 backdrop-blur-sm'
			>
				<div className='flex items-start justify-between gap-4'>
					<div>
						<h2 className='text-xl font-semibold text-ink-900'>
							Create new board
						</h2>
						<p className='mt-1 text-sm text-ink-600'>
							Give your board a name to get started.
						</p>
					</div>
					<button
						type='button'
						onClick={closeModal}
						className='rounded-full px-2 h-8 flex items-start text-3xl text-ink-500 transition hover:bg-surface-100 hover:text-ink-900 hover:cursor-pointer'
						aria-label='Close create board modal'
					>
						<span>&times;</span>
					</button>
				</div>

				<form
					onSubmit={handleCreateSubmit}
					className='mt-5 flex flex-col gap-4'
				>
					<FormInput
						label='Board name'
						id='board-name'
						type='text'
						name='boardName'
						placeholder='My Awesome Board'
						value={boardName}
						onChange={(event) => {
							setBoardName(event.target.value);
							setError(null);
						}}
						autoComplete='off'
						disabled={isSubmitting}
						error={error ?? undefined}
					/>

					<div className='flex items-center justify-end gap-3'>
						<button
							type='button'
							onClick={closeModal}
							className='rounded-lg border border-surface-300 bg-white px-4 py-2 text-sm font-medium text-ink-700 transition hover:border-primary-600 hover:text-primary hover:cursor-pointer'
						>
							Cancel
						</button>
						<FormButton
							isLoading={isSubmitting}
							loadingText='Creating...'
							className='rounded-lg border border-primary bg-primary px-4 py-2 text-sm font-medium text-white transition hover:bg-primary-700 hover:cursor-pointer disabled:cursor-not-allowed disabled:opacity-60'
						>
							Create
						</FormButton>
					</div>
				</form>
			</Modal>
		</>
	);
}
