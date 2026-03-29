"use client";
import { Link } from "react-router-dom";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import z from "zod";
import AuthCard from "../components/AuthCard";
import FormInput from "../components/FormInput";
import FormButton from "../components/FormButton";

type RegisterFormValues = {
	username: string;
	email: string;
	password: string;
};

const minUsernameLength = 3;
const maxUsernameLength = 50;
const minPasswordLength = 8;
const maxPasswordLength = 50;

const RegisterUserSchema = z
	.object({
		username: z
			.string()
			.min(
				minUsernameLength,
				`Username must be at least ${minUsernameLength} characters`,
			)
			.max(
				maxUsernameLength,
				`Username must be at most ${maxUsernameLength} characters`,
			),
		email: z.email("Enter a valid email address"),
		password: z
			.string()
			.min(
				minPasswordLength,
				`Password must be at least ${minPasswordLength} characters`,
			)
			.max(
				maxPasswordLength,
				`Password must be at most ${maxPasswordLength} characters`,
			),
	})
	.required();

export default function RegisterPage() {
	const {
		register,
		handleSubmit,
		formState: { errors, isSubmitting },
	} = useForm<RegisterFormValues>({
		resolver: zodResolver(RegisterUserSchema),
	});

	const onSubmit = async (data: RegisterFormValues) => {};

	return (
		<AuthCard
			title='Create account'
			subtitle='Sign up to start organizing your boards.'
		>
			<form
				className='grid gap-2'
				onSubmit={handleSubmit(onSubmit)}
				noValidate
			>
				<FormInput
					id='username'
					label='Username'
					type='text'
					autoComplete='username'
					{...register("username")}
					error={errors.username?.message}
				/>

				<FormInput
					id='email'
					label='Email'
					type='email'
					autoComplete='email'
					{...register("email")}
					error={errors.email?.message}
				/>

				<FormInput
					id='password'
					label='Password'
					type='password'
					autoComplete='new-password'
					{...register("password")}
					error={errors.password?.message}
				/>

				<FormButton
					isLoading={isSubmitting}
					loadingText='Creating account...'
				>
					Create account
				</FormButton>
			</form>

			<p className='mt-4 text-center text-sm text-ink-700'>
				Already have an account?{" "}
				<Link
					to='/login'
					className='font-semibold text-primary-700 hover:text-primary'
				>
					Log in
				</Link>
			</p>
		</AuthCard>
	);
}
