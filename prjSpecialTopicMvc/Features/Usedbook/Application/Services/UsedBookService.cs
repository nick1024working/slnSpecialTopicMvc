using AutoMapper;
using prjSpecialTopicMvc.Models;
using prjSpecialTopicMvc.Features.Usedbook.Infrastructure.DataAccess.UnitOfWork;
using prjSpecialTopicMvc.Features.Usedbook.Infrastructure.Repositories;
using prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Queries;
using prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Responses;
using prjSpecialTopicMvc.Features.Usedbook.Application.Errors;
using prjSpecialTopicMvc.Features.Usedbook.Enums;
using prjSpecialTopicMvc.Features.Usedbook.Utilities;
using System.Linq.Expressions;
using System;

namespace prjSpecialTopicMvc.Features.Usedbook.Application.Services
{
    public class UsedBookService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UsedBookRepository _usedBookRepository;
        private readonly UsedBookImageService _usedBookImageService;
        private readonly UsedBookImageRepository _usedBookImageRepository;
        private readonly ImageService _imageService;
        private readonly BookSaleTagRepository _saleTagRepository;
        private readonly ILogger<UsedBookService> _logger;

        public UsedBookService (
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UsedBookRepository usedBookRepository,
            UsedBookImageService usedBookImageService,
            UsedBookImageRepository usedBookImageRepository,
            ImageService imageService,
            BookSaleTagRepository saleTagRepository,
            ILogger<UsedBookService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _usedBookRepository = usedBookRepository;
            _usedBookImageService = usedBookImageService;
            _usedBookImageRepository = usedBookImageRepository;
            _imageService = imageService;
            _saleTagRepository = saleTagRepository;
            _logger = logger;
        }

