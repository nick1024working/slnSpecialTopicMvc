using prjSpecialTopicMvc.Models;
using prjSpecialTopicMvc.Features.Usedbook.Infrastructure.DataAccess.UnitOfWork;
using prjSpecialTopicMvc.Features.Usedbook.Infrastructure.Repositories;
using prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Responses;
using prjSpecialTopicMvc.Features.Usedbook.Application.Errors;
using prjSpecialTopicMvc.Features.Usedbook.Utilities;

namespace prjSpecialTopicMvc.Features.Usedbook.Application.Services
{
    public class BookCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly BookCategoryRepository _bookCategoryRepository;
        private readonly ILogger<BookCategoryService> _logger;

        public BookCategoryService(
            IUnitOfWork unitOfWork,
            BookCategoryRepository bookCategoryRepository,
            ILogger<BookCategoryService> logger
            )
        {
            _unitOfWork = unitOfWork;
            _bookCategoryRepository = bookCategoryRepository;
            _logger = logger;
        }

        /// <summary>
        /// 新增主題類別。
        /// </summary>
        public async Task<Result<int>> CreateAsync(CreateBookCategoryRequest request, CancellationToken ct = default)
        {
            try
            {
                var entity = new BookCategory
                {
                    Name = request.TopicName,
                };
                _bookCategoryRepository.Add(entity);
                await _unitOfWork.CommitAsync(ct);

                return Result<int>.Success(entity.Id);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<int>.Map(ex, _logger);
            }
        }

        /// <summary>
        /// 依照 Id 刪除主題類別。
        /// </summary>
        /// 此操作具備冪等性：若資料已存在，將不進行任何變更；若尚未存在，則新增追蹤紀錄。
        /// </remarks>
        [Obsolete("此方法禁止使用，因書本參考主題，DeleteBehavior.Restrict。", true)]
        public async Task<Result<Unit>> DeleteByIdAsync(int topicId, CancellationToken ct = default)
        {
            try
            {
                var commandResult = await _bookCategoryRepository.RemoveByIdAsync(topicId, ct);
                if (commandResult)
                    await _unitOfWork.CommitAsync(ct);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<Unit>.Map(ex, _logger);
            }
        }

        /// <summary>
        /// 查詢所有主題類別。
        /// </summary>
        public async Task<Result<IReadOnlyList<BookCategoryDto>>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var queryResult = await _bookCategoryRepository.GetAllAsync(ct);
                var dtos = queryResult
                    .Select(q => new BookCategoryDto
                    {
                        TopicId = q.Id,
                        TopicName = q.Name
                    })
                    .ToList();

                return Result<IReadOnlyList<BookCategoryDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<IReadOnlyList<BookCategoryDto>>.Map(ex, _logger);
            }
        }

        /// <summary>
        /// 依照 Id 更新主題類別。
        /// </summary>
        public async Task<Result<Unit>> UpdateTopicNameAsync(
            int topicId, UpdateBookCategoryRequest request, CancellationToken ct = default)
        {
            try
            {
                var entity = new BookCategory
                {
                    Id = topicId,
                    Name = request.TopicName,
                };

                var commandResult = await _bookCategoryRepository.UpdateAsync(entity, ct);
                if (commandResult == false)
                    return Result<Unit>.Failure("找不到要更新的主題類別", ErrorCodes.General.NotFound);

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
