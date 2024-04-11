namespace ViewModels.System;

public class ReportVM
{
    public int Id { get; set; }

    public int? ForumId { get; set; }

    public string Content { get; set; } = string.Empty;

    public string ReportUserId { get; set; } = string.Empty;

    public string ReportUserName { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }

    public bool IsProcessed { get; set; }

    public string Type { get; set; } = string.Empty;
}