        /// <summary>
        /// 新增書籍。
        /// </summary>
        public async Task<Result<Guid>> CreateAsync(Guid sellerId, CreateBookRequest request, CancellationToken ct = default)
        {
            Guid usedBookId = Guid.NewGuid();
            DateTime nowTime = DateTime.UtcNow;

            var entity = _mapper.Map<UsedBook>(request);
            entity.Id = usedBookId;
            entity.SellerId = sellerId;
            entity.IsSold = false;
            entity.IsActive = true;
            entity.Slug = usedBookId.ToString();
            entity.CreatedAt = nowTime;
            entity.UpdatedAt = nowTime;

            using var tx = _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                _usedBookRepository.Add(entity);
                var commandResult = await _usedBookImageService.CreateAsync(usedBookId, request.ImageList);
                if (!commandResult.IsSuccess)
                    throw new Exception(commandResult.ErrorMessage);

                await _unitOfWork.CommitAsync(ct);

                return Result<Guid>.Success(usedBookId);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(ct);
                return ExceptionToErrorResultMapper<Guid>.Map(ex, _logger);
            }
        }

        /// <summary>
        /// 根據 BookId 查詢書本完整公開資訊。
        /// </summary>
        public async Task<Result<PublicBookDetailDto>> GetPubicAsync(Guid bookId, CancellationToken ct = default)
        {
            try
            {
                var bookQueryResult = await _usedBookRepository.GetByBookIdAsync(bookId, ct);
                if (bookQueryResult == null)
                    return Result<PublicBookDetailDto>.Failure("找不到符合的資料", ErrorCodes.General.NotFound);

                var imageQueryResult = await _usedBookImageRepository.GetByBookIdAsync(bookId, ct);

                var dto = _mapper.Map<PublicBookDetailDto>(bookQueryResult);
                dto.ImageList = _mapper.Map<IEnumerable<BookImageDto>>(imageQueryResult);

                return Result<PublicBookDetailDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<PublicBookDetailDto>.Map(ex, _logger);
            }
        }

        /// <summary>
        /// 根據 BookId 查詢書本完整資訊 (未連結各屬性)。
        /// </summary>
        /// <exception cref="InvalidOperationException">查詢結果超過一筆時拋出，通常代表資料違反唯一性約束。</exception>
        public async Task<Result<UpdateBookRequest>> GetUpdatePayloadAsync(Guid bookId, CancellationToken ct = default)
        {
            try
            {
                var result = await _usedBookRepository.GetUpdatePayloadAsync(bookId, ct);
                if (result == null)
                    return Result<UpdateBookRequest>.Failure("找不到符合的資料", ErrorCodes.General.NotFound);

                // HACK: 直接傳DTO 沒時間改組了

                return Result<UpdateBookRequest>.Success(result);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<UpdateBookRequest>.Map(ex, _logger);
            }
        }

        /// <summary>
        /// 更新書本。
        /// </summary>
        public async Task<Result<Unit>> UpdateAsync(Guid bookId, UpdateBookRequest request, CancellationToken ct = default)
        {
            try
            {
                await _usedBookImageService.UpdateAsync(bookId, request.ImageList, ct);
                var commandResult = await _usedBookRepository.UpdateAsync(bookId, request, ct);

                if (commandResult == false)
                    return Result<Unit>.Failure("非此資源擁有者", ErrorCodes.General.NotFound);

                await _unitOfWork.CommitAsync(ct);

                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<Unit>.Map(ex, _logger);
            }
        }

        /// <summary>
        /// 改變指定書本啟禁用狀態，此操作具備冪等性。
        /// </summary>
        public async Task<Result<Unit>> UpdateActiveStatusAsync(Guid bookId, bool isActive, CancellationToken ct = default)
        {
            try
            {
               bool commandResult = await _usedBookRepository.UpdateActiveStatusAsync(bookId, isActive, ct);
                if (commandResult)
                    await _unitOfWork.CommitAsync(ct);

                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<Unit>.Map(ex, _logger);
            }
        }

        // TODO: 需要分頁 補呼叫鏈上 query + filter
        /// <summary>
        /// 查詢公開書本清單 (清單項目資料，非詳細資料)。
        /// </summary>
        public async Task<Result<IReadOnlyList<PublicBookListItemDto>>> GetPublicListAsync(BookListQuery query, CancellationToken ct = default)
        {
            try
            {
                var queryResult = await _usedBookRepository.GetPublicBookListAsync(ct);
                var dtoList = new List<PublicBookListItemDto>();
                foreach (var res in queryResult)
                {
                    var dto = _mapper.Map<PublicBookListItemDto>(res);

                    string? filePath = null;
                    if (res.CoverStorageProvider == StorageProvider.Local && res.CoverObjectKey != null)
                    {
                        filePath = "/" + _imageService.GetMainRelativePath(res.CoverObjectKey)?.Replace("\\", "/");
                    }
                    else if (res.CoverStorageProvider == StorageProvider.Cloudinary)
                    {
                        // TODO: 這裡放 Cloudinary 的處理方式
                    }

                    dto.CoverImageUrl = filePath ?? "/images/fallback-thumb.jpg";
                    dtoList.Add(dto);
                }

                return Result<IReadOnlyList<PublicBookListItemDto>>.Success(dtoList);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<IReadOnlyList<PublicBookListItemDto>>.Map(ex, _logger);
            }
        }

        // TODO: 需要分頁
        /// <summary>
        /// 根據 UserId 查詢該使用者書本清單 (清單項目資料，非詳細資料)。
        /// </summary>
        public async Task<Result<IReadOnlyList<UserBookListItemDto>>> GetUserBookListAsync(Guid userId, BookListQuery query, CancellationToken ct = default)
        {
            try
            {
                // 條件
                Expression<Func<UsedBook, bool>> predicate = b =>
                    (
                        string.IsNullOrWhiteSpace(query.BookStatus) ||
                        (query.BookStatus == "all") ||
                        (query.BookStatus == "unsold" && !b.IsSold) ||
                        (query.BookStatus == "onshelf" && b.IsOnShelf)
                    ) &&
                    (string.IsNullOrWhiteSpace(query.Keyword) || b.Title.Contains(query.Keyword)) &&
                    (!query.MinPrice.HasValue || b.SalePrice >= query.MinPrice) &&
                    (!query.MaxPrice.HasValue || b.SalePrice <= query.MaxPrice);

                // 排序
                Func<IQueryable<UsedBook>, IOrderedQueryable<UsedBook>> orderBy = q =>
                {
                    return query switch
                    {
                        _ when query.SortBy == "updated" && query.SortDir == "asc" => q.OrderBy(b => b.UpdatedAt),
                        _ when query.SortBy == "updated" && query.SortDir == "desc" => q.OrderByDescending(b => b.UpdatedAt),
                        _ when query.SortBy == "created" && query.SortDir == "asc" => q.OrderBy(b => b.CreatedAt),
                        _ when query.SortBy == "created" && query.SortDir == "desc" => q.OrderByDescending(b => b.CreatedAt),
                        _ when query.SortBy == "price" && query.SortDir == "asc" => q.OrderBy(b => b.SalePrice),
                        _ when query.SortBy == "price" && query.SortDir == "desc" => q.OrderByDescending(b => b.SalePrice),
                        _ => q.OrderByDescending(b => b.UpdatedAt) // fallback
                    };
                };

                var queryResult = await _usedBookRepository.GetUserBookListAsync(userId, predicate, orderBy, ct);
                var dtoList = new List<UserBookListItemDto>();
                foreach (var res in queryResult)
                {
                    var dto = _mapper.Map<UserBookListItemDto>(res);

                    string? filePath = null;
                    if (res.CoverStorageProvider == StorageProvider.Local && res.CoverObjectKey != null)
                    {
                        filePath = "/" + _imageService.GetThumbRelativePath(res.CoverObjectKey)?.Replace("\\", "/");
                    }
                    else if (res.CoverStorageProvider == StorageProvider.Cloudinary)
                    {
                        // TODO: 這裡放 Cloudinary 的處理方式
                    }

                    dto.CoverImageUrl = filePath ?? "/images/fallback-thumb.jpg";
                    dtoList.Add(dto);
                }

                return Result<IReadOnlyList<UserBookListItemDto>>.Success(dtoList);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<IReadOnlyList<UserBookListItemDto>>.Map(ex, _logger);
            }
        }

        // TODO: 補呼叫鏈上 query + filter
        /// <summary>
        /// 管理員查詢書本清單 (清單項目資料，非詳細資料)。
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Result<IReadOnlyList<AdminBookListItemDto>>> GetAdminBookListAsync(BookListQuery query, CancellationToken ct = default)
        {
            try
            {
                // 條件
                Expression<Func<UsedBook, bool>> predicate = b =>
                    (
                        string.IsNullOrWhiteSpace(query.BookStatus) ||
                        (query.BookStatus == "all") ||
                        (query.BookStatus == "unsold" && !b.IsSold) ||
                        (query.BookStatus == "onshelf" && b.IsOnShelf)
                    ) &&
                    (string.IsNullOrWhiteSpace(query.Keyword) || b.Title.Contains(query.Keyword)) &&
                    (!query.MinPrice.HasValue || b.SalePrice >= query.MinPrice) &&
                    (!query.MaxPrice.HasValue || b.SalePrice <= query.MaxPrice);

                // 排序
                Func<IQueryable<UsedBook>, IOrderedQueryable<UsedBook>> orderBy = q =>
                {
                    return query switch
                    {
                        _ when query.SortBy == "updated" && query.SortDir == "asc" => q.OrderBy(b => b.UpdatedAt),
                        _ when query.SortBy == "updated" && query.SortDir == "desc" => q.OrderByDescending(b => b.UpdatedAt),
                        _ when query.SortBy == "created" && query.SortDir == "asc" => q.OrderBy(b => b.CreatedAt),
                        _ when query.SortBy == "created" && query.SortDir == "desc" => q.OrderByDescending(b => b.CreatedAt),
                        _ when query.SortBy == "price" && query.SortDir == "asc" => q.OrderBy(b => b.SalePrice),
                        _ when query.SortBy == "price" && query.SortDir == "desc" => q.OrderByDescending(b => b.SalePrice),
                        _ => q.OrderByDescending(b => b.UpdatedAt) // fallback
                    };
                };

                var queryResult = await _usedBookRepository.GetAdminBookListAsync(predicate, orderBy, ct);
                var dtoList = new List<AdminBookListItemDto>();
                foreach (var res in queryResult)
                {
                    var dto = _mapper.Map<AdminBookListItemDto>(res);

                    string? filePath = null;
                    if (res.CoverStorageProvider == StorageProvider.Local && res.CoverObjectKey != null)
                    {
                        filePath = "/" + _imageService.GetThumbRelativePath(res.CoverObjectKey)?.Replace("\\", "/");
                    }
                    else if (res.CoverStorageProvider == StorageProvider.Cloudinary)
                    {
                        // TODO: 這裡放 Cloudinary 的處理方式
                    }

                    dto.CoverImageUrl = filePath ?? "/images/fallback-thumb.jpg";
                    dtoList.Add(dto);
                }

                return Result<IReadOnlyList<AdminBookListItemDto>>.Success(dtoList);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<IReadOnlyList<AdminBookListItemDto>>.Map(ex, _logger);
            }
        }

        /// <summary>
        /// 管理員為指定書籍指派促銷標籤
        /// </summary>
        public async Task<Result<Unit>> AddBookSaleTagAsync(Guid bookId, int tagId, CancellationToken ct = default)
        {
            try
            {
                // 檢查書籍是否存在
                UsedBook? bookWithTagsEntity = await _usedBookRepository.GetEntityByIdWithSaleTagsAsync(bookId, ct);
                if (bookWithTagsEntity == null)
                    return Result<Unit>.Failure("找不到目標書籍", ErrorCodes.General.NotFound);

                // 檢查重複
                if (bookWithTagsEntity.Tags.Any(t => t.Id == tagId))
                    return Result<Unit>.Failure("書籍已經有此銷售標籤", ErrorCodes.General.Conflict);

                // 檢查銷售標籤是否存在
                BookSaleTag? saleTagsEntity = await _saleTagRepository.GetEntityAsync(tagId, ct);
                if (saleTagsEntity == null)
                    return Result<Unit>.Failure("找不到目標銷售標籤", ErrorCodes.General.NotFound);

                bookWithTagsEntity.Tags.Add(saleTagsEntity);

                await _unitOfWork.CommitAsync(ct);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<Unit>.Map(ex, _logger);
            }
        }

        /// <summary>
        /// 管理員為指定書籍移除促銷標籤
        /// </summary>
        public async Task<Result<Unit>> RemoveBookSaleTagAsync(Guid bookId, int tagId, CancellationToken ct = default)
        {
            try
            {
                var commandResult = await _usedBookRepository.RemoveBookSaleTagAsync(bookId, tagId, ct);
                if (commandResult)
                    await _unitOfWork.CommitAsync(ct);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<Unit>.Map(ex, _logger);
            }
        }
    }
}
