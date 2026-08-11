using System.ComponentModel.DataAnnotations;

namespace Pawsome.API.DTOs.GioHang
{
    public class AddCartItemDto
    {
        [Required]
        public int VariantId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int SoLuong { get; set; } = 1;
    }
}