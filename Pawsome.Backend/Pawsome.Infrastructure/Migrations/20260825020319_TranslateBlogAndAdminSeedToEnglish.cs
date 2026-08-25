using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TranslateBlogAndAdminSeedToEnglish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Khu Admin đã chuyển hẳn sang tiếng Anh (migration "SeedAdminUserAndBlogPosts" seed
            // bằng tiếng Việt) - dịch lại đúng nghĩa nội dung 3 bài blog mẫu, tên chủ đề, và họ tên
            // tài khoản Admin sang tiếng Anh cho khớp giao diện. Guard theo đúng giá trị tiếng Việt
            // gốc (không chỉ theo id) để không ghi đè nếu ai đó đã tự sửa nội dung qua trang Blog
            // Management trước khi migration này chạy.
            migrationBuilder.Sql(@"
UPDATE users SET ho_ten = N'Administrator'
WHERE user_id = 1 AND ho_ten = N'Quản trị viên';

UPDATE blog_posts SET
    tieu_de = N'5 Tips for Caring for a New Puppy',
    chu_de = N'Dogs',
    noi_dung = N'A puppy freshly separated from its mother needs time to adjust to a new environment. Prepare a warm sleeping area, keep up with a full vaccination schedule, feed it puppy-specific food at regular times, and avoid contact with unfamiliar dogs outdoors until the core vaccinations are complete.'
WHERE post_id = 1 AND tieu_de = N'5 lưu ý khi chăm sóc chó con mới về nhà';

UPDATE blog_posts SET
    tieu_de = N'Nutrition Guide for Adult Cats',
    chu_de = N'Cats',
    noi_dung = N'Adult cats need a diet rich in animal protein with limited carbohydrates. Feed them 2-3 meals a day, always provide clean water, and consult a veterinarian before switching food abruptly to avoid digestive upset.'
WHERE post_id = 2 AND tieu_de = N'Chế độ dinh dưỡng cho mèo trưởng thành';

UPDATE blog_posts SET
    tieu_de = N'How to Prevent Common Seasonal Illnesses in Pets',
    chu_de = N'Health',
    noi_dung = N'Seasonal transitions make pets more prone to respiratory and digestive illnesses. Owners should keep pets warm, keep their living space clean, maintain a regular vaccination schedule, and watch for unusual signs such as loss of appetite or vomiting so they can be taken to the vet promptly.'
WHERE post_id = 3 AND tieu_de = N'Cách phòng bệnh thường gặp ở thú cưng theo mùa';
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE users SET ho_ten = N'Quản trị viên'
WHERE user_id = 1 AND ho_ten = N'Administrator';

UPDATE blog_posts SET
    tieu_de = N'5 lưu ý khi chăm sóc chó con mới về nhà',
    chu_de = N'Chó',
    noi_dung = N'Chó con mới tách mẹ cần thời gian thích nghi với môi trường mới. Hãy chuẩn bị chỗ ngủ ấm áp, lên lịch tiêm phòng đầy đủ, cho ăn đúng cữ với thức ăn dành riêng cho chó con, và tránh cho ra ngoài tiếp xúc với chó lạ trước khi hoàn thành các mũi tiêm cơ bản.'
WHERE post_id = 1 AND tieu_de = N'5 Tips for Caring for a New Puppy';

UPDATE blog_posts SET
    tieu_de = N'Chế độ dinh dưỡng cho mèo trưởng thành',
    chu_de = N'Mèo',
    noi_dung = N'Mèo trưởng thành cần khẩu phần giàu đạm động vật, hạn chế tinh bột. Nên chia 2-3 bữa/ngày, luôn có nước sạch, và tham khảo ý kiến bác sĩ thú y trước khi đổi loại thức ăn đột ngột để tránh rối loạn tiêu hóa.'
WHERE post_id = 2 AND tieu_de = N'Nutrition Guide for Adult Cats';

UPDATE blog_posts SET
    tieu_de = N'Cách phòng bệnh thường gặp ở thú cưng theo mùa',
    chu_de = N'Sức khỏe',
    noi_dung = N'Thời tiết giao mùa là lúc thú cưng dễ mắc các bệnh về đường hô hấp và tiêu hóa. Chủ nuôi nên giữ ấm, vệ sinh nơi ở sạch sẽ, tiêm phòng định kỳ và quan sát các dấu hiệu bất thường như bỏ ăn, nôn mửa để đưa đi khám kịp thời.'
WHERE post_id = 3 AND tieu_de = N'How to Prevent Common Seasonal Illnesses in Pets';
");
        }
    }
}
