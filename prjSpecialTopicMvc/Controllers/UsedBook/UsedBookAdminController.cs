using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicMvc.Controllers.UsedBook.Common;
using prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Queries;
using prjSpecialTopicMvc.Features.Usedbook.Extends;
using prjSpecialTopicMvc.Features.Usedbook.Application.Services;

namespace prjSpecialTopicMvc.Controllers.UsedBook
{
    [Route("usedbooks/admin")]
    [Authorize(Roles = "Admin")]
    public class UsedbookAdminController : BaseController
    {
        private readonly UsedBookService _usedBookService;

        public UsedbookAdminController(
            UsedBookService usedBookService)
        {
            _usedBookService = usedBookService;
        }

        // TODO: 補呼叫鏈上 query + filter
        /// <summary>
        /// 管理員查詢所有書籍清單。
        /// </summary>
        [HttpGet("books")]
        public async Task<IActionResult> GetAdminBookList([FromQuery] BookListQuery query)
        {
            // 呼叫 Service Layer
            var result = await _usedBookService.GetAdminBookListAsync(query);
            if (!result.IsSuccess)
                return RedirectToError(result);

            // 補 CoverUrl 路徑
            var baseUrl = Request.GetBaseUrl();
            foreach (var dto in result.Value)
            {
                if (!string.IsNullOrEmpty(dto.CoverImageUrl) && dto.CoverImageUrl.StartsWith("/"))
                {
                    dto.CoverImageUrl = $"{baseUrl}{dto.CoverImageUrl}";
                }
            }

            return View(result.Value);
        }

        [HttpGet("books/partial")]
        public async Task<IActionResult> GetUserBookListPartial([FromQuery] BookListQuery query)
        {
            // 呼叫 Service Layer
            var result = await _usedBookService.GetAdminBookListAsync(query);
            if (!result.IsSuccess)
                return RedirectToError(result);

            // 補 CoverUrl 路徑
            var baseUrl = Request.GetBaseUrl();
            foreach (var dto in result.Value)
            {
                if (!string.IsNullOrEmpty(dto.CoverImageUrl) && dto.CoverImageUrl.StartsWith("/"))
                {
                    dto.CoverImageUrl = $"{baseUrl}{dto.CoverImageUrl}";
                }
            }

            return PartialView("_AdminBookListTable", result.Value);
        }


        /// <summary>
        /// 管理員為指定書籍指派促銷標籤
        /// </summary>
        //[HttpPost("books/{bookId}/sale-tags/{tagId}")]
        //public async Task<ActionResult> AddBookSaleTag([FromRoute] Guid bookId, [FromRoute] int tagId)
        //{
        //    var result = await _usedBookService.AddBookSaleTagAsync(bookId, tagId);
        //    if (!result.IsSuccess)
        //        return RedirectToError(result);
        //    return NoContent();
        //}

        /// <summary>
        /// 管理員為指定書籍移除促銷標籤
        /// </summary>
        //[HttpDelete("books/{bookId}/sale-tags/{tagId}")]
        //public async Task<ActionResult> DeleteBookSaleTag([FromRoute] Guid bookId, [FromRoute] int tagId)
        //{
        //    var result = await _usedBookService.RemoveBookSaleTagAsync(bookId, tagId);
        //    if (!result.IsSuccess)
        //        return RedirectToError(result);
        //    return NoContent();
        //}

        /// <summary>
        /// 管理員更改指定訂單狀態
        /// </summary>
        //[HttpPatch("orders/{orderNo}")]
        //public async Task<ActionResult> UpdateOrderStatus([FromRoute] string orderNo, [FromBody] OrderStatus orderStatus, CancellationToken ct = default)
        //{
        //    var result = await _usedBookOrderService.UpdateOrderStatusAsync(orderNo, orderStatus, ct);
        //    if (!result.IsSuccess)
        //        return RedirectToError(result);

        //    return NoContent();
        //}

    }
}
