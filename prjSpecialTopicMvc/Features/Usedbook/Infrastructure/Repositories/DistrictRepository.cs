using Microsoft.EntityFrameworkCore;
using prjSpecialTopicMvc.Models;
using prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Results;

namespace prjSpecialTopicMvc.Features.Usedbook.Infrastructure.Repositories
{
    public class DistrictRepository
    {
        private readonly TeamAProjectContext _db;

        public DistrictRepository(TeamAProjectContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<DistrictResult>> GetByCountyIdAsync(int countyId, CancellationToken ct = default)
        {
            return await _db.Districts
                .AsNoTracking()
                .OrderBy(d => d.Id)
                .Where(d => d.CountyId == countyId)
                .Select(d => new DistrictResult
                {
                    Id = d.Id,
                    Name = d.Name,
                })
                .ToListAsync(ct);
        } 
    }
}
