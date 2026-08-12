using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Pawsome.API.DTOs.DonHang;
using Pawsome.Domain.Entities.DonHang;
using Pawsome.Infrastructure;

namespace Pawsome.API.Services.DonHang;

public class PaymentService : IPaymentService
{
    private readonly PawsomeDbContext _dbContext;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;
    private readonly IOrderService _orderService;

    public PaymentService(PawsomeDbContext dbContext, IHttpClientFactory httpClientFactory,
        IConfiguration config, IOrderService orderService)
    {
        _dbContext = dbContext;
        _httpClientFactory = httpClientFactory;
        _config = config;
        _orderService = orderService;
    }

    //MoMo

    public async Task<CreatePaymentResultDto> CreateMoMoPaymentAsync(int userId, int orderId)
    {
        var order = await LayDonHangHopLeAsync(userId, orderId);
        var momo = _config.GetSection("MoMo");
        var partnerCode = momo["PartnerCode"]!;
        var accessKey = momo["AccessKey"]!;
        var secretKey = momo["SecretKey"]!;

        var momoOrderId = $"{order.OrderId}-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
        var requestId = momoOrderId;
        var amount = ((long)order.ThanhTien).ToString();
        var orderInfo = $"Thanh toan don hang Pawsome #{order.OrderId}";
        var redirectUrl = momo["RedirectUrl"]!;
        var ipnUrl = momo["IpnUrl"]!;
        var extraData = "";
        var requestType = "payWithMethod";

        var rawSignature =
            $"accessKey={accessKey}&amount={amount}&extraData={extraData}" +
            $"&ipnUrl={ipnUrl}&orderId={momoOrderId}&orderInfo={orderInfo}" +
            $"&partnerCode={partnerCode}&redirectUrl={redirectUrl}" +
            $"&requestId={requestId}&requestType={requestType}";
        var signature = KyHmacSha256(rawSignature, secretKey);

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

        var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsync(momo["Endpoint"],
            new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json"));

        var responseJson = await response.Content.ReadFromJsonAsync<JsonElement>();
        if (!responseJson.TryGetProperty("payUrl", out var payUrlProp))
            throw new InvalidOperationException(
                $"MoMo tạo giao dịch thất bại: {responseJson.GetProperty("message").GetString()}");

        var payment = await TaoPaymentChoDonAsync(order.OrderId, "MoMo", order.ThanhTien, momoOrderId);
        return new CreatePaymentResultDto { PaymentId = payment.PaymentId, PayUrl = payUrlProp.GetString()! };
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
        var chuKyDung = KyHmacSha256(rawSignature, secretKey);

        if (chuKyDung != dto.Signature)
            throw new UnauthorizedAccessException("Chữ ký IPN MoMo không hợp lệ.");

        await XuLyKetQuaThanhToanAsync(dto.OrderId, dto.ResultCode == 0);
    }

    //VNPay

    public async Task<CreatePaymentResultDto> CreateVnPayPaymentAsync(int userId, int orderId, string clientIp)
    {
        var order = await LayDonHangHopLeAsync(userId, orderId);
        var vnpay = _config.GetSection("VNPay");
        var txnRef = $"{order.OrderId}-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        var vnpParams = new SortedList<string, string>(StringComparer.Ordinal)
        {
            ["vnp_Version"] = "2.1.0",
            ["vnp_Command"] = "pay",
            ["vnp_TmnCode"] = vnpay["TmnCode"]!,
            ["vnp_Amount"] = ((long)(order.ThanhTien * 100)).ToString(),
            ["vnp_CurrCode"] = "VND",
            ["vnp_TxnRef"] = txnRef,
            ["vnp_OrderInfo"] = $"Thanh toan don hang Pawsome {order.OrderId}",
            ["vnp_OrderType"] = "other",
            ["vnp_Locale"] = "vn",
            ["vnp_ReturnUrl"] = vnpay["ReturnUrl"]!,
            ["vnp_IpAddr"] = clientIp,
            ["vnp_CreateDate"] = DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss")
        };

        var (queryString, hashData) = BuildVnPayQuery(vnpParams);
        var secureHash = KyHmacSha512(hashData, vnpay["HashSecret"]!);
        var payUrl = $"{vnpay["PaymentUrl"]}?{queryString}&vnp_SecureHash={secureHash}";

        var payment = await TaoPaymentChoDonAsync(order.OrderId, "VNPay", order.ThanhTien, txnRef);
        return new CreatePaymentResultDto { PaymentId = payment.PaymentId, PayUrl = payUrl };
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
        var (_, hashData) = BuildVnPayQuery(vnpParams);
        var expectedHash = KyHmacSha512(hashData, vnpay["HashSecret"]!);

        if (!expectedHash.Equals(receivedHash, StringComparison.OrdinalIgnoreCase))
            return false;

        var txnRef = query.GetValueOrDefault("vnp_TxnRef");
        await XuLyKetQuaThanhToanAsync(txnRef!, query.GetValueOrDefault("vnp_ResponseCode") == "00");
        return true;
    }

    private async Task<Order> LayDonHangHopLeAsync(int userId, int orderId)
    {
        var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);
        if (order == null)
            throw new KeyNotFoundException("Không tìm thấy đơn hàng.");
        return order;
    }

