using Microsoft.EntityFrameworkCore;
using Pawsome.API.Common;
using Pawsome.API.DTOs.BlogQuanTri;
using Pawsome.API.Services.DonHang;
using Pawsome.Infrastructure;

namespace Pawsome.API.Services.BlogQuanTri;

// Báo cáo thống kê chi tiết (doanh thu/đơn hàng/sản phẩm theo khoảng ngày tự chọn). Đọc thẳng
// PawsomeDbContext giống DashboardService - chỉ đọc, không ghi (xem Pawsome_KhungDuAn.md mục 2,
// ghi chú (*)).
public class ReportService : IReportService
{
    // Cùng bộ trạng thái "không hợp lệ" và cùng lý do như DashboardService.cs: ChoXuLy = chưa
    // thanh toán xong (chưa có tiền thật), DaHuy/DaTraHang = đơn không còn giá trị. Không dùng
    // lại được HashSet của DashboardService (khác class) nên khai lại y hệt ở đây - 2 nơi phải
    // luôn đồng bộ nếu quy tắc đổi.
    private static readonly HashSet<string> TrangThaiKhongTinh = new()
    {
        OrderStatus.ChoXuLy, OrderStatus.DaHuy, OrderStatus.DaTraHang
    };

    private readonly PawsomeDbContext _dbContext;

