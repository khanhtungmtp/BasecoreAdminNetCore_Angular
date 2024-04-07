namespace ViewModels.Forum;

public class CommentCreateRequest
{
    public string Content { get; set; } = string.Empty;

    public int ForumId { get; set; }

    public int? ReplyId { get; set; }
    public string UserId { get; set; } = string.Empty;
}
