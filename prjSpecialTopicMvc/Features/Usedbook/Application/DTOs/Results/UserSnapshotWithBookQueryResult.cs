namespace prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Results
{
    public class UserSnapshotWithBookQueryResult
    {
        public Guid UserId { get; set; }
        public bool IsActive { get; set; }
        public string LegalName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal PlatformFee { get; set; }

        public Guid BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public bool IsOnShelf { get; set; }
        public bool IsSold { get; set; }
        public bool ISActive { get; set; }
        public decimal SalePrice { get; set; }
    }
}
