using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using prjSpecialTopicMvc.Models;
using prjSpecialTopicMvc.Models.MEbook; // 引用我們新的服務命名空間

namespace test1.Controllers
{
    public class EBookCategoriesController : Controller
    {
        // 註解：Controller 現在依賴的是服務介面，而不是直接依賴資料庫上下文。
        // 這讓 Controller 與資料庫的實作細節脫鉤，未來更容易維護和測試。
        private readonly IEBookCategoryService _categoryService;
        private readonly TeamAProjectContext _context; // 暫時保留 context 給 PopulateParentCategoryDropdownAsync 使用

        // 註解：透過建構函式，讓依賴注入系統傳入我們需要的服務。
        public EBookCategoriesController(IEBookCategoryService categoryService, TeamAProjectContext context)
        {
            _categoryService = categoryService;
            _context = context;
        }

        // GET: EBookCategories (列表頁)
        public async Task<IActionResult> List()
        {
            var categories = await _categoryService.GetAllCategoriesWithParentsAsync();
            // 註解：當您使用 return View(model); 且不指定視圖名稱時，
            // ASP.NET Core 會自動去尋找與目前 Action 方法同名的 .cshtml 檔案。
            // 因為您的 Action 方法叫做 List，所以它會正確地找到並使用 List.cshtml。
            return View(categories);
        }

        // GET: /EBookCategories/BooksInCategory/5
        public async Task<IActionResult> BooksInCategory(int? id)
        {
            if (id == null) return NotFound();

            // 註解：向服務層要求分類和其下的書籍資料。
            var category = await _categoryService.FindCategoryByIdAsync(id.Value);
            if (category == null) return NotFound("找不到指定的分類。");

            var books = await _categoryService.GetBooksByCategoryIdAsync(id.Value);

            // 註解：將資料打包，傳遞給 View。
            ViewBag.CategoryName = category.CategoryName;
            ViewBag.CategoryId = category.CategoryId;
            return View(books);
        }

        // GET: EBookCategories/Create
        public async Task<IActionResult> Create()
        {
            await PopulateParentCategoryDropdownAsync();
            return View();
        }

        // POST: EBookCategories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryName,ParentCategoryId")] EBookCategory eBookCategory)
        {
            // 註解：將「檢查分類名稱是否存在」的商業邏輯交給服務層處理。
            if (await _categoryService.CategoryNameExistsAsync(eBookCategory.CategoryName))
            {
                ModelState.AddModelError("CategoryName", "此分類名稱已存在。");
            }

            if (ModelState.IsValid)
            {
                // 註解：呼叫服務層來執行新增操作。
                await _categoryService.CreateCategoryAsync(eBookCategory);
                return RedirectToAction(nameof(List));
            }

            // 註解：如果驗證失敗，Controller 負責準備 View 所需的資料並返回。
            await PopulateParentCategoryDropdownAsync(eBookCategory.ParentCategoryId);
            return View(eBookCategory);
        }

        // GET: EBookCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            // 註解：呼叫服務層尋找要編輯的資料。
            var eBookCategory = await _categoryService.FindCategoryByIdAsync(id.Value);
            if (eBookCategory == null) return NotFound();

            await PopulateParentCategoryDropdownAsync(eBookCategory.ParentCategoryId);
            return View(eBookCategory);
        }

        // POST: EBookCategories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,CategoryName,ParentCategoryId")] EBookCategory eBookCategory)
        {
            if (id != eBookCategory.CategoryId) return NotFound();

            // 註解：將「檢查名稱是否與其他分類重複」的邏輯交給服務層。
            if (await _categoryService.CategoryNameExistsAsync(eBookCategory.CategoryName, id))
            {
                ModelState.AddModelError("CategoryName", "此分類名稱已被其他分類使用。");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 註解：呼叫服務層來執行更新操作。
                    await _categoryService.UpdateCategoryAsync(eBookCategory);
                }
                catch (DbUpdateConcurrencyException)
                {
                    // 註解：處理併發衝突是 Controller 的職責之一。
                    if (!await _categoryService.CategoryNameExistsAsync(eBookCategory.CategoryName, id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(List));
            }
            await PopulateParentCategoryDropdownAsync(eBookCategory.ParentCategoryId);
            return View(eBookCategory);
        }

        // GET: EBookCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            // 註解：呼叫服務層取得要刪除的資料（包含父分類，以便在 View 中顯示）。
            var eBookCategory = await _categoryService.GetCategoryWithParentAsync(id.Value);
            if (eBookCategory == null) return NotFound();

            return View(eBookCategory);
        }

        // POST: EBookCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // 註解：將「檢查分類是否正在被使用」的商業邏輯交給服務層。
            if (await _categoryService.IsCategoryInUseAsync(id))
            {
                TempData["ErrorMessage"] = "無法刪除此分類，因為已有書籍或子分類正在使用它。";
            }
            else
            {
                // 註解：如果檢查通過，則呼叫服務層執行刪除。
                await _categoryService.DeleteCategoryAsync(id);
                TempData["SuccessMessage"] = "分類已成功刪除。";
            }

            return RedirectToAction(nameof(List));
        }

        // 註解：這個輔助方法主要與 View 的呈現有關，可以暫時保留在 Controller。
        // 未來也可以考慮將其移到一個專門的「查詢服務」或「ViewModel 建構服務」中。
        private async Task PopulateParentCategoryDropdownAsync(object? selectedParent = null)
        {
            // 註解：1. 查詢所有分類，並使用 Include() 一併載入它們各自的父分類 (也就是祖父分類)
            // 這樣我們才能在顯示文字中存取到父分類的名稱。
            var allCategories = await _context.EBookCategories
                                            .Include(c => c.ParentCategory)
                                            .OrderBy(c => c.CategoryName)
                                            .AsNoTracking()
                                            .ToListAsync();

            // 註解：2. 手動建立 SelectListItem 列表，並產生我們想要的顯示文字格式
            var categoryOptions = allCategories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                // 註解：使用三元運算子來決定顯示格式
                Text = (c.ParentCategory != null)
                       ? $"({c.ParentCategory.CategoryName}) {c.CategoryName}" // 有父分類的格式
                       : c.CategoryName                                      // 沒有父分類的格式
            }).ToList();

            // 註解：3. 將這個處理好的列表放入 SelectList，並存入 ViewBag，同時設定好預選值
            ViewBag.ParentCategoryId = new SelectList(categoryOptions, "Value", "Text", selectedParent);
        }
    }
}