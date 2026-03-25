"use client";
import { Link } from "react-router-dom";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import z from "zod";

type RegisterFormValues = {
	username: string;
	email: string;
	password: string;
};

const RegisterUserSchema = z
	.object({
		username: z
			.string()
			.min(3, "Username must be at least 3 characters")
			.max(50, "Username must be at most 50 characters"),
		email: z.email("Enter a valid email address"),
		password: z
			.string()
			.min(8, "Password must be at least 8 characters")
			.max(100, "Password must be at most 100 characters"),
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
		<div className='min-h-screen bg-(--auth-page-bg) px-6 py-8 text-ink-900'>
			<div className='mx-auto flex min-h-[calc(100vh-4rem)] w-full max-w-115 items-center justify-center'>
				<div className='w-full rounded-2xl border border-surface-300 bg-(--auth-card-bg) p-8 shadow-(--auth-card-shadow)'>
					<h1 className='text-3xl font-bold leading-tight'>
						Create account
					</h1>
					<p className='mb-4 mt-2 text-ink-600'>
						Sign up to start organizing your boards.
					</p>
					<form
						className='grid gap-2'
						onSubmit={handleSubmit(onSubmit)}
						noValidate
					>
						<label
							htmlFor='username'
							className='mt-2 text-sm font-semibold text-ink-700'
						>
							Username
						</label>
						<input
							id='username'
							type='text'
							autoComplete='username'
							className='rounded-lg border border-surface-300 bg-white px-3.5 py-3 text-base outline-none transition focus:border-primary-600 focus:ring-4 focus:ring-primary-500/15'
							{...register("username")}
						/>
						{errors.username && (
							<span className='mb-0.5 text-sm text-danger-700'>
								{errors.username.message}
							</span>
						)}

						<label
							htmlFor='email'
							className='mt-2 text-sm font-semibold text-ink-700'
						>
							Email
						</label>
						<input
							id='email'
							type='email'
							autoComplete='email'
							className='rounded-lg border border-surface-300 bg-white px-3.5 py-3 text-base outline-none transition focus:border-primary-600 focus:ring-4 focus:ring-primary-500/15'
							{...register("email")}
						/>
						{errors.email && (
							<span className='mb-0.5 text-sm text-danger-700'>
								{errors.email.message}
							</span>
						)}

						<label
							htmlFor='password'
							className='mt-2 text-sm font-semibold text-ink-700'
						>
							Password
						</label>
						<input
							id='password'
							type='password'
							autoComplete='new-password'
							className='rounded-lg border border-surface-300 bg-white px-3.5 py-3 text-base outline-none transition focus:border-primary-600 focus:ring-4 focus:ring-primary-500/15'
							{...register("password")}
						/>
						{errors.password && (
							<span className='mb-0.5 text-sm text-danger-700'>
								{errors.password.message}
							</span>
						)}

						<button
							type='submit'
							disabled={isSubmitting}
							className='mt-4 rounded-lg bg-linear-to-r from-primary-600 to-accent-500 px-4 py-3 text-base font-bold text-white transition hover:-translate-y-0.5 disabled:cursor-not-allowed disabled:opacity-75 disabled:hover:translate-y-0'
						>
							{isSubmitting
								? "Creating account..."
								: "Create account"}
						</button>
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
				</div>
			</div>
		</div>
	);
}
