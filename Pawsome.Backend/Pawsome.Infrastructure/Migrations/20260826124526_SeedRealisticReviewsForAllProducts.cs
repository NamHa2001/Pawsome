using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedRealisticReviewsForAllProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Thay thế cho SeedProductReviews / SeedRemainingProductReviews / DiversifyProductReviewScores:
            // 3 migration đó gán cứng user_id 11-16 (tài khoản demo tự đăng ký trên máy của AnhThu),
            // nên trên máy khác không có các user_id đó thì điều kiện EXISTS luôn false và không insert
            // được dòng nào. Migration này không gán cứng user_id nào cả - tự lấy danh sách user
            // role_id = 1 (Customer) đang có thật trong DB tại thời điểm chạy, rồi rải review vòng tròn
            // (modulo) qua các user đó. Chỉ cần DB có ít nhất 1 khách hàng là đủ 2 review/sản phẩm cho
            // toàn bộ 24 sản phẩm, điểm số có chênh lệch (3-5 sao) cho giống thật. products.diem_danh_gia_tb
            // tự cập nhật qua trigger trg_reviews_sync_avg, không cần code nào khác động vào.
            migrationBuilder.Sql(@"
DECLARE @CustCount INT = (SELECT COUNT(*) FROM users WHERE role_id = 1);

IF @CustCount > 0
BEGIN
    SET IDENTITY_INSERT reviews ON;

    ;WITH Customers AS (
        SELECT user_id, ROW_NUMBER() OVER (ORDER BY user_id) - 1 AS idx
        FROM users WHERE role_id = 1
    ),
    ReviewSeed(review_id, product_id, so_sao, binh_luan) AS (
        SELECT * FROM (VALUES
        (500, 1,  5, N'All-in-one protection and my dog actually looks forward to taking it, tastes like a treat.'),
        (501, 1,  4, N'Works well, just wish the box came with more tablets for the price.'),
        (502, 2,  4, N'Odorless like advertised, lasted almost the full 8 months before I noticed it wearing off.'),
        (503, 2,  3, N'Helped with ticks but my dog kept scratching at the collar for the first week.'),
        (504, 3,  5, N'Fast-acting, fleas were gone within a day of the first dose.'),
        (505, 3,  4, N'Does the job every month, easy to remember since it''s chewable.'),
        (506, 4,  4, N'Simple monthly routine, my dog stopped scratching within days.'),
        (507, 4,  3, N'Works but the spot gets a bit greasy for a day or two after application.'),
        (508, 5,  5, N'Started killing fleas within 30 minutes, exactly as promised on the label.'),
        (509, 5,  4, N'Great for a quick flea emergency, though it doesn''t have lasting protection.'),
        (510, 6,  4, N'My cat tolerates the application well, no more scratching from fleas.'),
        (511, 6,  5, N'Reliable monthly treatment, used it for months with zero issues.'),
        (512, 7,  5, N'Covers fleas, ear mites, and heartworm in one dose - worth the price.'),
        (513, 7,  4, N'Good broad-spectrum protection, cat didn''t mind the application at all.'),
        (514, 8,  4, N'Quick relief for a flea outbreak, worked within the hour.'),
        (515, 8,  3, N'Effective short-term but had to combine with a longer-lasting product.'),
        (516, 9,  5, N'Only need to apply once every 3 months, huge time saver.'),
        (517, 9,  4, N'Long-lasting and effective, just make sure the fur is fully dry first.'),
        (518, 10, 5, N'Noticed better mobility in my horse after a few weeks of consistent use.'),
        (519, 10, 4, N'Good natural option, coat looked noticeably healthier after a month.'),
        (520, 11, 4, N'Straightforward to dose, no resistance from the horse during application.'),
        (521, 11, 5, N'Fecal count dropped significantly after using this on schedule.'),
        (522, 12, 5, N'My budgies seem more active since I started adding this to their water.'),
        (523, 12, 4, N'Easy to mix in, no strong smell that would put the birds off.'),
        (524, 13, 3, N'Helped reduce mites but needed a couple more applications than expected.'),
        (525, 13, 4, N'Worked well on the cage bars, birds seem less irritated now.'),
        (526, 14, 4, N'Easy to apply and my dog didn''t react badly to it, works as expected for flea control.'),
        (527, 14, 5, N'Covers heartworm too, convenient to only need one product.'),
        (528, 15, 5, N'Vet recommended this specifically and it''s lived up to the hype.'),
        (529, 15, 4, N'Simple monthly chew, my dog never refuses it.'),
        (530, 16, 5, N'Long-lasting protection, only need to give it once every 3 months.'),
        (531, 16, 4, N'My dog is a picky eater but he takes this chew without any fuss.'),
        (532, 17, 5, N'Great value, lasts for months and I haven''t found a single flea since.'),
        (533, 17, 4, N'Cat got used to wearing it after a couple days, works well since.'),
        (534, 18, 4, N'Covers more parasites than the regular spot-on, worth the upgrade.'),
        (535, 18, 5, N'Cat stays flea-free for months, very convenient dosing schedule.'),
        (536, 19, 4, N'Good broad-spectrum option, cat tolerated it fine.'),
        (537, 19, 3, N'Works but application left a slightly oily patch for a day.'),
        (538, 20, 5, N'Simple and effective, exactly what our vet suggested.'),
        (539, 20, 5, N'No fleas since we started, very happy with this.'),
        (540, 21, 3, N'Worked eventually but took longer than expected to clear up.'),
        (541, 21, 4, N'Did the job, cat handled the application well.'),
        (542, 22, 4, N'Coat is visibly shinier after a month of regular use.'),
        (543, 22, 4, N'Helped calm down the dry patches on his back.'),
        (544, 23, 3, N'Some improvement but not a miracle fix for severe anxiety.'),
        (545, 23, 3, N'Mild effect, might work better combined with training.'),
        (546, 24, 5, N'Cat tolerates it much better than the spot-on version we tried before.'),
        (547, 24, 4, N'Good alternative for cats who dislike being touched on the neck.')
        ) AS v(review_id, product_id, so_sao, binh_luan)
    )
    INSERT INTO reviews (review_id, product_id, user_id, so_sao, binh_luan, trang_thai)
    SELECT rs.review_id, rs.product_id, c.user_id, rs.so_sao, rs.binh_luan, N'da_duyet'
    FROM ReviewSeed rs
    JOIN Customers c ON c.idx = (rs.review_id - 500) % @CustCount
    WHERE NOT EXISTS (SELECT 1 FROM reviews r WHERE r.review_id = rs.review_id);

    SET IDENTITY_INSERT reviews OFF;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM reviews WHERE review_id BETWEEN 500 AND 547;
");
        }
    }
}
