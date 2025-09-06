import api from "@/lib/axios";
import { UserProfileDto, UpdateProfileRequest, ChangePasswordRequest } from "@/interfaces/UserProfile";
import { LoginRequest, RegisterRequest } from "@/interfaces/Auth";


export const authApi = {
    login: async (data: LoginRequest) => {
        const res = await api.post("/Auth/login", data);
        return res.data;
    },
    register: async (data: RegisterRequest) => {
        const res = await api.post("/Auth/register", data);
        return res.data;
    }
}

export const userProfileApi = {
    getMyProfile: async (): Promise<UserProfileDto> => {
        const res = await api.get("/UserProfile/profile");
        return res.data;
    },
    
    getUserProfile: async (userId: string): Promise<UserProfileDto> => {
        const res = await api.get(`/UserProfile/${userId}`);
        return res.data;
    },
    
    updateProfile: async (data: UpdateProfileRequest): Promise<UserProfileDto> => {
        const res = await api.put("/UserProfile/profile", data);
        return res.data;
    },
    
    changePassword: async (data: ChangePasswordRequest): Promise<{ message: string }> => {
        const res = await api.put("/UserProfile/password", data);
        return res.data;
    }
}