import { useActionState, useRef, useState } from "react";
import z from "zod";
import { extractApiErrorMessage } from "../../lib/extractApiErrorMessage";
import ErrorMessage from "../ErrorMessage";
import FormButton from "../FormButton";

const CreateCardSchema = z.object({
	title: z
		.string()
		.trim()
		.min(1, "Card title is required.")
		.max(120, "Card title cannot be longer than 120 characters."),
});

type CreateCardFormState = {
	error: string | null;
};

type BoardColumnCreateCardProps = {
	onCreateCard: (title: string) => Promise<void>;
	isCreatingCard?: boolean;
};

export default function BoardColumnCreateCard({
	onCreateCard,
	isCreatingCard = false,
}: BoardColumnCreateCardProps) {
	const [isFormVisible, setIsFormVisible] = useState(false);
	const [newCardTitle, setNewCardTitle] = useState("");
	const formRef = useRef<HTMLFormElement | null>(null);

	const createCardAction = async (
		_previousState: CreateCardFormState,
		formData: FormData,
	): Promise<CreateCardFormState> => {
		const parsed = CreateCardSchema.safeParse({
			title: formData.get("title"),
		});

		if (!parsed.success) {
			return {
				error:
					parsed.error.issues[0]?.message ??
					"Please provide a valid card title.",
			};
		}

		try {
			await onCreateCard(parsed.data.title);
			formRef.current?.reset();
			setNewCardTitle("");
			setIsFormVisible(false);
			return { error: null };
		} catch (error) {
			return {
				error: extractApiErrorMessage(
					error,
					"We could not create this card. Please try again.",
				),
			};
		}
	};

	const [formState, submitCreateCardAction, isPending] = useActionState<
		CreateCardFormState,
		FormData
	>(createCardAction, { error: null });
	const isSubmitting = isPending || isCreatingCard;
	const isTitleEmpty = newCardTitle.trim().length === 0;
	const shouldShowError = Boolean(formState.error) && isTitleEmpty;

	if (!isFormVisible) {
		return (
			<FormButton
				type='button'
				onClick={() => {
					setNewCardTitle("");
					setIsFormVisible(true);
				}}
				className='mt-4 rounded-lg border border-dashed border-surface-400 bg-white px-3 py-2 text-sm font-medium text-ink-700 transition hover:border-primary-600 hover:text-primary hover:cursor-pointer'
			>
				Create Card
			</FormButton>
		);
	}

	return (
		<form
			ref={formRef}
			action={submitCreateCardAction}
			className='mt-4 flex flex-col gap-2'
		>
			<input
				name='title'
				type='text'
				value={newCardTitle}
				placeholder='Card title'
				className='w-full rounded-lg border border-surface-300 bg-white px-3 py-2 text-sm text-ink-900 placeholder:text-ink-500 focus:border-primary focus:outline-none focus:ring-2 focus:ring-primary-200'
				disabled={isSubmitting}
				onChange={(event) => {
					setNewCardTitle(event.target.value);
				}}
				autoFocus
			/>
			{shouldShowError ? (
				<ErrorMessage message={formState.error!}></ErrorMessage>
			) : null}
			<FormButton
				isLoading={isSubmitting}
				loadingText='Creating...'
				className='rounded-lg border border-primary bg-primary px-3 py-2 text-sm font-medium text-white transition hover:bg-primary-700 hover:cursor-pointer disabled:cursor-not-allowed disabled:opacity-60'
			>
				Create Card
			</FormButton>
		</form>
	);
}
