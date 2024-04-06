using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

[Table("LabelInForums")]
public class LabelInForum
{
    public int ForumId { get; set; }

    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string LabelId { get; set; } = string.Empty;
}
