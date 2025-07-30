using prjSpecialTopicMvc.Features.Usedbook.Enums;

namespace prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Responses
{
    public class AdminOrderListItemDto
    {
        public int Id { get; set; }
        public string OrderNo { get; set; } = null!;
        public OrderStatus OrderStatus { get; set; }

        public Guid BuyerId { get; set; }
        public string BuyerLegalName { get; set; } = null!;

        public Guid SellerId { get; set; }
        public string SellerLegalName { get; set; } = null!;

        public Guid BookId { get; set; }
        public string BookName { get; set; } = null!;
        public decimal SalePrice { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}