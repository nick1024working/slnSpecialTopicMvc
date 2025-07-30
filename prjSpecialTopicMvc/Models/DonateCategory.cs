using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicMvc.Models;

public partial class DonateCategory
{
    public int DonateCategoriesId { get; set; }
    [Required(ErrorMessage = "請輸入分類名稱")]
    [Display(Name = "分類名稱")]
    public string CategoriesName { get; set; } = null!;

    public virtual ICollection<DonateProject> DonateProjects { get; set; } = new List<DonateProject>();
}
