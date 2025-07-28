using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicMvc.Controllers;
using prjSpecialTopicMvc.Features.Usedbook.Utilities;

namespace prjSpecialTopicMvc.Controllers.UsedBook.Common
{
    /// <summary>
    /// 提供共用的 Controller 功能基底類別，例如導向錯誤頁與型別安全的 Redirect 方法。
    /// </summary>
    public abstract class BaseController : Controller
    {
        /// <summary>
        /// 將結果導向至錯誤頁面，並攜帶錯誤代碼與錯誤訊息。
        /// </summary>
        protected ActionResult RedirectToError<T>(Result<T> result)
            => RedirectToAction("Error", "Home", new { code = result.ErrorCode, message = result.ErrorMessage });

        protected ActionResult RedirectToError(string errorCode, string errorMessage = "")
            => RedirectToAction("Error", "Home", new { code = errorCode, message = errorMessage });

        /// <summary>
        /// 導向至 HomeController 的 Index 頁面。
        /// </summary>
        /// <returns>RedirectToActionResult</returns>
        protected IActionResult RedirectToHome()
            => RedirectToAction(nameof(HomeController.Index), "Home");

        // 尚未實現
        /// <summary>
        /// 將請求重新導向至指定 Controller 的指定 Action，並透過 Expression 提供型別安全。
        /// </summary>
    }
}
