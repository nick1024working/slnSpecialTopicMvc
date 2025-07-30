using prjSpecialTopicMvc.Features.Usedbook.Enums;

namespace prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Results
{
    public class PublicBookListItemQueryResult
    {
        public StorageProvider CoverStorageProvider { get; set; }
        public string CoverObjectKey { get; set; } = string.Empty;

        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Authors { get; set; } = string.Empty;
        public decimal SalePrice { get; set; }
        public string ConditionRating { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;
    }
}
