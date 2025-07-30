using AutoMapper;
using prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Responses;
using prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Results;
using prjSpecialTopicMvc.Models;

namespace prjSpecialTopicMvc.Features.Usedbook.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateBookRequest, UsedBook>();

        // BLL 從 RawUsedBookQueryResult 轉回 更新的格式 
        CreateMap<RawUsedBookQueryResult, UpdateBookRequest>();

        // BLL UserService 轉換
        CreateMap<PublicBookListItemQueryResult, PublicBookListItemDto>();
        CreateMap<UserBookListItemQueryResult, UserBookListItemDto>();
        CreateMap<AdminBookListItemQueryResult, AdminBookListItemDto>();

        CreateMap<UsedBookQueryResult, PublicBookDetailDto>();

        CreateMap<UsedBookImageQueryResult, BookImageDto>();
        
        CreateMap<SaleTagQueryResult, BookSaleTagDto>();
    }
}
