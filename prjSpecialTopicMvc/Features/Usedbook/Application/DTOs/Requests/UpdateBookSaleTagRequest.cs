using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Requests
{
    /// <summary>
    /// 用於更新促銷標籤名稱的請求資料格式。
    /// 僅允許修改 SaleTag 的名稱欄位。
    /// </summary>
    public class UpdateBookSaleTagRequest
    {
        /// <summary>
        /// 要更新的促銷標籤名稱。
        /// </summary>
        [DisplayName("促銷標籤名稱")]
        [Required(ErrorMessage = "促銷標籤名稱為必填欄位")]
        public string SaleTagName { get; set; } = string.Empty;
    }
}
