using Microsoft.AspNetCore.Mvc.Rendering;
using prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicMvc.Features.Usedbook.Extends;

namespace prjSpecialTopicMvc.ViewModels.UsedBook
{
    public class CreateBookViewModel
    {
        // 送出後 model binding 的型別（表格對應使用）
        public CreateBookRequest Request { get; set; } = new CreateBookRequest();

        // 下拉選單選項（UI 使用）
        public List<SelectListItem>? BookBindings { get; set; }
        public List<ExtendedSelectListItem>? BookConditionRatings { get; set; }
        public List<SelectListItem>? ContentRatings { get; set; }
        public List<SelectListItem>? Counties { get; set; }
        public List<SelectListItem>? Languages { get; set; }
    }
}