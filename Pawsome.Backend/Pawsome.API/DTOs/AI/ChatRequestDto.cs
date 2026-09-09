namespace Pawsome.API.DTOs.AI;

public class ChatRequestDto
{
    public string TinNhanMoi { get; set; } = null!;

    // Không lưu lịch sử ở server (chưa có bảng riêng cho việc này) - Frontend tự giữ trong phiên
    // làm việc và gửi kèm lại mỗi lần hỏi tiếp, để AI hiểu được ngữ cảnh của các câu hỏi trước.
    public List<ChatMessageDto>? LichSu { get; set; }
}
