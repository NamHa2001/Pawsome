using Microsoft.EntityFrameworkCore;
using Pawsome.API.DTOs.BlogQuanTri;
using Pawsome.API.Services.DonHang;
using Pawsome.Infrastructure;

namespace Pawsome.API.Services.BlogQuanTri;

public class DashboardService : IDashboardService
{
    // Chuỗi trạng thái review đã duyệt/chờ duyệt không có hằng số dùng chung kiểu
    // OrderStatus.cs (ReviewService.cs vẫn gõ tay "cho_duyet"/"da_duyet") - khai const
    // cục bộ ở đây để không phải gõ lặp chuỗi thô nhiều chỗ trong chính file này, nhưng
    // không tự thêm hằng số dùng chung vào Services/SanPham (thuộc Phần 2).
    private const string TrangThaiChoDuyet = "cho_duyet";

    // Đơn "không hợp lệ" cho thống kê = đã hủy hoặc đã hoàn trả. Dùng HashSet.Contains
    // (EF Core dịch được sang SQL "NOT IN (...)", giống cách OrderStatus.TatCa đang được
    // dùng ở nơi khác) thay vì gọi 1 local function trong Where - local function không
    // dịch được sang SQL, Where sẽ ném lỗi runtime "could not be translated".
    private static readonly HashSet<string> TrangThaiKhongTinh = new()
    {
        OrderStatus.DaHuy, OrderStatus.DaTraHang
    };

    private readonly PawsomeDbContext _dbContext;

    public DashboardService(PawsomeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DashboardStatsDto> GetStatsAsync()
    {
        // Việt Nam dùng UTC+7 quanh năm (không có giờ mùa hè) nên cộng/trừ cố định 7 giờ là
        // đủ, không cần TimeZoneInfo. NgayDat lưu bằng DateTime.UtcNow (OrderService.cs) nên
        // phải quy đổi y hệt - nếu so thẳng theo UTC, ranh giới đầu/cuối tháng sẽ lệch 7
        // tiếng so với "tháng này" thực tế của admin xem dashboard tại VN.
        //
        // Tính sẵn khoảng [dauThangUtc, ketThucThangUtc) của "tháng này theo giờ VN", quy đổi
        // ngược về UTC, rồi so trực tiếp NgayDat >= / < (không bọc hàm quanh cột trong Where)
        // - viết kiểu "NgayDat.AddHours(7).Year == ... && .Month == ..." vẫn đúng kết quả
        // nhưng bọc hàm lên cột khiến SQL Server không dùng được index trên NgayDat (nếu sau
        // này có), buộc phải quét toàn bảng mỗi lần load dashboard.
        var nowVn = DateTime.UtcNow.AddHours(7);
        var dauThangVn = new DateTime(nowVn.Year, nowVn.Month, 1);
        var dauThangUtc = dauThangVn.AddHours(-7);
        var ketThucThangUtc = dauThangVn.AddMonths(1).AddHours(-7);

        // 3 query dưới đây chạy tuần tự (không dùng Task.WhenAll) vì cùng chia sẻ 1
        // PawsomeDbContext - DbContext không an toàn để 2 thao tác async chạy đồng thời
        // trên cùng 1 instance (sẽ ném InvalidOperationException lúc chạy thật).

        // Gộp số đơn + doanh thu tháng này vào 1 query duy nhất (thay vì CountAsync rồi
        // SumAsync riêng), lọc đơn hợp lệ ngay trong outer Where thay vì lặp lại điều kiện
        // trong cả Count và Sum - trước đây "New Orders" đếm cả đơn đã hủy còn "Monthly
        // Revenue" thì không, khiến 2 con số đầu trang lệch tiêu chí lọc với nhau. Loại
        // trừ cả DaTraHang: trả hàng được duyệt (DuyetTraHangAsync trong OrderService.cs)
        // chuyển trạng thái sang DaTraHang nhưng KHÔNG chỉnh lại ThanhTien - nếu chỉ loại
        // trừ DaHuy như trước, đơn đã hoàn trả vẫn bị tính vào doanh thu/số đơn/sản phẩm
        // bán chạy. Đơn đang chờ duyệt trả hàng (ChoTraHang) vẫn tính vì chưa có quyết định
        // cuối cùng. DoanhThu = Order.ThanhTien (đã gồm phí ship, đã trừ giảm giá) - đây là
        // số tiền thực thu, khác với "DoanhThu" trong sanPhamBanChay bên dưới (xem ghi chú).
        var thongKeDon = await _dbContext.Orders
            .Where(o => o.NgayDat >= dauThangUtc && o.NgayDat < ketThucThangUtc
                && !TrangThaiKhongTinh.Contains(o.TrangThai))
            .GroupBy(_ => 1)
            .Select(g => new { SoDon = g.Count(), DoanhThu = g.Sum(o => o.ThanhTien) })
            .FirstOrDefaultAsync();

        // Cùng khung thời gian "tháng này" và cùng điều kiện đơn hợp lệ với thongKeDon ở
        // trên - trước đây bảng bán chạy xếp hạng trên toàn bộ lịch sử đơn hàng trong khi 2
        // thẻ Doanh thu/Đơn hàng chỉ tính tháng này, khiến 1 sản phẩm bán tốt từ lâu vẫn
        // đứng đầu dù tháng này không bán được gì - lệch với phần còn lại của trang, đồng
        // thời quét toàn bộ OrderItems vô thời hạn. SanPhamBanChayDto.DoanhThu = tổng
        // SoLuong*DonGia theo TỪNG sản phẩm - phí ship và giảm giá coupon nằm ở cấp đơn
        // hàng, không có cách phân bổ hợp lý về từng dòng sản phẩm, nên số này KHÔNG cộng
        // dồn khớp với DoanhThuThangNay ở trên (đã trừ giảm giá + cộng ship toàn đơn) - đây
        // là 2 con số có ý nghĩa khác nhau (doanh thu thực thu toàn đơn vs. giá trị bán ra
        // theo sản phẩm), không phải sai số.
        var sanPhamBanChay = await _dbContext.OrderItems
            .Where(oi => oi.Order.NgayDat >= dauThangUtc && oi.Order.NgayDat < ketThucThangUtc
                && !TrangThaiKhongTinh.Contains(oi.Order.TrangThai))
            .GroupBy(oi => new { oi.Variant.ProductId, oi.Variant.Product.Ten })
            .Select(g => new SanPhamBanChayDto
            {
                ProductId = g.Key.ProductId,
                Ten = g.Key.Ten,
                SoLuongBan = g.Sum(oi => oi.SoLuong),
                DoanhThu = g.Sum(oi => oi.SoLuong * oi.DonGia)
            })
            .OrderByDescending(sp => sp.SoLuongBan)
            .Take(10)
            .ToListAsync();

        // Đếm trực tiếp trên bảng reviews thay vì gọi IReviewService.GetChoDuyetAsync()
        // (hàm đó load + map cả danh sách đầy đủ chỉ để lấy .Count) - DashboardController
        // được phép đọc trực tiếp (Pawsome_KhungDuAn.md mục 2 ghi chú (*): "chỉ đọc, không
        // ghi").
        var soDanhGiaChoDuyet = await _dbContext.Reviews.CountAsync(r => r.TrangThai == TrangThaiChoDuyet);

        return new DashboardStatsDto
        {
            DoanhThuThangNay = thongKeDon?.DoanhThu ?? 0m,
            SoDonThangNay = thongKeDon?.SoDon ?? 0,
            SoDanhGiaChoDuyet = soDanhGiaChoDuyet,
            SanPhamBanChay = sanPhamBanChay
        };
    }
}
