using Microsoft.EntityFrameworkCore;

namespace prjSpecialTopicMvc.Models.MEbook
{
    public class EBookCategoryService : IEBookCategoryService
    {
        private readonly TeamAProjectContext _context;

        public EBookCategoryService(TeamAProjectContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EBookCategory>> GetAllCategoriesWithParentsAsync()
        {
            return await _context.EBookCategories.Include(e => e.ParentCategory).ToListAsync();
        }

        public async Task<EBookCategory?> FindCategoryByIdAsync(int id)
        {
            return await _context.EBookCategories.FindAsync(id);
        }

        public async Task<EBookCategory?> GetCategoryWithParentAsync(int id)
        {
            return await _context.EBookCategories
                .Include(e => e.ParentCategory)
                .FirstOrDefaultAsync(m => m.CategoryId == id);
        }

        public async Task<IEnumerable<EBookMain>> GetBooksByCategoryIdAsync(int id)
        {
            return await _context.EBookMains
                                 .Where(b => b.CategoryId == id)
                                 .ToListAsync();
        }

        public async Task CreateCategoryAsync(EBookCategory category)
        {
            _context.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(EBookCategory category)
        {
            _context.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _context.EBookCategories.FindAsync(id);
            if (category != null)
            {
                _context.EBookCategories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> CategoryNameExistsAsync(string name, int? excludeId = null)
        {
            var query = _context.EBookCategories.AsQueryable();
            if (excludeId.HasValue)
            {
                query = query.Where(c => c.CategoryId != excludeId.Value);
            }
            return await query.AnyAsync(c => c.CategoryName == name);
        }

        public async Task<bool> IsCategoryInUseAsync(int id)
        {
            bool isInUseByBook = await _context.EBookMains.AnyAsync(b => b.CategoryId == id);
            bool isInUseByChild = await _context.EBookCategories.AnyAsync(c => c.ParentCategoryId == id);
            return isInUseByBook || isInUseByChild;
        }
    }
}