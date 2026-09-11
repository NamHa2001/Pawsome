namespace Pawsome.API.Services.GioHang;

// Xử lý "đặt hàng tự động" theo đúng YC-6.1 (tự tạo đơn khi tới hạn) và YC-6.3 (nhắc nhở
// trước khi xử lý) - trước đây AutoOrder chỉ lưu ngày kế tiếp mà không có job nào thực sự chạy
// tới hạn (phát hiện khi audit code vs SRS ngày 2026-09-11, xem
// KiemThuPhanMem/DoiChieu_Phan3_GioHang.md). Hệ thống không lưu thẻ/token thanh toán nào nên
// không thể tự động trừ tiền qua MoMo/VNPay (cần khách tương tác trực tiếp trên cổng thanh
// toán) - "tự động" ở đây dừng ở mức: tự tạo đơn hàng thật (trừ tồn kho, cộng PawPoints, dùng
// địa chỉ mặc định của khách) ở trạng thái chờ xử lý như đơn thường, rồi gửi email yêu cầu
// khách vào hoàn tất thanh toán. Quyết định phạm vi này đã được trưởng nhóm xác nhận.
public class AutoOrderProcessingService : BackgroundService
{
    private static readonly TimeSpan ChuKyChay = TimeSpan.FromHours(24);
    private const int SoNgayNhacTruoc = 3;

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AutoOrderProcessingService> _logger;

    public AutoOrderProcessingService(IServiceScopeFactory scopeFactory, ILogger<AutoOrderProcessingService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var autoOrderService = scope.ServiceProvider.GetRequiredService<IAutoOrderService>();

                var soDonTao = await autoOrderService.XuLyDonDenHanAsync();
                var soNhacNho = await autoOrderService.GuiNhacNhoTruocHanAsync(SoNgayNhacTruoc);

                _logger.LogInformation(
                    "AutoOrderProcessingService: đã tạo {SoDonTao} đơn tới hạn, gửi {SoNhacNho} email nhắc nhở.",
                    soDonTao, soNhacNho);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AutoOrderProcessingService: lỗi khi xử lý đặt hàng tự động.");
            }

            try
            {
                await Task.Delay(ChuKyChay, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // Ứng dụng đang dừng - thoát vòng lặp bình thường.
            }
        }
    }
}
