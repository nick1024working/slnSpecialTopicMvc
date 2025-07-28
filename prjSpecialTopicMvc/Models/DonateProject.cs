using System;
using System.Collections.Generic;

namespace prjSpecialTopicMvc.Models;

public partial class DonateProject
{
    public int DonateProjectId { get; set; }

    public int DonateCategoriesId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal TargetAmount { get; set; }

    public decimal? CurrentAmount { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public virtual DonateCategory DonateCategories { get; set; } = null!;

    public virtual ICollection<DonateImage> DonateImages { get; set; } = new List<DonateImage>();

    public virtual ICollection<DonatePlan> DonatePlans { get; set; } = new List<DonatePlan>();
}
