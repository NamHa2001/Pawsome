namespace Pawsome.API.Common;

// Tách riêng công thức tính tổng số trang để các Service cần kẹp "trang" về giá trị hợp lệ
// TRƯỚC khi Skip/Take (VD BlogService.SearchAsync) có thể dùng chung, không phải chép lại
// công thức - tránh trôi lệch nếu sau này cách tính đổi (VD làm tròn khác).
public static class PhanTrangHelper
{
    public static int TinhTongSoTrang(int tongSo, int soDong) =>
        soDong == 0 ? 0 : (int)Math.Ceiling(tongSo / (double)soDong);
}

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => PhanTrangHelper.TinhTongSoTrang(TotalCount, PageSize);
}
