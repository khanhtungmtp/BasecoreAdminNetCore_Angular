using API.Models.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

[Table("Forums")]
public class Forum : IDateTracking
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Range(1, Double.PositiveInfinity)]
    public int CategoryId { get; set; }

    [MaxLength(500)]
    [Required]
    public required string Title { get; set; }

    [MaxLength(500)]
    [Required]
    [Column(TypeName = "varchar(500)")]
    public required string SeoAlias { get; set; }

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Environment { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Problem { get; set; } = string.Empty;

    public string StepToReproduce { get; set; } = string.Empty;

    [MaxLength(500)]
    public string ErrorMessage { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Workaround { get; set; } = string.Empty;

    public string Note { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public required string OwnerUserId { get; set; }

    public string Labels { get; set; } = string.Empty;

    public DateTime CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public int? NumberOfComments { get; set; }

    public int? NumberOfVotes { get; set; }

    public int? NumberOfReports { get; set; }
    public int? ViewCount { get; set; }
}
