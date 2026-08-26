using Microsoft.AspNetCore.Mvc;
using Pawsome.API.Common;
using Pawsome.API.Common.Email;
using Pawsome.API.DTOs.GioHang;
using Pawsome.API.DTOs.SanPham;
using Pawsome.API.Services.GioHang;
using Pawsome.API.Services.SanPham;

namespace Pawsome.API.Controllers.SanPham;

// Endpoint cho khối "Subscribe & Get Instant 15% Discount" (welcome-bonus, trang chủ). Xác minh
// reCAPTCHA thật với Google, sau đó tạo 1 coupon 15% RIÊNG cho email đó qua ICouponService (API
// có sẵn của Phần 3) với SoLuong = 1 - dùng 1 lần là hết, tránh việc 1 mã chung bị lan truyền và
// dùng lại mà không cần nhập email thật. Mã cũng được gửi qua email thật (fire-and-forget, giống
// AuthService.SendResetOtpAsync) - vẫn trả thẳng mã trong response để frontend hiện ngay, phòng
// khi gửi mail thất bại (SMTP lỗi) không làm hỏng cả luồng subscribe.
[ApiController]
[Route("api/[controller]")]
public class SubscribeController : ControllerBase
{
    private readonly IRecaptchaService _recaptchaService;
    private readonly ICouponService _couponService;
    private readonly IEmailService _emailService;

    public SubscribeController(IRecaptchaService recaptchaService, ICouponService couponService, IEmailService emailService)
    {
        _recaptchaService = recaptchaService;
        _couponService = couponService;
        _emailService = emailService;
    }

    [HttpPost]
    public async Task<IActionResult> Subscribe([FromBody] SubscribeRequestDto dto)
    {
        var hopLe = await _recaptchaService.VerifyAsync(dto.RecaptchaToken);
        if (!hopLe)
            return BadRequest(ApiResponse<object>.Fail("reCAPTCHA verification failed. Please try again."));

        var coupon = await TaoCouponChaoMungAsync();
        GuiEmailCoupon(dto.Email, coupon.MaCode);

        return Ok(ApiResponse<object>.Ok(new { maCode = coupon.MaCode }, "Subscribed successfully!"));
    }

    private void GuiEmailCoupon(string email, string maCode)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await _emailService.SendAsync(
                    email,
                    "Your PawSome discount code",
                    $"Thank you for subscribing to PawSome!\n\nYour 15% discount code: {maCode}\n\nThis code can only be used once.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gửi email coupon: {ex.Message}");
            }
        });
    }

    private async Task<CouponDto> TaoCouponChaoMungAsync()
    {
        for (var lanThu = 0; lanThu < 5; lanThu++)
        {
            var maCode = $"WELCOME15-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";

            try
            {
                return await _couponService.TaoAsync(new CreateCouponDto
                {
                    MaCode = maCode,
                    LoaiGiam = "percent",
                    GiaTri = 15,
                    SoLuong = 1
                });
            }
            catch (InvalidOperationException)
            {
                // Trùng mã (cực hiếm) - thử sinh mã khác.
            }
        }

        throw new InvalidOperationException("Could not generate a unique coupon code. Please try again.");
    }
}
