using prjSpecialTopicMvc.Features.Usedbook.Enums;

namespace prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Responses
{
    public class UserOrderListItemDto
    {
        public string OrderNo { get; set; } = null!;
        public OrderStatus OrderStatus { get; set; }

        public string BuyerLegalName { get; set; } = null!;
        public string BuyerPhone { get; set; } = null!;

        public string SellerLegalName { get; set; } = null!;
        public string SellerPhone { get; set; } = null!;

        public Guid BookId { get; set; }
        public string BookName { get; set; } = null!;
        public decimal SalePrice { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
