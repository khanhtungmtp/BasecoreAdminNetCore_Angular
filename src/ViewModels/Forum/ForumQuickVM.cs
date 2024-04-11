namespace ViewModels.Forum;

public class ForumQuickVM
{
    public int Id { get; set; }

    public int? CategoryId { get; set; }

    public string CategoryAlias { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string SeoAlias { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int? ViewCount { get; set; } = 0;

    public DateTime CreatedDate { get; set; }

    public int? NumberOfVotes { get; set; } = 0;

    public int? NumberOfComments { get; set; } = 0;
}
