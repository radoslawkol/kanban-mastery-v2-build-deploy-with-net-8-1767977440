import { Link, Navigate, useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import z from "zod";
import FormInput from "../components/FormInput";
import FormButton from "../components/FormButton";
import AuthCard from "../components/AuthCard";
import { useLoginUser } from "../hooks/useLoginMutation";
import { toast } from "react-toastify";
import { extractApiErrorMessage } from "../lib/extractApiErrorMessage";

type LoginFormValues = {
	email: string;
	password: string;
};

const LoginSchema = z.object({
	email: z.email("Enter a valid email address"),
	password: z.string().min(1, "Password is required"),
});

export default function LoginPage() {
	const loginMutation = useLoginUser();
	const navigate = useNavigate();

	const token = localStorage.getItem("token");
	if (token) {
		return <Navigate to='/dashboard' replace />;
	}

	const {
		register,
		handleSubmit,
		formState: { errors },
	} = useForm<LoginFormValues>({
		resolver: zodResolver(LoginSchema),
	});

	const onSubmit = async (data: LoginFormValues) => {
		loginMutation.mutate(data, {
			onSuccess: () => {
				navigate("/dashboard", { replace: true });
			},
			onError: (error) => {
				toast.error(
					extractApiErrorMessage(error, "Invalid credentials"),
				);
			},
		});
	};

	return (
		<AuthCard
			title='Welcome back'
			subtitle='Log in to continue to your boards.'
		>
			<form
				className='grid gap-2'
				onSubmit={handleSubmit(onSubmit)}
				noValidate
			>
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
					autoComplete='current-password'
					{...register("password")}
					error={errors.password?.message}
				/>

				<FormButton
					isLoading={loginMutation.isPending}
					loadingText='Logging in...'
				>
					Log in
				</FormButton>
			</form>

			<p className='mt-4 text-center text-sm text-ink-700'>
				No account yet?{" "}
				<Link
					to='/register'
					className='font-semibold text-primary-700 hover:text-primary'
				>
					Create one
				</Link>
			</p>
		</AuthCard>
	);
}
