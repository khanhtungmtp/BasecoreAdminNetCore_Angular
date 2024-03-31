﻿using API.Models.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

[Table("Reports")]
public class Report : IDateTracking
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int? KnowledgeBaseId { get; set; }

    public int? CommentId { get; set; }

    [MaxLength(500)]
    public string Content { get; set; } = string.Empty;

    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string ReportUserId { get; set; } = string.Empty;

    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }

    public bool IsProcessed { get; set; }

    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string Type { get; set; } = string.Empty;
}
