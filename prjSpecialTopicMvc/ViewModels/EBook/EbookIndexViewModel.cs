using Microsoft.AspNetCore.Mvc.Rendering;
using prjSpecialTopicMvc.Models;

namespace prjSpecialTopicMvc.ViewModels.EBook
{
    public class EbookIndexViewModel
    {
        /// <summary>
        /// 存放從資料庫查詢出來、要顯示在頁面上的電子書列表
        /// </summary>
        // ******** 已修正 ********
        // 加上 = new List<EBookMain>(); 來給予初始值，避免 C# 的 non-nullable 警告 (CS8618)
        public List<EBookMain> Ebooks { get; set; } = new List<EBookMain>();

        /// <summary>
        /// 用來接收和顯示使用者在搜尋框中輸入的關鍵字
        /// </summary>
        public string? Keyword { get; set; }

        /// <summary>
        /// 用來接收和顯示使用者選擇的篩選條件 (例如 "AvailableOnly")
        /// </summary>
        public string? AvailabilityFilter { get; set; }

        /// <summary>
        /// 提供給篩選下拉選單的選項列表
        /// </summary>
        public List<SelectListItem> AvailabilityOptions { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "AvailableOnly", Text = "只顯示上架" },
            new SelectListItem { Value = "UnavailableOnly", Text = "只顯示下架" },
            new SelectListItem { Value = "All", Text = "顯示全部" }
        };
    }
}
