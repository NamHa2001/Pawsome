using System.ComponentModel.DataAnnotations;

namespace Pawsome.API.DTOs.GioHang
{
    public class ApplyCouponDto
    {
        [Required(ErrorMessage = "Vui lòng nhập mã giảm giá")]
        public string MaCode { get; set; } = null!;
    }
}