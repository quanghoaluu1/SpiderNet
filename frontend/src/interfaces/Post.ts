export enum PostPrivacy {
  Public = 0,
  Friends = 1,
  Private = 2
}

export enum ReactionType {
  Like = 0,      // 👍
  Love = 1,      // ❤️
  Haha = 2,      // 😂
  Wow = 3,       // 😮
  Sad = 4,       // 😢
  Angry = 5      // 😠
}

export enum MediaType {
  Image = 0,
  Video = 1,
  Gif = 2
}

export interface ReactionTypeCount {
  type: ReactionType;
  count: number;
  emoji: string;
  name: string;
}

export interface ReactionSummaryDto {
  totalCount: number;
  likesCount: number;
  lovesCount: number;
  hahaCount: number;
  wowCount: number;
  sadCount: number;
  angryCount: number;
  topReactions: ReactionTypeCount[];
}

export interface ReactionDto {
  userId: string;
  userFullName: string;
  username: string;
  userAvatarUrl?: string;
  type: ReactionType;
  typeEmoji: string;
  typeName: string;
  createdAt: string;
}

export interface PostDto {
  id: string;
  userId: string;
  content: string;
  imageUrl?: string;
  videoUrl?: string;
  privacy: PostPrivacy;
  createdAt: string;
  updatedAt: string;
  
  // User info
  userFullName: string;
  userDisplayName: string;
  username: string;
  userAvatarUrl?: string;
  
  // Interaction info
  reactionsSummary: ReactionSummaryDto;
  currentUserReaction?: ReactionType;
  commentsCount: number;
  isLikedByCurrentUser: boolean;
  isOwnPost: boolean;
  
  // Time formatting
  timeAgo: string;
}

export interface PostDetailDto extends PostDto {
  comments: CommentDto[];
  recentReactions: ReactionDto[];
}

export interface CreatePostRequest {
  content: string;
  privacy: PostPrivacy;
  image?: File;
  video?: File;
}

export interface UpdatePostRequest {
  content?: string;
  privacy?: PostPrivacy;
}

export interface AddReactionRequest {
  type: ReactionType;
}

// Comment interfaces
export interface CommentReactionSummaryDto {
  totalCount: number;
  likesCount: number;
  lovesCount: number;
  hahaCount: number;
  wowCount: number;
  sadCount: number;
  angryCount: number;
  topReactions: ReactionTypeCount[];
}

export interface CommentDto {
  id: string;
  postId: string;
  userId: string;
  parentCommentId?: string;
  content: string;
  createdAt: string;
  updatedAt: string;
  imageUrl?: string;
  videoUrl?: string;
  gifUrl?: string;
  mediaType?: MediaType;
  hasMedia: boolean;
  
  // User info
  userFullName: string;
  userDisplayName: string;
  username: string;
  userAvatarUrl?: string;
  
  // Interaction info
  reactionsSummary: CommentReactionSummaryDto;
  currentUserReaction?: ReactionType;
  repliesCount: number;
  
  // Meta info
  timeAgo: string;
  isOwnComment: boolean;
  isReply: boolean;
  isEdited: boolean;
  
  // Nested replies
  replies: CommentDto[];
  hasMoreReplies: boolean;
}

export interface CreateCommentRequest {
  content: string;
  parentCommentId?: string;
  image?: File;
  video?: File;
  gif?: File;
}

export interface UpdateCommentRequest {
  content: string;
}

export interface AddCommentReactionRequest {
  type: ReactionType;
}

export interface CommentReactionDto {
  userId: string;
  userFullName: string;
  username: string;
  userAvatarUrl?: string;
  type: ReactionType;
  typeEmoji: string;
  typeName: string;
  createdAt: string;
}
