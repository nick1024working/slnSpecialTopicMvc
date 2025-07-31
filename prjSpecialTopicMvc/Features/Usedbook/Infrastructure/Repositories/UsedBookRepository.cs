using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Results;
using prjSpecialTopicMvc.Features.Usedbook.Enums;
using prjSpecialTopicMvc.Models;

namespace prjSpecialTopicMvc.Features.Usedbook.Infrastructure.Repositories
{
    public class UsedBookRepository
    {
        private readonly TeamAProjectContext _db;

        public UsedBookRepository(TeamAProjectContext db)
        {
            _db = db;
        }

        /// <summary>
        /// 新增書本。
        /// </summary>
        public void Add(UsedBook entity)
        {
            _db.UsedBooks.Add(entity);
        }

        /// <summary>
        /// 更新書本
        /// </summary>
        /// <exception cref="InvalidOperationException">查詢結果超過一筆時拋出，通常代表資料違反唯一性約束。</exception>
        public async Task<bool> UpdateAsync(Guid id, UpdateBookRequest request, CancellationToken ct = default)
        {
            var entity = await _db.UsedBooks
                .SingleOrDefaultAsync(b => b.Id == id, ct);
            if (entity == null)
                return false;

            entity.SalePrice = request.SalePrice;
            entity.Title = request.Title;
            entity.Authors = request.Authors;
            entity.ConditionRatingId = request.ConditionRatingId;
            entity.ConditionDescription = request.ConditionDescription;
            entity.Edition = request.Edition;
            entity.Publisher = request.Publisher;
            entity.PublicationDate = request.PublicationDate;
            entity.Isbn = request.Isbn;
            entity.BindingId = request.BindingId;
            entity.LanguageId = request.LanguageId;
            entity.Pages = request.Pages;
            entity.ContentRatingId = request.ContentRatingId;
            entity.IsOnShelf = request.IsOnShelf;
            entity.UpdatedAt = DateTime.UtcNow;

            return true;
        }

        /// <summary>
        /// 根據指定的 BookId 更新啟用狀態（軟刪除/還原）。
        /// </summary>
        /// <exception cref="InvalidOperationException">查詢結果超過一筆時拋出，通常代表資料違反唯一性約束。</exception>
        public async Task<bool> UpdateActiveStatusAsync(Guid id, bool isActive, CancellationToken ct = default)
        {
            var result = await _db.UsedBooks
                .SingleOrDefaultAsync(b => b.Id == id, ct);
            if (result == null)
                return false;
            result.IsActive = isActive;
            return true;
        }

        /// <summary>
        /// 根據 BookId 查詢書本完整資訊。
        /// </summary>
        /// <exception cref="InvalidOperationException">查詢結果超過一筆時拋出，通常代表資料違反唯一性約束。</exception>
        public async Task<UsedBookQueryResult?> GetByBookIdAsync(Guid bookId, CancellationToken ct = default)
        {
            var queryResult = await _db.UsedBooks
                .Where(b => b.Id == bookId)
                .Select(b => new UsedBookQueryResult
                {
                    Id = b.Id,
                    SellerId = b.SellerId,
                    SellerCountyName = b.SellerDistrict.County.Name,
                    SellerDistrictName = b.SellerDistrict.Name,
                    SalePrice = b.SalePrice,
                    Title = b.Title,
                    Authors = b.Authors,
                    ConditionRatingName = b.ConditionRating.Name,

                    ConditionDescription = b.ConditionDescription,
                    Edition = b.Edition,
                    Publisher = b.Publisher,
                    PublicationDate = b.PublicationDate,
                    Isbn = b.Isbn,
                    BindingName = b.Binding != null ? b.Binding.Name : "",
                    LanguageName = b.Language != null ? b.Language.Name : "",
                    Pages = b.Pages,
                    ContentRatingName = b.ContentRating.Name,

                    IsOnShelf = b.IsOnShelf,
                    IsSold = b.IsSold,
                    IsActive = b.IsActive,
                    Slug = b.Slug,

                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt
                })
                .SingleOrDefaultAsync(ct);

            return queryResult;
        }

