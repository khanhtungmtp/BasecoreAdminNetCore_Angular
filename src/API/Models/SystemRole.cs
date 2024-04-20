using Microsoft.AspNetCore.Identity;

namespace API.Models;

public class SystemRole : IdentityRole
{
    public string? Description { get; set; } = string.Empty;
}
