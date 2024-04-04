using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace API.Models;
public class User : IdentityUser
{
    public User() { }

    public User(string id, string userName, string fullName, string email, string phoneNumber, DateTime dateOfBirth)
    {
        Id = id;
        UserName = userName;
        FullName = fullName;
        Email = email;
        PhoneNumber = phoneNumber;
        DateOfBirth = dateOfBirth;
    }
    [MaxLength(50)]
    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public DateTime DateOfBirth { get; set; }

    public int? NumberOfKnowledgeBases { get; set; }

    public int? NumberOfVotes { get; set; }

    public int? NumberOfReports { get; set; }
    public DateTime CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }
}
