using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pawsome.API.Common;
using Pawsome.API.DTOs.BlogQuanTri;
using Pawsome.API.Services.BlogQuanTri;

namespace Pawsome.API.Controllers.BlogQuanTri;

// Báo cáo thống kê chi tiết (doanh thu/đơn hàng/sản phẩm theo khoảng ngày tự chọn), khác trang
// tổng quan cố định "tháng này" của DashboardController. Chỉ đọc, không ghi (xem
// Pawsome_KhungDuAn.md mục 2, ghi chú (*)).
[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin")]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("doanh-thu")]
    public async Task<IActionResult> LayDoanhThu([FromQuery] ReportDateRangeFilterDto filter, [FromQuery] string nhom = "Ngay")
    {
        var result = await _reportService.LayDoanhThuTheoKyAsync(filter, nhom);
        return Ok(ApiResponse<List<DoanhThuTheoKyDto>>.Ok(result));
    }

    [HttpGet("don-hang-theo-trang-thai")]
    public async Task<IActionResult> LayDonHangTheoTrangThai([FromQuery] ReportDateRangeFilterDto filter)
    {
        var result = await _reportService.LayDonHangTheoTrangThaiAsync(filter);
        return Ok(ApiResponse<List<DonHangTheoTrangThaiDto>>.Ok(result));
    }

    [HttpGet("san-pham")]
    public async Task<IActionResult> LaySanPham([FromQuery] SanPhamReportFilterDto filter)
    {
        var result = await _reportService.LaySanPhamAsync(filter);
        return Ok(ApiResponse<PagedResult<SanPhamBanChayDto>>.Ok(result));
    }

    [HttpGet("danh-muc")]
    public async Task<IActionResult> LayDanhMuc([FromQuery] ReportDateRangeFilterDto filter)
    {
        var result = await _reportService.LayDoanhThuTheoDanhMucAsync(filter);
        return Ok(ApiResponse<List<DoanhThuTheoDanhMucDto>>.Ok(result));
    }

    [HttpGet("thuong-hieu")]
    public async Task<IActionResult> LayThuongHieu([FromQuery] ReportDateRangeFilterDto filter)
    {
        var result = await _reportService.LayDoanhThuTheoThuongHieuAsync(filter);
        return Ok(ApiResponse<List<DoanhThuTheoThuongHieuDto>>.Ok(result));
    }

    // filter kiểu SanPhamReportFilterDto (superset của ReportDateRangeFilterDto) để 1 action
    // phục vụ được cả 5 loại xuất mà không cần bind lại query string nhiều lần.
    [HttpGet("xuat-csv")]
    public async Task<IActionResult> XuatCsv([FromQuery] string loai, [FromQuery] SanPhamReportFilterDto filter, [FromQuery] string nhom = "Ngay")
    {
        byte[] noiDung;
        string tenFile;

        switch (loai)
        {
            case "doanh-thu":
                var doanhThu = await _reportService.LayDoanhThuTheoKyAsync(filter, nhom);
                noiDung = CsvExportHelper.TaoFile(
                    new[] { "Ky", "SoDon", "DoanhThu" },
                    doanhThu.Select(d => new object[] { d.Ky, d.SoDon, d.DoanhThu }));
                tenFile = "bao-cao-doanh-thu.csv";
                break;

            case "don-hang":
                var donHang = await _reportService.LayDonHangTheoTrangThaiAsync(filter);
                noiDung = CsvExportHelper.TaoFile(
                    new[] { "TrangThai", "SoLuong" },
                    donHang.Select(d => new object[] { d.TrangThai, d.SoLuong }));
                tenFile = "bao-cao-don-hang-theo-trang-thai.csv";
                break;

            case "san-pham":
                var sanPham = await _reportService.LayTatCaSanPhamAsync(filter);
                noiDung = CsvExportHelper.TaoFile(
                    new[] { "SanPham", "SoLuongBan", "DoanhThu" },
                    sanPham.Select(d => new object[] { d.Ten, d.SoLuongBan, d.DoanhThu }));
                tenFile = "bao-cao-san-pham.csv";
                break;

            case "danh-muc":
                var danhMuc = await _reportService.LayDoanhThuTheoDanhMucAsync(filter);
                noiDung = CsvExportHelper.TaoFile(
                    new[] { "DanhMuc", "SoLuongBan", "DoanhThu" },
                    danhMuc.Select(d => new object[] { d.TenDanhMuc, d.SoLuongBan, d.DoanhThu }));
                tenFile = "bao-cao-danh-muc.csv";
                break;

            case "thuong-hieu":
                var thuongHieu = await _reportService.LayDoanhThuTheoThuongHieuAsync(filter);
                noiDung = CsvExportHelper.TaoFile(
                    new[] { "ThuongHieu", "SoLuongBan", "DoanhThu" },
                    thuongHieu.Select(d => new object[] { d.TenThuongHieu, d.SoLuongBan, d.DoanhThu }));
                tenFile = "bao-cao-thuong-hieu.csv";
                break;

            default:
                return BadRequest(ApiResponse<string>.Fail("Loại báo cáo không hợp lệ."));
        }

        return File(noiDung, "text/csv", tenFile);
    }
}
