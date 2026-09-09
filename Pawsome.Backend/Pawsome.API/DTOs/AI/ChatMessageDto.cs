namespace Pawsome.API.DTOs.AI;

// 1 lượt trong lịch sử hội thoại mà Frontend gửi kèm mỗi request, để backend biết ngữ cảnh
// trước đó (bản thân request tới Gemini là stateless, không tự nhớ lịch sử giữa các lần gọi).
public class ChatMessageDto
{
    // "nguoi_dung" hoặc "bot" - Frontend tự gắn khi hiển thị, không dùng thẳng role "user"/"model"
    // của Gemini để không rò rỉ chi tiết nhà cung cấp AI ra khỏi tầng DTO.
    public string Vai { get; set; } = null!;
    public string NoiDung { get; set; } = null!;
}
