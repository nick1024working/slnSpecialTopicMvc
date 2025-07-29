using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicMvc.Controllers.UsedBook.Common;
using prjSpecialTopicMvc.Features.Usedbook.Application.Authentication;
using prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Queries;
using prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Responses;
using prjSpecialTopicMvc.Features.Usedbook.Application.Errors;
using prjSpecialTopicMvc.Features.Usedbook.Application.Services;
using prjSpecialTopicMvc.ViewModels.UsedBook;

namespace prjSpecialTopicMvc.Controllers.UsedBook
{
    [Route("usedbooks")]
    public class UsedBookController : BaseController
    {

        private readonly LookupService _lookupService;
        private readonly UsedBookService _usedBookservice;


        public UsedBookController(
            LookupService lookupService,
            UsedBookService usedBookService)
        {
            _lookupService = lookupService;
            _usedBookservice = usedBookService;
        }

        [HttpGet("index")]
        public IActionResult Index() => RedirectToAction("GetPublicBookList");

        /// <summary>
        /// 取得公開書籍列表
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PublicBookListItemDto>>> GetPublicBookList([FromQuery] BookListQuery query, CancellationToken ct)
        {
            // TODO : 假傳 BookListQuery 直接不篩選呼叫
            BookListQuery q = new BookListQuery();
            var result = await _usedBookservice.GetPublicListAsync(q, ct);
            if (!result.IsSuccess)
                return RedirectToError(result);

            return View(result.Value);
        }

        /// <summary>
        /// 取得公開書籍列表
        /// </summary>
        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<PublicBookDetailDto>> GetPublicDetail(Guid id, CancellationToken ct)
        {
            var result = await _usedBookservice.GetPubicAsync(id, ct);
            if (!result.IsSuccess)
                return RedirectToError(result);

            return View(result.Value);
        }

        /// <summary>
        /// 新增書本
        /// </summary>
        [HttpGet("create")]
        [Authorize(Roles = RoleNames.Seller)]
        public async Task<IActionResult> CreateBook(CancellationToken ct)
        {
            // 取得所有 UI下拉選單 List<SelectListItem>

            var lookupResult = await _lookupService.GetAllUsedBookSelectListsAsync(ct);
            if (!lookupResult.IsSuccess)
                return RedirectToError(lookupResult);

            // 組裝 ViewModel
            var vm = new CreateBookViewModel
            {
                BookBindings = lookupResult.Value.BookBindings,
                BookConditionRatings = lookupResult.Value.BookConditionRatings,
                ContentRatings = lookupResult.Value.ContentRatings,
                Counties = lookupResult.Value.Counties,
                Languages = lookupResult.Value.Languages
            };

            return View(vm);
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBook([FromForm] CreateBookViewModel vm, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                //var details = ModelState
                //    .Where(x => x.Value.Errors.Count > 0)
                //    .Select(x => new { x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) })
                //    .ToList();
                return View(vm);
            }

            // 嘗試取出 claims 中的 userId
            if (AuthHelper.GetUserId(User) is not Guid userId)
                return RedirectToAction("Error", "Home", new { code = ErrorCodes.Auth.Unauthorized, message = "授權失敗" });

            if (await _usedBookservice.CreateAsync(userId, vm.Request, ct) is var commandResult && !commandResult.IsSuccess)
                return RedirectToError(commandResult);

            TempData["msg"] = "已新增二手書！";
            return RedirectToAction(nameof(UsedBookUserController.GetUserBookList), "UsedBookUser");
        }

        /// <summary>
        /// 修改指定書本
        /// </summary>
        [HttpGet("update/{id:Guid}")]
        public async Task<IActionResult> UpdateBook(Guid id, [FromQuery] string from, CancellationToken ct)
        {

            var result = await _usedBookservice.GetUpdatePayloadAsync(id, ct);
            if (!result.IsSuccess)
                return RedirectToError(result);

            // 取得所有 UI下拉選單 List<SelectListItem>

            var lookupResult = await _lookupService.GetAllUsedBookSelectListsAsync(ct);
            if (!lookupResult.IsSuccess)
                return RedirectToError(lookupResult);

            // 組裝 ViewModel
            var vm = new UpdateBookViewModel
            {
                BookId = id,
                Request = result.Value,
                BookBindings = lookupResult.Value.BookBindings,
                BookConditionRatings = lookupResult.Value.BookConditionRatings,
                ContentRatings = lookupResult.Value.ContentRatings,
                Counties = lookupResult.Value.Counties,
                Languages = lookupResult.Value.Languages,

                From = from
            };

            return View(vm);
        }

        [HttpPost("update/{id:Guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBook([FromRoute] Guid id, [FromForm] UpdateBookViewModel vm, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                //var details = ModelState
                //    .Where(x => x.Value.Errors.Count > 0)
                //    .Select(x => new { x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) })
                //    .ToList();
                return View(vm);
            }

            var result = await _usedBookservice.UpdateAsync(id, vm.Request, ct);
            if (!result.IsSuccess)
                return RedirectToError(result);

            TempData["msg"] = "書本已更新。";

            // 回跳來源頁（安全性檢查）
            if (!string.IsNullOrEmpty(vm.From) && Url.IsLocalUrl(vm.From))
                return Redirect(vm.From);

            return RedirectToAction(nameof(UsedBookUserController.GetUserBookList), "UsedBookUser");
        }


        /// <summary>
        /// 軟刪除書籍。
        /// </summary>
        [HttpPost("delete/{id:Guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBook([FromRoute] Guid id, CancellationToken ct)
        {
            var result = await _usedBookservice.UpdateActiveStatusAsync(id, false, ct);
            if (!result.IsSuccess)
                return RedirectToError(result);

            return RedirectToAction(nameof(UsedBookUserController.GetUserBookList), "UsedBookUser");
        }

    }
}
