using Microsoft.EntityFrameworkCore;
using prjSpecialTopicMvc.Models;
using prjSpecialTopicMvc.Features.Usedbook.Application.DTOs.Results;
using prjSpecialTopicMvc.Features.Usedbook.Application.Interfaces;

namespace prjSpecialTopicMvc.Features.Usedbook.Infrastructure.Repositories
{
    public class ContentRatingRepository
    {
        private readonly TeamAProjectContext _db;

        public ContentRatingRepository(TeamAProjectContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<IHasIdName>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.ContentRatings
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new ContentRatingResult
                {
                    Id = x.Id,
                    Name = x.Name,
                })
                .ToListAsync(ct);
        } 
    }
}
