using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Queries
{
    /// <summary>
    /// 查詢書籍清單 (PLP) 的條件。
    /// </summary>
    public class BookListQuery
    {
        /// <summary>關鍵字搜尋 (書名 / 作者 / ISBN)。</summary>
        public string? Keyword { get; init; }

        /// <summary>主分類 ID；若為 null 表示全部。</summary>
        public Guid? CategoryId { get; init; }

        /// <summary>多重標籤 (tag) 篩選。以半形逗號分隔，如 &quot;1,2,3&quot;。</summary>
        //[BindProperty(BindingBehavior.Never)]
        //public IReadOnlyList<Guid>? TagIds { get; init; }

        /// <summary>價格下限。</summary>
        [Range(0, 999_999)]
        public decimal? MinPrice { get; init; }

        /// <summary>價格上限。</summary>
        [Range(0, 999_999)]
        public decimal? MaxPrice { get; init; }

        /// <summary>排序欄位。</summary>
        [RegularExpression("created|price|views")]
        public string SortBy { get; init; } = "created";

        /// <summary>排序方向。</summary>
        [RegularExpression("asc|desc")]
        public string SortDir { get; init; } = "desc";

        /// <summary>頁碼 (從 1 開始)。</summary>
        //[Range(1, int.MaxValue)]
        //public int Page { get; init; } = 1;

        /// <summary>每頁筆數。</summary>
        //[Range(1, 100)]
        //public int PageSize { get; init; } = 20;
    }
}
