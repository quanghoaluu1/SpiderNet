'use client';

import { useState, useEffect } from 'react';
import PostCard from './PostCard';
import { PostDto } from '@/interfaces/Post';
import { postApi } from '@/services/api';
import { toast } from 'sonner';

interface PostFeedProps {
  refreshTrigger?: number;
}

export default function PostFeed({ refreshTrigger }: PostFeedProps) {
  const [posts, setPosts] = useState<PostDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);

  const fetchPosts = async (pageNum: number = 1, reset: boolean = false) => {
    try {
      const newPosts = await postApi.getNewsFeed(pageNum, 20);
      
      if (reset) {
        setPosts(newPosts);
      } else {
        setPosts(prev => [...prev, ...newPosts]);
      }
      
      setHasMore(newPosts.length === 20);
      setPage(pageNum);
    } catch (error) {
      console.error('Error fetching posts:', error);
      toast.error('Failed to load posts');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchPosts(1, true);
  }, [refreshTrigger]);

  const handlePostUpdate = (updatedPost: PostDto) => {
    setPosts(prev => prev.map(post => 
      post.id === updatedPost.id ? updatedPost : post
    ));
  };

  const handlePostDelete = (postId: string) => {
    setPosts(prev => prev.filter(post => post.id !== postId));
  };

  const loadMorePosts = () => {
    if (!isLoading && hasMore) {
      setIsLoading(true);
      fetchPosts(page + 1, false);
    }
  };

  if (isLoading && posts.length === 0) {
    return (
      <div className="space-y-6">
        {[...Array(3)].map((_, i) => (
          <div key={i} className="glass-morphism rounded-lg shadow-xl p-4 border border-white/10 animate-pulse">
            <div className="flex items-center space-x-3 mb-4">
              <div className="w-10 h-10 bg-white/20 rounded-full"></div>
              <div className="space-y-2">
                <div className="w-32 h-4 bg-white/20 rounded"></div>
                <div className="w-20 h-3 bg-white/20 rounded"></div>
              </div>
            </div>
            <div className="space-y-3">
              <div className="w-full h-4 bg-white/20 rounded"></div>
              <div className="w-3/4 h-4 bg-white/20 rounded"></div>
            </div>
            <div className="w-full h-48 bg-white/20 rounded-lg mt-4"></div>
          </div>
        ))}
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {posts.length === 0 ? (
        <div className="glass-morphism rounded-lg shadow-xl p-8 text-center border border-white/10">
          <div className="text-white/60 mb-4">
            <div className="w-16 h-16 mx-auto mb-4 bg-white/10 rounded-full flex items-center justify-center">
              <span className="text-2xl">📝</span>
            </div>
            <h3 className="text-lg font-semibold text-white mb-2">No posts yet</h3>
            <p className="text-sm">Be the first to share something amazing!</p>
          </div>
        </div>
      ) : (
        <>
          {posts.map((post) => (
            <PostCard 
              key={post.id} 
              post={post} 
              onPostUpdate={handlePostUpdate}
              onPostDelete={handlePostDelete}
            />
          ))}
          
          {/* Load More Button */}
          {hasMore && (
            <div className="text-center py-6">
              <button
                onClick={loadMorePosts}
                disabled={isLoading}
                className="bg-spiderman-blue hover:bg-spiderman-blue/80 text-white px-6 py-2 rounded-lg font-medium transition-colors disabled:opacity-50"
              >
                {isLoading ? 'Loading...' : 'Load More Posts'}
              </button>
            </div>
          )}
        </>
      )}
    </div>
  );
}
