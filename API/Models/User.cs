using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace API.Models;
public class User : IdentityUser
{
    [MaxLength(50)]
    [Required]
    public required string FullName { get; set; }

    [Required]
    public DateTime DateOfBirth { get; set; }

    public int? NumberOfKnowledgeBases { get; set; }

    public int? NumberOfVotes { get; set; }

    public int? NumberOfReports { get; set; }
}
