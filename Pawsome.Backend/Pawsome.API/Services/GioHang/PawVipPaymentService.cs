using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Pawsome.API.DTOs.DonHang;
using Pawsome.API.DTOs.GioHang;
using Pawsome.API.Services.DonHang;
using Pawsome.Domain.Entities.GioHang;
using Pawsome.Infrastructure;

namespace Pawsome.API.Services.GioHang
{
    // Thanh toán mua gói PawVip - tách riêng khỏi PaymentService/orders vì đây không phải mua
    // hàng vật lý (không địa chỉ giao, không sản phẩm/tồn kho). Dùng chung logic ký chữ ký MoMo/
    // VNPay qua PaymentGatewaySigner, không đụng gì tới luồng thanh toán đơn hàng hiện có.
    public class PawVipPaymentService : IPawVipPaymentService
    {
        private readonly PawsomeDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly IPawVipService _pawVipService;

        public PawVipPaymentService(PawsomeDbContext context, IHttpClientFactory httpClientFactory,
            IConfiguration config, IPawVipService pawVipService)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _config = config;
            _pawVipService = pawVipService;
        }

        public async Task<CreatePaymentResultDto> CreateMoMoPaymentAsync(int userId, string tier)
        {
            var soTien = LayGiaTheoTier(tier);
            var payment = await TaoPawVipPaymentAsync(userId, tier, soTien, "MoMo");

            var momo = _config.GetSection("MoMo");
            var partnerCode = momo["PartnerCode"]!;
            var accessKey = momo["AccessKey"]!;
            var secretKey = momo["SecretKey"]!;

            // Tiền tố "pv" để trang kết quả (pawvip-ket-qua) phân biệt được với mã giao dịch
            // của đơn hàng thường - 2 luồng redirect về 2 URL khác nhau nên không bắt buộc,
            // nhưng giữ để không lẫn lộn nếu sau này gộp trang kết quả.
            var momoOrderId = $"pv{payment.PawVipPaymentId}-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
            var requestId = momoOrderId;
            var amount = ((long)soTien).ToString();
            var orderInfo = $"Nang cap PawVip goi {tier} - Pawsome";
            var redirectUrl = momo["PawVipRedirectUrl"]!;
            var ipnUrl = momo["PawVipIpnUrl"]!;
            var extraData = "";
            var requestType = "payWithMethod";

            var rawSignature =
                $"accessKey={accessKey}&amount={amount}&extraData={extraData}" +
                $"&ipnUrl={ipnUrl}&orderId={momoOrderId}&orderInfo={orderInfo}" +
                $"&partnerCode={partnerCode}&redirectUrl={redirectUrl}" +
                $"&requestId={requestId}&requestType={requestType}";
            var signature = PaymentGatewaySigner.KyHmacSha256(rawSignature, secretKey);

            var body = new
            {
                partnerCode,
                accessKey,
                requestId,
                amount,
                orderId = momoOrderId,
                orderInfo,
                redirectUrl,
                ipnUrl,
                extraData,
                requestType,
                signature,
                lang = "vi"
            };

            // Nếu gọi MoMo lỗi (mạng, timeout, MoMo từ chối) thì dọn luôn dòng pawvip_payments
            // vừa tạo - nếu không sẽ để lại 1 dòng "cho_thanh_toan" mồ côi vĩnh viễn (không có
            // MaGiaoDich nên IPN không bao giờ tìm thấy để cập nhật).
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.PostAsync(momo["Endpoint"],
                    new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json"));

                var responseJson = await response.Content.ReadFromJsonAsync<JsonElement>();
                if (!responseJson.TryGetProperty("payUrl", out var payUrlProp))
                    throw new InvalidOperationException(
                        $"MoMo tạo giao dịch thất bại: {responseJson.GetProperty("message").GetString()}");

                payment.MaGiaoDich = momoOrderId;
                await _context.SaveChangesAsync();

                return new CreatePaymentResultDto { PaymentId = payment.PawVipPaymentId, PayUrl = payUrlProp.GetString()! };
            }
            catch
            {
                _context.PawVipPayments.Remove(payment);
                await _context.SaveChangesAsync();
                throw;
            }
        }

