namespace ViewModels.System;

public class ReportCreateRequest
{
    public int? ForumId { get; set; }

    public string Content { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}
