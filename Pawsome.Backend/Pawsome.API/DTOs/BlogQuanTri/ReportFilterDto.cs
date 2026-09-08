namespace Pawsome.API.DTOs.BlogQuanTri;

// Không truyền TuNgay/DenNgay thì ReportService tự mặc định về đầu tháng hiện tại -> hôm nay
// (giờ VN), giống hệt cách DashboardService.GetStatsAsync tính "tháng này" - để số liệu khớp
// nhau khi admin không chỉnh bộ lọc.
public class ReportDateRangeFilterDto
{
    public DateTime? TuNgay { get; set; }
    public DateTime? DenNgay { get; set; }
}

public class SanPhamReportFilterDto : ReportDateRangeFilterDto
{
    public int? CategoryId { get; set; }
    public int? BrandId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    // "SoLuongBan" (mặc định) hoặc "DoanhThu".
    public string SapXepTheo { get; set; } = "SoLuongBan";
}
