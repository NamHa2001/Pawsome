using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pawsome.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminUserAndBlogPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Bọc IF NOT EXISTS để tránh vi phạm khóa chính nếu máy nào đó đã có sẵn
            // user_id=1 hoặc post_id 1-3 (ví dụ tự đăng ký tài khoản test trước khi pull
            // migration này về) - cùng pattern đã dùng ở SeedSanPhamData, vá lại đúng sự
            // cố PK violation từng gặp phải với migration này khi chưa có guard.
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM users WHERE user_id = 1)
BEGIN
    SET IDENTITY_INSERT users ON;
    INSERT INTO users (user_id, email, ho_ten, ngay_cap_nhat, ngay_tao, password_hash, role_id, so_dien_thoai, trang_thai)
    VALUES (1, N'admin@pawsome.vn', N'Quản trị viên', '20260101', '20260101',
            N'$2a$11$VOB3KJqGRdIg7DYWqdKumuDJRPl4ED3pUwy36ArnXBg39m9Gz8wB.', 2, NULL, N'active');
    SET IDENTITY_INSERT users OFF;
END

IF NOT EXISTS (SELECT 1 FROM blog_posts WHERE post_id IN (1, 2, 3))
BEGIN
    SET IDENTITY_INSERT blog_posts ON;
    INSERT INTO blog_posts (post_id, anh_dai_dien, chu_de, ngay_dang, noi_dung, tac_gia_id, tieu_de) VALUES
        (1, NULL, N'Chó', '20260102', N'Chó con mới tách mẹ cần thời gian thích nghi với môi trường mới. Hãy chuẩn bị chỗ ngủ ấm áp, lên lịch tiêm phòng đầy đủ, cho ăn đúng cữ với thức ăn dành riêng cho chó con, và tránh cho ra ngoài tiếp xúc với chó lạ trước khi hoàn thành các mũi tiêm cơ bản.', 1, N'5 lưu ý khi chăm sóc chó con mới về nhà'),
        (2, NULL, N'Mèo', '20260103', N'Mèo trưởng thành cần khẩu phần giàu đạm động vật, hạn chế tinh bột. Nên chia 2-3 bữa/ngày, luôn có nước sạch, và tham khảo ý kiến bác sĩ thú y trước khi đổi loại thức ăn đột ngột để tránh rối loạn tiêu hóa.', 1, N'Chế độ dinh dưỡng cho mèo trưởng thành'),
        (3, NULL, N'Sức khỏe', '20260104', N'Thời tiết giao mùa là lúc thú cưng dễ mắc các bệnh về đường hô hấp và tiêu hóa. Chủ nuôi nên giữ ấm, vệ sinh nơi ở sạch sẽ, tiêm phòng định kỳ và quan sát các dấu hiệu bất thường như bỏ ăn, nôn mửa để đưa đi khám kịp thời.', 1, N'Cách phòng bệnh thường gặp ở thú cưng theo mùa');
    SET IDENTITY_INSERT blog_posts OFF;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "blog_posts",
                keyColumn: "post_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "blog_posts",
                keyColumn: "post_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "blog_posts",
                keyColumn: "post_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "user_id",
                keyValue: 1);
        }
    }
}
