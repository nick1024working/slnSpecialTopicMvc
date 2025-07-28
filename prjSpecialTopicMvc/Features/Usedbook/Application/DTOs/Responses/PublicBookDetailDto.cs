using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Responses
{
    public class PublicBookDetailDto
    {
        [Display(Name = "書本編號")]
        public Guid Id { get; set; }

        [Display(Name = "圖片清單")]
        public IEnumerable<BookImageDto> ImageList { get; set; } = new List<BookImageDto>();

        [Display(Name = "賣家ID")]
        public Guid SellerId { get; set; }

        [Display(Name = "賣家縣市")]
        public string SellerCountyName { get; set; } = string.Empty;

        [Display(Name = "賣家鄉鎮")]
        public string SellerDistrictName { get; set; } = string.Empty;

        [Display(Name = "售價")]
        public decimal SalePrice { get; set; }

        [Display(Name = "書名")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "作者")]
        public string Authors { get; set; } = string.Empty;

        [Display(Name = "書況評等")]
        public string ConditionRatingName { get; set; } = string.Empty;

        [Display(Name = "書況描述")]
        public string? ConditionDescription { get; set; }

        [Display(Name = "版本")]
        public string? Edition { get; set; }

        [Display(Name = "出版社")]
        public string? Publisher { get; set; }

        [Display(Name = "出版日期")]
        public DateOnly? PublicationDate { get; set; }

        [Display(Name = "ISBN")]
        public string? Isbn { get; set; }

        [Display(Name = "裝訂方式")]
        public string? BindingName { get; set; }

        [Display(Name = "語言")]
        public string? LanguageName { get; set; }

        [Display(Name = "頁數")]
        public int? Pages { get; set; }

        [Display(Name = "內容分級")]
        public string ContentRatingName { get; set; } = string.Empty;

        [Display(Name = "網址別名")]
        public string Slug { get; set; } = string.Empty;

        [Display(Name = "建立時間")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "最後更新")]
        public DateTime UpdatedAt { get; set; }
    }
}
