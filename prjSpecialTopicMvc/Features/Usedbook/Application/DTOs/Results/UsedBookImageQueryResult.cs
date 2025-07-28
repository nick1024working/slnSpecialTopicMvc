using prjSpecialTopicMvc.Features.Usedbook.Enums;

namespace prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Results
{
    public class UsedBookImageQueryResult
    {
        public int Id { get; set; }
        public bool IsCover { get; set; }
        public byte ImageIndex { get; set; }
        public StorageProvider StorageProvider { get; set; }
        public string ObjectKey { get; set; } = null!;
        public byte[] Sha256 { get; set; } = null!;
    }
}
