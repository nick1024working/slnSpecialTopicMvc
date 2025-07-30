using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicMvc.Models.MEbook;
using prjSpecialTopicMvc.ViewModels.EBook;

namespace prjSpecialTopicMvc.Controllers.EBook
{
    public class EbookController : Controller
    {
        // 註解：Controller 現在只依賴服務層介面，不再直接操作 DbContext 或 WebHostEnvironment。
        private readonly IEbookService _ebookService;

        public EbookController(IEbookService ebookService)
        {
            _ebookService = ebookService;
        }

        // GET: /Ebook/List
        public async Task<IActionResult> List(string? keyword, string availabilityFilter = "AvailableOnly")
        {
            // 註解：直接呼叫服務取得 ViewModel
            var viewModel = await _ebookService.GetEbooksForListAsync(keyword, availabilityFilter);
            return View(viewModel);
        }

        // GET: Ebook/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null) return NotFound();
            var eBookMain = await _ebookService.GetEbookForDetailsAsync(id.Value);
            if (eBookMain == null) return NotFound();
            return View(eBookMain);
        }

        // GET: /Ebook/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = await _ebookService.GetNewEbookFormAsync();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EbookFormViewModel viewModel)
        {
            // 註解：移除前端 JS 後，我們必須在後端手動清除所有我們會自行處理的欄位的驗證錯誤。
            // 這能確保即使表單送來時某些欄位是空的，驗證也能順利通過，
            // 讓我們有機會在後續的服務層邏輯中處理它們。

            // 處理「新增分類」時，分類ID下拉選單會是空的，所以要移除相關錯誤。
            ModelState.Remove("Ebook.CategoryId");
            ModelState.Remove("Ebook.Category");

            // 處理「未上傳檔案」時的情況，所以要移除相關錯誤。
            ModelState.Remove("Ebook.EBookPosition");
            ModelState.Remove("Ebook.EBookDataType");

            // 註解：現在 ModelState.IsValid 可以專心檢查其他我們沒有手動處理的欄位 (如書名、作者等)。
            if (ModelState.IsValid)
            {
                await _ebookService.CreateEbookAsync(viewModel);
                return RedirectToAction(nameof(List));
            }

            // 如果驗證失敗（例如書名為空），需要重新準備表單資料並返回
            var formViewModel = await _ebookService.GetNewEbookFormAsync();
            viewModel.CategoryOptions = formViewModel.CategoryOptions;
            return View(viewModel);
        }

        // GET: /Ebook/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return NotFound();
            try
            {
                var viewModel = await _ebookService.GetEbookForEditAsync(id.Value);
                return View(viewModel);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // 在 EbookController.cs 中

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, EbookFormViewModel viewModel)
        {
            if (id != viewModel.Ebook.EbookId) return NotFound();

            // 【*** 新增的修正 ***】
            // 註解：在檢查 ModelState.IsValid 之前，手動移除我們將會自行處理的欄位的驗證錯誤。
            // 這樣做可以確保即使表單送來時這些欄位是空的，驗證也能順利通過，
            // 讓我們有機會在後續的服務層邏輯中處理它們。
            ModelState.Remove("Ebook.Category");
            ModelState.Remove("Ebook.EBookPosition");
            ModelState.Remove("Ebook.EBookDataType");

            // 註解：現在 ModelState.IsValid 可以正確地檢查其他欄位 (如書名、作者等)。
            if (ModelState.IsValid)
            {
                await _ebookService.UpdateEbookAsync(id, viewModel);
                return RedirectToAction(nameof(List));
            }

            // 如果驗證失敗，需要重新準備表單資料
            var formViewModel = await _ebookService.GetEbookForEditAsync(id);
            viewModel.CategoryOptions = formViewModel.CategoryOptions;
            viewModel.ExistingImages = formViewModel.ExistingImages;
            return View(viewModel);
        }

        // POST: /Ebook/ToggleAvailability/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAvailability(long id)
        {
            await _ebookService.ToggleAvailabilityAsync(id);
            return RedirectToAction(nameof(List));
        }

        // GET: /Ebook/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null) return NotFound();
            var ebook = await _ebookService.FindEbookByIdAsync(id.Value);
            if (ebook == null) return NotFound();
            return View(ebook);
        }

        // POST: /Ebook/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            // 註解：使用元組解構式 (Tuple deconstruction) 來接收方法回傳的多個值
            // var (變數1, 變數2) = ...
            var (Success, ErrorMessage) = await _ebookService.DeleteEbookAsync(id);

            if (Success)
            {
                TempData["SuccessMessage"] = "電子書已成功刪除。";
            }
            else
            {
                TempData["ErrorMessage"] = ErrorMessage;
            }
            return RedirectToAction(nameof(List));
        }

        // ... 在 EbookController 中，加入以下兩個新的 Action 方法 ...

        // GET: /Ebook/ManageTags/5
        public async Task<IActionResult> ManageTags(long? id)
        {
            if (id == null) return NotFound();

            var viewModel = await _ebookService.GetTagsForEbookAsync(id.Value);
            if (viewModel == null) return NotFound();

            return View(viewModel);
        }

        // POST: /Ebook/ManageTags/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageTags(long ebookId, List<int> selectedLabelIds)
        {
            await _ebookService.UpdateEbookTagsAsync(ebookId, selectedLabelIds);
            TempData["SuccessMessage"] = "標籤已成功更新。";
            return RedirectToAction(nameof(List));
        }
    }
}
