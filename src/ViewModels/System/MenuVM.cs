namespace ViewModels.System;

public class MenuVM
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public string ParentId { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;
    public string FunctionId { get; set; } = string.Empty;
    public string CommandId { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}
