namespace ViewModels.Forum;

public class VoteVM
{
    public int ForumId { get; set; }
    public string UserId { get; set; } = string.Empty;

    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
}
