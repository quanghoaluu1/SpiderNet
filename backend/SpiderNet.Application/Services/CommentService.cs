using Mapster;
using SpiderNet.Application.DTOs.Comment;
using SpiderNet.Application.DTOs.Reaction;
using SpiderNet.Application.Extensions;
using SpiderNet.Application.Interfaces;
using SpiderNet.Application.Interfaces.Repositories;
using SpiderNet.Application.Interfaces.Services;
using SpiderNet.Domain.Common;
using SpiderNet.Domain.Entities;
using SpiderNet.Domain.Enum;

namespace SpiderNet.Application.Services;

public class CommentService : ICommentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICloudinaryService _cloudinaryService;
    public CommentService(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
    {
        _unitOfWork = unitOfWork;
        _cloudinaryService = cloudinaryService;
    }
    public async Task<Result<CommentDto>> AddCommentAsync(Guid postId, Guid userId, CreateCommentRequest request)
    {
        try
        {
            // Validation
            if (string.IsNullOrWhiteSpace(request.Content) && 
                request.Image == null && 
                request.Video == null && 
                request.Gif == null)
            {
                return Result<CommentDto>.Failure("Comment must have content or media");
            }

            // Validate only one media type
            var mediaCount = 0;
            if (request.Image != null) mediaCount++;
            if (request.Video != null) mediaCount++;
            if (request.Gif != null) mediaCount++;
            
            if (mediaCount > 1)
                return Result<CommentDto>.Failure("Comment can only have one type of media");

            // Validate parent comment
            if (request.ParentCommentId.HasValue)
            {
                var parentComment = await _unitOfWork.CommentRepository.GetByIdAsync(request.ParentCommentId.Value);
                if (parentComment == null)
                    return Result<CommentDto>.Failure("Parent comment not found");
                
                if (parentComment.PostId != postId)
                    return Result<CommentDto>.Failure("Parent comment does not belong to this post");
            }
            
            var postExists = await _unitOfWork.PostRepository.GetByIdAsync(postId);
            if (postExists is null)
                return Result<CommentDto>.Failure("Post not found");

            var userExists = await _unitOfWork.UserRepository.GetByIdAsync(userId);  
            if (userExists is null)
                return Result<CommentDto>.Failure("User not found");
                
            // Create comment
            var comment = request.Adapt<Comment>();
            comment.PostId = postId;
            comment.UserId = userId;

            comment.Post = null;
            comment.User = null;
            comment.ParentComment = null;
            
            // Handle media upload
            try
            {
                if (request.Image != null)
                {
                    var imageResult = await _cloudinaryService.UploadImageAsync(request.Image, "social-network/comments");
                    comment.ImageUrl = imageResult.Url;
                    comment.ImagePublicId = imageResult.PublicId;
                    comment.MediaType = MediaType.Image;
                }
                else if (request.Video != null)
                {
                    var videoResult = await _cloudinaryService.UploadVideoAsync(request.Video, "social-network/comments");
                    comment.VideoUrl = videoResult.Url;
                    comment.VideoPublicId = videoResult.PublicId;
                    comment.MediaType = MediaType.Video;
                }
                else if (request.Gif != null)
                {
                    var gifResult = await _cloudinaryService.UploadGifAsync(request.Gif, "social-network/comments");
                    comment.GifUrl = gifResult.Url;
                    comment.GifPublicId = gifResult.PublicId;
                    comment.MediaType = MediaType.Gif;
                }
            }
            catch (Exception ex)
            {
                return Result<CommentDto>.Failure($"Media upload failed: {ex.Message}");
            }

            var createdComment = await _unitOfWork.CommentRepository.CreateAsync(comment);
            var commentDto = MapToCommentDto(createdComment, userId);

            return Result<CommentDto>.Success(commentDto);
        }
        catch (Exception ex)
        {
            return Result<CommentDto>.Failure($"An error occurred while adding comment: {ex.Message}");
        }
    }

    public async Task<Result<bool>> DeleteCommentAsync(Guid commentId, Guid userId)
    {
        try
        {
            if (!await _unitOfWork.CommentRepository.IsCommentOwnerAsync(commentId, userId))
                return Result<bool>.Failure("You can only delete your own comments");

            // Get comment to clean up media
            var comment = await _unitOfWork.CommentRepository.GetByIdAsync(commentId);
            if (comment != null)
            {
                // Delete media from Cloudinary
                try
                {
                    if (!string.IsNullOrEmpty(comment.ImagePublicId))
                        await _cloudinaryService.DeleteResourceAsync(comment.ImagePublicId);
                    
                    if (!string.IsNullOrEmpty(comment.VideoPublicId))
                        await _cloudinaryService.DeleteResourceAsync(comment.VideoPublicId);
                    
                    if (!string.IsNullOrEmpty(comment.GifPublicId))
                        await _cloudinaryService.DeleteResourceAsync(comment.GifPublicId);
                }
                catch (Exception ex)
                {
                    // Log media deletion failure but continue with comment deletion
                    // Media cleanup failure shouldn't prevent comment deletion
                }
            }

            var result = await _unitOfWork.CommentRepository.DeleteAsync(commentId);
            return Result<bool>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"An error occurred while deleting comment: {ex.Message}");
        }
    }
    
    public async Task<Result<IEnumerable<CommentDto>>> GetPostCommentsAsync(Guid postId, Guid? currentUserId = null, int pageNumber = 1, int pageSize = 20)
    {
        try
        {
            var comments = await _unitOfWork.CommentRepository.GetPostCommentsAsync(postId, pageNumber, pageSize);
            var commentDtos = comments.Select(c => MapToCommentDto(c, currentUserId, includeReplies: true)).ToList();

            return Result<IEnumerable<CommentDto>>.Success(commentDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<CommentDto>>.Failure($"An error occurred while retrieving comments: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<CommentDto>>> GetCommentRepliesAsync(Guid parentCommentId, Guid? currentUserId = null, int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            var replies = await _unitOfWork.CommentRepository.GetCommentRepliesAsync(parentCommentId, pageNumber, pageSize);
            var replyDtos = replies.Select(r => MapToCommentDto(r, currentUserId)).ToList();

            return Result<IEnumerable<CommentDto>>.Success(replyDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<CommentDto>>.Failure($"An error occurred while retrieving comment replies: {ex.Message}");
        }
    }

    public async Task<Result<CommentReactionDto>> AddCommentReactionAsync(Guid commentId, Guid userId, ReactionType reactionType)
    {
        try
        {
            var existingReaction = await _unitOfWork.CommentReactionRepository.GetReactionAsync(commentId, userId);

            CommentReaction reaction;

            if (existingReaction != null)
            {
                if (existingReaction.Type == reactionType)
                {
                    await _unitOfWork.CommentReactionRepository.RemoveReactionAsync(commentId, userId);
                    return Result<CommentReactionDto>.Success(null);
                }
                else
                {
                    existingReaction.Type = reactionType;
                    reaction = await _unitOfWork.CommentReactionRepository.UpdateReactionAsync(existingReaction);
                }
            }
            else
            {
                reaction = new CommentReaction
                {
                    CommentId = commentId,
                    UserId = userId,
                    Type = reactionType
                };
                reaction = await _unitOfWork.CommentReactionRepository.AddReactionAsync(reaction);
            }
            
            var getReaction = await _unitOfWork.CommentReactionRepository.GetReactionAsync(commentId, userId);
            var reactionDto = getReaction.Adapt<CommentReactionDto>();
            return Result<CommentReactionDto>.Success(reactionDto);
        }
        catch (Exception ex)
        {
            return Result<CommentReactionDto>.Failure($"An error occurred while adding comment reaction: {ex.Message}");
        }
    }

    public async Task<Result<CommentDto>> UpdateCommentAsync(Guid commentId, Guid userId, UpdateCommentRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Content))
                return Result<CommentDto>.Failure("Comment content is required");

            var comment = await _unitOfWork.CommentRepository.GetByIdAsync(commentId);
            if (comment == null)
                return Result<CommentDto>.Failure("Comment not found");

            if (comment.UserId != userId)
                return Result<CommentDto>.Failure("You can only edit your own comments");

            request.Adapt(comment);
            var updatedComment = await _unitOfWork.CommentRepository.UpdateAsync(comment);
            var commentDto = MapToCommentDto(updatedComment, userId);

            return Result<CommentDto>.Success(commentDto);
        }
        catch (Exception ex)
        {
            return Result<CommentDto>.Failure($"An error occurred while updating comment: {ex.Message}");
        }
    }
    public async Task<Result<bool>> RemoveCommentReactionAsync(Guid commentId, Guid userId)
    {
        try
        {
            var result = await _unitOfWork.CommentReactionRepository.RemoveReactionAsync(commentId, userId);
            return Result<bool>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"An error occurred while removing comment reaction: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<CommentReactionDto>>> GetCommentReactionsAsync(Guid commentId, ReactionType? type = null, int limit = 20)
    {
        try
        {
            var reactions = type.HasValue 
                ? await _unitOfWork.CommentReactionRepository.GetReactionsByTypeAsync(commentId, type.Value, limit)
                : await _unitOfWork.CommentReactionRepository.GetCommentReactionsAsync(commentId, limit);

            var reactionDtos = reactions.Adapt<IEnumerable<CommentReactionDto>>();
            return Result<IEnumerable<CommentReactionDto>>.Success(reactionDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<CommentReactionDto>>.Failure($"An error occurred while retrieving comment reactions: {ex.Message}");
        }
    }
    
    private CommentDto MapToCommentDto(Comment comment, Guid? currentUserId, bool includeReplies = false)
    {
        var getComment = _unitOfWork.CommentRepository.GetByIdAsync(comment.Id).Result;
        var dto = getComment.Adapt<CommentDto>();
        
        // Set computed fields
        dto.ReactionsSummary = MapToCommentReactionSummary(comment.Reactions);
        dto.CurrentUserReaction = currentUserId.HasValue 
            ? comment.Reactions.FirstOrDefault(r => r.UserId == currentUserId.Value)?.Type
            : null;
        dto.IsOwnComment = currentUserId.HasValue && comment.UserId == currentUserId.Value;
        dto.HasMedia = comment.HasMedia;

        // Include replies for top-level comments
        if (includeReplies && !comment.IsReply && comment.Replies.Any())
        {
            dto.Replies = comment.Replies
                .Where(r => !r.IsDeleted)
                .Take(3)
                .Select(r => MapToCommentDto(r, currentUserId))
                .ToList();
                
            dto.HasMoreReplies = comment.RepliesCount > 3;
        }

        return dto;
    }
    
    private CommentReactionSummaryDto MapToCommentReactionSummary(ICollection<CommentReaction> reactions)
    {
        var grouped = reactions.GroupBy(r => r.Type).ToList();
        
        var summary = new CommentReactionSummaryDto
        {
            TotalCount = reactions.Count,
            LikesCount = grouped.FirstOrDefault(g => g.Key == ReactionType.Like)?.Count() ?? 0,
            LovesCount = grouped.FirstOrDefault(g => g.Key == ReactionType.Love)?.Count() ?? 0,
            HahaCount = grouped.FirstOrDefault(g => g.Key == ReactionType.Haha)?.Count() ?? 0,
            WowCount = grouped.FirstOrDefault(g => g.Key == ReactionType.Wow)?.Count() ?? 0,
            SadCount = grouped.FirstOrDefault(g => g.Key == ReactionType.Sad)?.Count() ?? 0,
            AngryCount = grouped.FirstOrDefault(g => g.Key == ReactionType.Angry)?.Count() ?? 0
        };

        summary.TopReactions = grouped
            .OrderByDescending(g => g.Count())
            .Take(3)
            .Select(g => new ReactionTypeCount
            {
                Type = g.Key,
                TypeEmoji = g.Key.GetEmoji(),
                TypeName = g.Key.GetDisplayName(),
                Count = g.Count()
            })
            .ToList();

        return summary;
    }
}