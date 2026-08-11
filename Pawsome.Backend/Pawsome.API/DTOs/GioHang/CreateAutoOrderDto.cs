using System.ComponentModel.DataAnnotations;

namespace Pawsome.API.DTOs.GioHang
{
    public class CreateAutoOrderDto
    {
        [Required]
        public int VariantId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int SoLuong { get; set; }

        [Required, RegularExpression("^(weekly|monthly|quarterly|yearly)$",
            ErrorMessage = "Tần suất phải là weekly, monthly, quarterly hoặc yearly")]
        public string TanSuat { get; set; } = null!;
    }
}