namespace prjSpecialTopicMvc.ViewModels.EBook
{
    // 註解：用於顯示單一標籤的資訊 (ID、名稱、是否已被選中)
    public class AssignedLabelViewModel
    {
        public int LabelId { get; set; }
        public string LabelName { get; set; } = null!;
        public bool IsAssigned { get; set; }
    }

    // 註解：用於標籤管理頁面的主 ViewModel
    public class ManageEbookTagsViewModel
    {
        public long EbookId { get; set; }
        public string EbookName { get; set; } = null!;
        // 註解：包含所有可用標籤的列表，以及它們是否被指派給本書的狀態
        public List<AssignedLabelViewModel> AllLabels { get; set; } = new();
    }
}
