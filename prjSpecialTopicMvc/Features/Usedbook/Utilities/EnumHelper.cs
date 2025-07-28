using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace prjSpecialTopicMvc.Features.Usedbook.Utilities
{
    /// <summary>
    /// 提供 Enum 相關的工具方法。
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// 取得指定 Enum 成員的顯示名稱 (Display Name)。
        /// 會先嘗試讀取成員上的 <see cref="DisplayAttribute"/>，若未設定則回傳 Enum 成員名稱字串。
        /// </summary>
        /// <param name="enumValue">要取得顯示名稱的 Enum 成員。</param>
        /// <returns>
        /// 回傳 <see cref="DisplayAttribute.Name"/> 設定值，若未設定則回傳 Enum 成員的名稱字串。
        /// </returns>
        /// <exception cref="ArgumentNullException">當 <paramref name="enumValue"/> 為 null 時擲出例外。</exception>
        public static string GetDisplayName(Enum? enumValue)
        {
            if (enumValue == null)
                throw new ArgumentNullException(nameof(enumValue));

            var type = enumValue.GetType();
            var name = enumValue.ToString();
            var field = type.GetField(name);
            if (field == null)
                return name;

            var displayAttribute = field.GetCustomAttribute<DisplayAttribute>();
            return displayAttribute?.Name ?? name;
        }
    }
}
