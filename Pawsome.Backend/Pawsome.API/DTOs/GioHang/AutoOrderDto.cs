namespace Pawsome.API.DTOs.GioHang
{
    public class AutoOrderDto
    {
        public int AutoOrderId { get; set; }
        public int VariantId { get; set; }
        public string TenSanPham { get; set; } = null!;
        public string? HinhAnh { get; set; }
        public int SoLuong { get; set; }
        public string TanSuat { get; set; } = null!;
        public DateOnly NgayKeTiep { get; set; }
        public string TrangThai { get; set; } = null!;
    }
}