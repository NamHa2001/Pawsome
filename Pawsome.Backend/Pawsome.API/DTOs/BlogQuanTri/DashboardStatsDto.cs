namespace Pawsome.API.DTOs.BlogQuanTri;

public class DashboardStatsDto
{
    public decimal DoanhThuThangNay { get; set; }
    public int SoDonThangNay { get; set; }
    public int SoDanhGiaChoDuyet { get; set; }
    public List<SanPhamBanChayDto> SanPhamBanChay { get; set; } = new();
}

public class SanPhamBanChayDto
{
    public int ProductId { get; set; }
    public string Ten { get; set; } = null!;
    public int SoLuongBan { get; set; }
    public decimal DoanhThu { get; set; }
}
