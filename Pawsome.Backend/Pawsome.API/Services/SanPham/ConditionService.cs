using Microsoft.EntityFrameworkCore;
using Pawsome.API.DTOs.SanPham;
using Pawsome.Infrastructure;

namespace Pawsome.API.Services.SanPham;

public class ConditionService : IConditionService
{
    private readonly PawsomeDbContext _dbContext;

    public ConditionService(PawsomeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ConditionDto>> GetAllAsync()
    {
        return await _dbContext.Conditions
            .OrderBy(c => c.ConditionId)
            .Select(c => new ConditionDto
            {
                ConditionId = c.ConditionId,
                TenTinhTrang = c.TenTinhTrang
            })
            .ToListAsync();
    }
}
