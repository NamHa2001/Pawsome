using System.Text;

namespace Pawsome.API.Common;

// Tự viết CSV thay vì thêm thư viện ngoài (ClosedXML/EPPlus) - export báo cáo (ReportController)
// chỉ cần bảng phẳng đơn giản, chưa có tiền lệ export nào trong repo để biện minh cho việc thêm
// dependency mới. Ghi kèm UTF-8 BOM vì Excel mặc định đoán ANSI khi thiếu BOM, làm tiếng Việt
// (dấu) hiển thị lỗi font khi mở file .csv.
public static class CsvExportHelper
{
    public static byte[] TaoFile(IEnumerable<string> tieuDe, IEnumerable<IEnumerable<object>> cacHang)
    {
        var sb = new StringBuilder();
        sb.AppendLine(string.Join(',', tieuDe.Select(ThoatChuoi)));
        foreach (var hang in cacHang)
            sb.AppendLine(string.Join(',', hang.Select(o => ThoatChuoi(o?.ToString() ?? string.Empty))));

        return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
    }

    private static string ThoatChuoi(string gia)
    {
        if (gia.Contains(',') || gia.Contains('"') || gia.Contains('\n'))
            return $"\"{gia.Replace("\"", "\"\"")}\"";
        return gia;
    }
}
