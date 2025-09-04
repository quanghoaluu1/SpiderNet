'use client';

import Image from 'next/image';
import { Video, Plus } from 'lucide-react';
import { User } from '@/types';

interface CreatePostProps {
  user: User;
}

export default function CreatePost({ user }: CreatePostProps) {
  return (
    <div className="glass-morphism rounded-lg shadow-xl p-4 mb-6 border border-white/10">
      <div className="flex items-center space-x-3">
        <div className="bg-spiderman-red p-1 rounded-full">
          <Image src="/icons/icon.png" alt={user.firstName} width={36} height={36} className="rounded-full" />
        </div>
        <input
          type="text"
          placeholder={`What's on your mind, ${user.firstName}?`}
          className="flex-1 bg-white/10 border border-white/20 rounded-full px-4 py-2 outline-none text-white placeholder-white/60 focus:ring-2 focus:ring-spiderman-blue backdrop-blur-sm"
        />
      </div>
      <div className="flex justify-between items-center mt-3 pt-3 border-t border-white/20">
        <div className="flex space-x-4">
          <button className="flex items-center space-x-2 text-white/70 hover:text-spiderman-blue transition-colors">
            <Video className="w-5 h-5" />
            <span className="text-sm">Live Video</span>
          </button>
          <button className="flex items-center space-x-2 text-white/70 hover:text-spiderman-red transition-colors">
            <Plus className="w-5 h-5" />
            <span className="text-sm">Photo/Video</span>
          </button>
        </div>
        <button className="bg-spiderman-red text-white px-4 py-2 rounded-lg text-sm font-medium hover:bg-spiderman-dark-red transition-colors shadow-lg">
          Post
        </button>
      </div>
    </div>
  );
}
