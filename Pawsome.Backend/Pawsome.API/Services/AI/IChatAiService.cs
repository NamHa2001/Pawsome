using Pawsome.API.DTOs.AI;

namespace Pawsome.API.Services.AI;

// Trợ lý AI tư vấn sản phẩm Pawsome theo triệu chứng/tình trạng thú cưng (widget "PawSome AI
// Assistant" dùng chung ở shared/components/chat-ai - không thuộc riêng Phần nào trong 5 Phần,
// giống Common/AuditLog, do trưởng nhóm phụ trách). Chỉ ĐỌC dữ liệu sản phẩm thật của Phần 2 qua
// IProductService/ICategoryService/IConditionService, không tự ghi gì xuống DB.
public interface IChatAiService
{
    Task<ChatResponseDto> ChatAsync(ChatRequestDto request);
}
