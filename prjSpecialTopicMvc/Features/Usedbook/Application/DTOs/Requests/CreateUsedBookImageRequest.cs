using System.ComponentModel.DataAnnotations;
using prjSpecialTopicMvc.Features.Usedbook.Enums;

namespace prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Requests
{
    public class CreateUsedBookImageRequest
    {
        [Required]
        public bool? IsCover { get; set; }

        [Required]
        [EnumDataType(typeof(StorageProvider))]
        public StorageProvider StorageProvider { get; set; }

        [Required]
        [MaxLength(300)]
        public string ObjectKey { get; set; } = null!;

        [Required]
        [RegularExpression(@"^[A-Za-z0-9+/]{43}[A-Za-z0-9+/=]{1}$")]
        public string Sha256 { get; set; } = null!;
    }
}
