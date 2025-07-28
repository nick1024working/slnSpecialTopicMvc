namespace prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Results
{
    public class UserSnapshotQueryResult
    {
        public Guid UserId { get; set; }
        public bool IsActive { get; set; }
        public string LegalName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Email { get; set; } = string.Empty;
        public decimal? PlatformFee { get; set; }
    }
}
