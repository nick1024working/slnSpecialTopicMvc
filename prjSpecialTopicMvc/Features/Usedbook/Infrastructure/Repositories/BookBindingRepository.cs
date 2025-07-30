using Microsoft.EntityFrameworkCore;
using prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Results;
using prjSpecialTopicMvc.Features.Usedbook.Application.Interfaces;
using prjSpecialTopicMvc.Models;

namespace prjSpecialTopicMvc.Features.Usedbook.Infrastructure.Repositories
{
    public class BookBindingRepository
    {
        private readonly TeamAProjectContext _db;

        public BookBindingRepository(TeamAProjectContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<IHasIdName>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.BookBindings
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new BookBindingResult
                {
                    Id = x.Id,
                    Name = x.Name,
                })
                .ToListAsync(ct);
        } 
    }
}
