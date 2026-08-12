using Microsoft.EntityFrameworkCore;
using Pawsome.API.Common;
using Pawsome.API.DTOs.DonHang;
using Pawsome.Infrastructure;

namespace Pawsome.API.Services.DonHang;

public class PawPointsService : IPawPointsService
{
    private readonly PawsomeDbContext _dbContext;

    public PawPointsService(PawsomeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PawPointsBalanceDto> GetBalanceAsync(int userId)
    {
        var soDu = await _dbContext.Users
            .Where(u => u.UserId == userId)
            .Select(u => (int?)u.DiemPawpoints)
            .FirstOrDefaultAsync();

        if (soDu == null)
            throw new KeyNotFoundException("Không tìm thấy người dùng.");

        return new PawPointsBalanceDto { UserId = userId, SoDuHienTai = soDu.Value };
    }

    public async Task<PagedResult<PawPointsTransactionDto>> GetHistoryAsync(int userId, PawPointsHistoryFilterDto filter)
    {
        var query = _dbContext.PawPointsTransactions.Where(t => t.UserId == userId);

        if (!string.IsNullOrWhiteSpace(filter.Loai))
            query = query.Where(t => t.Loai == filter.Loai);

        query = query.OrderByDescending(t => t.NgayGiaoDich);

        var tongSo = await query.CountAsync();
        var trang = filter.Page < 1 ? 1 : filter.Page;
        var soDong = filter.PageSize < 1 ? 10 : Math.Min(filter.PageSize, 50);

        var items = await query
            .Skip((trang - 1) * soDong)
            .Take(soDong)
            .Select(t => new PawPointsTransactionDto
            {
                TransactionId = t.TransactionId,
                OrderId = t.OrderId,
                SoDiem = t.SoDiem,
                Loai = t.Loai,
                NgayGiaoDich = t.NgayGiaoDich
            })
            .ToListAsync();

        return new PagedResult<PawPointsTransactionDto>
        {
            Items = items,
            TotalCount = tongSo,
            PageNumber = trang,
            PageSize = soDong
        };
    }
}