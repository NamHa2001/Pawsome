using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedRemainingProductReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Tiếp nối SeedProductReviews - thêm 2 đánh giá (đã duyệt) cho 16 sản phẩm còn lại
            // (đang 0 đánh giá) để không sản phẩm nào hiện 0 sao. Cùng cơ chế guard như migration
            // trước: chỉ chèn nếu review_id chưa tồn tại VÀ user_id thật sự có trên máy đang chạy
            // (users 11-16 là tài khoản demo cục bộ, không phải bảng seed cố định).
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 26) AND EXISTS (SELECT 1 FROM users WHERE user_id = 13)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (26, 2, 13, 5, N'Durable collar, my dog has worn it for months with no skin irritation.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 27) AND EXISTS (SELECT 1 FROM users WHERE user_id = 15)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (27, 2, 15, 4, N'Does the job, though it took a couple weeks to notice fewer ticks.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 28) AND EXISTS (SELECT 1 FROM users WHERE user_id = 14)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (28, 4, 14, 4, N'Simple monthly routine, my dog stopped scratching within days.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 29) AND EXISTS (SELECT 1 FROM users WHERE user_id = 16)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (29, 4, 16, 5, N'Consistent results every month, will keep repurchasing.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 30) AND EXISTS (SELECT 1 FROM users WHERE user_id = 11)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (30, 5, 11, 4, N'Works fast when we spot a flea between regular treatments.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 31) AND EXISTS (SELECT 1 FROM users WHERE user_id = 12)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (31, 5, 12, 3, N'Effective but wears off quicker than I expected.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 32) AND EXISTS (SELECT 1 FROM users WHERE user_id = 15)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (32, 7, 15, 5, N'Covers everything we need in one application, very convenient for a multi-cat household.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 33) AND EXISTS (SELECT 1 FROM users WHERE user_id = 13)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (33, 7, 13, 4, N'My cat handled it well, no more ear mite issues since we started.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 34) AND EXISTS (SELECT 1 FROM users WHERE user_id = 16)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (34, 8, 16, 5, N'Quick relief, saw fleas dying off within the hour like advertised.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 35) AND EXISTS (SELECT 1 FROM users WHERE user_id = 14)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (35, 8, 14, 4, N'Handy for emergencies between the monthly spot-on treatment.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 36) AND EXISTS (SELECT 1 FROM users WHERE user_id = 11)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (36, 9, 11, 5, N'Great long-lasting protection, one dose covers the whole season.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 37) AND EXISTS (SELECT 1 FROM users WHERE user_id = 12)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (37, 9, 12, 4, N'My cat didn''t mind the application at all, coat looks healthy too.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 38) AND EXISTS (SELECT 1 FROM users WHERE user_id = 15)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (38, 11, 15, 5, N'Straightforward dosing and my horse had no reaction to it.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 39) AND EXISTS (SELECT 1 FROM users WHERE user_id = 16)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (39, 11, 16, 4, N'Did the job for our regular deworming schedule.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 40) AND EXISTS (SELECT 1 FROM users WHERE user_id = 13)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (40, 13, 13, 4, N'Easy to spray and my birds didn''t seem bothered by it.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 41) AND EXISTS (SELECT 1 FROM users WHERE user_id = 14)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (41, 13, 14, 5, N'Cleared up the mite problem within a couple of applications.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 42) AND EXISTS (SELECT 1 FROM users WHERE user_id = 11)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (42, 15, 11, 5, N'My dog takes it easily every month, no more worries about heartworm.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 43) AND EXISTS (SELECT 1 FROM users WHERE user_id = 12)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (43, 15, 12, 4, N'Reliable protection, just remember to keep it on a strict monthly schedule.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 44) AND EXISTS (SELECT 1 FROM users WHERE user_id = 15)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (44, 18, 15, 5, N'Covers fleas, ticks, and heartworm in one go - exactly what we needed for our cat.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 45) AND EXISTS (SELECT 1 FROM users WHERE user_id = 16)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (45, 18, 16, 4, N'Good all-in-one option, application was quick and easy.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 46) AND EXISTS (SELECT 1 FROM users WHERE user_id = 13)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (46, 19, 13, 5, N'Broad coverage and my cat tolerates it well every month.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 47) AND EXISTS (SELECT 1 FROM users WHERE user_id = 14)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (47, 19, 14, 4, N'Works as expected, just a bit strong-smelling right after application.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 48) AND EXISTS (SELECT 1 FROM users WHERE user_id = 11)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (48, 20, 11, 4, N'Basic and effective flea treatment, does what it says.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 49) AND EXISTS (SELECT 1 FROM users WHERE user_id = 12)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (49, 20, 12, 5, N'Affordable option that''s kept our cat flea-free for months.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 50) AND EXISTS (SELECT 1 FROM users WHERE user_id = 15)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (50, 21, 15, 5, N'Cleared up the worm issue our vet flagged during the checkup.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 51) AND EXISTS (SELECT 1 FROM users WHERE user_id = 16)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (51, 21, 16, 4, N'Simple spot-on application, cat didn''t react badly at all.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 52) AND EXISTS (SELECT 1 FROM users WHERE user_id = 13)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (52, 22, 13, 5, N'Noticeable improvement in my dog''s coat shine after a few weeks.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 53) AND EXISTS (SELECT 1 FROM users WHERE user_id = 14)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (53, 22, 14, 4, N'Helped with the dry, flaky skin patches he had before.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 54) AND EXISTS (SELECT 1 FROM users WHERE user_id = 11)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (54, 23, 11, 4, N'Seemed to calm my dog down during the last thunderstorm season.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 55) AND EXISTS (SELECT 1 FROM users WHERE user_id = 12)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (55, 23, 12, 5, N'Noticed less pacing and whining when we''re away from home now.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 56) AND EXISTS (SELECT 1 FROM users WHERE user_id = 15)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (56, 24, 15, 5, N'Easy spray bottle, my cat tolerates it better than the spot-on version.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 57) AND EXISTS (SELECT 1 FROM users WHERE user_id = 16)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (57, 24, 16, 4, N'Works well and the alcohol-free formula didn''t irritate her skin.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM reviews WHERE review_id BETWEEN 26 AND 57;
");
        }
    }
}
