using Pawsome.API.DTOs.BlogQuanTri;

namespace Pawsome.API.Services.BlogQuanTri;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetStatsAsync();
}
