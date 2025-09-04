export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
}

export interface Post {
  id: number;
  user: {
    name: string;
    avatar: string;
  };
  content: string;
  image?: string;
  likes: number;
  comments: number;
  shares: number;
  timestamp: string;
}
