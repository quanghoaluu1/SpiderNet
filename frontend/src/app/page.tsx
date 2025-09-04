'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import Image from 'next/image';
import Header from '@/components/Header';
import LeftSidebar from '@/components/LeftSidebar';
import RightSidebar from '@/components/RightSidebar';
import CreatePost from '@/components/CreatePost';
import PostFeed from '@/components/PostFeed';
import Background from '@/components/Background';
import { User, Post } from '@/types';

export default function HomePage() {
  const router = useRouter();
  const [user, setUser] = useState<User | null>(null);
  const [posts] = useState<Post[]>([
    {
      id: 1,
      user: {
        name: "Spider-Man",
        avatar: "/icons/icon.png"
      },
      content: "Just swinging through the neighborhood! 🕷️ Always remember, with great power comes great responsibility.",
      image: "/icons/icon.png",
      likes: 1247,
      comments: 89,
      shares: 34,
      timestamp: "2 hours ago"
    },
    {
      id: 2,
      user: {
        name: "Peter Parker",
        avatar: "/icons/icon.png"
      },
      content: "Working on some new web-shooters in the lab today. Science is amazing! 🔬",
      likes: 892,
      comments: 56,
      shares: 23,
      timestamp: "4 hours ago"
    },
    {
      id: 3,
      user: {
        name: "Daily Bugle",
        avatar: "/icons/icon.png"
      },
      content: "BREAKING: Spider-Man saves another day in New York City! Our photographers captured these amazing shots.",
      image: "/icons/icon.png",
      likes: 2341,
      comments: 156,
      shares: 78,
      timestamp: "6 hours ago"
    }
  ]);

  useEffect(() => {
    if (typeof window !== 'undefined') {
      const userData = localStorage.getItem('user');
      if (userData) {
        setUser(JSON.parse(userData));
      } else {
        router.push('/login');
      }
    }
  }, [router]);

  const handleLogout = () => {
    if (typeof window !== 'undefined') {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
    }
    router.push('/login');
  };

  if (!user) {
    return (
      <div className="min-h-screen bg-bg-black flex items-center justify-center">
        <div className="relative">
          <div className="animate-spin rounded-full h-32 w-32 border-b-2 border-spiderman-red"></div>
          <div className="absolute inset-0 flex items-center justify-center">
            <Image src="/icons/icon.png" alt="Loading" width={24} height={24} className="rounded-full" />
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-bg-black">
      <Background />
      
      <div className="relative z-10">
        <Header user={user} onLogout={handleLogout} />
        
        <div className="max-w-7xl mx-auto px-4 py-6">
          <div className="grid grid-cols-1 lg:grid-cols-12 gap-6">
            <LeftSidebar user={user} />
            
            <div className="lg:col-span-6">
              <CreatePost user={user} />
              <PostFeed posts={posts} />
            </div>
            
            <RightSidebar />
          </div>
        </div>
      </div>
    </div>
  );
}
