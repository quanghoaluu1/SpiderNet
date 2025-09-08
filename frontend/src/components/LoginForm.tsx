'use client';

import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { authApi } from '@/services/api';
import { LoginRequest } from '@/interfaces/Auth';
import { useRouter } from 'next/navigation';
import Image from "next/image";
import { Eye, EyeOff } from "lucide-react";

interface LoginFormProps {
    onSwitchToRegister: () => void;
}

export default function LoginForm({ onSwitchToRegister }: LoginFormProps) {
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState('');
    const [showPassword, setShowPassword] = useState(false);
    const router = useRouter();

    const {
        register,
        handleSubmit,
        formState: { errors },
    } = useForm<LoginRequest>();

    const onSubmit = async (data: LoginRequest) => {
        setIsLoading(true);
        setError('');

        try {
            const response = await authApi.login(data);
            if (typeof window !== 'undefined') {
                localStorage.setItem('token', response.token);
                localStorage.setItem('user', JSON.stringify(response.user));
            }
            router.push('/');
        } catch (err: unknown) {
            const errorMessage = err instanceof Error && 'response' in err && err.response && 
                typeof err.response === 'object' && 'data' in err.response && 
                err.response.data && typeof err.response.data === 'object' && 
                'message' in err.response.data && typeof err.response.data.message === 'string'
                ? err.response.data.message
                : 'Đăng nhập thất bại';
            setError(errorMessage);
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="min-h-screen flex items-center justify-center bg-bg-black">
            {/* Background Effects */}
            <div className="fixed inset-0 opacity-30">
                <div className="absolute top-20 left-20 w-96 h-96 bg-spiderman-red/20 rounded-full blur-3xl animate-pulse"></div>
                <div className="absolute bottom-20 right-20 w-96 h-96 bg-spiderman-blue/20 rounded-full blur-3xl animate-pulse delay-1000"></div>
            </div>

            <div className="relative z-10 max-w-lg w-full space-y-8 p-8">
                <div className="glass-morphism rounded-2xl shadow-2xl p-8 pt-2">
                    {/* Header */}
                    <div className="text-center">
                        <div className={"bg-spiderman-red mx-auto h-1/2 w-1/3 rounded-full flex items-center justify-center mb-4 shadow-lg"}>
                            <Image src="/icons/icon.png" alt={"Icon"} width={100} height={100} className="mx-auto"/>
                        </div>
                        <h2 className="text-3xl font-bold text-white mb-2">Login</h2>
                        <p className="text-sm text-white/80">
                           Welcome back! Please enter your details to log in.
                        </p>
                    </div>

                    {/* Error Message */}
                    {error && (
                        <div className="mt-4 p-4 bg-red-500/20 border border-red-400/50 text-red-100 rounded-lg backdrop-blur-sm">
                            <span className="block sm:inline">{error}</span>
                        </div>
                    )}

                    {/* Form */}
                    <form className="mt-8 space-y-6" onSubmit={handleSubmit(onSubmit)}>
                        <div>
                            <label htmlFor="email" className="block text-sm font-medium text-white/90 mb-2">
                                Email or Username
                            </label>
                            <input
                                id="email"
                                type="text"
                                {...register('email', {
                                    required: 'Email or username is required',
                                    // pattern: {
                                    //     value: /^\S+@\S+$/i,
                                    //     message: 'Email không hợp lệ',
                                    // },
                                })}
                                className="w-full px-4 py-3 bg-white/10 border border-white/20 text-white placeholder-white/50
                         rounded-lg focus:outline-none focus:ring-2 focus:ring-spiderman-red focus:border-transparent
                         backdrop-blur-sm transition-all duration-200"
                                placeholder="Enter your email or username"
                            />
                            {errors.email && (
                                <p className="mt-1 text-sm text-red-300">{errors.email.message}</p>
                            )}
                        </div>

                        <div>
                            <label htmlFor="password" className="block text-sm font-medium text-white/90 mb-2">
                                Password
                            </label>
                            <div className="relative">
                                <input
                                    id="password"
                                    type={showPassword ? "text" : "password"}
                                    {...register('password', {
                                        required: 'Password is required',
                                        minLength: {
                                            value: 6,
                                            message: 'Mật khẩu phải có ít nhất 6 ký tự',
                                        },
                                    })}
                                    className="w-full px-4 py-3 pr-12 bg-white/10 border border-white/20 text-white placeholder-white/50
                             rounded-lg focus:outline-none focus:ring-2 focus:ring-spiderman-red focus:border-transparent
                             backdrop-blur-sm transition-all duration-200"
                                    placeholder="Enter your password"
                                />
                                <button
                                    type="button"
                                    className="absolute inset-y-0 right-0 pr-3 flex items-center text-white/50 hover:text-white transition-colors"
                                    onClick={() => setShowPassword(!showPassword)}
                                >
                                    {showPassword ? (
                                        <EyeOff className="h-5 w-5" />
                                    ) : (
                                        <Eye className="h-5 w-5" />
                                    )}
                                </button>
                            </div>
                            {errors.password && (
                                <p className="mt-1 text-sm text-red-300">{errors.password.message}</p>
                            )}
                        </div>

                        <div className="flex items-center justify-between">
                            <div className="flex items-center">
                                <input
                                    id="remember-me"
                                    name="remember-me"
                                    type="checkbox"
                                    className="h-4 w-4 text-spiderman-red focus:ring-spiderman-red border-gray-300 rounded"
                                />
                                <label htmlFor="remember-me" className="ml-2 block text-sm text-white/80">
                                    Remember me
                                </label>
                            </div>

                            <div className="text-sm">
                                <a href="#" className="font-medium text-spiderman-blue hover:text-spiderman-blue/80 transition-colors">
                                    Forgot your password?
                                </a>
                            </div>
                        </div>

                        <button
                            type="submit"
                            disabled={isLoading}
                            className="group relative w-full flex justify-center py-3 px-4 border border-transparent
                       text-sm font-medium rounded-lg text-white bg-spiderman-red hover:bg-spiderman-dark-red
                       focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-spiderman-red
                       disabled:opacity-50 disabled:cursor-not-allowed transition-all duration-200
                       transform hover:scale-105 active:scale-95 shadow-lg hover:shadow-xl cursor-pointer"
                        >
                            {isLoading ? (
                                <div className="flex items-center">
                                    <svg className="animate-spin -ml-1 mr-3 h-5 w-5 text-white" fill="none" viewBox="0 0 24 24">
                                        <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                                        <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                                    </svg>
                                    Logging in...
                                </div>
                            ) : (
                                'Log In'
                            )}
                        </button>

                        <div className="text-center">
                            <p className="text-sm text-white/80">
                                Don&apos;t have an account?{' '}
                                <button
                                    type="button"
                                    onClick={onSwitchToRegister}
                                    className="font-medium text-spiderman-blue hover:text-spiderman-blue/80 transition-colors cursor-pointer"
                                >
                                    Sign Up Now
                                </button>
                            </p>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    );
}
