"use client";

import { useState } from "react";
import { useForm, Controller } from "react-hook-form";
import { authApi } from "@/services/api";
import { RegisterRequest } from "@/interfaces/Auth";
import { useRouter } from "next/navigation";
import Image from "next/image";
import { format } from "date-fns";
import { CalendarIcon, ChevronDownIcon } from "lucide-react";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Calendar } from "@/components/ui/calendar";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";

interface RegisterFormProps {
  onSwitchToLogin: () => void;
}

export default function RegisterForm({ onSwitchToLogin }: RegisterFormProps) {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState("");
  const router = useRouter();
  const [open, setOpen] = useState(false);
  const [date, setDate] = useState<Date | undefined>(undefined);

  const {
    register,
    handleSubmit,
    watch,
    control,
    formState: { errors },
  } = useForm<RegisterRequest>();

  const password = watch("password");

  const onSubmit = async (data: RegisterRequest) => {
    setIsLoading(true);
    setError("");

    try {
      const response = await authApi.register(data);
      if (typeof window !== "undefined") {
        localStorage.setItem("token", response.token);
        localStorage.setItem("user", JSON.stringify(response.user));
      }
      router.push("/");
    } catch (err: unknown) {
      setError(
        (err as { response?: { data?: { message?: string } } }).response?.data
          ?.message || "Registration failed"
      );
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="max-h-screen flex items-center justify-center bg-bg-black py-12">
      {/* Background Effects */}
      <div className="fixed inset-0 opacity-20">
        <div className="absolute top-20 right-20 w-80 h-80 bg-spiderman-blue/30 rounded-full blur-3xl animate-bounce"></div>
        <div className="absolute bottom-20 left-20 w-80 h-80 bg-spiderman-gold/20 rounded-full blur-3xl animate-pulse"></div>
      </div>

      <div className="relative z-10 max-w-xl w-full space-y-8 p-8">
        <div className="glass-morphism rounded-2xl shadow-2xl p-8">
          {/* Header */}
          <div className="text-center">
            <div className="mx-auto h-1/2 w-1/3 bg-spiderman-blue rounded-full flex items-center justify-center mb-4 shadow-lg">
              <Image
                src="/icons/icon.png"
                alt={"Icon"}
                width={100}
                height={100}
                className="mx-auto"
              />
            </div>
            <h2 className="text-3xl font-bold text-white mb-2">Sign Up</h2>
            <p className="text-sm text-white/80">
              Create your account and start your adventure with us!
            </p>
          </div>

          {/* Error Message */}
          {error && (
            <div className="mt-2 p-2 bg-red-500/20 border border-red-400/50 text-red-100 rounded-lg backdrop-blur-sm">
              <span className="block sm:inline">{error}</span>
            </div>
          )}

          {/* Form */}
          <form className="mt-8 space-y-6" onSubmit={handleSubmit(onSubmit)}>
            <div className="grid grid-cols-2 gap-4">
              <div className="w-full space-y-2">
                <label
                  htmlFor="firstName"
                  className="block text-sm font-medium text-white/90 mb-2"
                >
                  First Name
                </label>
                <input
                  id="firstName"
                  type="text"
                  {...register("firstName", {
                    required: "First name is required",
                    minLength: {
                      value: 2,
                      message: "First name must be at least 2 characters",
                    },
                  })}
                  className="w-full px-3 py-3 bg-white/10 border border-white/20 text-white placeholder-white/50
                           rounded-lg focus:outline-none focus:ring-2 focus:ring-spiderman-blue focus:border-transparent
                           backdrop-blur-sm transition-all duration-200"
                  placeholder="Enter your first name"
                />
                {errors.firstName && (
                  <p className="mt-1 text-sm text-red-300">
                    {errors.firstName.message}
                  </p>
                )}
              </div>

              <div>
                <label
                  htmlFor="lastName"
                  className="block text-sm font-medium text-white/90 mb-2"
                >
                  Last Name
                </label>
                <input
                  id="lastName"
                  type="text"
                  {...register("lastName", {
                    required: "Last name is required",
                    minLength: {
                      value: 2,
                      message: "Last name must be at least 2 characters",
                    },
                  })}
                  className="w-full px-3 py-3 bg-white/10 border border-white/20 text-white placeholder-white/50
                           rounded-lg focus:outline-none focus:ring-2 focus:ring-spiderman-blue focus:border-transparent
                           backdrop-blur-sm transition-all duration-200"
                  placeholder="Enter your last name"
                />
                {errors.lastName && (
                  <p className="mt-1 text-sm text-red-300">
                    {errors.lastName.message}
                  </p>
                )}
              </div>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div>
                <label
                  htmlFor="birthDate"
                  className="block text-sm font-medium text-white/90 mb-2"
                >
                  Birthday
                </label>
                <Controller
                  name="birthDate"
                  control={control}
                  rules={{
                    required: "Birthday is required",
                    validate: (value) => {
                      if (!value) return "Birthday is required";
                      const birthDate = new Date(value);
                      const today = new Date();
                      const age = today.getFullYear() - birthDate.getFullYear();
                      return age >= 13 || "You must be at least 13 years old";
                    },
                  }}
                  render={({ field }) => (
                    <Popover open={open} onOpenChange={setOpen}>
                      <PopoverTrigger asChild>
                        <Button
                          variant="outline"
                          id="date"
                          className="w-48 justify-between font-normal bg-white/10 border border-white/20 hover:bg-white/10 hover:text-white text-white"
                        >
                          {field.value ? new Date(field.value).toLocaleDateString() : "Select date"}
                          <ChevronDownIcon />
                        </Button>
                      </PopoverTrigger>
                      <PopoverContent
                        className="w-auto overflow-hidden p-0 bg-black border-white/20"
                        align="start"
                      >
                        <Calendar
                          mode="single"
                          selected={field.value ? new Date(field.value) : undefined}
                          captionLayout="dropdown"
                          onSelect={(date) => {
                            field.onChange(date);
                            setDate(date);
                            setOpen(false);
                          }}
                          className="bg-dark-blue text-white"
                        />
                      </PopoverContent>
                    </Popover>
                  )}
                />
                {errors.birthDate && (
                  <p className="mt-1 text-sm text-red-300">
                    {errors.birthDate.message}
                  </p>
                )}
              </div>

              <div>
                <label
                  htmlFor="gender"
                  className="block text-sm font-medium text-white/90 mb-2"
                >
                  Gender
                </label>
                <Controller
                  name="gender"
                  control={control}
                  rules={{ required: "Gender is required" }}
                  render={({ field }) => (
                    <Select onValueChange={field.onChange} value={field.value}>
                      <SelectTrigger
                        className="w-full h-12 px-3 py-3 border border-white/20 text-white
                                               rounded-lg focus:outline-none focus:ring-2 focus:ring-spiderman-blue focus:border-transparent
                                               backdrop-blur-sm transition-all duration-200"
                      >
                        <SelectValue
                          placeholder="Select gender"
                          className="text-white"
                        />
                      </SelectTrigger>
                      <SelectContent className="!bg-dark-blue border border-white/20 backdrop-blur-sm">
                        <SelectItem
                          value="male"
                          className="text-white hover:bg-white/10 focus:bg-white/10"
                        >
                          Male
                        </SelectItem>
                        <SelectItem
                          value="female"
                          className="text-white hover:bg-white/10 focus:bg-white/10"
                        >
                          Female
                        </SelectItem>
                        <SelectItem
                          value="other"
                          className="text-white hover:bg-white/10 focus:bg-white/10"
                        >
                          Other
                        </SelectItem>
                      </SelectContent>
                    </Select>
                  )}
                />
                {errors.gender && (
                  <p className="mt-1 text-sm text-red-300">
                    {errors.gender.message}
                  </p>
                )}
              </div>
            </div>

            <div>
              <label
                htmlFor="email"
                className="block text-sm font-medium text-white/90 mb-2"
              >
                Email
              </label>
              <input
                id="email"
                type="email"
                {...register("email", {
                  required: "Email is required",
                  pattern: {
                    value: /^\S+@\S+$/i,
                    message: "Invalid email format",
                  },
                })}
                className="w-full px-4 py-3 bg-white/10 border border-white/20 text-white placeholder-white/50
                         rounded-lg focus:outline-none focus:ring-2 focus:ring-spiderman-blue focus:border-transparent
                         backdrop-blur-sm transition-all duration-200"
                placeholder="Enter your email"
              />
              {errors.email && (
                <p className="mt-1 text-sm text-red-300">
                  {errors.email.message}
                </p>
              )}
            </div>

            <div>
              <label
                htmlFor="password"
                className="block text-sm font-medium text-white/90 mb-2"
              >
                Password
              </label>
              <input
                id="password"
                type="password"
                {...register("password", {
                  required: "Password is required",
                  // minLength: {
                  //   value: 6,
                  //   message: "Password must be at least 6 characters",
                  // },
                  // pattern: {
                  //   value: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/,
                  //   message:
                  //     "Password must contain at least 1 uppercase, 1 lowercase and 1 number",
                  // },
                })}
                className="w-full px-4 py-3 bg-white/10 border border-white/20 text-white placeholder-white/50
                         rounded-lg focus:outline-none focus:ring-2 focus:ring-spiderman-blue focus:border-transparent
                         backdrop-blur-sm transition-all duration-200"
                placeholder="Enter your password"
              />
              {errors.password && (
                <p className="mt-1 text-sm text-red-300">
                  {errors.password.message}
                </p>
              )}
            </div>

            <div>
              <label
                htmlFor="confirmPassword"
                className="block text-sm font-medium text-white/90 mb-2"
              >
                Confirm Password
              </label>
              <input
                id="confirmPassword"
                type="password"
                {...register("confirmPassword", {
                  required: "Password confirmation is required",
                  validate: (value) =>
                    value === password || "Passwords do not match",
                })}
                className="w-full px-4 py-3 bg-white/10 border border-white/20 text-white placeholder-white/50
                         rounded-lg focus:outline-none focus:ring-2 focus:ring-spiderman-blue focus:border-transparent
                         backdrop-blur-sm transition-all duration-200"
                placeholder="Confirm your password"
              />
              {errors.confirmPassword && (
                <p className="mt-1 text-sm text-red-300">
                  {errors.confirmPassword.message}
                </p>
              )}
            </div>

            <div className="flex items-center">
              <input
                id="agree-terms"
                name="agree-terms"
                type="checkbox"
                required
                className="h-4 w-4 text-spiderman-blue focus:ring-spiderman-blue border-gray-300 rounded"
              />
              <label
                htmlFor="agree-terms"
                className="ml-2 block text-sm text-white/80"
              >
                I agree with{" "}
                <a
                  href="#"
                  className="text-spiderman-red hover:text-spiderman-dark-red"
                >
                  Terms of Service
                </a>{" "}
                and{" "}
                <a
                  href="#"
                  className="text-spiderman-red hover:text-spiderman-dark-red"
                >
                  Privacy Policy
                </a>
              </label>
            </div>

            <button
              type="submit"
              disabled={isLoading}
              className="group relative w-full flex justify-center py-3 px-4 border border-transparent
                       text-sm font-medium rounded-lg text-white bg-spiderman-blue hover:bg-spiderman-light-blue
                       focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-spiderman-blue
                       disabled:opacity-50 disabled:cursor-not-allowed transition-all duration-200
                       transform hover:scale-105 active:scale-95 shadow-lg hover:shadow-xl cursor-pointer"
            >
              {isLoading ? (
                <div className="flex items-center">
                  <svg
                    className="animate-spin -ml-1 mr-3 h-5 w-5 text-white"
                    fill="none"
                    viewBox="0 0 24 24"
                  >
                    <circle
                      className="opacity-25"
                      cx="12"
                      cy="12"
                      r="10"
                      stroke="currentColor"
                      strokeWidth="4"
                    ></circle>
                    <path
                      className="opacity-75"
                      fill="currentColor"
                      d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                    ></path>
                  </svg>
                  Signing Up...
                </div>
              ) : (
                "Sign Up"
              )}
            </button>

            <div className="text-center">
              <p className="text-sm text-white/80">
                Already had an account?{" "}
                <button
                  type="button"
                  onClick={onSwitchToLogin}
                  className="font-medium text-spiderman-red hover:text-spiderman-dark-red transition-colors cursor-pointer"
                >
                  Login Now
                </button>
              </p>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
