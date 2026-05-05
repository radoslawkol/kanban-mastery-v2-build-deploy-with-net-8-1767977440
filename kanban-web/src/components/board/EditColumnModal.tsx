import { useState } from "react";
import type { SubmitEvent } from "react";
import Modal from "react-modal";
import { toast } from "react-toastify";
import z from "zod";
import { extractApiErrorMessage } from "../../lib/extractApiErrorMessage";
import { useUpdateColumnMutation } from "../../hooks/useUpdateColumnMutation";
import FormInput from "../FormInput";
import FormButton from "../FormButton";

const EditColumnSchema = z.object({
	columnName: z
		.string()
		.min(1, "Column name is required.")
		.min(2, "Column name must be at least 2 characters.")
		.max(30, "Column name must be no more than 30 characters."),
});

type EditColumnModalProps = {
	boardId: string;
	columnId: string;
	initialName: string;
};

export default function EditColumnModal({
	boardId,
	columnId,
	initialName,
}: EditColumnModalProps) {
	const [isOpen, setIsOpen] = useState(false);
	const [columnName, setColumnName] = useState(initialName);
	const [error, setError] = useState<string | null>(null);
	const updateColumnMutation = useUpdateColumnMutation();
	const isSubmitting = updateColumnMutation.isPending;

	const closeModal = () => {
		setIsOpen(false);
		setColumnName(initialName);
		setError(null);
		updateColumnMutation.reset();
	};

	const handleEditSubmit = (event: SubmitEvent<HTMLFormElement>) => {
		event.preventDefault();

		if (columnName === initialName) {
			closeModal();
			return;
		}

		const parsed = EditColumnSchema.safeParse({ columnName });
		if (!parsed.success) {
			setError(
				parsed.error.issues[0]?.message ??
					"Please provide a valid column name.",
			);
			return;
		}

		setError(null);

		updateColumnMutation.mutate(
			{ boardId, columnId, name: parsed.data.columnName },
			{
				onSuccess: () => {
					toast.success("Column updated successfully!");
					closeModal();
				},
				onError: (updateError) => {
					const message = extractApiErrorMessage(
						updateError,
						"We could not update this column. Please try again.",
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
				className='rounded px-2 py-1 text-xs text-ink-500 transition hover:bg-surface-200 hover:text-ink-700'
				title='Edit column'
			>
				Edit
			</button>

			<Modal
				isOpen={isOpen}
				onRequestClose={closeModal}
				shouldCloseOnOverlayClick
				shouldCloseOnEsc
				contentLabel='Edit column'
				className='mx-auto mt-24 w-[min(92vw,32rem)] rounded-2xl border border-surface-200 bg-white p-6 shadow-2xl outline-none'
				overlayClassName='fixed inset-0 z-50 bg-slate-950/60 px-4 backdrop-blur-sm'
			>
				<div className='flex items-start justify-between gap-4'>
					<div>
						<h2 className='text-xl font-semibold text-ink-900'>
							Edit column
						</h2>
						<p className='mt-1 text-sm text-ink-600'>
							Update the column name.
						</p>
					</div>
					<button
						type='button'
						onClick={closeModal}
						className='rounded-full px-2 h-8 flex items-start text-3xl text-ink-500 transition hover:bg-surface-100 hover:text-ink-900 hover:cursor-pointer'
						aria-label='Close edit column modal'
					>
						<span>&times;</span>
					</button>
				</div>

				<form
					onSubmit={handleEditSubmit}
					className='mt-5 flex flex-col gap-4'
				>
					<FormInput
						label='Column name'
						id='edit-column-name'
						type='text'
						name='columnName'
						placeholder='Column name'
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
							loadingText='Updating...'
							className='rounded-lg border border-primary bg-primary px-4 py-2 text-sm font-medium text-white transition hover:bg-primary-700 hover:cursor-pointer disabled:cursor-not-allowed disabled:opacity-60'
						>
							Update
						</FormButton>
					</div>
				</form>
			</Modal>
		</>
	);
}
