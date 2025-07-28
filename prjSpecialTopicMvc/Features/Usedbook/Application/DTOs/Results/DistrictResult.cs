using prjSpecialTopicMvc.Features.Usedbook.Application.Interfaces;

namespace prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Results
{
    public class DistrictResult : IHasIdName
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
