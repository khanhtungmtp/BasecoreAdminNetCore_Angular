namespace ViewModels.Forum;

public class VoteVM
{
    public int ForumId { get; set; }
    public string UserId { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
