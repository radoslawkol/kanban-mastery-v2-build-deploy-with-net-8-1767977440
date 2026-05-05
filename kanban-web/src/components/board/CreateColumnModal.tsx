import { useState } from "react";
import type { SubmitEvent } from "react";
import Modal from "react-modal";
import { toast } from "react-toastify";
import z from "zod";
import { extractApiErrorMessage } from "../../lib/extractApiErrorMessage";
import { useCreateColumnMutation } from "../../hooks/useCreateColumnMutation";
import FormInput from "../FormInput";
import FormButton from "../FormButton";

const CreateColumnSchema = z.object({
	columnName: z
		.string()
		.min(1, "Column name is required.")
		.min(2, "Column name must be at least 2 characters.")
		.max(30, "Column name must be no more than 30 characters."),
});

type CreateColumnModalProps = {
	boardId: string;
};

export default function CreateColumnModal({ boardId }: CreateColumnModalProps) {
	const [isOpen, setIsOpen] = useState(false);
	const [columnName, setColumnName] = useState("");
	const [error, setError] = useState<string | null>(null);
	const createColumnMutation = useCreateColumnMutation();
	const isSubmitting = createColumnMutation.isPending;

	const closeModal = () => {
		setIsOpen(false);
		setColumnName("");
		setError(null);
		createColumnMutation.reset();
	};

	const handleCreateSubmit = (event: SubmitEvent<HTMLFormElement>) => {
		event.preventDefault();

		const parsed = CreateColumnSchema.safeParse({ columnName });
		if (!parsed.success) {
			setError(
				parsed.error.issues[0]?.message ??
					"Please provide a valid column name.",
			);
			return;
		}

		setError(null);

		createColumnMutation.mutate(
			{ boardId, name: parsed.data.columnName },
			{
				onSuccess: () => {
					toast.success("Column created successfully!");
					closeModal();
				},
				onError: (createError) => {
					const message = extractApiErrorMessage(
						createError,
						"We could not create this column. Please try again.",
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
				className='rounded-lg border border-primary-600 bg-primary-50 px-3 py-2 text-sm font-medium text-primary-700 transition hover:border-primary-700 hover:bg-primary-100 hover:cursor-pointer'
			>
				+ Add Column
			</button>

			<Modal
				isOpen={isOpen}
				onRequestClose={closeModal}
				shouldCloseOnOverlayClick
				shouldCloseOnEsc
				contentLabel='Create a new column'
				className='mx-auto mt-24 w-[min(92vw,32rem)] rounded-2xl border border-surface-200 bg-white p-6 shadow-2xl outline-none'
				overlayClassName='fixed inset-0 z-50 bg-slate-950/60 px-4 backdrop-blur-sm'
			>
				<div className='flex items-start justify-between gap-4'>
					<div>
						<h2 className='text-xl font-semibold text-ink-900'>
							Create new column
						</h2>
						<p className='mt-1 text-sm text-ink-600'>
							Add a new column to organize your tasks.
						</p>
					</div>
					<button
						type='button'
						onClick={closeModal}
						className='rounded-full px-2 h-8 flex items-start text-3xl text-ink-500 transition hover:bg-surface-100 hover:text-ink-900 hover:cursor-pointer'
						aria-label='Close create column modal'
					>
						<span>&times;</span>
					</button>
				</div>

				<form
					onSubmit={handleCreateSubmit}
					className='mt-5 flex flex-col gap-4'
				>
					<FormInput
						label='Column name'
						id='column-name'
						type='text'
						name='columnName'
						placeholder='To Do'
						value={columnName}
						onChange={(event) => {
							setColumnName(event.target.value);
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
