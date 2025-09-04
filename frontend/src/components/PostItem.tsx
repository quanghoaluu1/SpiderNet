'use client';

import Image from 'next/image';
import { Heart, MessageCircle, Share } from 'lucide-react';
import { Post } from '@/types';

interface PostItemProps {
  post: Post;
}

export default function PostItem({ post }: PostItemProps) {
  return (
    <div className="glass-morphism rounded-lg shadow-xl border border-white/10">
      {/* Post Header */}
      <div className="p-4 border-b border-white/20">
        <div className="flex items-center space-x-3">
          <div className="bg-spiderman-blue p-1 rounded-full">
            <Image src={post.user.avatar} alt={post.user.name} width={36} height={36} className="rounded-full" />
          </div>
          <div>
            <h3 className="font-semibold text-white">{post.user.name}</h3>
            <p className="text-sm text-white/60">{post.timestamp}</p>
          </div>
        </div>
      </div>

      {/* Post Content */}
      <div className="p-4">
        <p className="mb-4 text-white/90">{post.content}</p>
        {post.image && (
          <div className="mb-4">
            <Image
              src={post.image}
              alt="Post content"
              width={500}
              height={300}
              className="w-full h-64 object-cover rounded-lg border border-white/10"
            />
          </div>
        )}
      </div>

      {/* Post Actions */}
      <div className="border-t border-white/20 p-4">
        <div className="flex justify-between items-center mb-3">
          <span className="text-sm text-white/70">{post.likes} likes</span>
          <span className="text-sm text-white/70">{post.comments} comments • {post.shares} shares</span>
        </div>
        <div className="flex justify-between">
          <button className="flex items-center space-x-2 text-white/70 hover:text-spiderman-red px-4 py-2 rounded-lg hover:bg-white/10 flex-1 justify-center transition-colors">
            <Heart className="w-5 h-5" />
            <span>Like</span>
          </button>
          <button className="flex items-center space-x-2 text-white/70 hover:text-spiderman-blue px-4 py-2 rounded-lg hover:bg-white/10 flex-1 justify-center transition-colors">
            <MessageCircle className="w-5 h-5" />
            <span>Comment</span>
          </button>
          <button className="flex items-center space-x-2 text-white/70 hover:text-spiderman-gold px-4 py-2 rounded-lg hover:bg-white/10 flex-1 justify-center transition-colors">
            <Share className="w-5 h-5" />
            <span>Share</span>
          </button>
        </div>
      </div>
    </div>
  );
}
