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
            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "user_id", "email", "ho_ten", "ngay_cap_nhat", "ngay_tao", "password_hash", "role_id", "so_dien_thoai", "trang_thai" },
                values: new object[] { 1, "admin@pawsome.vn", "Quản trị viên", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "$2a$11$VOB3KJqGRdIg7DYWqdKumuDJRPl4ED3pUwy36ArnXBg39m9Gz8wB.", 2, null, "active" });

            migrationBuilder.InsertData(
                table: "blog_posts",
                columns: new[] { "post_id", "anh_dai_dien", "chu_de", "ngay_dang", "noi_dung", "tac_gia_id", "tieu_de" },
                values: new object[,]
                {
                    { 1, null, "Chó", new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chó con mới tách mẹ cần thời gian thích nghi với môi trường mới. Hãy chuẩn bị chỗ ngủ ấm áp, lên lịch tiêm phòng đầy đủ, cho ăn đúng cữ với thức ăn dành riêng cho chó con, và tránh cho ra ngoài tiếp xúc với chó lạ trước khi hoàn thành các mũi tiêm cơ bản.", 1, "5 lưu ý khi chăm sóc chó con mới về nhà" },
                    { 2, null, "Mèo", new DateTime(2026, 1, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mèo trưởng thành cần khẩu phần giàu đạm động vật, hạn chế tinh bột. Nên chia 2-3 bữa/ngày, luôn có nước sạch, và tham khảo ý kiến bác sĩ thú y trước khi đổi loại thức ăn đột ngột để tránh rối loạn tiêu hóa.", 1, "Chế độ dinh dưỡng cho mèo trưởng thành" },
                    { 3, null, "Sức khỏe", new DateTime(2026, 1, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Thời tiết giao mùa là lúc thú cưng dễ mắc các bệnh về đường hô hấp và tiêu hóa. Chủ nuôi nên giữ ấm, vệ sinh nơi ở sạch sẽ, tiêm phòng định kỳ và quan sát các dấu hiệu bất thường như bỏ ăn, nôn mửa để đưa đi khám kịp thời.", 1, "Cách phòng bệnh thường gặp ở thú cưng theo mùa" }
                });
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