        public async Task HandleMoMoIpnAsync(MoMoIpnRequestDto dto)
        {
            var momo = _config.GetSection("MoMo");
            var accessKey = momo["AccessKey"]!;
            var secretKey = momo["SecretKey"]!;

            var rawSignature =
                $"accessKey={accessKey}&amount={dto.Amount}&extraData={dto.ExtraData}" +
                $"&message={dto.Message}&orderId={dto.OrderId}&orderInfo={dto.OrderInfo}" +
                $"&orderType={dto.OrderType}&partnerCode={dto.PartnerCode}&payType={dto.PayType}" +
                $"&requestId={dto.RequestId}&responseTime={dto.ResponseTime}" +
                $"&resultCode={dto.ResultCode}&transId={dto.TransId}";
            var chuKyDung = PaymentGatewaySigner.KyHmacSha256(rawSignature, secretKey);

            if (chuKyDung != dto.Signature)
                throw new UnauthorizedAccessException("Chữ ký IPN MoMo không hợp lệ.");

            await XuLyKetQuaThanhToanAsync(dto.OrderId, dto.ResultCode == 0);
        }

        public async Task<CreatePaymentResultDto> CreateVnPayPaymentAsync(int userId, string tier, string clientIp)
        {
            var soTien = LayGiaTheoTier(tier);
            var payment = await TaoPawVipPaymentAsync(userId, tier, soTien, "VNPay");

            var vnpay = _config.GetSection("VNPay");
            var txnRef = $"pv{payment.PawVipPaymentId}-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

            var vnpParams = new SortedList<string, string>(StringComparer.Ordinal)
            {
                ["vnp_Version"] = "2.1.0",
                ["vnp_Command"] = "pay",
                ["vnp_TmnCode"] = vnpay["TmnCode"]!,
                ["vnp_Amount"] = ((long)(soTien * 100)).ToString(),
                ["vnp_CurrCode"] = "VND",
                ["vnp_TxnRef"] = txnRef,
                ["vnp_OrderInfo"] = $"Nang cap PawVip goi {tier} Pawsome",
                ["vnp_OrderType"] = "other",
                ["vnp_Locale"] = "vn",
                ["vnp_ReturnUrl"] = vnpay["PawVipReturnUrl"]!,
                ["vnp_IpAddr"] = clientIp,
                ["vnp_CreateDate"] = DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss")
            };

            var (queryString, hashData) = PaymentGatewaySigner.BuildVnPayQuery(vnpParams);
            var secureHash = PaymentGatewaySigner.KyHmacSha512(hashData, vnpay["HashSecret"]!);
            var payUrl = $"{vnpay["PaymentUrl"]}?{queryString}&vnp_SecureHash={secureHash}";

            payment.MaGiaoDich = txnRef;
            await _context.SaveChangesAsync();

            return new CreatePaymentResultDto { PaymentId = payment.PawVipPaymentId, PayUrl = payUrl };
        }

        public async Task<bool> HandleVnPayIpnAsync(Dictionary<string, string> query)
        {
            if (!query.TryGetValue("vnp_SecureHash", out var receivedHash))
                return false;

            var vnpay = _config.GetSection("VNPay");
            var vnpParams = new SortedList<string, string>(StringComparer.Ordinal);
            foreach (var kv in query)
            {
                if (kv.Key is "vnp_SecureHash" or "vnp_SecureHashType") continue;
                vnpParams[kv.Key] = kv.Value;
            }
            var (_, hashData) = PaymentGatewaySigner.BuildVnPayQuery(vnpParams);
            var expectedHash = PaymentGatewaySigner.KyHmacSha512(hashData, vnpay["HashSecret"]!);

            if (!expectedHash.Equals(receivedHash, StringComparison.OrdinalIgnoreCase))
                return false;

            var txnRef = query.GetValueOrDefault("vnp_TxnRef");
            await XuLyKetQuaThanhToanAsync(txnRef!, query.GetValueOrDefault("vnp_ResponseCode") == "00");
            return true;
        }

