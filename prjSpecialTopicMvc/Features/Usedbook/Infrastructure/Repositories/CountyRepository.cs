using Microsoft.EntityFrameworkCore;
using prjSpecialTopicMvc.Models;
using prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Results;
using prjSpecialTopicMvc.Features.Usedbook.Application.Interfaces;

namespace prjSpecialTopicMvc.Features.Usedbook.Infrastructure.Repositories
{
    public class CountyRepository
    {
        private readonly TeamAProjectContext _db;

        public CountyRepository(TeamAProjectContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<IHasIdName>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.Counties
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new CountyResult
                {
                    Id = x.Id,
                    Name = x.Name,
                })
                .ToListAsync(ct);
        } 
    }
}
