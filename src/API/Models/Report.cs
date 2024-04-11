using API.Models.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

[Table("Reports")]
public class Report : IDateTracking
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int? ForumId { get; set; }

    public int? CommentId { get; set; }

    [MaxLength(500)]
    public string Content { get; set; } = string.Empty;

    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string ReportUserId { get; set; } = string.Empty;

    public bool IsProcessed { get; set; }

    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string Type { get; set; } = string.Empty;

    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}
