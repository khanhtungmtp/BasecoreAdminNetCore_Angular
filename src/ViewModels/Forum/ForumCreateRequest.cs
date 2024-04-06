using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ViewModels.Forum;

public class ForumCreateRequest
{
    public int? Id { get; set; }

    [Display(Name = "Danh mục")]
    public int CategoryId { get; set; }

    [Display(Name = "Tiêu đề")]
    public string Title { get; set; } = string.Empty;

    public string SeoAlias { get; set; } = string.Empty;

    [Display(Name = "Mô tả")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Môi trường")]
    public string Environment { get; set; } = string.Empty;

    [Display(Name = "Vấn đề gặp phải")]
    public string Problem { get; set; } = string.Empty;

    [Display(Name = "Các bước tái hiện")]
    public string StepToReproduce { get; set; } = string.Empty;

    [Display(Name = "Lỗi")]
    public string ErrorMessage { get; set; } = string.Empty;

    public string OwnerUserId { get; set; } = string.Empty;

    [Display(Name = "Cách xử lý nhanh")]
    public string Workaround { get; set; } = string.Empty;

    [Display(Name = "Giải pháp")]
    public string Note { get; set; } = string.Empty;

    [Display(Name = "Nhãn")]
    public string[] Labels { get; set; } = [];

    [Display(Name = "Tệp đính kèm")]
    public List<IFormFile>? Attachments { get; set; }
}
