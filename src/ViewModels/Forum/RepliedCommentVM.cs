namespace ViewModels.Forum;

public class RepliedCommentVM
{
    public string RepliedName { get; set; } = string.Empty;

    public string CommentContent { get; set; } = string.Empty;
    public string ForumTitle { get; set; } = string.Empty;

    public int ForumId { get; set; }

    public string ForumSeoAlias { get; set; } = string.Empty;
}
