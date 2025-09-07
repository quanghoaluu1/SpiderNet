using Mapster;
using SpiderNet.Application.DTOs.Comment;
using SpiderNet.Application.DTOs.Post;
using SpiderNet.Application.DTOs.Reaction;
using SpiderNet.Application.Extensions;
using SpiderNet.Application.Interfaces;
using SpiderNet.Application.Interfaces.Repositories;
using SpiderNet.Application.Interfaces.Services;
using SpiderNet.Domain.Common;
using SpiderNet.Domain.Entities;
using SpiderNet.Domain.Enum;

namespace SpiderNet.Application.Services;

public class PostService : IPostService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICloudinaryService _cloudinaryService;

    public PostService(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
    {
        _unitOfWork = unitOfWork;
        _cloudinaryService = cloudinaryService;
    }
    public async Task<Result<PostDto>> CreatePostAsync(Guid userId, CreatePostRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Content) && request.Image is null && request.Video is null)
            {
                return Result<PostDto>.Failure("Post must have content or at least one image or video.");
            }
            
            var post = request.Adapt<Post>();
            post.UserId = userId;
            
            if (request.Image is not null)
            {
                try
                {
                    var imageResult = await _cloudinaryService.UploadImageAsync(request.Image, "spidernet/posts");
                    post.ImageUrl = imageResult.Url;
                    post.ImagePublicId = imageResult.PublicId;
                }
                catch (Exception e)
                {
                    return Result<PostDto>.Failure($"Upload image failed: {e.Message}");
                }
            }

            if (request.Video is not null)
            {
                try
                {
                    var videoResult = await _cloudinaryService.UploadVideoAsync(request.Video, "spidernet/posts");
                    post.VideoUrl = videoResult.Url;
                    post.VideoPublicId = videoResult.PublicId;
                }
                catch (Exception e)
                {
                    if (!string.IsNullOrEmpty(post.ImagePublicId))
                    {
                        try
                        {
                            await _cloudinaryService.DeleteResourceAsync(post.ImagePublicId);
                        }
                        catch
                        {
                            // Ignore cleanup failures
                        }
                    }
                    return Result<PostDto>.Failure($"Upload video failed: {e.Message}");
                }
            }
            
            var createdPost = await _unitOfWork.PostRepository.CreateAsync(post);
            var postDto = await MapToPostDtoAsync(createdPost, userId);
            
            return Result<PostDto>.Success(postDto);
        }
        catch (Exception ex)
        {
            return Result<PostDto>.Failure($"An error occurred while creating post: {ex.Message}");
        }
    }

    public async Task<Result<PostDetailDto>> GetPostAsync(Guid postId, Guid? currentUserId = null)
    {
        try
        {
            var post = await _unitOfWork.PostRepository.GetDetailByIdAsync(postId);
            if (post == null)
                return Result<PostDetailDto>.Failure("Post not found");

            var postDto = await MapToPostDetailDtoAsync(post, currentUserId);
            return Result<PostDetailDto>.Success(postDto);
        }
        catch (Exception ex)
        {
            return Result<PostDetailDto>.Failure($"An error occurred while retrieving post: {ex.Message}");
        }
    }

    public async Task<Result<PostDto>> UpdatePostAsync(Guid postId, Guid userId, UpdatePostRequest request)
    {
        try
        {
            var post = await _unitOfWork.PostRepository.GetByIdAsync(postId);
            if (post == null)
                return Result<PostDto>.Failure("Post not found");

            if (post.UserId != userId)
                return Result<PostDto>.Failure("You can only edit your own posts");

            request.Adapt(post);

            var updatedPost = await _unitOfWork.PostRepository.UpdateAsync(post);
            var postDto = await MapToPostDtoAsync(updatedPost, userId);

            return Result<PostDto>.Success(postDto);
        }
        catch (Exception ex)
        {
            return Result<PostDto>.Failure($"An error occurred while updating post: {ex.Message}");
        }
    }

    public async Task<Result<bool>> DeletePostAsync(Guid postId, Guid userId)
    {
        try
        {
            var post = await _unitOfWork.PostRepository.GetByIdAsync(postId);
            if (post == null)
                return Result<bool>.Failure("Post not found");

            if (post.UserId != userId)
                return Result<bool>.Failure("You can only delete your own posts");

            // Clean up media files
            try
            {
                if (!string.IsNullOrEmpty(post.ImagePublicId))
                    await _cloudinaryService.DeleteResourceAsync(post.ImagePublicId);

                if (!string.IsNullOrEmpty(post.VideoPublicId))
                    await _cloudinaryService.DeleteResourceAsync(post.VideoPublicId);
            }
            catch (Exception ex)
            {
                // Log media cleanup failure but continue with post deletion
                // Media cleanup failure shouldn't prevent post deletion
            }

            var result = await _unitOfWork.PostRepository.DeleteAsync(postId);
            return Result<bool>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"An error occurred while deleting post: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<PostDto>>> GetUserPostsAsync(Guid userId, Guid? currentUserId = null, int pageNumber = 1, int pageSize = 20)
    {
        try
        {
            var posts = await _unitOfWork.PostRepository.GetUserPostsAsync(userId, pageNumber, pageSize);
            var postDtos = new List<PostDto>();

            foreach (var post in posts)
            {
                var dto = await MapToPostDtoAsync(post, currentUserId);
                postDtos.Add(dto);
            }

            return Result<IEnumerable<PostDto>>.Success(postDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<PostDto>>.Failure($"An error occurred while retrieving user posts: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<PostDto>>> GetNewsFeedAsync(Guid userId, int pageNumber = 1, int pageSize = 20)
    {
        try
        {
            var posts = await _unitOfWork.PostRepository.GetNewsFeedAsync(userId, pageNumber, pageSize);
            var postDtos = new List<PostDto>();

            foreach (var post in posts)
            {
                var dto = await MapToPostDtoAsync(post, userId);
                postDtos.Add(dto);
            }

            return Result<IEnumerable<PostDto>>.Success(postDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<PostDto>>.Failure($"An error occurred while retrieving news feed: {ex.Message}");
        }
    }

    public async Task<Result<ReactionDto>> AddReactionAsync(Guid postId, Guid userId, ReactionType reactionType)
    {
        try
        {
            var existingReaction = await _unitOfWork.ReactionRepository.GetReactionAsync(postId, userId);

            Reaction reaction;

            if (existingReaction != null)
            {
                if (existingReaction.Type == reactionType)
                {
                    await _unitOfWork.ReactionRepository.RemoveReactionAsync(postId, userId);
                    return Result<ReactionDto>.Success(null!); // Removed
                }
                else
                {
                    existingReaction.Type = reactionType;
                    reaction = await _unitOfWork.ReactionRepository.UpdateReactionAsync(existingReaction);
                }
            }
            else
            {
                reaction = new Reaction
                {
                    PostId = postId,
                    UserId = userId,
                    Type = reactionType
                };
                reaction = await _unitOfWork.ReactionRepository.AddReactionAsync(reaction);
            }
            
            var reactionDto = reaction.Adapt<ReactionDto>();
            return Result<ReactionDto>.Success(reactionDto);
        }
        catch (Exception ex)
        {
            return Result<ReactionDto>.Failure($"An error occurred while adding reaction: {ex.Message}");
        }
    }

    public async Task<Result<bool>> RemoveReactionAsync(Guid postId, Guid userId)
    {
        try
        {
            var result = await _unitOfWork.ReactionRepository.RemoveReactionAsync(postId, userId);
            return Result<bool>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"An error occurred while removing reaction: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<ReactionDto>>> GetPostReactionsAsync(Guid postId, ReactionType? type = null, int limit = 20)
    {
        try
        {
            var reactions = type.HasValue 
                ? await _unitOfWork.ReactionRepository.GetReactionsByTypeAsync(postId, type.Value, limit)
                : await _unitOfWork.ReactionRepository.GetPostReactionsAsync(postId, limit);

            var reactionDtos = reactions.Adapt<IEnumerable<ReactionDto>>();
            return Result<IEnumerable<ReactionDto>>.Success(reactionDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<ReactionDto>>.Failure($"An error occurred while retrieving post reactions: {ex.Message}");
        }
    }

    private async Task<PostDto?> MapToPostDtoAsync(Post post, Guid? currentUserId = null)
    {
        var postWithNavigation = await _unitOfWork.PostRepository.GetByIdAsync(post.Id);
        if (postWithNavigation == null)
            return null;
        var postDto = postWithNavigation.Adapt<PostDto>();
        postDto.ReactionsSummary = await MapToReactionSummaryAsync(post.Reactions);
        postDto.CurrentUserReaction = currentUserId.HasValue 
            ? post.Reactions.FirstOrDefault(r => r.UserId == currentUserId.Value)?.Type
            : null;
        postDto.IsOwnPost = currentUserId.HasValue && post.UserId == currentUserId.Value;
        return postDto;
    }
    
    private async Task<PostDetailDto> MapToPostDetailDtoAsync(Post post, Guid? currentUserId)
    {
        var baseDto = await MapToPostDtoAsync(post, currentUserId);
        var detailDto = baseDto.Adapt<PostDetailDto>();

        // Map comments
        detailDto.Comments = post.Comments
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.CreatedAt)
            .Select(c => MapToCommentDto(c, currentUserId))
            .ToList();

        // Map recent reactions
        detailDto.RecentReactions = post.Reactions
            .OrderByDescending(r => r.CreatedAt)
            .Take(10)
            .Adapt<List<ReactionDto>>();

        return detailDto;
    }

    private async Task<ReactionSummaryDto> MapToReactionSummaryAsync(ICollection<Reaction> reactions)
    {
        var grouped = reactions.GroupBy(r => r.Type).ToList();
        
        var summary = new ReactionSummaryDto
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

    private CommentDto MapToCommentDto(Comment comment, Guid? currentUserId)
    {
        var dto = comment.Adapt<CommentDto>();
        
        // Set computed fields
        dto.ReactionsSummary = MapToCommentReactionSummary(comment.Reactions);
        dto.CurrentUserReaction = currentUserId.HasValue 
            ? comment.Reactions.FirstOrDefault(r => r.UserId == currentUserId.Value)?.Type
            : null;
        dto.IsOwnComment = currentUserId.HasValue && comment.UserId == currentUserId.Value;

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