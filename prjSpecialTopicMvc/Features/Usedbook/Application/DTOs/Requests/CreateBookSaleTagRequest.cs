using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Requests
{
    public class CreateSaleTagRequest
    {
        [Required(ErrorMessage = "促銷標籤名稱為必填欄位")]
        [StringLength(50, ErrorMessage = "不可超過 50 字")]
        public string SaleTagName { get; set; } = string.Empty;
    }
}
