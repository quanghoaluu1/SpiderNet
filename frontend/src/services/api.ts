import api from "@/lib/axios";
import { UserProfileDto, UpdateProfileRequest, ChangePasswordRequest } from "@/interfaces/UserProfile";
import { LoginRequest, RegisterRequest } from "@/interfaces/Auth";
import { 
  PostDto, 
  PostDetailDto, 
  CreatePostRequest, 
  UpdatePostRequest, 
  AddReactionRequest, 
  ReactionDto,
  CommentDto,
  CreateCommentRequest,
  UpdateCommentRequest,
  AddCommentReactionRequest,
  CommentReactionDto,
  ReactionType
} from "@/interfaces/Post";


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

export const postApi = {
    // Post operations
    createPost: async (data: CreatePostRequest): Promise<PostDto> => {
        const formData = new FormData();
        formData.append('content', data.content);
        formData.append('privacy', data.privacy.toString());
        if (data.image) formData.append('image', data.image);
        if (data.video) formData.append('video', data.video);
        
        const res = await api.post("/Post", formData, {
            headers: { 'Content-Type': 'multipart/form-data' }
        });
        return res.data;
    },
    
    getPost: async (id: string): Promise<PostDetailDto> => {
        const res = await api.get(`/Post/${id}`);
        return res.data;
    },
    
    updatePost: async (id: string, data: UpdatePostRequest): Promise<PostDto> => {
        const res = await api.put(`/Post/${id}`, data);
        return res.data;
    },
    
    deletePost: async (id: string): Promise<{ message: string }> => {
        const res = await api.delete(`/Post/${id}`);
        return res.data;
    },
    
    getNewsFeed: async (page: number = 1, size: number = 20): Promise<PostDto[]> => {
        const res = await api.get(`/Post/feed?page=${page}&size=${size}`);
        return res.data;
    },
    
    getUserPosts: async (userId: string, page: number = 1, size: number = 20): Promise<PostDto[]> => {
        const res = await api.get(`/Post/users/${userId}?page=${page}&size=${size}`);
        return res.data;
    },
    
    // Post reactions
    addReaction: async (postId: string, type: ReactionType): Promise<{ reaction?: ReactionDto, message: string }> => {
        const res = await api.post(`/Post/${postId}/reactions`, { type });
        return res.data;
    },
    
    removeReaction: async (postId: string): Promise<{ message: string }> => {
        const res = await api.delete(`/Post/${postId}/reactions`);
        return res.data;
    },
    
    getPostReactions: async (postId: string, type?: ReactionType, limit: number = 20): Promise<ReactionDto[]> => {
        const params = new URLSearchParams();
        if (type !== undefined) params.append('type', type.toString());
        params.append('limit', limit.toString());
        
        const res = await api.get(`/Post/${postId}/reactions?${params}`);
        return res.data;
    }
}

export const commentApi = {
    // Comment operations
    getPostComments: async (postId: string, page: number = 1, size: number = 20): Promise<CommentDto[]> => {
        const res = await api.get(`/Comment/posts/${postId}?page=${page}&size=${size}`);
        return res.data;
    },
    
    getCommentReplies: async (commentId: string, page: number = 1, size: number = 10): Promise<CommentDto[]> => {
        const res = await api.get(`/Comment/${commentId}/replies?page=${page}&size=${size}`);
        return res.data;
    },
    
    addComment: async (postId: string, data: CreateCommentRequest): Promise<CommentDto> => {
        const formData = new FormData();
        formData.append('content', data.content);
        if (data.parentCommentId) formData.append('parentCommentId', data.parentCommentId);
        if (data.image) formData.append('image', data.image);
        if (data.video) formData.append('video', data.video);
        if (data.gif) formData.append('gif', data.gif);
        
        const res = await api.post(`/Comment/posts/${postId}`, formData, {
            headers: { 'Content-Type': 'multipart/form-data' }
        });
        return res.data;
    },
    
    updateComment: async (commentId: string, data: UpdateCommentRequest): Promise<CommentDto> => {
        const res = await api.put(`/Comment/${commentId}`, data);
        return res.data;
    },
    
    deleteComment: async (commentId: string): Promise<{ message: string }> => {
        const res = await api.delete(`/Comment/${commentId}`);
        return res.data;
    },
    
    // Comment reactions
    addCommentReaction: async (commentId: string, type: ReactionType): Promise<{ reaction?: CommentReactionDto, message: string }> => {
        const res = await api.post(`/Comment/${commentId}/reactions`, { type });
        return res.data;
    },
    
    removeCommentReaction: async (commentId: string): Promise<{ message: string }> => {
        const res = await api.delete(`/Comment/${commentId}/reactions`);
        return res.data;
    },
    
    getCommentReactions: async (commentId: string, type?: ReactionType, limit: number = 20): Promise<CommentReactionDto[]> => {
        const params = new URLSearchParams();
        if (type !== undefined) params.append('type', type.toString());
        params.append('limit', limit.toString());
        
        const res = await api.get(`/Comment/${commentId}/reactions?${params}`);
        return res.data;
    }
}