        /// <summary>
        /// 根據 BookId 查詢書本完整資訊 (未連結各屬性)。
        /// </summary>
        /// <exception cref="InvalidOperationException">查詢結果超過一筆時拋出，通常代表資料違反唯一性約束。</exception>
        public async Task<UpdateBookRequest?> GetUpdatePayloadAsync(Guid bookId, CancellationToken ct = default)
        {
            var queryResult = await _db.UsedBooks
                .AsNoTracking()
                .Where(b => b.Id == bookId)
                .Select(b => new UpdateBookRequest
                {
                    ImageList = b.UsedBookImages
                        .OrderBy(bi => bi.ImageIndex)
                        .Select(bi => new UpdateUsedBookImageRequest
                        {
                            Id = bi.Id,
                            IsCover = bi.IsCover,
                            StorageProvider = (StorageProvider)bi.StorageProvider,
                            ObjectKey = bi.ObjectKey,
                            Sha256 = Convert.ToBase64String(bi.Sha256)
                        })
                        .ToList(),

                    SellerCountyId = b.SellerDistrict.CountyId,
                    SellerDistrictId = b.SellerDistrictId,
                    SalePrice = b.SalePrice,
                    Title = b.Title,
                    Authors = b.Authors,
                    ConditionRatingId = b.ConditionRatingId,

                    ConditionDescription = b.ConditionDescription,
                    Edition = b.Edition,
                    Publisher = b.Publisher,
                    PublicationDate = b.PublicationDate,
                    Isbn = b.Isbn,
                    BindingId = b.BindingId,
                    LanguageId = b.LanguageId,
                    Pages = b.Pages,
                    ContentRatingId = b.ContentRatingId,

                    IsOnShelf = b.IsOnShelf
                })
                .SingleOrDefaultAsync(ct);

            return queryResult;
        }

        /// <summary>
        /// 根據指定的 UserId 查詢所有書本完整資訊。
        /// </summary>
        //public async Task<IReadOnlyList<UsedBookQueryResult>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)

        /// <summary>
        /// 查詢所有書本完整資訊。
        /// </summary>
        //public async Task<IReadOnlyList<UsedBookQueryResult>> GetAllAsync(CancellationToken ct = default)

        // TODO: 需要分頁 補呼叫鏈上 query + filter
        /// <summary>
        /// 查詢公開書本清單 (清單項目資料，非詳細資料)。
        /// </summary>
        public async Task<IReadOnlyList<PublicBookListItemQueryResult>> GetPublicBookListAsync(CancellationToken ct = default)
        {
            // TODO: 補 filter
            return await _db.UsedBooks
                .AsNoTracking()
                .Include(b => b.UsedBookImages)
                .Include(b => b.ConditionRating)
                .Where(b => b.IsOnShelf && b.IsActive && b.UsedBookImages.Any(x => x.IsCover))
                .OrderByDescending(b => b.UpdatedAt)
                .Select(b => new PublicBookListItemQueryResult
                {
                    CoverStorageProvider = (StorageProvider)b.UsedBookImages.First(x => x.IsCover).StorageProvider,
                    CoverObjectKey = b.UsedBookImages.First(x => x.IsCover).ObjectKey,
                    Id = b.Id,
                    Title = b.Title,
                    SalePrice = b.SalePrice,
                    Authors = b.Authors,
                    ConditionRating = b.ConditionRating.Name,
                    Slug = b.Slug
                })
                .ToListAsync(ct);
        }

        // TODO: 需要分頁
        /// <summary>
        /// 根據 UserId 查詢該使用者書本清單 (清單項目資料，非詳細資料)。
        /// </summary>
        public async Task<IReadOnlyList<UserBookListItemQueryResult>> GetUserBookListAsync(
            Guid userId,
            Expression<Func<UsedBook, bool>> predicate,
            Func<IQueryable<UsedBook>, IOrderedQueryable<UsedBook>> orderBy,
            CancellationToken ct = default)
        {
            // 1. 建立查詢（包含條件與關聯載入）
            var query = _db.UsedBooks
                .AsNoTracking()
                .Where(b => b.SellerId == userId && b.IsActive)
                .Where(predicate)
                .Include(b => b.UsedBookImages)
                .Include(b => b.ContentRating);

            // 2. 套用排序
            var orderedQuery = orderBy(query);

            // 3. 投影成結果
            var result = await orderedQuery
            .Select(b => new
            {
                Book = b,
                CoverImage = b.UsedBookImages.FirstOrDefault(x => x.IsCover)
            })
            .Select(x => new UserBookListItemQueryResult
            {
                Id = x.Book.Id,
                SellerId = x.Book.SellerId,
                SalePrice = x.Book.SalePrice,
                Title = x.Book.Title,
                ConditionRating = x.Book.ContentRating.Name,
                IsOnShelf = x.Book.IsOnShelf,
                IsSold = x.Book.IsSold,
                Slug = x.Book.Slug,
                CoverStorageProvider = (StorageProvider?)x.CoverImage.StorageProvider,
                CoverObjectKey = x.CoverImage.ObjectKey,
                CreatedAt = x.Book.CreatedAt,
                UpdatedAt = x.Book.UpdatedAt,
            })
            .ToListAsync(ct);

            return result;
        }

