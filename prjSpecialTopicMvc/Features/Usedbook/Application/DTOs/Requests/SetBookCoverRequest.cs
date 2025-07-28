using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Requests
{
    public class SetBookCoverRequest
    {
        // HACK: 最好自訂 ValidationAttribute
        [Required]
        [Range(0, 100)]
        public int? ImageId { get; set; }
    }
}
