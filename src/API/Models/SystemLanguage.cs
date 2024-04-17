using System.ComponentModel.DataAnnotations;

namespace API.Models;

public partial class SystemLanguage
{
    [Key]
    [Required]
    [MaxLength(50)]
    public string LanguageCode { get; set; } = string.Empty;
    [MaxLength(50)]
    public string LanguageName { get; set; } = string.Empty;
    [Required]
    public string UrlImage { get; set; } = string.Empty;
    public int SortOrder { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;
}