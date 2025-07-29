using System;
using System.Collections.Generic;

namespace prjSpecialTopicMvc.Models;

public partial class DonateCategory
{
    public int DonateCategoriesId { get; set; }

    public string CategoriesName { get; set; } = null!;

    public virtual ICollection<DonateProject> DonateProjects { get; set; } = new List<DonateProject>();
}
