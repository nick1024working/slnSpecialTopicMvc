using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicMvc.Models;
using prjSpecialTopicMvc.Models.MEbook;

namespace test1.Controllers
{
    public class LabelsController : Controller
    {
        private readonly ILabelsService _labelsService;

        public LabelsController(ILabelsService labelsService)
        {
            _labelsService = labelsService;
        }

        // GET: /Labels 或 /Labels/Index
        public async Task<IActionResult> Index()
        {
            var labels = await _labelsService.GetAllLabelsAsync();
            return View(labels);
        }

        // 【*** 新增的 Action ***】
        // GET: /Labels/Create
        public IActionResult Create()
        {
            return View();
        }

        // 【*** 新增的 Action ***】
        // POST: /Labels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LabelName")] Label label)
        {
            if (await _labelsService.LabelNameExistsAsync(label.LabelName))
            {
                ModelState.AddModelError("LabelName", "此標籤名稱已存在。");
            }

            if (ModelState.IsValid)
            {
                await _labelsService.CreateLabelAsync(label);
                return RedirectToAction(nameof(Index));
            }
            return View(label);
        }

        // GET: /Labels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var label = await _labelsService.FindLabelByIdAsync(id.Value);
            if (label == null) return NotFound();
            return View(label);
        }

        // POST: /Labels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LabelId,LabelName")] Label label)
        {
            if (id != label.LabelId) return NotFound();

            if (await _labelsService.LabelNameExistsAsync(label.LabelName, id))
            {
                ModelState.AddModelError("LabelName", "此標籤名稱已被其他標籤使用。");
            }

            if (ModelState.IsValid)
            {
                await _labelsService.UpdateLabelAsync(label);
                return RedirectToAction(nameof(Index));
            }
            return View(label);
        }

        // 【*** 新增的 Action ***】
        // GET: /Labels/BooksWithLabel/5
        public async Task<IActionResult> BooksWithLabel(int? id)
        {
            if (id == null) return NotFound();

            var label = await _labelsService.FindLabelByIdAsync(id.Value);
            if (label == null) return NotFound("找不到指定的標籤。");

            var books = await _labelsService.GetBooksByLabelIdAsync(id.Value);

            ViewBag.LabelName = label.LabelName;
            return View(books);
        }

        // GET: /Labels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var label = await _labelsService.FindLabelByIdAsync(id.Value);
            if (label == null) return NotFound();

            // 註解：在顯示刪除頁面前，先檢查此標籤是否正在被使用
            if (await _labelsService.IsLabelInUseAsync(id.Value))
            {
                // 如果正在被使用，就去取得是哪些書在使用它
                ViewBag.BlockingBooks = await _labelsService.GetBooksByLabelIdAsync(id.Value);
            }

            return View(label);
        }

        // POST: /Labels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (await _labelsService.IsLabelInUseAsync(id))
            {
                TempData["ErrorMessage"] = "無法刪除此標籤，因為已有書籍正在使用它。";
            }
            else
            {
                await _labelsService.DeleteLabelAsync(id);
                TempData["SuccessMessage"] = "標籤已成功刪除。";
            }
            return RedirectToAction(nameof(Index));
        }

        // AJAX Action for creating labels from the ManageTags page
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Produces("application/json")]
        public async Task<IActionResult> CreateLabelAjax([FromBody] Label newLabel)
        {
            if (string.IsNullOrWhiteSpace(newLabel.LabelName))
            {
                return BadRequest(new { success = false, message = "標籤名稱不可為空。" });
            }

            if (await _labelsService.LabelNameExistsAsync(newLabel.LabelName))
            {
                return BadRequest(new { success = false, message = "此標籤名稱已存在。" });
            }

            var createdLabel = await _labelsService.CreateLabelAsync(newLabel);
            return Ok(new { success = true, message = "標籤已新增", label = createdLabel });
        }
    }
}