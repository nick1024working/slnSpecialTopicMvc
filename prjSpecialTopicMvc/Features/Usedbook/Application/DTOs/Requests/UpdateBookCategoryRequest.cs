using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Requests
{
    /// <summary>
    /// 用於更新主題類別名稱的請求資料格式。
    /// 僅允許修改 Topic 的名稱欄位。
    /// </summary>
    public class UpdateBookCategoryRequest
    {
        /// <summary>
        /// 要更新的主題類別名稱。
        /// </summary>
        [Required(ErrorMessage = "主題類別名稱為必填欄位")]
        public string TopicName { get; set; } = string.Empty;
    }
}
