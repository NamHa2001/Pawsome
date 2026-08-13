namespace Pawsome.API.Services.DonHang;

// orders.trang_thai và payments.trang_thai KHÔNG có CHECK constraint ở tầng DB (khác
// reviews.trang_thai/auto_orders.trang_thai) - Database.sql không ép giá trị, nên tầng
// ứng dụng phải tự thống nhất 1 bộ mã duy nhất. Trước đây OrderService.cs và
// PaymentService.cs mỗi nơi tự khai riêng hằng số/gõ chuỗi tay ("cho_thanh_toan" vs
// "cho_xu_ly"...) - gộp về đây để 2 file luôn dùng chung, tránh lệch chính tả.
public static class OrderStatus
{
    public const string ChoXuLy = "cho_xu_ly";
    public const string DangXuLy = "dang_xu_ly";
    public const string DaGiaoVan = "da_giao_van";
    public const string DaGiao = "da_giao";
    public const string DaHuy = "da_huy";
    public const string ChoTraHang = "cho_tra_hang";
    public const string DaTraHang = "da_tra_hang";

    public static readonly IReadOnlySet<string> TatCa = new HashSet<string>
    {
        ChoXuLy, DangXuLy, DaGiaoVan, DaGiao, DaHuy, ChoTraHang, DaTraHang
    };

    public static readonly IReadOnlySet<string> ChoPhepKhachHuy = new HashSet<string> { ChoXuLy, DangXuLy };
}

public static class PaymentStatus
{
    public const string ChoThanhToan = "cho_thanh_toan";
    public const string ThanhCong = "thanh_cong";
    public const string ThatBai = "that_bai";
}
