using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Pawsome.API.Services.DonHang
{
    // Trích từ PaymentService.cs - logic ký chữ ký/build query MoMo & VNPay không đổi theo
    // luồng đơn hàng hay PawVip, nên tách dùng chung giữa PaymentService (đơn hàng) và
    // PawVipPaymentService (gói PawVip) thay vì chép lại 2 nơi - tránh 1 nơi vá bảo mật mà
    // nơi kia quên.
    public static class PaymentGatewaySigner
    {
        public static (string queryString, string hashData) BuildVnPayQuery(SortedList<string, string> vnpParams)
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

        public static string KyHmacSha256(string data, string key)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            return Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(data))).ToLower();
        }

        public static string KyHmacSha512(string data, string key)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            return Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(data))).ToLower();
        }
    }
}
