using System.ComponentModel.DataAnnotations;

namespace Pawsome.API.DTOs.SanPham;

public class ReviewRequestDto
{
    [Required(ErrorMessage = "Sản phẩm không được để trống")]
    public int ProductId { get; set; }

    [Range(1, 5, ErrorMessage = "Số sao phải từ 1 đến 5")]
    public byte SoSao { get; set; }

    public string? BinhLuan { get; set; }

    // Bắt buộc chấm cả 3 mục - đánh giá gửi từ nay về sau đều có dữ liệu thật, không còn hiện số
    // giả cứng ở giao diện như trước.
    [Range(1, 5, ErrorMessage = "Điểm chất lượng sản phẩm phải từ 1 đến 5")]
    public byte DiemChatLuong { get; set; }

    [Range(1, 5, ErrorMessage = "Điểm giá trị sản phẩm phải từ 1 đến 5")]
    public byte DiemGiaTri { get; set; }

    [Range(1, 5, ErrorMessage = "Điểm hài lòng của thú cưng phải từ 1 đến 5")]
    public byte DiemHaiLongThuCung { get; set; }
}