        public async Task<PawVipPaymentStatusDto?> GetStatusAsync(int userId, int pawVipPaymentId)
        {
            var payment = await _context.PawVipPayments
                .FirstOrDefaultAsync(p => p.PawVipPaymentId == pawVipPaymentId && p.UserId == userId);

            return payment == null ? null : new PawVipPaymentStatusDto
            {
                PawVipPaymentId = payment.PawVipPaymentId,
                Tier = payment.Tier,
                TrangThai = payment.TrangThai
            };
        }

        private static decimal LayGiaTheoTier(string tier)
        {
            if (!PawVipTiers.GiaNam.TryGetValue(tier, out var gia))
                throw new InvalidOperationException("Gói PawVip không hợp lệ.");
            return gia;
        }

        private async Task<PawVipPayment> TaoPawVipPaymentAsync(int userId, string tier, decimal soTien, string phuongThuc)
        {
            // Chặn tạo giao dịch mới nếu đang có giao dịch PawVip khác CÒN MỚI (trong phiên
            // MoMo/VNPay thật, ~15 phút) chưa xử lý xong - tránh bấm nhiều lần/mở nhiều tab tạo
            // hàng loạt giao dịch trùng lặp, dễ khiến khách nhầm lẫn trả 2 lần. Không chặn vĩnh
            // viễn như kiểm tra ban đầu: nếu khách bỏ ngang 1 phiên thanh toán (đóng tab, đổi ý)
            // thì sau 15 phút giao dịch đó coi như đã "nguội", không còn tính là đang chờ nữa -
            // nếu không sẽ khóa khách khỏi việc mua PawVip vĩnh viễn chỉ vì 1 lần bỏ ngang.
            var moc15Phut = DateTime.UtcNow.AddMinutes(-15);
            var dangCho = await _context.PawVipPayments
                .AnyAsync(p => p.UserId == userId && p.TrangThai == PaymentStatus.ChoThanhToan && p.NgayTao >= moc15Phut);
            if (dangCho)
                throw new InvalidOperationException(
                    "Bạn đang có 1 giao dịch PawVip chờ thanh toán. Vui lòng hoàn tất hoặc đợi vài phút rồi thử lại.");

            var payment = new PawVipPayment
            {
                UserId = userId,
                Tier = tier,
                SoTien = soTien,
                PhuongThuc = phuongThuc,
                TrangThai = PaymentStatus.ChoThanhToan,
                NgayTao = DateTime.UtcNow
            };
            _context.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        private async Task XuLyKetQuaThanhToanAsync(string maGiaoDich, bool thanhCong)
        {
            var payment = await _context.PawVipPayments.FirstOrDefaultAsync(p => p.MaGiaoDich == maGiaoDich);
            if (payment == null) return; // gateway có thể gọi IPN nhiều lần, không tìm thấy thì bỏ qua

            // Gateway gọi IPN lặp lại cho 1 giao dịch ĐÃ xử lý xong là chuyện bình thường (MoMo/
            // VNPay tự retry nếu không nhận phản hồi kịp) - nếu không chặn ở đây, KichHoatAsync
            // sẽ chạy lại và cộng thêm 1 năm nữa miễn phí trên hạn đã gia hạn từ lần xử lý trước.
            if (payment.TrangThai != PaymentStatus.ChoThanhToan) return;

            if (thanhCong)
            {
                payment.TrangThai = PaymentStatus.ThanhCong;
                payment.NgayThanhToan = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Chỉ set user.PawVipTier ở đây - sau khi chữ ký gateway đã xác nhận thanh toán
                // thật, không phải lúc bấm nút như thiết kế cũ.
                await _pawVipService.KichHoatAsync(payment.UserId, payment.Tier);
            }
            else
            {
                payment.TrangThai = PaymentStatus.ThatBai;
                await _context.SaveChangesAsync();
            }
        }
    }
}
