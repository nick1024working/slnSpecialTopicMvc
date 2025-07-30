using Microsoft.EntityFrameworkCore;

namespace prjSpecialTopicMvc.Models.MEbook
{

    // 註解：實作 ILabelsService 介面，處理所有與標籤相關的商業邏輯
    public class LabelsService : ILabelsService
    {
        private readonly TeamAProjectContext _context;

        public LabelsService(TeamAProjectContext context)
        {
            _context = context;
        }

        public async Task<List<Label>> GetAllLabelsAsync()
        {
            return await _context.Labels.OrderBy(l => l.LabelName).ToListAsync();
        }

        public async Task<Label?> FindLabelByIdAsync(int id)
        {
            return await _context.Labels.FindAsync(id);
        }

        public async Task<Label> CreateLabelAsync(Label newLabel)
        {
            _context.Labels.Add(newLabel);
            await _context.SaveChangesAsync();
            return newLabel;
        }

        public async Task UpdateLabelAsync(Label labelToUpdate)
        {
            _context.Update(labelToUpdate);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteLabelAsync(int id)
        {
            var label = await _context.Labels.FindAsync(id);
            if (label != null)
            {
                _context.Labels.Remove(label);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> LabelNameExistsAsync(string name, int? excludeId = null)
        {
            var query = _context.Labels.AsQueryable();
            if (excludeId.HasValue)
            {
                query = query.Where(l => l.LabelId != excludeId.Value);
            }
            return await query.AnyAsync(l => l.LabelName == name);
        }

        public async Task<bool> IsLabelInUseAsync(int id)
        {
            // 檢查是否有任何 EBookMain 實體在其 Labels 集合中包含此標籤
            return await _context.EBookMains.AnyAsync(b => b.Labels.Any(l => l.LabelId == id));
        }

        // ... 在 LabelsService 類別中，加入以下新方法 ...

        public async Task<List<EBookMain>> GetBooksByLabelIdAsync(int labelId)
        {
            // 註解：查詢所有 EBookMain，條件是其 Labels 集合中，
            // 包含任何一個 LabelId 符合傳入值的標籤。
            return await _context.EBookMains
                                 .Where(b => b.Labels.Any(l => l.LabelId == labelId))
                                 .ToListAsync();
        }
    }
}