    public ReportService(PawsomeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Quy đổi TuNgay/DenNgay (ngày lịch theo giờ VN, không có time) thành khoảng UTC nửa-mở
    // [TuUtc, DenUtc) để so trực tiếp với Order.NgayDat (lưu UTC) - giống cách DashboardService
    // tính "tháng này", tránh bọc hàm quanh cột trong Where khiến mất khả năng dùng index.
    // Không truyền thì mặc định đầu tháng hiện tại -> hôm nay.
    private static (DateTime TuUtc, DateTime DenUtc) TinhKhoangNgayUtc(DateTime? tuNgay, DateTime? denNgay)
    {
        var nowVn = DateTime.UtcNow.AddHours(7);
        var tuVn = (tuNgay ?? new DateTime(nowVn.Year, nowVn.Month, 1)).Date;
        var denVn = (denNgay ?? nowVn).Date.AddDays(1); // biên trên loại trừ = đầu ngày kế tiếp
        if (denVn <= tuVn) denVn = tuVn.AddDays(1); // đảm bảo khoảng luôn hợp lệ (tối thiểu 1 ngày)

        return (tuVn.AddHours(-7), denVn.AddHours(-7));
    }

    public async Task<List<DoanhThuTheoKyDto>> LayDoanhThuTheoKyAsync(ReportDateRangeFilterDto filter, string nhom)
    {
        var (tuUtc, denUtc) = TinhKhoangNgayUtc(filter.TuNgay, filter.DenNgay);

        // Lọc trong SQL theo khoảng ngày (index-friendly), fetch tối thiểu 2 cột cần thiết rồi
        // gom nhóm theo ngày/tháng giờ VN ở phía C# - GroupBy theo NgayDat.AddHours(7).Date
        // ngay trong LINQ-to-Entities không dịch được sang SQL (ném lỗi runtime), khác với so
        // sánh biên trên/dưới (chỉ cần dịch 1 lần, không bọc cột).
        var donHang = await _dbContext.Orders
            .Where(o => o.NgayDat >= tuUtc && o.NgayDat < denUtc && !TrangThaiKhongTinh.Contains(o.TrangThai))
            .Select(o => new { o.NgayDat, o.ThanhTien })
            .ToListAsync();

        var theoNhom = nhom == "Thang"
            ? donHang.GroupBy(o => new DateTime(o.NgayDat.AddHours(7).Year, o.NgayDat.AddHours(7).Month, 1))
            : donHang.GroupBy(o => o.NgayDat.AddHours(7).Date);

        return theoNhom
            .Select(g => new DoanhThuTheoKyDto
            {
                TuNgayKy = g.Key,
                Ky = nhom == "Thang" ? g.Key.ToString("MM/yyyy") : g.Key.ToString("dd/MM/yyyy"),
                SoDon = g.Count(),
                DoanhThu = g.Sum(o => o.ThanhTien)
            })
            .OrderBy(dto => dto.TuNgayKy)
            .ToList();
    }

    public async Task<List<DonHangTheoTrangThaiDto>> LayDonHangTheoTrangThaiAsync(ReportDateRangeFilterDto filter)
    {
        var (tuUtc, denUtc) = TinhKhoangNgayUtc(filter.TuNgay, filter.DenNgay);

        // Không loại trừ TrangThaiKhongTinh ở đây - mục đích của báo cáo này chính là cho admin
        // thấy cả số đơn đã hủy/trả hàng trong khoảng thời gian, khác với báo cáo doanh thu.
        var demTheoTrangThai = await _dbContext.Orders
            .Where(o => o.NgayDat >= tuUtc && o.NgayDat < denUtc)
            .GroupBy(o => o.TrangThai)
            .Select(g => new { TrangThai = g.Key, SoLuong = g.Count() })
            .ToDictionaryAsync(x => x.TrangThai, x => x.SoLuong);

        // Trả đủ 7 trạng thái kể cả 0 đơn, để giao diện luôn vẽ đủ hàng thay vì thiếu dòng khi
        // 1 trạng thái không phát sinh đơn nào trong khoảng ngày đã chọn.
        return OrderStatus.TatCa
            .Select(trangThai => new DonHangTheoTrangThaiDto
            {
                TrangThai = trangThai,
                SoLuong = demTheoTrangThai.GetValueOrDefault(trangThai, 0)
            })
            .ToList();
    }

    private IQueryable<SanPhamBanChayDto> TruyVanSanPham(SanPhamReportFilterDto filter)
    {
        var (tuUtc, denUtc) = TinhKhoangNgayUtc(filter.TuNgay, filter.DenNgay);

        var query = _dbContext.OrderItems
            .Where(oi => oi.Order.NgayDat >= tuUtc && oi.Order.NgayDat < denUtc
                && !TrangThaiKhongTinh.Contains(oi.Order.TrangThai));

        if (filter.CategoryId.HasValue)
            query = query.Where(oi => oi.Variant.Product.CategoryId == filter.CategoryId.Value);
        if (filter.BrandId.HasValue)
            query = query.Where(oi => oi.Variant.Product.BrandId == filter.BrandId.Value);

        var nhomTheoSanPham = query
            .GroupBy(oi => new { oi.Variant.ProductId, oi.Variant.Product.Ten })
            .Select(g => new SanPhamBanChayDto
            {
                ProductId = g.Key.ProductId,
                Ten = g.Key.Ten,
                SoLuongBan = g.Sum(oi => oi.SoLuong),
                DoanhThu = g.Sum(oi => oi.SoLuong * oi.DonGia)
            });

        return filter.SapXepTheo == "DoanhThu"
            ? nhomTheoSanPham.OrderByDescending(sp => sp.DoanhThu)
            : nhomTheoSanPham.OrderByDescending(sp => sp.SoLuongBan);
    }

    public async Task<PagedResult<SanPhamBanChayDto>> LaySanPhamAsync(SanPhamReportFilterDto filter)
    {
        var query = TruyVanSanPham(filter);

        var tongSo = await query.CountAsync();
        var trang = filter.Page < 1 ? 1 : filter.Page;
        var soDong = filter.PageSize < 1 ? 10 : Math.Min(filter.PageSize, 50);
        var items = await query.Skip((trang - 1) * soDong).Take(soDong).ToListAsync();

        return new PagedResult<SanPhamBanChayDto>
        {
            Items = items,
            TotalCount = tongSo,
            PageNumber = trang,
            PageSize = soDong
        };
    }

    // Dùng riêng cho xuất CSV - lấy toàn bộ danh sách, không phân trang (khác LaySanPhamAsync
    // vốn cap PageSize tối đa 50 cho hiển thị trên trang).
    public Task<List<SanPhamBanChayDto>> LayTatCaSanPhamAsync(SanPhamReportFilterDto filter) =>
        TruyVanSanPham(filter).ToListAsync();

    public async Task<List<DoanhThuTheoDanhMucDto>> LayDoanhThuTheoDanhMucAsync(ReportDateRangeFilterDto filter)
    {
        var (tuUtc, denUtc) = TinhKhoangNgayUtc(filter.TuNgay, filter.DenNgay);

        return await _dbContext.OrderItems
            .Where(oi => oi.Order.NgayDat >= tuUtc && oi.Order.NgayDat < denUtc
                && !TrangThaiKhongTinh.Contains(oi.Order.TrangThai))
            .GroupBy(oi => new { oi.Variant.Product.CategoryId, oi.Variant.Product.Category.TenDanhMuc })
            .Select(g => new DoanhThuTheoDanhMucDto
            {
                CategoryId = g.Key.CategoryId,
                TenDanhMuc = g.Key.TenDanhMuc,
                SoLuongBan = g.Sum(oi => oi.SoLuong),
                DoanhThu = g.Sum(oi => oi.SoLuong * oi.DonGia)
            })
            .OrderByDescending(d => d.DoanhThu)
            .ToListAsync();
    }

    public async Task<List<DoanhThuTheoThuongHieuDto>> LayDoanhThuTheoThuongHieuAsync(ReportDateRangeFilterDto filter)
    {
        var (tuUtc, denUtc) = TinhKhoangNgayUtc(filter.TuNgay, filter.DenNgay);

        // BrandId/Brand.TenThuongHieu có thể null (Product.BrandId là FK optional) - gộp nhóm
        // các sản phẩm không gắn thương hiệu vào 1 dòng "Không thương hiệu" thay vì loại bỏ.
        return await _dbContext.OrderItems
            .Where(oi => oi.Order.NgayDat >= tuUtc && oi.Order.NgayDat < denUtc
                && !TrangThaiKhongTinh.Contains(oi.Order.TrangThai))
            .GroupBy(oi => new
            {
                oi.Variant.Product.BrandId,
                TenThuongHieu = oi.Variant.Product.Brand != null ? oi.Variant.Product.Brand.TenThuongHieu : null
            })
            .Select(g => new DoanhThuTheoThuongHieuDto
            {
                BrandId = g.Key.BrandId,
                TenThuongHieu = g.Key.TenThuongHieu ?? "Không thương hiệu",
                SoLuongBan = g.Sum(oi => oi.SoLuong),
                DoanhThu = g.Sum(oi => oi.SoLuong * oi.DonGia)
            })
            .OrderByDescending(d => d.DoanhThu)
            .ToListAsync();
    }
}
