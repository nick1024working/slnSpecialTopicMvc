using Microsoft.EntityFrameworkCore;
using prjSpecialTopicMvc.Models;
using prjSpecialTopicMvc.Models;
using prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Results;

namespace prjSpecialTopicMvc.Features.Usedbook.Infrastructure.Repositories
{
    public class BookSaleTagRepository
    {
        private readonly TeamAProjectContext _db;

        public BookSaleTagRepository(TeamAProjectContext db)
        {
            _db = db;
        }

        /// <summary>
        /// 直接返回促銷標籤實體。
        /// </summary>
        public async Task<BookSaleTag?> GetEntityAsync(int id, CancellationToken ct = default) =>
            await _db.BookSaleTags
            .SingleOrDefaultAsync(st => st.Id == id, ct);


        /// <summary>
        /// 建立新銷售標籤。
        /// </summary>
        public void Add(BookSaleTag entity)
        {
            _db.BookSaleTags.Add(entity);
        }

        /// <summary>
        /// 檢查是否存在。
        /// </summary>
        public async Task<bool> ExistsAsync(int tagId, CancellationToken ct = default) =>
             await _db.BookSaleTags.AsNoTracking().AnyAsync(t => t.Id == tagId, ct);

        /// <summary>
        /// 更新指定銷售標籤的名稱。
        /// </summary>
        /// <return>
        /// 傳回 <c>true</c> 表示已找到並標記更新目標（尚未呼叫 <see cref="DbContext.SaveChangesAsync()" />）。
        /// 傳回 <c>false</c> 表示找不到符合條件的紀錄。
        /// </return>
        /// <exception cref="InvalidOperationException">查詢結果超過一筆時拋出，通常代表資料違反唯一性約束。</exception>
        public async Task<bool> UpdateAsync(BookSaleTag entity, CancellationToken ct = default)
        {
            var queryResult = await _db.BookSaleTags
                .SingleOrDefaultAsync(st => st.Id == entity.Id, ct);
            if (queryResult == null)
                return false;

            queryResult.Name = entity.Name;

            return true;
        }

        /// <summary>
        /// 刪除指定銷售標籤。
        /// </summary>
        /// <return>
        /// 傳回 <c>true</c> 表示已找到並標記刪除目標（尚未呼叫 <see cref="DbContext.SaveChangesAsync()" />）。
        /// 傳回 <c>false</c> 表示找不到符合條件的紀錄。
        /// </return>
        /// <exception cref="InvalidOperationException">查詢結果超過一筆時拋出，通常代表資料違反唯一性約束。</exception>
        public async Task<bool> DeleteByIdAsync(int id, CancellationToken ct = default)
        {
            var queryResult = await _db.BookSaleTags
                .SingleOrDefaultAsync(st => st.Id == id, ct);
            if (queryResult == null)
                return false;
            _db.BookSaleTags.Remove(queryResult);
            return true;
        }

        /// <summary>
        /// 根據 Id 查詢銷售標籤。
        /// </summary>
        /// <exception cref="InvalidOperationException">查詢結果超過一筆時拋出，通常代表資料違反唯一性約束。</exception>
        public async Task<SaleTagQueryResult?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var queryResult = await _db.BookSaleTags
                .AsNoTracking()
                .Where(st => st.Id == id)
                .Select(st => new SaleTagQueryResult
                {
                    Id = st.Id,
                    Name = st.Name,
                })
                .SingleOrDefaultAsync(ct);
            return queryResult;
        }

        /// <summary>
        /// 根據 BookId 查詢所有銷售標籤。
        /// </summary>
        public async Task<IReadOnlyList<SaleTagQueryResult>> GetByBookIdAsync(Guid bookId, CancellationToken ct = default)
        {
            var queryResult = await _db.UsedBooks
                .AsNoTracking()
                .Where(b => b.Id == bookId)
                .SelectMany(b => b.Tags)
                .Select(st => new SaleTagQueryResult
                {
                    Id = st.Id,
                    Name = st.Name,
                })
                .ToListAsync(ct);
            return queryResult;
        }

        /// <summary>
        /// 查詢所有銷售標籤清單。
        /// </summary>
        public async Task<IReadOnlyList<SaleTagQueryResult>> GetAllAsync(CancellationToken ct = default)
        {
            var queryResult = await _db.BookSaleTags
                .AsNoTracking()
                .Select(st => new SaleTagQueryResult
                {
                    Id = st.Id,
                    Name = st.Name,
                })
                .OrderBy(t => t.Id)
                .ToListAsync(ct);
            return queryResult;
        }

    }
}
