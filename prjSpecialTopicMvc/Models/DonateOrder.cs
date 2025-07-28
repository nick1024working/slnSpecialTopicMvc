using System;
using System.Collections.Generic;

namespace prjSpecialTopicMvc.Models;

public partial class DonateOrder
{
    public int DonateOrderId { get; set; }

    public Guid Uid { get; set; }

    public decimal TotalAmount { get; set; }

    public string? PaymentMethod { get; set; }

    public DateTime? PaymentDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<DonateOrderItem> DonateOrderItems { get; set; } = new List<DonateOrderItem>();

    public virtual User UidNavigation { get; set; } = null!;
}
