using prjSpecialTopicMvc.Features.Usedbook.Enums;

namespace prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Responses
{
    public class OrderDetailDto
    {
        public string OrderNo { get; set; } = null!;
        public OrderStatus OrderStatus { get; set; }

        public string BuyerLegalName { get; set; } = null!;
        public string BuyerPhone { get; set; } = null!;
        public string BuyerEmail { get; set; } = null!;

        public string SellerLegalName { get; set; } = null!;
        public string SellerPhone { get; set; } = null!;
        public string SellerEmail { get; set; } = null!;

        public string BookName { get; set; } = null!;
        public decimal SalePrice { get; set; }

        public DateTime CreatedAt { get; set; }

        public AdminView? Admin { get; set; }

        public class AdminView
        {
            public int Id { get; set; }
            public Guid BuyerId { get; set; }
            public Guid SellerId { get; set; }
            public Guid BookId { get; set; }
        }
    }

}
