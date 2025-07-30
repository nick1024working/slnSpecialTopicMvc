using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicMvc.Models;

public partial class DonateProject
{
    public int DonateProjectId { get; set; }
    [Required(ErrorMessage = "請選擇分類")]
    [Display(Name = "分類")]
    public int DonateCategoriesId { get; set; }

    public Guid Uid { get; set; }
    [Required(ErrorMessage = "請輸入標題")]
    [Display(Name = "標題")]
    public string ProjectTitle { get; set; } = null!;
    [Required(ErrorMessage = "請輸入簡介")]
    [Display(Name = "簡介")]
    public string? ProjectDescription { get; set; }
    [Required(ErrorMessage = "請輸入金額")]
    [Display(Name = "目標金額")]
    public decimal TargetAmount { get; set; }
    [Required(ErrorMessage = "請輸入金額")]
    [Display(Name = "目前金額")]
    public decimal CurrentAmount { get; set; }
    [Required(ErrorMessage = "請輸入日期")]
    [Display(Name = "開始日期")]
    public DateOnly StartDate { get; set; }
    [Required(ErrorMessage = "請輸入日期")]
    [Display(Name = "結束日期")]
    public DateOnly EndDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
    [Display(Name = "狀態")]
    public string Status { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public virtual DonateCategory? DonateCategories { get; set; }

    public virtual ICollection<DonateImage> DonateImages { get; set; } = new List<DonateImage>();

    public virtual ICollection<DonatePlan> DonatePlans { get; set; } = new List<DonatePlan>();
}
