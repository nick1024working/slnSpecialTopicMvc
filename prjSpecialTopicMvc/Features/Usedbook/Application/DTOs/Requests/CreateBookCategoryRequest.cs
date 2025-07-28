using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Requests
{
    public class CreateBookCategoryRequest
    {
        [Required(ErrorMessage = "主題類別名稱為必填欄位")]
        [StringLength(50, ErrorMessage = "不可超過 50 字")]
        public string TopicName { get; set; } = string.Empty;
    }
}
