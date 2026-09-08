namespace Pawsome.API.DTOs.BlogQuanTri;

public class DoanhThuTheoKyDto
{
    // Dùng để sắp xếp theo thời gian tăng dần - Ky chỉ là nhãn hiển thị (chuỗi đã format).
    public DateTime TuNgayKy { get; set; }
    public string Ky { get; set; } = null!;
    public int SoDon { get; set; }
    public decimal DoanhThu { get; set; }
}

public class DonHangTheoTrangThaiDto
{
    public string TrangThai { get; set; } = null!;
    public int SoLuong { get; set; }
}

public class DoanhThuTheoDanhMucDto
{
    public int CategoryId { get; set; }
    public string TenDanhMuc { get; set; } = null!;
    public int SoLuongBan { get; set; }
    public decimal DoanhThu { get; set; }
}

public class DoanhThuTheoThuongHieuDto
{
    public int? BrandId { get; set; }
    public string TenThuongHieu { get; set; } = null!;
    public int SoLuongBan { get; set; }
    public decimal DoanhThu { get; set; }
}
