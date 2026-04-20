import { useState } from "react";
import Modal from "react-modal";
import { toast } from "react-toastify";
import { extractApiErrorMessage } from "../../lib/extractApiErrorMessage";
import { useDeleteColumnMutation } from "../../hooks/useDeleteColumnMutation";

type DeleteColumnModalProps = {
	boardId: string;
	columnId: string;
	columnName: string;
};

export default function DeleteColumnModal({
	boardId,
	columnId,
	columnName,
}: DeleteColumnModalProps) {
	const [isOpen, setIsOpen] = useState(false);
	const deleteColumnMutation = useDeleteColumnMutation();
	const isSubmitting = deleteColumnMutation.isPending;

	const closeModal = () => {
		setIsOpen(false);
		deleteColumnMutation.reset();
	};

	const handleDeleteConfirm = () => {
		deleteColumnMutation.mutate(
			{ boardId, columnId },
			{
				onSuccess: () => {
					toast.success("Column deleted successfully!");
					closeModal();
				},
				onError: (deleteError) => {
					const message = extractApiErrorMessage(
						deleteError,
						"We could not delete this column. Please try again.",
					);
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
				className='rounded px-2 py-1 text-xs text-danger-600 transition hover:bg-danger-50 hover:text-danger-700'
				title='Delete column'
			>
				Delete
			</button>

			<Modal
				isOpen={isOpen}
				onRequestClose={closeModal}
				shouldCloseOnOverlayClick
				shouldCloseOnEsc
				contentLabel='Delete column confirmation'
				className='mx-auto mt-24 w-[min(92vw,32rem)] rounded-2xl border border-surface-200 bg-white p-6 shadow-2xl outline-none'
				overlayClassName='fixed inset-0 z-50 bg-slate-950/60 px-4 backdrop-blur-sm'
			>
				<div className='flex items-start justify-between gap-4'>
					<div>
						<h2 className='text-xl font-semibold text-ink-900'>
							Delete column
						</h2>
						<p className='mt-1 text-sm text-ink-600'>
							This action cannot be undone.
						</p>
					</div>
					<button
						type='button'
						onClick={closeModal}
						className='rounded-full px-2 h-8 flex items-start text-3xl text-ink-500 transition hover:bg-surface-100 hover:text-ink-900 hover:cursor-pointer'
						aria-label='Close delete column modal'
					>
						<span>&times;</span>
					</button>
				</div>

				<div className='mt-5 rounded-lg bg-danger-50 p-4 text-sm text-danger-700'>
					<p>
						Are you sure you want to delete the column{" "}
						<strong>"{columnName}"</strong>? This will also delete
						all cards in this column.
					</p>
				</div>

				<div className='mt-6 flex items-center justify-end gap-3'>
					<button
						type='button'
						onClick={closeModal}
						className='rounded-lg border border-surface-300 bg-white px-4 py-2 text-sm font-medium text-ink-700 transition hover:border-primary-600 hover:text-primary hover:cursor-pointer'
						disabled={isSubmitting}
					>
						Cancel
					</button>
					<button
						type='button'
						onClick={handleDeleteConfirm}
						disabled={isSubmitting}
						className='rounded-lg border border-danger-700 text-danger-700 px-4 py-2 text-sm font-medium hover:text-white transition hover:bg-danger-700 hover:cursor-pointer disabled:cursor-not-allowed disabled:opacity-60'
					>
						{isSubmitting ? "Deleting..." : "Delete"}
					</button>
				</div>
			</Modal>
		</>
	);
}
