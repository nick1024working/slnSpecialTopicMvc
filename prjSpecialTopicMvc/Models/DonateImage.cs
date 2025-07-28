using System;
using System.Collections.Generic;

namespace prjSpecialTopicMvc.Models;

public partial class DonateImage
{
    public int DonateProjectId { get; set; }

    public int ImageId { get; set; }

    public string? DonateImageUrl { get; set; }

    public bool? IsMain { get; set; }

    public int? SortOrder { get; set; }

    public virtual DonateProject DonateProject { get; set; } = null!;
}
