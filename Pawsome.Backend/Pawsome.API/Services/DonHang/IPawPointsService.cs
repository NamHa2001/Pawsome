using Pawsome.API.Common;
using Pawsome.API.DTOs.DonHang;

namespace Pawsome.API.Services.DonHang;
public interface IPawPointsService
{
    Task<PawPointsBalanceDto> GetBalanceAsync(int userId);
    Task<PagedResult<PawPointsTransactionDto>> GetHistoryAsync(int userId, PawPointsHistoryFilterDto filter);
}