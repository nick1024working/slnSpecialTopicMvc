using Microsoft.AspNetCore.Mvc.Rendering;
using prjSpecialTopicMvc.Features.Usedbook.Extends;

namespace prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Responses
{
    public class AllUsedBookLookupSelectListsDto
    {
        public List<SelectListItem> BookBindings { get; set; } = [];
        public List<ExtendedSelectListItem> BookConditionRatings { get; set; } = [];
        public List<SelectListItem> ContentRatings { get; set; } = [];
        public List<SelectListItem> Counties { get; set; } = [];
        public List<SelectListItem> Languages { get; set; } = [];
    }
}
