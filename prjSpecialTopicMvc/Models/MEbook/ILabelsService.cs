namespace prjSpecialTopicMvc.Models.MEbook
{
    // 註解：定義 Label 服務的合約
    public interface ILabelsService
    {
        Task<List<Label>> GetAllLabelsAsync();
        Task<Label?> FindLabelByIdAsync(int id);
        Task<Label> CreateLabelAsync(Label newLabel);
        Task UpdateLabelAsync(Label labelToUpdate);
        Task DeleteLabelAsync(int id);
        Task<bool> LabelNameExistsAsync(string name, int? excludeId = null);
        Task<bool> IsLabelInUseAsync(int id);
        // 註解：依據標籤 ID 取得其下的所有書籍
        Task<List<EBookMain>> GetBooksByLabelIdAsync(int labelId);
    }
}
