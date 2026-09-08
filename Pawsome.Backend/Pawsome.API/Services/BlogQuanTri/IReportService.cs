using Pawsome.API.Common;
using Pawsome.API.DTOs.BlogQuanTri;

namespace Pawsome.API.Services.BlogQuanTri;

public interface IReportService
{
    Task<List<DoanhThuTheoKyDto>> LayDoanhThuTheoKyAsync(ReportDateRangeFilterDto filter, string nhom);
    Task<List<DonHangTheoTrangThaiDto>> LayDonHangTheoTrangThaiAsync(ReportDateRangeFilterDto filter);
    Task<PagedResult<SanPhamBanChayDto>> LaySanPhamAsync(SanPhamReportFilterDto filter);
    Task<List<SanPhamBanChayDto>> LayTatCaSanPhamAsync(SanPhamReportFilterDto filter);
    Task<List<DoanhThuTheoDanhMucDto>> LayDoanhThuTheoDanhMucAsync(ReportDateRangeFilterDto filter);
    Task<List<DoanhThuTheoThuongHieuDto>> LayDoanhThuTheoThuongHieuAsync(ReportDateRangeFilterDto filter);
}
