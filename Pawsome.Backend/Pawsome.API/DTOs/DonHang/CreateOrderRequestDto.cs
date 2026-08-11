using System.ComponentModel.DataAnnotations;

namespace Pawsome.API.DTOs.DonHang;

public class CreateOrderRequestDto
{
    [Required(ErrorMessage = "Vui lòng chọn địa chỉ giao hàng")]
    public int AddressId { get; set; }

    public int? CouponId { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn đơn vị vận chuyển")]
    public string DonViVanChuyen { get; set; } = null!; // GHN / GHTK / ViettelPost
}