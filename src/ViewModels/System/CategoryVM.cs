namespace ViewModels.System;

public class CategoryVM
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public string SeoAlias { get; set; } = string.Empty;

    public string SeoDescription { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public int? ParentId { get; set; }

    public int? NumberOfTickets { get; set; }
}
