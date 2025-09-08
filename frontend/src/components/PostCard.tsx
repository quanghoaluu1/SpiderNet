'use client';

import { useState } from 'react';
import Image from 'next/image';
import { MoreHorizontal, MessageCircle, Share, Globe, Users, Lock } from 'lucide-react';
import { PostDto, ReactionType, PostPrivacy } from '@/interfaces/Post';
import { postApi } from '@/services/api';
import ReactionPicker from './ReactionPicker';
import VideoPlayer from './VideoPlayer';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Button } from '@/components/ui/button';
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from '@/components/ui/dropdown-menu';
import { toast } from 'sonner';
import { cn } from '@/lib/utils';

interface PostCardProps {
  post: PostDto;
  onPostUpdate?: (updatedPost: PostDto) => void;
  onPostDelete?: (postId: string) => void;
}

export default function PostCard({ post, onPostUpdate, onPostDelete }: PostCardProps) {
  const [isReacting, setIsReacting] = useState(false);
  const [showComments, setShowComments] = useState(false);
  const [localPost, setLocalPost] = useState<PostDto>(post);

  const handleReaction = async (type: ReactionType) => {
    setIsReacting(true);
    try {
      const result = await postApi.addReaction(localPost.id, type);
      
      // Update local post state
      const updatedPost: PostDto = {
        ...localPost,
        currentUserReaction: result.reaction ? type : undefined,
        reactionsSummary: {
          ...localPost.reactionsSummary,
          totalCount: result.reaction 
            ? localPost.reactionsSummary.totalCount + 1 
            : localPost.reactionsSummary.totalCount - 1,
          // Update specific reaction counts
          likesCount: type === ReactionType.Like 
            ? (result.reaction ? localPost.reactionsSummary.likesCount + 1 : localPost.reactionsSummary.likesCount - 1)
            : localPost.reactionsSummary.likesCount,
          lovesCount: type === ReactionType.Love 
            ? (result.reaction ? localPost.reactionsSummary.lovesCount + 1 : localPost.reactionsSummary.lovesCount - 1)
            : localPost.reactionsSummary.lovesCount,
          hahaCount: type === ReactionType.Haha 
            ? (result.reaction ? localPost.reactionsSummary.hahaCount + 1 : localPost.reactionsSummary.hahaCount - 1)
            : localPost.reactionsSummary.hahaCount,
          wowCount: type === ReactionType.Wow 
            ? (result.reaction ? localPost.reactionsSummary.wowCount + 1 : localPost.reactionsSummary.wowCount - 1)
            : localPost.reactionsSummary.wowCount,
          sadCount: type === ReactionType.Sad 
            ? (result.reaction ? localPost.reactionsSummary.sadCount + 1 : localPost.reactionsSummary.sadCount - 1)
            : localPost.reactionsSummary.sadCount,
          angryCount: type === ReactionType.Angry 
            ? (result.reaction ? localPost.reactionsSummary.angryCount + 1 : localPost.reactionsSummary.angryCount - 1)
            : localPost.reactionsSummary.angryCount,
        }
      };
      
      setLocalPost(updatedPost);
      onPostUpdate?.(updatedPost);
      
    } catch (error) {
      console.error('Error reacting to post:', error);
      toast.error('Failed to react to post');
    } finally {
      setIsReacting(false);
    }
  };

  const handleDeletePost = async () => {
    try {
      await postApi.deletePost(localPost.id);
      toast.success('Post deleted successfully');
      onPostDelete?.(localPost.id);
    } catch (error) {
      console.error('Error deleting post:', error);
      toast.error('Failed to delete post');
    }
  };

  const getPrivacyIcon = () => {
    switch (localPost.privacy) {
      case PostPrivacy.Public:
        return <Globe className="w-3 h-3 text-white/60" />;
      case PostPrivacy.Friends:
        return <Users className="w-3 h-3 text-white/60" />;
      case PostPrivacy.Private:
        return <Lock className="w-3 h-3 text-white/60" />;
      default:
        return <Globe className="w-3 h-3 text-white/60" />;
    }
  };

  const getTopReactions = () => {
    const reactions = [
      { type: ReactionType.Like, count: localPost.reactionsSummary.likesCount, emoji: '👍' },
      { type: ReactionType.Love, count: localPost.reactionsSummary.lovesCount, emoji: '❤️' },
      { type: ReactionType.Haha, count: localPost.reactionsSummary.hahaCount, emoji: '😂' },
      { type: ReactionType.Wow, count: localPost.reactionsSummary.wowCount, emoji: '😮' },
      { type: ReactionType.Sad, count: localPost.reactionsSummary.sadCount, emoji: '😢' },
      { type: ReactionType.Angry, count: localPost.reactionsSummary.angryCount, emoji: '😠' }
    ];

    return reactions
      .filter(r => r.count > 0)
      .sort((a, b) => b.count - a.count)
      .slice(0, 3);
  };

  const topReactions = getTopReactions();

  return (
    <div className="glass-morphism rounded-lg shadow-xl p-4 mb-6 border border-white/10">
      {/* Header */}
      <div className="flex items-center justify-between mb-3">
        <div className="flex items-center space-x-3">
          <Avatar className="h-10 w-10">
            <AvatarImage 
              src={localPost.userAvatarUrl || "/icons/icon.png"} 
              alt={localPost.userFullName}
            />
            <AvatarFallback className="bg-gradient-to-br from-spiderman-red to-spiderman-blue text-white">
              {localPost.userFullName.split(' ').map(n => n[0]).join('')}
            </AvatarFallback>
          </Avatar>
          <div>
            <div className="flex items-center space-x-2">
              <h3 className="font-semibold text-white">{localPost.userDisplayName}</h3>
              {getPrivacyIcon()}
            </div>
            <p className="text-sm text-white/60">{localPost.timeAgo}</p>
          </div>
        </div>
        
        {localPost.isOwnPost && (
          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <Button variant="ghost" size="sm" className="text-white/70 hover:text-white">
                <MoreHorizontal className="w-4 h-4" />
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent className="bg-black/90 border-white/20">
              <DropdownMenuItem 
                onClick={handleDeletePost}
                className="text-red-400 hover:bg-red-500/20"
              >
                Delete Post
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        )}
      </div>

      {/* Content */}
      {localPost.content && (
        <div className="mb-4">
          <p className="text-white text-base leading-relaxed whitespace-pre-wrap">
            {localPost.content}
          </p>
        </div>
      )}

      {/* Media */}
      {localPost.imageUrl && (
        <div className="mb-4 rounded-lg overflow-hidden">
          <Image
            src={localPost.imageUrl}
            alt="Post image"
            width={600}
            height={400}
            className="w-full h-auto object-cover cursor-pointer hover:opacity-90 transition-opacity"
          />
        </div>
      )}

      {localPost.videoUrl && (
        <div className="mb-4">
          <div className="text-white/70 text-xs mb-2">Debug: Video URL = {localPost.videoUrl}</div>
          <VideoPlayer
            src={localPost.videoUrl}
            controls={true}
            fluid={true}
          />
          
          {/* Fallback native video player */}
          <div className="mt-2 text-white/70 text-xs">
            Fallback player:
            <video 
              src={localPost.videoUrl} 
              controls 
              className="w-full mt-1 rounded-lg"
              style={{ maxHeight: '400px' }}
            >
              Your browser does not support the video tag.
            </video>
          </div>
        </div>
      )}

      {/* Reaction Summary */}
      {localPost.reactionsSummary.totalCount > 0 && (
        <div className="flex items-center justify-between py-2 mb-3 border-b border-white/10">
          <div className="flex items-center space-x-1">
            {/* Top reaction emojis */}
            <div className="flex -space-x-1">
              {topReactions.map((reaction) => (
                <div
                  key={reaction.type}
                  className="w-6 h-6 bg-white/10 rounded-full flex items-center justify-center text-sm border border-white/20"
                >
                  {reaction.emoji}
                </div>
              ))}
            </div>
            <span className="text-white/70 text-sm ml-2">
              {localPost.reactionsSummary.totalCount}
            </span>
          </div>
          
          <div className="flex items-center space-x-4 text-white/70 text-sm">
            {localPost.commentsCount > 0 && (
              <span>{localPost.commentsCount} comments</span>
            )}
          </div>
        </div>
      )}

      {/* Action Buttons */}
      <div className="flex items-center justify-around pt-2">
        <ReactionPicker
          onReactionSelect={handleReaction}
          currentReaction={localPost.currentUserReaction}
          className={cn(
            "flex-1 flex justify-center",
            isReacting && "opacity-50 pointer-events-none"
          )}
        />
        
        <Button
          variant="ghost"
          onClick={() => setShowComments(!showComments)}
          className="flex-1 flex items-center justify-center space-x-2 text-white/70 hover:text-white hover:bg-white/10 py-2"
        >
          <MessageCircle className="w-5 h-5" />
          <span className="text-sm font-medium">Comment</span>
        </Button>
        
        <Button
          variant="ghost"
          className="flex-1 flex items-center justify-center space-x-2 text-white/70 hover:text-white hover:bg-white/10 py-2"
        >
          <Share className="w-5 h-5" />
          <span className="text-sm font-medium">Share</span>
        </Button>
      </div>

      {/* Comments Section */}
      {showComments && (
        <div className="mt-4 pt-4 border-t border-white/20">
          <div className="text-white/70 text-sm">
            Comments feature coming soon...
          </div>
        </div>
      )}
    </div>
  );
}
