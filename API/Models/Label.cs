using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

[Table("Labels")]
public class Label
{
    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string Id { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}
