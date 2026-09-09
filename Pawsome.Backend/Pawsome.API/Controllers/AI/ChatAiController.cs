using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Pawsome.API.Common;
using Pawsome.API.DTOs.AI;
using Pawsome.API.Services.AI;

namespace Pawsome.API.Controllers.AI;

// Công khai, không yêu cầu đăng nhập - widget chat hiển thị ở cả trang khách chưa đăng nhập
// (trang chủ, danh sách sản phẩm...), giống các Controller đọc công khai khác (Products, Conditions).
// [EnableRateLimiting]: mỗi request có thể gọi paid API Gemini tới 4 lần (xem ChatAiService) - giới
// hạn theo IP để tránh bị spam gây phát sinh chi phí (policy "ChatAi" khai báo ở Program.cs).
[ApiController]
[Route("api/ai")]
[EnableRateLimiting("ChatAi")]
public class ChatAiController : ControllerBase
{
    private readonly IChatAiService _chatAiService;

    public ChatAiController(IChatAiService chatAiService)
    {
        _chatAiService = chatAiService;
    }

    [HttpPost("chat")]
    public async Task<IActionResult> Chat([FromBody] ChatRequestDto request)
    {
        try
        {
            var ketQua = await _chatAiService.ChatAsync(request);
            return Ok(ApiResponse<ChatResponseDto>.Ok(ketQua));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<ChatResponseDto>.Fail(ex.Message));
        }
    }
}
