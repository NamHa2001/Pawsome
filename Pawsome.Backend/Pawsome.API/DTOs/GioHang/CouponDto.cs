namespace Pawsome.API.DTOs.GioHang
{
    public class CouponDto
    {
        public int CouponId { get; set; }
        public string MaCode { get; set; } = null!;
        public string LoaiGiam { get; set; } = null!;
        public decimal GiaTri { get; set; }
        public DateOnly? NgayBatDau { get; set; }
        public DateOnly? NgayKetThuc { get; set; }
        public int? SoLuong { get; set; }
        public bool DangHieuLuc { get; set; }
    }
}