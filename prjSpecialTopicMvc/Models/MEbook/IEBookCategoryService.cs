using prjSpecialTopicMvc.Models;

namespace prjSpecialTopicMvc.Models.MEbook
{
    // 註解：定義 EBookCategory 服務的合約 (能做哪些事)
    public interface IEBookCategoryService
    {
        // 取得所有分類 (包含上層分類資訊)
        Task<IEnumerable<EBookCategory>> GetAllCategoriesWithParentsAsync();

        // 依據 ID 尋找分類
        Task<EBookCategory?> FindCategoryByIdAsync(int id);

        // 依據 ID 尋找分類 (包含上層分類資訊，給 Delete GET 頁面使用)
        Task<EBookCategory?> GetCategoryWithParentAsync(int id);

        // 依據分類 ID 取得其下的所有書籍
        Task<IEnumerable<EBookMain>> GetBooksByCategoryIdAsync(int id);

        // 新增分類
        Task CreateCategoryAsync(EBookCategory category);

        // 更新分類
        Task UpdateCategoryAsync(EBookCategory category);

        // 刪除分類
        Task DeleteCategoryAsync(int id);

        // 檢查分類名稱是否重複 (給 Controller 驗證用)
        Task<bool> CategoryNameExistsAsync(string name, int? excludeId = null);

        // 檢查分類是否正在被使用
        Task<bool> IsCategoryInUseAsync(int id);
    }
}