using API.Models.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

[Table("Votes")]
public class Vote : IDateTracking
{
    public int ForumId { get; set; }

    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string UserId { get; set; } = string.Empty;

    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
}
