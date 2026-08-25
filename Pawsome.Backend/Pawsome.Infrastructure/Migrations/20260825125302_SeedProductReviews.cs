using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedProductReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Đánh giá mẫu (đã duyệt) cho 8 sản phẩm để catalog sinh động hơn - mỗi sản phẩm đúng
            // 2 đánh giá (số chẵn). Gán cho các tài khoản khách hàng demo (user_id 11-16) đã tạo
            // trước đó trên máy này qua chức năng đăng ký thật - CÁC ID NÀY CÓ THỂ KHÔNG TỒN TẠI
            // trên máy khác (users không phải bảng seed cố định, mỗi máy tự đăng ký khác nhau).
            // Vì vậy mỗi INSERT có thêm điều kiện "user đó phải thật sự tồn tại" - nếu không có,
            // dòng review tương ứng sẽ tự bỏ qua (không lỗi FK), chỉ áp dụng đủ trên máy nào đã có
            // sẵn đúng các tài khoản demo này. products.diem_danh_gia_tb tự cập nhật qua trigger
            // trg_reviews_sync_avg, không cần code nào khác động vào.
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 14) AND EXISTS (SELECT 1 FROM users WHERE user_id = 12)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (14, 14, 12, 4, N'Easy to apply and my dog didn''t react badly to it. Works as expected for flea control.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 15) AND EXISTS (SELECT 1 FROM users WHERE user_id = 16)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (15, 1, 16, 4, N'Convenient all-in-one chewable, my dog takes it like a treat every month.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 16) AND EXISTS (SELECT 1 FROM users WHERE user_id = 11)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (16, 10, 11, 5, N'Noticed better mobility in my horse after a few weeks of consistent use.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 17) AND EXISTS (SELECT 1 FROM users WHERE user_id = 15)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (17, 17, 15, 5, N'Great value, lasts for months and I haven''t found a single flea since.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 18) AND EXISTS (SELECT 1 FROM users WHERE user_id = 13)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (18, 16, 13, 5, N'Long-lasting protection, only need to give it once every 3 months.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 19) AND EXISTS (SELECT 1 FROM users WHERE user_id = 14)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (19, 16, 14, 4, N'My dog is a picky eater but he takes this chew without any fuss.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 20) AND EXISTS (SELECT 1 FROM users WHERE user_id = 15)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (20, 3, 15, 5, N'Fast-acting, fleas were gone within a day.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 21) AND EXISTS (SELECT 1 FROM users WHERE user_id = 12)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (21, 3, 12, 4, N'Works well, just a bit pricier than other brands I''ve tried.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 22) AND EXISTS (SELECT 1 FROM users WHERE user_id = 16)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (22, 6, 16, 4, N'My cat tolerates the application well, no more scratching from fleas.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 23) AND EXISTS (SELECT 1 FROM users WHERE user_id = 11)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (23, 6, 11, 5, N'Reliable monthly treatment, I''ve used it for months with no issues.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 24) AND EXISTS (SELECT 1 FROM users WHERE user_id = 13)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (24, 12, 13, 5, N'My budgies seem more active since I started adding this to their water.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 25) AND EXISTS (SELECT 1 FROM users WHERE user_id = 14)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (25, 12, 14, 4, N'Easy to mix in, no strong smell that would put the birds off.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM reviews WHERE review_id BETWEEN 14 AND 25;
");
        }
    }
}
