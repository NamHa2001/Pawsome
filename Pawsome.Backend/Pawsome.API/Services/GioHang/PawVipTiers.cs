namespace Pawsome.API.Services.GioHang
{
    // Hằng số dùng chung giữa PawVipService (kích hoạt) và OrderService (áp giảm giá lúc chốt
    // đơn) - 3 mức % khớp đúng UI đã có sẵn (Basic/Advanced/VIP), không phải mức 30% đồng nhất
    // trong kế hoạch ban đầu.
    public static class PawVipTiers
    {
        public static readonly IReadOnlyDictionary<string, decimal> PhanTramGiam = new Dictionary<string, decimal>
        {
            ["thuong"] = 0.05m,
            ["nang-cao"] = 0.10m,
            ["vip"] = 0.20m
        };

        // Giá thuê bao/năm - khớp đúng DANH_SACH_GOI_PAWVIP ở pawvip.ts (frontend)
        public static readonly IReadOnlyDictionary<string, decimal> GiaNam = new Dictionary<string, decimal>
        {
            ["thuong"] = 594000m,
            ["nang-cao"] = 1188000m,
            ["vip"] = 2376000m
        };

        // Gói theo NĂM (giá hiển thị "/year") - phải hết hạn thật, không phải mua 1 lần dùng
        // vĩnh viễn. Dùng chung ở OrderService (áp giảm giá) và UserService (trả về profile)
        // để tránh mỗi nơi tự viết lại điều kiện kiểm tra hạn.
        public static bool ConHieuLuc(string? tier, DateOnly? hetHan, DateOnly homNay)
            => tier != null && hetHan.HasValue && hetHan.Value >= homNay;
    }
}
