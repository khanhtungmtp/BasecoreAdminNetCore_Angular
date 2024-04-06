namespace ViewModels.System;

public class PermissionScreenVm
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string ParentId { get; set; } = string.Empty;

    public bool HasCreate { get; set; }

    public bool HasUpdate { get; set; }

    public bool HasDelete { get; set; }

    public bool HasView { get; set; }

    public bool HasApprove { get; set; }
}
