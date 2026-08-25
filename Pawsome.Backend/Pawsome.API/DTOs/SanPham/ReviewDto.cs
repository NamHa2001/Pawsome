namespace Pawsome.API.DTOs.SanPham;

public class ReviewDto
{
    public int ReviewId { get; set; }
    public int ProductId { get; set; }
    public int UserId { get; set; }
    public string TenNguoiDanhGia { get; set; } = null!;
    public byte SoSao { get; set; }
    public string? BinhLuan { get; set; }
    public string TrangThai { get; set; } = null!;
    public DateTime NgayTao { get; set; }

    // Các trường dưới chỉ set khi trả về từ GetNoiBatAsync (đánh giá tiêu biểu trang chủ) - cần
    // biết đánh giá thuộc sản phẩm nào (tên, ảnh, điểm trung bình) vì hiển thị gộp nhiều sản phẩm
    // cùng lúc, khác GetByProductAsync (đã biết sẵn productId từ tham số truyền vào nên không cần).
    public string? TenSanPham { get; set; }
    public string? AnhSanPham { get; set; }
    public decimal? DiemDanhGiaTbSanPham { get; set; }
}
