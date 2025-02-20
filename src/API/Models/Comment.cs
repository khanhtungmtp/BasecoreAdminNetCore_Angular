﻿using API.Models.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

[Table("Comments")]
public class Comment : IDateTracking
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [MaxLength(500)]
    [Required]
    public required string Content { get; set; }

    [Required]
    [Range(1, Double.PositiveInfinity)]
    public int ForumId { get; set; }

    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string OwnwerUserId { get; set; } = string.Empty;
    public int? ReplyId { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}
