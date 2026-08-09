using System.ComponentModel.DataAnnotations;

namespace Pawsome.API.DTOs.BlogQuanTri;

public class WishlistRequestDto
{
    [Required(ErrorMessage = "Sản phẩm không được để trống")]
    public int ProductId { get; set; }
}
