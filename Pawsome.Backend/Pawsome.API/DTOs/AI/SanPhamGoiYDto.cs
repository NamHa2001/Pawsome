namespace Pawsome.API.DTOs.AI;

// Sản phẩm THẬT lấy từ ProductService.SearchAsync (Phần 2) mà AI đã tra cứu được khi trả lời -
// Frontend hiển thị thành thẻ sản phẩm có link thật, tránh trường hợp AI chỉ mô tả suông bằng
// chữ (không kiểm chứng được tên/giá có đúng với sản phẩm đang bán hay không).
public class SanPhamGoiYDto
{
    public int ProductId { get; set; }
    public string Ten { get; set; } = null!;
    public decimal? GiaTu { get; set; }
    public string? AnhChinh { get; set; }
}
