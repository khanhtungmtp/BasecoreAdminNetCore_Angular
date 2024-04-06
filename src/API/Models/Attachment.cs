using API.Models.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;
[Table("Attachments")]
public class Attachment : IDateTracking
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public required string FileName { get; set; }

    [Required]
    [MaxLength(200)]
    public required string FilePath { get; set; }

    [Required]
    [MaxLength(4)]
    [Column(TypeName = "varchar(4)")]
    public required string FileType { get; set; }

    [Required]
    public long FileSize { get; set; }

    public int? ForumId { get; set; }

    public int? CommentId { get; set; }

    [Required]
    [MaxLength(10)]
    [Column(TypeName = "varchar(10)")]
    public required string Type { get; set; }

    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
}
