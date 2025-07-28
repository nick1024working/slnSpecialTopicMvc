using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicMvc.Controllers.UsedBook.Common;
using prjSpecialTopicMvc.Features.Usedbook.Application.Authentication;
using prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicMvc.Features.Usedbook.Application.Services;

namespace prjSpecialTopicMvc.Controllers.UsedBook
{
    [Route("usedbooks/sale-tags")]
    [Authorize(Roles = RoleNames.Admin)]
    public class UsedBookSaleTagController : BaseController
    {
        private readonly BookSaleTagService _bookSaleTagService;

        public UsedBookSaleTagController(BookSaleTagService saleTagService)
        {
            _bookSaleTagService = saleTagService;
        }

        [HttpGet]
        public IActionResult Index() => RedirectToAction(nameof(SaleTagList));

        /// <summary>
        /// 查詢所有促銷標籤資訊，供管理員查看。
        /// </summary>
        [HttpGet("list")]
        public async Task<IActionResult> SaleTagList(CancellationToken ct)
        {
            var result = await _bookSaleTagService.GetAllAsync(ct);
            if (!result.IsSuccess)
                return RedirectToError(result);

            return View(result.Value);
        }

        /// <summary>
        /// 管理員新增促銷標籤。
        /// </summary>
        [HttpGet("create")]
        public IActionResult CreateSaleTag() => View();

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSaleTag([FromForm] CreateSaleTagRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(request);

            var result = await _bookSaleTagService.CreateAsync(request, ct);
            if (!result.IsSuccess)
                return RedirectToError(result);

            TempData["msg"] = "促銷標籤已新增。";
            return RedirectToAction(nameof(SaleTagList));
        }

        /// <summary>
        /// 管理員刪除促銷標籤。
        /// </summary>
        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSaleTag(int id, CancellationToken ct)
        {
            var result = await _bookSaleTagService.DeleteAsync(id, ct);
            if (!result.IsSuccess)
                return RedirectToError(result);

            TempData["msg"] = "促銷標籤已刪除。";
            return RedirectToAction(nameof(SaleTagList));
        }

        /// <summary>
        /// 修改指定促銷標籤的名稱，需擁有管理員權限。
        /// </summary>
        [HttpGet("update/{id}")]
        public async Task<IActionResult> UpdateSaleTagName(int id, CancellationToken ct)
        {
            var result = await _bookSaleTagService.GetByIdAsync(id, ct);
            if (!result.IsSuccess)
                return RedirectToError(result);

            var request = new UpdateBookSaleTagRequest
            {
                SaleTagName = result.Value.Name
            };

            ViewData["Id"] = id;
            return View(request);
        }

        [HttpPost("update/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateSaleTagName(int id, [FromForm] UpdateBookSaleTagRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(request);

            var result = await _bookSaleTagService.UpdateSaleTagNameAsync(id, request, ct);
            if (!result.IsSuccess)
                return RedirectToError(result);

            TempData["msg"] = "促銷標籤已更新。";
            return RedirectToAction(nameof(SaleTagList));
        }
    }
}
