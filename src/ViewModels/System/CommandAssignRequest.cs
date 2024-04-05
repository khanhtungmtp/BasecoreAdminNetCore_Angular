namespace ViewModels.System;

public class CommandAssignRequest
{
    public string[] CommandIds { get; set; } = [];

    public bool AddToAllFunctions { get; set; }
}
