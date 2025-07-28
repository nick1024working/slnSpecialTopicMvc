namespace prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Results
{
    public class RawUsedBookQueryResult
    {
        public Guid Id { get; set; }

        public Guid SellerId { get; set; }
        public int SellerDistrictId { get; set; }
        public decimal SalePrice { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Authors { get; set; } = string.Empty;
        public int ConditionRatingId { get; set; }

        public string? ConditionDescription { get; set; }
        public string? Edition { get; set; }
        public string? Publisher { get; set; }
        public DateOnly? PublicationDate { get; set; }
        public string? Isbn { get; set; }
        public int? BindingId { get; set; }
        public int? LanguageId { get; set; }
        public int? Pages { get; set; }
        public int ContentRatingId { get; set; }

        /* --- 狀態欄位 --- */
        public bool IsOnShelf { get; set; }
        public bool IsSold { get; set; }
        public bool IsActive { get; set; }
        public string Slug { get; set; } = string.Empty;

        /* --- 審計欄位 --- */
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
