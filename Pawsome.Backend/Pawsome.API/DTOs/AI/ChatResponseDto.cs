namespace Pawsome.API.DTOs.AI;

public class ChatResponseDto
{
    public string TraLoi { get; set; } = null!;

    // Chỉ có giá trị khi AI đã gọi hàm tra cứu sản phẩm thật trong lúc trả lời (xem ChatAiService).
    public List<SanPhamGoiYDto> SanPham { get; set; } = new();
}
