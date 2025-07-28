using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicMvc.Controllers.UsedBook.Common;
using prjSpecialTopicMvc.Features.Usedbook.Application.Authentication;
using prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Responses;
using prjSpecialTopicMvc.Features.Usedbook.Application.Errors;
using prjSpecialTopicMvc.Features.Usedbook.Enums;
using prjSpecialTopicMvc.Features.Usedbook.Extends;
using prjSpecialTopicMvc.Features.Usedbook.Application.Services;

namespace prjSpecialTopicMvc.Controllers.UsedBook
{
    [Route("usedbooks/users")]
    public class UsedBookUserController : BaseController
    {
        private readonly UsedBookService _usedBookService;

        public UsedBookUserController(
            UsedBookService bookService)
        {
            _usedBookService = bookService;
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            // 嘗試取出 claims 中的使用者資訊
            if (AuthHelper.GetUserId(User) is not Guid userId)
                return RedirectToAction("Error", "Home", new { code = ErrorCodes.Auth.Unauthorized, message = "授權失敗" });
            if (User.FindFirst(ClaimTypes.Name)?.Value is not string userName)
                return RedirectToError(ErrorCodes.Auth.InvalidUserContext, "嘗試取出 claims 中的 userName 失敗");
            if (AuthHelper.GetRole(User) is not Role role)
                return RedirectToAction("Error", "Home", new { code = ErrorCodes.Auth.Unauthorized, message = "授權失敗" });

            UserClaimInfoDto dto = new UserClaimInfoDto
            {
                UserId = userId.ToString(),
                UserName = userName,
                RoleName = role.ToString(),
            };

            return View(dto);
        }

        /// <summary>
        /// 取得使用者所有書本資訊列表
        /// </summary>
        [HttpGet("me/books")]
        public async Task<IActionResult> GetUserBookList()
        {
            // 嘗試取出 claims 中的 userId
            if (AuthHelper.GetUserId(User) is not Guid userId)
                return RedirectToAction("Error", "Home", new { code = ErrorCodes.Auth.Unauthorized, message = "授權失敗" });

            // 呼叫 Service Layer
            var result = await _usedBookService.GetUserBookListAsync(userId);
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

        //[HttpPatch("{orderNo}")]
        //[Authorize(Policy = AuthorizationPolicies.CanAccessOrder)]
        //public async Task<ActionResult> CancelOrder([FromRoute] string orderNo, CancellationToken ct = default)
        //{
        //    var result = await _orderService.CancelOrderAsync(orderNo, ct);
        //    if (!result.IsSuccess)
        //        return RedirectToError(result);

        //    return Ok(result.Value);
        //}

        //[HttpPatch("{orderNo}/delivery-face-to-face")]
        //[Authorize(Policy = AuthorizationPolicies.CanAccessOrder)]
        //public async Task<ActionResult> ConfirmFaceToFaceDelivery([FromRoute] string orderNo, CancellationToken ct = default)
        //{
        //    // 嘗試取出 claims 中的 OrderRole
        //    var orderRoleRaw = User.FindFirst("OrderRole")?.Value;
        //    if (!Enum.TryParse<OrderRole>(orderRoleRaw, out var orderRole))
        //        return RedirectToAction("Error", "Home", new { code = ErrorCodes.Auth.Unauthorized, message = "授權失敗" });

        //    var result = await _orderService.ConfirmFaceToFaceDeliveryAsync(orderNo, orderRole, ct);
        //    if (!result.IsSuccess)
        //        return RedirectToError(result);

        //    return NoContent();
        //}

        // UNDONE: 這個 controller 在 UserController
        //[HttpGet("me/orders")]
        //public async Task<ActionResult<IEnumerable<UserOrderListItemDto>>> GetUserOrderList(CancellationToken ct = default)
        //{
        //    // 嘗試取出 claims 中的 userId
        //    if (AuthHelper.GetUserId(User, _logger) is not Guid userId)
        //        return RedirectToAction("Error", "Home", new { code = ErrorCodes.Auth.Unauthorized, message = "授權失敗" });

        //    var result = await _orderService.GetUserOrderListAsync(userId, ct);
        //    if (!result.IsSuccess)
        //        return RedirectToError(result);

        //    return Ok(result.Value);
        //}

    }
}
