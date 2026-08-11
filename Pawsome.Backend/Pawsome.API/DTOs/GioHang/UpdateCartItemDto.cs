using System.ComponentModel.DataAnnotations;

namespace Pawsome.API.DTOs.GioHang
{
    public class UpdateCartItemDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int SoLuong { get; set; }
    }
}