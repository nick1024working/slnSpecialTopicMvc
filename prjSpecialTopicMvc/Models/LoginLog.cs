using System;
using System.Collections.Generic;

namespace prjSpecialTopicMvc.Models;

public partial class LoginLog
{
    public int LogId { get; set; }

    public Guid Uid { get; set; }

    public DateTime LoginDate { get; set; }

    public virtual User UidNavigation { get; set; } = null!;
}
