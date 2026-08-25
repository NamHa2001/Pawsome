using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DiversifyProductReviewScores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Gần như mọi sản phẩm trước đó chỉ có cặp điểm (4,5) -> điểm TB đồng loạt 4.5, nhìn
            // giả. Thêm 2 đánh giá/sản phẩm với điểm số đa dạng (2-5 sao) cho 22/24 sản phẩm (trừ
            // 2 sản phẩm đã lệch điểm sẵn) để điểm trung bình trải đều 3.5-5.0, giống thật hơn.
            // Cùng cơ chế guard như 2 migration trước: chỉ chèn nếu review_id chưa tồn tại VÀ
            // user_id thật sự có trên máy đang chạy (users 11-16 là tài khoản demo cục bộ).
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 58) AND EXISTS (SELECT 1 FROM users WHERE user_id = 11)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (58, 1, 11, 3, N'Decent but my dog seemed a little drowsy the first day after taking it.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 59) AND EXISTS (SELECT 1 FROM users WHERE user_id = 12)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (59, 1, 12, 3, N'Works okay, wouldn''t say it''s dramatically better than cheaper options.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 60) AND EXISTS (SELECT 1 FROM users WHERE user_id = 14)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (60, 2, 14, 3, N'Fine for the price, though the buckle feels a bit flimsy.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 61) AND EXISTS (SELECT 1 FROM users WHERE user_id = 15)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (61, 2, 15, 4, N'Held up well through several baths, still smells fresh.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 62) AND EXISTS (SELECT 1 FROM users WHERE user_id = 13)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (62, 3, 13, 5, N'Best flea chew we''ve tried, our dog stopped scratching almost overnight.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 63) AND EXISTS (SELECT 1 FROM users WHERE user_id = 16)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (63, 3, 16, 5, N'Vet recommended this specifically and it''s lived up to the hype.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 64) AND EXISTS (SELECT 1 FROM users WHERE user_id = 11)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (64, 4, 11, 2, N'Didn''t notice much difference, still found a few fleas after two applications.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 65) AND EXISTS (SELECT 1 FROM users WHERE user_id = 12)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (65, 4, 12, 4, N'Works fine once you get the application technique right.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 66) AND EXISTS (SELECT 1 FROM users WHERE user_id = 13)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (66, 6, 13, 4, N'Solid monthly treatment, easy to apply without much fuss.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 67) AND EXISTS (SELECT 1 FROM users WHERE user_id = 14)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (67, 6, 14, 4, N'No complaints, does what it''s supposed to.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 68) AND EXISTS (SELECT 1 FROM users WHERE user_id = 15)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (68, 7, 15, 3, N'Good coverage but the smell lingers on my cat''s fur for a day or two.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 69) AND EXISTS (SELECT 1 FROM users WHERE user_id = 16)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (69, 7, 16, 5, N'Covers everything in one dose, exactly what a multi-cat household needs.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 70) AND EXISTS (SELECT 1 FROM users WHERE user_id = 11)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (70, 8, 11, 2, N'Didn''t work as fast as advertised for my cat.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 71) AND EXISTS (SELECT 1 FROM users WHERE user_id = 12)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (71, 8, 12, 3, N'Helped a little but I had to combine it with another treatment.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 72) AND EXISTS (SELECT 1 FROM users WHERE user_id = 13)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (72, 9, 13, 5, N'Long-lasting and my cat barely notices the application.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 73) AND EXISTS (SELECT 1 FROM users WHERE user_id = 14)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (73, 9, 14, 4, N'Reliable protection through the warmer months.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 74) AND EXISTS (SELECT 1 FROM users WHERE user_id = 15)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (74, 11, 15, 3, N'Did the job but the taste seems off-putting, had to mix it with feed.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 75) AND EXISTS (SELECT 1 FROM users WHERE user_id = 16)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (75, 11, 16, 3, N'Average results, nothing to complain about but nothing amazing either.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 76) AND EXISTS (SELECT 1 FROM users WHERE user_id = 11)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (76, 12, 11, 5, N'My birds are noticeably more energetic since we started this.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 77) AND EXISTS (SELECT 1 FROM users WHERE user_id = 12)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (77, 12, 12, 5, N'Great supplement, easy to mix and no bad smell.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 78) AND EXISTS (SELECT 1 FROM users WHERE user_id = 13)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (78, 13, 13, 4, N'Cleared up the mites within a week of regular use.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 79) AND EXISTS (SELECT 1 FROM users WHERE user_id = 14)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (79, 13, 14, 3, N'Works but you need to be consistent with weekly applications.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 80) AND EXISTS (SELECT 1 FROM users WHERE user_id = 15)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (80, 14, 15, 5, N'Fantastic results, no more fleas and his coat looks shinier too.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 81) AND EXISTS (SELECT 1 FROM users WHERE user_id = 16)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (81, 14, 16, 5, N'Highly recommend, easiest monthly treatment we''ve used.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 82) AND EXISTS (SELECT 1 FROM users WHERE user_id = 11)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (82, 15, 11, 2, N'My dog vomited once after taking it, had to switch products.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 83) AND EXISTS (SELECT 1 FROM users WHERE user_id = 12)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (83, 15, 12, 5, N'No issues at all, heartworm test came back clean at the vet visit.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 84) AND EXISTS (SELECT 1 FROM users WHERE user_id = 13)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (84, 16, 13, 3, N'Works but the effect seems to wear off a bit before the 3-month mark.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 85) AND EXISTS (SELECT 1 FROM users WHERE user_id = 14)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (85, 16, 14, 4, N'Convenient dosing schedule, my dog takes it without a fight.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 86) AND EXISTS (SELECT 1 FROM users WHERE user_id = 15)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (86, 17, 15, 4, N'Good value, my cat''s fur still looks and smells clean.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 87) AND EXISTS (SELECT 1 FROM users WHERE user_id = 16)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (87, 17, 16, 4, N'Effective and low-maintenance, exactly what I wanted.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 88) AND EXISTS (SELECT 1 FROM users WHERE user_id = 11)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (88, 18, 11, 5, N'Covers fleas, ticks, and heartworm - simplified our whole routine.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 89) AND EXISTS (SELECT 1 FROM users WHERE user_id = 12)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (89, 18, 12, 3, N'Works but pricier than I expected for what it offers.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 90) AND EXISTS (SELECT 1 FROM users WHERE user_id = 13)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (90, 19, 13, 2, N'Didn''t seem very effective against fleas specifically.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 91) AND EXISTS (SELECT 1 FROM users WHERE user_id = 14)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (91, 19, 14, 4, N'Good broad-spectrum option, cat tolerated it fine.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 92) AND EXISTS (SELECT 1 FROM users WHERE user_id = 15)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (92, 20, 15, 5, N'Simple and effective, exactly what our vet suggested.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 93) AND EXISTS (SELECT 1 FROM users WHERE user_id = 16)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (93, 20, 16, 5, N'No fleas since we started, very happy with this.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 94) AND EXISTS (SELECT 1 FROM users WHERE user_id = 11)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (94, 21, 11, 3, N'Worked eventually but took longer than expected to clear up.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 95) AND EXISTS (SELECT 1 FROM users WHERE user_id = 12)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (95, 21, 12, 4, N'Did the job, cat handled the application well.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 96) AND EXISTS (SELECT 1 FROM users WHERE user_id = 13)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (96, 22, 13, 4, N'Coat is visibly shinier after a month of regular use.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 97) AND EXISTS (SELECT 1 FROM users WHERE user_id = 14)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (97, 22, 14, 4, N'Helped calm down the dry patches on his back.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 98) AND EXISTS (SELECT 1 FROM users WHERE user_id = 15)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (98, 23, 15, 3, N'Some improvement but not a miracle fix for severe anxiety.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 99) AND EXISTS (SELECT 1 FROM users WHERE user_id = 16)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (99, 23, 16, 3, N'Mild effect, might work better combined with training.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 100) AND EXISTS (SELECT 1 FROM users WHERE user_id = 11)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (100, 24, 11, 5, N'Cat tolerates it much better than the spot-on version we tried before.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END

IF NOT EXISTS (SELECT 1 FROM reviews WHERE review_id = 101) AND EXISTS (SELECT 1 FROM users WHERE user_id = 12)
BEGIN
    SET IDENTITY_INSERT reviews ON;
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai) VALUES
        (101, 24, 12, 4, N'Good alternative for cats who dislike being touched on the neck.', N'da_duyet');
    SET IDENTITY_INSERT reviews OFF;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM reviews WHERE review_id BETWEEN 58 AND 101;
");
        }
    }
}
