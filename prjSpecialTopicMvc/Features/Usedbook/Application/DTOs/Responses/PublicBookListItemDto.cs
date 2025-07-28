using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Responses
{
    public class PublicBookListItemDto
    {
        [Display(Name = "封面圖片URL")]
        public string CoverImageUrl { get; set; } = string.Empty;

        [Display(Name = "書本編號")]
        public Guid Id { get; set; }

        [Display(Name = "書名")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "作者")]
        public string Authors { get; set; } = string.Empty;

        [Display(Name = "售價")]
        public decimal SalePrice { get; set; }

        [Display(Name = "書況評等")]
        public string ConditionRating { get; set; } = string.Empty;

        [Display(Name = "網址")]
        public string Slug { get; set; } = string.Empty;
    }
}
