import { useState } from "react";
import type { SubmitEvent } from "react";
import Modal from "react-modal";
import { toast } from "react-toastify";
import z from "zod";
import { extractApiErrorMessage } from "../../lib/extractApiErrorMessage";
import { useInviteBoardMemberMutation } from "../../hooks/useInviteBoardMemberMutation";
import FormInput from "../FormInput";
import FormButton from "../FormButton";

const InviteMemberSchema = z.object({
	email: z.email("Enter a valid email address."),
});

type BoardInviteModalProps = {
	boardId: string;
};

export default function BoardInviteModal({ boardId }: BoardInviteModalProps) {
	const [isOpen, setIsOpen] = useState(false);
	const [email, setEmail] = useState("");
	const [error, setError] = useState<string | null>(null);
	const inviteBoardMemberMutation = useInviteBoardMemberMutation();
	const isSubmitting = inviteBoardMemberMutation.isPending;

	const closeModal = () => {
		setIsOpen(false);
		setEmail("");
		setError(null);
		inviteBoardMemberMutation.reset();
	};

	const handleInviteSubmit = (event: SubmitEvent<HTMLFormElement>) => {
		event.preventDefault();

		const parsed = InviteMemberSchema.safeParse({ email });
		if (!parsed.success) {
			setError(
				parsed.error.issues[0]?.message ??
					"Enter a valid email address.",
			);
			return;
		}

		setError(null);

		inviteBoardMemberMutation.mutate(
			{
				boardId,
				email: parsed.data.email,
			},
			{
				onSuccess: () => {
					toast.success("Invitation sent successfully.");
					closeModal();
				},
				onError: (inviteError) => {
					const message = extractApiErrorMessage(
						inviteError,
						"We could not send this invitation. Please try again.",
					);
					setError(message);
					toast.error(message);
				},
			},
		);
	};

	return (
		<>
			<FormButton
				type='button'
				onClick={() => setIsOpen(true)}
				className='rounded-lg border border-primary-600 bg-primary-50 px-3 py-2 text-sm font-medium text-primary-700 transition hover:border-primary-700 hover:bg-primary-100 hover:cursor-pointer'
			>
				Invite User
			</FormButton>

			<Modal
				isOpen={isOpen}
				onRequestClose={closeModal}
				shouldCloseOnOverlayClick
				shouldCloseOnEsc
				contentLabel='Invite user to board'
				className='mx-auto mt-24 w-[min(92vw,32rem)] rounded-2xl border border-surface-200 bg-white p-6 shadow-2xl outline-none'
				overlayClassName='fixed inset-0 z-50 bg-slate-950/60 px-4 backdrop-blur-sm'
			>
				<div className='flex items-start justify-between gap-4'>
					<div>
						<h2 className='text-xl font-semibold text-ink-900'>
							Invite teammate
						</h2>
						<p className='mt-1 text-sm text-ink-600'>
							Send an invitation to join this board.
						</p>
					</div>
					<button
						type='button'
						onClick={closeModal}
						className='rounded-full px-2 h-8 flex items-start text-3xl text-ink-500 transition hover:bg-surface-100 hover:text-ink-900 hover:cursor-pointer'
						aria-label='Close invite modal'
					>
						<span>&times;</span>
					</button>
				</div>

				<form
					onSubmit={handleInviteSubmit}
					className='mt-5 flex flex-col gap-4'
				>
					<FormInput
						label='Email address'
						id='invite-email'
						type='text'
						name='email'
						placeholder='teammate@example.com'
						value={email}
						onChange={(event) => {
							setEmail(event.target.value);
							setError(null);
						}}
						autoComplete='email'
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
							loadingText='Inviting...'
							className='rounded-lg border border-primary bg-primary px-4 py-2 text-sm font-medium text-white transition hover:bg-primary-700 hover:cursor-pointer disabled:cursor-not-allowed disabled:opacity-60'
						>
							Invite
						</FormButton>
					</div>
				</form>
			</Modal>
		</>
	);
}
