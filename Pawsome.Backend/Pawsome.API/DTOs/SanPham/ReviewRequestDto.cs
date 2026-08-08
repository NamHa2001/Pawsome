using System.ComponentModel.DataAnnotations;

namespace Pawsome.API.DTOs.SanPham;

public class ReviewRequestDto
{
    [Required(ErrorMessage = "Sản phẩm không được để trống")]
    public int ProductId { get; set; }

    [Range(1, 5, ErrorMessage = "Số sao phải từ 1 đến 5")]
    public byte SoSao { get; set; }

    public string? BinhLuan { get; set; }
}
