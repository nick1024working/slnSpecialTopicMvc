using Microsoft.AspNetCore.Mvc.Rendering;
using prjSpecialTopicMvc.Features.Usedbook.Infrastructure.Repositories;
using prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Responses;
using prjSpecialTopicMvc.Features.Usedbook.Application.Errors;
using prjSpecialTopicMvc.Features.Usedbook.Extends;
using prjSpecialTopicMvc.Features.Usedbook.Utilities;

namespace prjSpecialTopicMvc.Features.Usedbook.Application.Services
{
    public class LookupService
    {
        private readonly BookBindingRepository _bookBindingRepository;
        private readonly BookConditionRatingRepository _bookConditionRatingRepository;
        private readonly ContentRatingRepository _contentRatingRepository;
        private readonly CountyRepository _countyRepository;
        private readonly DistrictRepository _districtRepository;
        private readonly LanguageRepository _languageRepository;
        private readonly ILogger<LookupService> _logger;

        public LookupService(
            BookBindingRepository bookBindingRepository,
            BookConditionRatingRepository bookConditionRatingRepository,
            ContentRatingRepository contentRatingRepository,
            CountyRepository countyRepository,
            DistrictRepository districtRepository,
            LanguageRepository languageRepository,
            ILogger<LookupService> logger)
        {
            _bookBindingRepository = bookBindingRepository;
            _bookConditionRatingRepository = bookConditionRatingRepository;
            _contentRatingRepository = contentRatingRepository;
            _countyRepository = countyRepository;
            _districtRepository = districtRepository;
            _languageRepository = languageRepository;
            _logger = logger;
        }


        /// <summary>
        /// 讀取所有 UsedBook需要的下拉選單資料。
        /// </summary>
        public async Task<Result<AllUsedBookLookupSelectListsDto>> GetAllUsedBookSelectListsAsync(CancellationToken ct = default)
        {
            try
            {
                var bookBindingResult = await GetBookBindingSelectListAsync(ct);
                if (!bookBindingResult.IsSuccess)
                    return Result<AllUsedBookLookupSelectListsDto>.Failure("Lookup.GetBookBindingSelectListAsync 發生錯誤", ErrorCodes.General.Unexpected);

                var bookConditionResult = await GetBookConditionRatingSelectListAsync(ct);
                if (!bookConditionResult.IsSuccess)
                    return Result<AllUsedBookLookupSelectListsDto>.Failure("Lookup.GetBookConditionRatingSelectListAsync 發生錯誤", ErrorCodes.General.Unexpected);

                var contentRatingResult = await GetContentRatingSelectListAsync(ct);
                if (!contentRatingResult.IsSuccess)
                    return Result<AllUsedBookLookupSelectListsDto>.Failure("Lookup.GetContentRatingSelectListAsync 發生錯誤", ErrorCodes.General.Unexpected);

                var countyResult = await GetCountySelectListAsync(ct);
                if (!countyResult.IsSuccess)
                    return Result<AllUsedBookLookupSelectListsDto>.Failure("Lookup.GetCountySelectListAsync 發生錯誤", ErrorCodes.General.Unexpected);

                var languageResult = await GetLanguageSelectListAsync(ct);
                if (!languageResult.IsSuccess)
                    return Result<AllUsedBookLookupSelectListsDto>.Failure("Lookup.GetLanguageSelectListAsync 發生錯誤", ErrorCodes.General.Unexpected);

                var dto = new AllUsedBookLookupSelectListsDto
                {
                    BookBindings = bookBindingResult.Value,
                    BookConditionRatings = bookConditionResult.Value,
                    ContentRatings = contentRatingResult.Value,
                    Counties = countyResult.Value,
                    Languages = languageResult.Value
                };

                return Result<AllUsedBookLookupSelectListsDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<AllUsedBookLookupSelectListsDto>.Map(ex, _logger, nameof(GetBookConditionRatingSelectListAsync));
            }
        }

