using SpiderNet.Application.DTOs.Comment;
using SpiderNet.Application.DTOs.Reaction;

namespace SpiderNet.Application.DTOs.Post;

public class PostDetailDto: PostDto
{
    public List<CommentDto> Comments { get; set; } = new();
    public List<ReactionDto> RecentReactions { get; set; } = new();
}