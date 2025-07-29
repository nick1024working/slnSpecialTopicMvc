using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using prjSpecialTopicMvc.Models;

namespace prjSpecialTopicMvc.ViewModels.EBook
{

    /// <summary>
    /// 專為電子書新增與編輯表單設計的 ViewModel
    /// </summary>
    public class EbookFormViewModel
    {
        // 繫結 EBookMain 的主要資料
        public EBookMain Ebook { get; set; } = new EBookMain();

        // --- 分類處理 ---
        // 用於顯示現有分類的下拉選單
        public IEnumerable<SelectListItem>? CategoryOptions { get; set; }

        // 用於接收使用者輸入的新分類名稱
        [Display(Name = "或輸入新分類")]
        public string? NewCategoryName { get; set; }

        // 【*** 新增此屬性 ***】
        [Display(Name = "分類")] // 讓標籤顯示為「分類」
        public int? SelectedCategoryId { get; set; } // 使用可為 null 的 int?

        // --- 檔案上傳 ---
        [Display(Name = "電子書檔案 (EPUB, PDF, etc.)")]
        public IFormFile? EbookFile { get; set; }

        [Display(Name = "封面圖 (可上傳多張)")]
        public List<IFormFile>? CoverImages { get; set; }

        // --- 編輯時使用 ---
        // 用於顯示已存在的圖片
        public List<EBookImage>? ExistingImages { get; set; }

        // 用於接收要刪除的圖片 ID
        public List<long>? ImagesToDelete { get; set; }
        // 註解：用於接收從編輯表單提交的、被選為新主封面的圖片 ID
        public long? NewPrimaryImageId { get; set; }

    }
}