        /// <summary>
        /// 讀取所有 BookBinding，並轉換為 <see cref="SelectListItem"/> 物件列表，供前端下拉選單使用。
        /// </summary>
        public async Task<Result<List<SelectListItem>>> GetBookBindingSelectListAsync(CancellationToken ct = default)
        {
            try
            {
                var data = await _bookBindingRepository.GetAllAsync(ct);
                var list = data.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
                return Result<List<SelectListItem>>.Success(list);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<List<SelectListItem>>.Map(ex, _logger);
            }
        }

        /// <summary>
        /// 讀取所有 BookConditionRating，並轉換為 <see cref="ExtendedSelectListItem"/> 物件列表，供前端下拉選單使用。
        /// </summary>
        public async Task<Result<List<ExtendedSelectListItem>>> GetBookConditionRatingSelectListAsync(CancellationToken ct = default)
        {
            try
            {
                var data = await _bookConditionRatingRepository.GetAllAsync(ct);
                var list = data.Select(x => new ExtendedSelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                    Description = x.Description
                }).ToList();
                return Result<List<ExtendedSelectListItem>>.Success(list);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<List<ExtendedSelectListItem>>.Map(ex, _logger);
            }
        }

        /// <summary>
        /// 讀取所有 ContentRating，並轉換為 <see cref="SelectListItem"/> 物件列表，供前端下拉選單使用。
        /// </summary>
        public async Task<Result<List<SelectListItem>>> GetContentRatingSelectListAsync(CancellationToken ct = default)
        {
            try
            {
                var data = await _contentRatingRepository.GetAllAsync(ct);
                var list = data.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
                return Result<List<SelectListItem>>.Success(list);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<List<SelectListItem>>.Map(ex, _logger);
            }
        }

        /// <summary>
        /// 讀取所有 County，並轉換為 <see cref="SelectListItem"/> 物件列表，供前端下拉選單使用。
        /// </summary>
        public async Task<Result<List<SelectListItem>>> GetCountySelectListAsync(CancellationToken ct = default)
        {
            try
            {
                var data = await _countyRepository.GetAllAsync(ct);
                var list = data.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
                return Result<List<SelectListItem>>.Success(list);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<List<SelectListItem>>.Map(ex, _logger);
            }
        }

        /// <summary>
        /// 讀取所有 District，並轉換為 <see cref="SelectListItem"/> 物件列表，供前端下拉選單使用。
        /// </summary>
        public async Task<Result<List<SelectListItem>>> GetDistrictSelectListByCountyIdAsync(int countyId, CancellationToken ct = default)
    {
        try
        {
            var data = await _districtRepository.GetByCountyIdAsync(countyId, ct);
            var list = data.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
            return Result<List<SelectListItem>>.Success(list);
        }
        catch (Exception ex)
        {
            return ExceptionToErrorResultMapper<List<SelectListItem>>.Map(ex, _logger);
        }
    }

        /// <summary>
        /// 讀取所有 Language，並轉換為 <see cref="SelectListItem"/> 物件列表，供前端下拉選單使用。
        /// </summary>
        public async Task<Result<List<SelectListItem>>> GetLanguageSelectListAsync(CancellationToken ct = default)
        {
            try
            {
                var data = await _languageRepository.GetAllAsync(ct);
                var list = data.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
                return Result<List<SelectListItem>>.Success(list);
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<List<SelectListItem>>.Map(ex, _logger);
            }
        }

        /// <summary>
        /// 讀取 BookConditionRatingDescription 若無則回空
        /// </summary>
        public async Task<Result<string>> GetBookConditionRatingDescriptionAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var data = await _bookConditionRatingRepository.GetDescriptionByIdAsync(id, ct);

                return Result<string>.Success(data ?? "");
            }
            catch (Exception ex)
            {
                return ExceptionToErrorResultMapper<string>.Map(ex, _logger);
            }
        }
    }
}
