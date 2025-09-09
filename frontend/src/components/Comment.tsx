'use client';

import { useState } from 'react';
import Image from 'next/image';
import { MoreHorizontal, Reply } from 'lucide-react';
import { CommentDto, ReactionType } from '@/interfaces/Post';
import { commentApi } from '@/services/api';
import ReactionPicker from './ReactionPicker';
import VideoPlayer from './VideoPlayer';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Button } from '@/components/ui/button';
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from '@/components/ui/dropdown-menu';
import { toast } from 'sonner';
import { cn } from '@/lib/utils';

interface CommentProps {
  comment: CommentDto;
  onCommentUpdate?: (updatedComment: CommentDto) => void;
  onCommentDelete?: (commentId: string) => void;
  onReply?: (parentId: string) => void;
  depth?: number;
  maxDepth?: number;
}

export default function Comment({ 
  comment, 
  onCommentUpdate, 
  onCommentDelete, 
  onReply,
  depth = 0,
  maxDepth = 3
}: CommentProps) {
  const [isReacting, setIsReacting] = useState(false);
  const [showReplies, setShowReplies] = useState(false);
  const [localComment, setLocalComment] = useState<CommentDto>(comment);

  const handleReaction = async (type: ReactionType | undefined) => {
    if (type === undefined) {
      // Handle removing reaction - you may need to implement removeCommentReaction API
      return;
    }
    
    setIsReacting(true);
    try {
      const result = await commentApi.addCommentReaction(localComment.id, type);
      
      // Update local comment state
      const updatedComment: CommentDto = {
        ...localComment,
        currentUserReaction: result.reaction ? type : undefined,
        reactionsSummary: {
          ...localComment.reactionsSummary,
          totalCount: result.reaction 
            ? localComment.reactionsSummary.totalCount + 1 
            : localComment.reactionsSummary.totalCount - 1,
          // Update specific reaction counts
          likesCount: type === ReactionType.Like 
            ? (result.reaction ? localComment.reactionsSummary.likesCount + 1 : localComment.reactionsSummary.likesCount - 1)
            : localComment.reactionsSummary.likesCount,
          lovesCount: type === ReactionType.Love 
            ? (result.reaction ? localComment.reactionsSummary.lovesCount + 1 : localComment.reactionsSummary.lovesCount - 1)
            : localComment.reactionsSummary.lovesCount,
          hahaCount: type === ReactionType.Haha 
            ? (result.reaction ? localComment.reactionsSummary.hahaCount + 1 : localComment.reactionsSummary.hahaCount - 1)
            : localComment.reactionsSummary.hahaCount,
          wowCount: type === ReactionType.Wow 
            ? (result.reaction ? localComment.reactionsSummary.wowCount + 1 : localComment.reactionsSummary.wowCount - 1)
            : localComment.reactionsSummary.wowCount,
          sadCount: type === ReactionType.Sad 
            ? (result.reaction ? localComment.reactionsSummary.sadCount + 1 : localComment.reactionsSummary.sadCount - 1)
            : localComment.reactionsSummary.sadCount,
          angryCount: type === ReactionType.Angry 
            ? (result.reaction ? localComment.reactionsSummary.angryCount + 1 : localComment.reactionsSummary.angryCount - 1)
            : localComment.reactionsSummary.angryCount,
        }
      };
      
      setLocalComment(updatedComment);
      onCommentUpdate?.(updatedComment);
      
    } catch (error) {
      console.error('Error reacting to comment:', error);
      toast.error('Failed to react to comment');
    } finally {
      setIsReacting(false);
    }
  };

  const handleDeleteComment = async () => {
    try {
      await commentApi.deleteComment(localComment.id);
      toast.success('Comment deleted successfully');
      onCommentDelete?.(localComment.id);
    } catch (error) {
      console.error('Error deleting comment:', error);
      toast.error('Failed to delete comment');
    }
  };

  const getTopReactions = () => {
    const reactions = [
      { type: ReactionType.Like, count: localComment.reactionsSummary.likesCount, emoji: '👍' },
      { type: ReactionType.Love, count: localComment.reactionsSummary.lovesCount, emoji: '❤️' },
      { type: ReactionType.Haha, count: localComment.reactionsSummary.hahaCount, emoji: '😂' },
      { type: ReactionType.Wow, count: localComment.reactionsSummary.wowCount, emoji: '😮' },
      { type: ReactionType.Sad, count: localComment.reactionsSummary.sadCount, emoji: '😢' },
      { type: ReactionType.Angry, count: localComment.reactionsSummary.angryCount, emoji: '😠' }
    ];

    return reactions
      .filter(r => r.count > 0)
      .sort((a, b) => b.count - a.count)
      .slice(0, 3);
  };

  const topReactions = getTopReactions();
  const canReply = depth < maxDepth;

  return (
    <div className={cn(
      "flex space-x-3",
      depth > 0 && "ml-8 mt-3",
      depth === 0 && "mb-4"
    )}>
      {/* Avatar */}
      <Avatar className="h-8 w-8 flex-shrink-0">
        <AvatarImage 
          src={localComment.userAvatarUrl || "/icons/icon.png"} 
          alt={localComment.userFullName}
        />
        <AvatarFallback className="bg-gradient-to-br from-spiderman-red to-spiderman-blue text-white text-xs">
          {localComment.userFullName.split(' ').map(n => n[0]).join('')}
        </AvatarFallback>
      </Avatar>

      <div className="flex-1">
        {/* Comment Bubble */}
        <div className="bg-white/10 backdrop-blur-sm rounded-2xl px-4 py-3 border border-white/20">
          <div className="flex items-center justify-between mb-1">
            <h4 className="font-semibold text-white text-sm">{localComment.userDisplayName}</h4>
            {localComment.isOwnComment && (
              <DropdownMenu>
                <DropdownMenuTrigger asChild>
                  <Button variant="ghost" size="sm" className="h-6 w-6 p-0 text-white/70 hover:text-white">
                    <MoreHorizontal className="w-3 h-3" />
                  </Button>
                </DropdownMenuTrigger>
                <DropdownMenuContent className="bg-black/90 border-white/20">
                  <DropdownMenuItem 
                    onClick={handleDeleteComment}
                    className="text-red-400 hover:bg-red-500/20"
                  >
                    Delete Comment
                  </DropdownMenuItem>
                </DropdownMenuContent>
              </DropdownMenu>
            )}
          </div>
          
          {/* Comment Content */}
          <p className="text-white text-sm leading-relaxed whitespace-pre-wrap">
            {localComment.content}
          </p>

          {/* Comment Media */}
          {localComment.imageUrl && (
            <div className="mt-2 rounded-lg overflow-hidden">
              <Image
                src={localComment.imageUrl}
                alt="Comment image"
                width={300}
                height={200}
                className="w-full h-auto max-w-xs object-cover"
              />
            </div>
          )}

          {localComment.videoUrl && (
            <div className="mt-2 max-w-xs">
              <VideoPlayer
                src={localComment.videoUrl}
                controls={true}
                fluid={true}
              />
            </div>
          )}

          {localComment.gifUrl && (
            <div className="mt-2 rounded-lg overflow-hidden">
              <Image
                src={localComment.gifUrl}
                alt="Comment GIF"
                width={200}
                height={150}
                className="w-auto h-auto max-w-xs max-h-40 object-cover"
              />
            </div>
          )}
        </div>

        {/* Reaction Summary */}
        {localComment.reactionsSummary.totalCount > 0 && (
          <div className="flex items-center space-x-1 mt-1 ml-4">
            <div className="flex -space-x-1">
              {topReactions.map((reaction) => (
                <div
                  key={reaction.type}
                  className="w-4 h-4 bg-white/10 rounded-full flex items-center justify-center text-xs border border-white/20"
                >
                  {reaction.emoji}
                </div>
              ))}
            </div>
            <span className="text-white/60 text-xs">
              {localComment.reactionsSummary.totalCount}
            </span>
          </div>
        )}

        {/* Action Buttons */}
        <div className="flex items-center space-x-4 mt-2 ml-4">
          <div className="text-xs text-white/60">{localComment.timeAgo}</div>
          
          <ReactionPicker
            onReactionSelect={handleReaction}
            currentReaction={localComment.currentUserReaction}
            className={cn(
              isReacting && "opacity-50 pointer-events-none"
            )}
          />
          
          {canReply && (
            <Button
              variant="ghost"
              size="sm"
              onClick={() => onReply?.(localComment.id)}
              className="text-white/60 hover:text-white text-xs p-1 h-auto"
            >
              <Reply className="w-3 h-3 mr-1" />
              Reply
            </Button>
          )}
        </div>

        {/* Replies */}
        {localComment.repliesCount > 0 && (
          <div className="mt-3">
            {!showReplies ? (
              <Button
                variant="ghost"
                size="sm"
                onClick={() => setShowReplies(true)}
                className="text-spiderman-blue hover:text-spiderman-blue/80 text-xs p-0 h-auto font-medium"
              >
                View {localComment.repliesCount} {localComment.repliesCount === 1 ? 'reply' : 'replies'}
              </Button>
            ) : (
              <div>
                <Button
                  variant="ghost"
                  size="sm"
                  onClick={() => setShowReplies(false)}
                  className="text-white/60 hover:text-white text-xs p-0 h-auto mb-2"
                >
                  Hide replies
                </Button>
                {localComment.replies.map((reply) => (
                  <Comment
                    key={reply.id}
                    comment={reply}
                    onCommentUpdate={onCommentUpdate}
                    onCommentDelete={onCommentDelete}
                    onReply={onReply}
                    depth={depth + 1}
                    maxDepth={maxDepth}
                  />
                ))}
                {localComment.hasMoreReplies && (
                  <Button
                    variant="ghost"
                    size="sm"
                    className="text-spiderman-blue hover:text-spiderman-blue/80 text-xs p-0 h-auto mt-2 ml-8"
                  >
                    Load more replies...
                  </Button>
                )}
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
}
