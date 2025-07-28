using System.ComponentModel.DataAnnotations;
using prjSpecialTopicMvc.Features.Usedbook.Enums;

namespace prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Requests
{
    public class CreateOrderMetaRequest
    {
        [Required]
        public Guid BookId { get; set; }

        [Required]
        [Range(0, 1)]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        [Range(0, 0)]
        public DeliveryMethod DeliveryMethod { get; set; }
    }
}