        // TODO: 需要分頁 補呼叫鏈上 query + filter
        /// <summary>
        /// 管理員查詢書本清單 (清單項目資料，非詳細資料)。
        /// </summary>
        public async Task<IReadOnlyList<AdminBookListItemQueryResult>> GetAdminBookListAsync(
            Expression<Func<UsedBook, bool>> predicate,
            Func<IQueryable<UsedBook>, IOrderedQueryable<UsedBook>> orderBy,
            CancellationToken ct = default)
        {
            // 1. 建立查詢（包含條件與關聯載入）
            var query = _db.UsedBooks
                .AsNoTracking()
                .Where(predicate)
                .Include(b => b.UsedBookImages)
                .Include(b => b.ContentRating);

            // 2. 套用排序
            var orderedQuery = orderBy(query);

            // 3. 投影成結果
            var result = await orderedQuery
            .Select(b => new
            {
                Book = b,
                CoverImage = b.UsedBookImages.FirstOrDefault(x => x.IsCover)
            })
            .Select(x => new AdminBookListItemQueryResult
            {
                Id = x.Book.Id,
                SellerId = x.Book.SellerId,
                SalePrice = x.Book.SalePrice,
                Title = x.Book.Title,
                ConditionRating = x.Book.ContentRating.Name,
                IsOnShelf = x.Book.IsOnShelf,
                IsActive = x.Book.IsActive,
                IsSold = x.Book.IsSold,
                Slug = x.Book.Slug,
                CoverStorageProvider = (StorageProvider?)x.CoverImage.StorageProvider,
                CoverObjectKey = x.CoverImage.ObjectKey,
                CreatedAt = x.Book.CreatedAt,
                UpdatedAt = x.Book.UpdatedAt,
            })
            .ToListAsync(ct);

            return result;
        }

        /// <summary>
        /// 直接返回書本實體 (含促銷標籤)。
        /// </summary>
        public async Task<UsedBook?> GetEntityByIdWithSaleTagsAsync(Guid id, CancellationToken ct = default) =>
            await _db.UsedBooks
            .Include(b => b.Tags)
            .SingleOrDefaultAsync(b => b.Id == id, ct);

        /// <summary>
        /// 為指定書籍建立一個 SaleTag 關聯
        /// </summary>
        public async Task<bool> AddSaleTagAsync(Guid bookId, int tagId, CancellationToken ct = default)
        {
            var book = await _db.UsedBooks
                .Include(b => b.Tags)
                .FirstOrDefaultAsync(b => b.Id == bookId, ct);

            if (book == null || book.Tags.Any(t => t.Id == tagId))
                return false;

            var trackedTag = await _db.BookSaleTags.FirstOrDefaultAsync(t => t.Id == tagId, ct);
            if (trackedTag == null)
                return false;

            book.Tags.Add(trackedTag);
            return true;
        }

        /// <summary>
        /// 把指定書籍移除促銷標籤
        /// </summary>
        public async Task<bool> RemoveBookSaleTagAsync(Guid bookId, int tagId, CancellationToken ct = default)
        {
            var bookWithTagsEntity = await _db.UsedBooks
                .Include(b => b.Tags)
                .FirstOrDefaultAsync(b => b.Id == bookId, ct);

            if (bookWithTagsEntity == null)
                return false;

            var saleTagToRemove = bookWithTagsEntity.Tags
                .SingleOrDefault(st => st.Id == tagId);
            if (saleTagToRemove == null)
                return false;

            bookWithTagsEntity.Tags.Remove(saleTagToRemove);
            return true;
        }
    }
}
