using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Requests
{
    public class CreateOrderRequest
    {
        [Required]
        public CreateOrderMetaRequest Meta { get; set; } = default!;

        // NOTE: 這個現在不需要
        public CreateFaceToFaceRequest? FaceToFace { get; set; }

        // NOTE: 暫時不開放
        public CreateEscrowRequest? Escrow { get; set; }
    }
}
