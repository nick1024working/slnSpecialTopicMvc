using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using prjSpecialTopicMvc.Models;
using System.Collections.Generic;

namespace prjSpecialTopicMvc.ViewModels.Forum
{
    public class ForumPostViewModel
    {
        [ValidateNever] // ✅ 不驗證
        public ForumPost ForumPost { get; set; } = new ForumPost();

        public int? SelectedCategoryId { get; set; }
        public int? SelectedFilterId { get; set; }

        [ValidateNever] // ✅ 不驗證
        public IEnumerable<SelectListItem> Categories { get; set; }
        [ValidateNever] // ✅ 不驗證
        public IEnumerable<SelectListItem> Filters { get; set; }
    }
}
