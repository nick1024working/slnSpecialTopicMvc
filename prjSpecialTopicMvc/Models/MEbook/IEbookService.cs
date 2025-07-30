using prjSpecialTopicMvc.ViewModels.EBook;

namespace prjSpecialTopicMvc.Models.MEbook
{
    // 註解：定義電子書服務的合約。
    public interface IEbookService
    {
        Task<EbookIndexViewModel> GetEbooksForListAsync(string? keyword, string availabilityFilter);
        Task<EBookMain?> GetEbookForDetailsAsync(long id);
        Task<EbookFormViewModel> GetEbookForEditAsync(long id);
        Task<EbookFormViewModel> GetNewEbookFormAsync();
        Task CreateEbookAsync(EbookFormViewModel viewModel);
        Task UpdateEbookAsync(long id, EbookFormViewModel viewModel);
        Task ToggleAvailabilityAsync(long id);
        Task<EBookMain?> FindEbookByIdAsync(long id);
        // 註解：將回傳類型從 Task<bool> 改為 Task<(bool Success, string ErrorMessage)>
        // 這樣方法就能非同步地回傳一個包含兩個具名欄位的元組
        Task<(bool Success, string ErrorMessage)> DeleteEbookAsync(long id);

        // 註解：為指定的電子書取得標籤管理所需的 ViewModel
        Task<ManageEbookTagsViewModel?> GetTagsForEbookAsync(long ebookId);

        // 註解：更新指定電子書的標籤指派
        Task UpdateEbookTagsAsync(long ebookId, IEnumerable<int> selectedLabelIds);
    }
}
