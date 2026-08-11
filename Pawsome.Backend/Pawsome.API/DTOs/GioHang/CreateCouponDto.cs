using System.ComponentModel.DataAnnotations;

namespace Pawsome.API.DTOs.GioHang
{
    public class CreateCouponDto
    {
        [Required, MaxLength(50)]
        public string MaCode { get; set; } = null!;

        [Required, RegularExpression("^(percent|fixed)$", ErrorMessage = "Loại giảm phải là 'percent' hoặc 'fixed'")]
        public string LoaiGiam { get; set; } = null!;

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá trị giảm phải lớn hơn 0")]
        public decimal GiaTri { get; set; }

        public DateOnly? NgayBatDau { get; set; }
        public DateOnly? NgayKetThuc { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải >= 0")]
        public int? SoLuong { get; set; }
    }
}