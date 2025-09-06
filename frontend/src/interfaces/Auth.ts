export interface LoginRequest {
    emailOrUsername: string;
    password: string;
}
export interface RegisterRequest {
    email: string;
    username: string;
    password: string;
    confirmPassword: string;
    firstName: string;
    lastName: string;
    birthDate: string;
    gender: string;
}
export interface AuthResponse {
    token: string;
    user: {
        id: string;
        email: string;
        firstName: string;
        lastName: string;
        birthDate: string;
        gender: string;
    };
}