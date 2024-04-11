namespace ViewModels.Forum;

public class CommentVM
{
    public int Id { get; set; }

    public string Content { get; set; } = string.Empty;

    public int? ForumId { get; set; }

    public string ForumTitle { get; set; } = string.Empty;

    public string ForumSeoAlias { get; set; } = string.Empty;

    public string OwnerUserId { get; set; } = string.Empty;

    public string OwnerName { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? ReplyId { get; set; }

    public List<CommentVM> Children { get; set; } = [];
}