    private async Task<Payment> TaoPaymentChoDonAsync(int orderId, string phuongThuc, decimal soTien, string maGiaoDich)
    {
        var payment = new Payment
        {
            OrderId = orderId,
            PhuongThuc = phuongThuc,
            SoTien = soTien,
            TrangThai = "cho_thanh_toan",
            MaGiaoDich = maGiaoDich
        };
        _dbContext.Payments.Add(payment);
        await _dbContext.SaveChangesAsync();
        return payment;
    }

    private async Task XuLyKetQuaThanhToanAsync(string maGiaoDich, bool thanhCong)
    {
        var payment = await _dbContext.Payments.FirstOrDefaultAsync(p => p.MaGiaoDich == maGiaoDich);
        if (payment == null) return; // gateway có thể gọi IPN nhiều lần, không tìm thấy thì bỏ qua

        if (thanhCong)
        {
            payment.TrangThai = "thanh_cong";
            payment.NgayThanhToan = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            await _orderService.UpdateTrangThaiAsync(payment.OrderId, "dang_xu_ly");
        }
        else
        {
            payment.TrangThai = "that_bai";
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<PaymentDto?> GetByOrderIdAsync(int userId, int orderId)
    {
        var payment = await _dbContext.Payments
            .Include(p => p.Order)
            .Where(p => p.OrderId == orderId && p.Order.UserId == userId)
            .OrderByDescending(p => p.PaymentId)
            .FirstOrDefaultAsync();

        return payment == null ? null : new PaymentDto
        {
            PaymentId = payment.PaymentId,
            OrderId = payment.OrderId,
            PhuongThuc = payment.PhuongThuc,
            SoTien = payment.SoTien,
            TrangThai = payment.TrangThai,
            MaGiaoDich = payment.MaGiaoDich,
            NgayThanhToan = payment.NgayThanhToan
        };
    }

    private static (string queryString, string hashData) BuildVnPayQuery(SortedList<string, string> vnpParams)
    {
        var query = new StringBuilder();
        var hashData = new StringBuilder();
        foreach (var kv in vnpParams)
        {
            if (string.IsNullOrEmpty(kv.Value)) continue;
            hashData.Append(WebUtility.UrlEncode(kv.Key)).Append('=').Append(WebUtility.UrlEncode(kv.Value)).Append('&');
            query.Append(WebUtility.UrlEncode(kv.Key)).Append('=').Append(WebUtility.UrlEncode(kv.Value)).Append('&');
        }
        if (hashData.Length > 0) hashData.Length--;
        if (query.Length > 0) query.Length--;
        return (query.ToString(), hashData.ToString());
    }

    private static string KyHmacSha256(string data, string key)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        return Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(data))).ToLower();
    }

    private static string KyHmacSha512(string data, string key)
    {
        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
        return Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(data))).ToLower();
    }
}