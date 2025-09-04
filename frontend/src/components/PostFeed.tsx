'use client';

import PostItem from './PostItem';
import { Post } from '@/types';

interface PostFeedProps {
  posts: Post[];
}

export default function PostFeed({ posts }: PostFeedProps) {
  return (
    <div className="space-y-6">
      {posts.map((post) => (
        <PostItem key={post.id} post={post} />
      ))}
    </div>
  );
}
