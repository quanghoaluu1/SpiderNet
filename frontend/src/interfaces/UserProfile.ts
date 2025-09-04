export interface UserProfileDto {
    id: string;
    email: string;
    username: string;
    firstName: string;
    lastName: string;
    fullName: string;
    displayName: string;
    bio?: string;
    avatarUrl?: string;
    coverPhotoUrl?: string;
    dateOfBirth?: string;
    location?: string;
    website?: string;
    phoneNumber?: string;
    gender?: string;
    createdAt: string;
    isEmailConfirmed: boolean;
    memberSince: string;
    age: number;
    isOwnProfile: boolean;
}

export interface UpdateProfileRequest {
    firstName?: string;
    lastName?: string;
    username?: string;
    bio?: string;
    dateOfBirth?: string;
    location?: string;
    website?: string;
    phoneNumber?: string;
    gender?: string;
}

export interface ChangePasswordRequest {
    currentPassword: string;
    newPassword: string;
    confirmNewPassword: string;
}

export interface ApiResponse<T> {
    isSuccess: boolean;
    data?: T;
    errorMessage?: string;
    errors?: Record<string, string[]>;
}
