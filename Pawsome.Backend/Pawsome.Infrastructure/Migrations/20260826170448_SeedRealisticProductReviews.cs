using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedRealisticProductReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 3 migration review trước (SeedProductReviews, SeedRemainingProductReviews,
            // DiversifyProductReviewScores) chỉ chạy được khi máy đã có sẵn đúng user_id 11-16 (tài
            // khoản demo tự đăng ký trên máy tác giả) - trên máy sạch/máy khác không có các user đó
            // thì toàn bộ INSERT bị bỏ qua, review = 0 dòng dù migration "chạy thành công". Migration
            // này seed lại ĐẦY ĐỦ toàn bộ đánh giá thật (đã duyệt + vài cái từ chối để test kiểm
            // duyệt), resolve product qua tên (ten) và user qua email - nếu email đó không tồn tại
            // trên máy đang chạy thì tự rơi về user đầu tiên có sẵn (luôn có admin), không phụ thuộc
            // ID hay tài khoản demo cụ thể nào. Guard theo (product, nội dung bình luận) để chạy lại
            // nhiều lần không bị trùng. diem_danh_gia_tb tự cập nhật qua trigger
            // trg_reviews_sync_avg, không cần code nào khác động vào.
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Revolution for Dogs') AND binh_luan = N'My dog loves this, no more fleas after just one use!')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Revolution for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'admin@pawsome.vn'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'My dog loves this, no more fleas after just one use!', N'tu_choi');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Simparica Trio for Dogs') AND binh_luan = N'Kills fleas and ticks fast, plus my dog stays worm-free all season.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Simparica Trio for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'admin@pawsome.vn'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'Kills fleas and ticks fast, plus my dog stays worm-free all season.', N'tu_choi');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Frontline Plus for Cats') AND binh_luan = N'Works well for fleas but I wish the applicator was easier to use.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Frontline Plus for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'admin@pawsome.vn'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'Works well for fleas but I wish the applicator was easier to use.', N'tu_choi');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Dorwest Horse Herbal Supplement') AND binh_luan = N'My horse''s joints seem much more comfortable since we started this.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Dorwest Horse Herbal Supplement'), ISNULL((SELECT user_id FROM users WHERE email = N'admin@pawsome.vn'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'My horse''s joints seem much more comfortable since we started this.', N'tu_choi');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Seresto Collar for Cats') AND binh_luan = N'No more ticks and the collar barely smells, my cat doesn''t mind wearing it.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Seresto Collar for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'admin@pawsome.vn'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'No more ticks and the collar barely smells, my cat doesn''t mind wearing it.', N'tu_choi');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Revolution for Dogs') AND binh_luan = N'My dog loves this, no more fleas after just one use!')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Revolution for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'rose.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'My dog loves this, no more fleas after just one use!', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Simparica Trio for Dogs') AND binh_luan = N'Kills fleas and ticks fast, plus my dog stays worm-free all season.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Simparica Trio for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'mark.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'Kills fleas and ticks fast, plus my dog stays worm-free all season.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Dorwest Horse Herbal Supplement') AND binh_luan = N'My horse''s joints seem much more comfortable since we started this.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Dorwest Horse Herbal Supplement'), ISNULL((SELECT user_id FROM users WHERE email = N'emma.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'My horse''s joints seem much more comfortable since we started this.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Seresto Collar for Cats') AND binh_luan = N'No more ticks and the collar barely smells, my cat doesn''t mind wearing it.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Seresto Collar for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'john.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'No more ticks and the collar barely smells, my cat doesn''t mind wearing it.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Revolution for Dogs') AND binh_luan = N'Easy to apply and my dog didn''t react badly to it. Works as expected for flea control.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Revolution for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'anh632thu@gmail.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'Easy to apply and my dog didn''t react badly to it. Works as expected for flea control.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Simparica Trio for Dogs') AND binh_luan = N'Convenient all-in-one chewable, my dog takes it like a treat every month.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Simparica Trio for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'john.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'Convenient all-in-one chewable, my dog takes it like a treat every month.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Dorwest Horse Herbal Supplement') AND binh_luan = N'Noticed better mobility in my horse after a few weeks of consistent use.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Dorwest Horse Herbal Supplement'), ISNULL((SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'Noticed better mobility in my horse after a few weeks of consistent use.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Seresto Collar for Cats') AND binh_luan = N'Great value, lasts for months and I haven''t found a single flea since.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Seresto Collar for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'emma.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'Great value, lasts for months and I haven''t found a single flea since.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Bravecto Chews for Dogs') AND binh_luan = N'Long-lasting protection, only need to give it once every 3 months.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Bravecto Chews for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'rose.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'Long-lasting protection, only need to give it once every 3 months.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Bravecto Chews for Dogs') AND binh_luan = N'My dog is a picky eater but he takes this chew without any fuss.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Bravecto Chews for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'mark.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'My dog is a picky eater but he takes this chew without any fuss.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'NexGard Chewables for Dogs') AND binh_luan = N'Fast-acting, fleas were gone within a day.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'NexGard Chewables for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'emma.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'Fast-acting, fleas were gone within a day.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'NexGard Chewables for Dogs') AND binh_luan = N'Works well, just a bit pricier than other brands I''ve tried.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'NexGard Chewables for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'anh632thu@gmail.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'Works well, just a bit pricier than other brands I''ve tried.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Frontline Plus for Cats') AND binh_luan = N'My cat tolerates the application well, no more scratching from fleas.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Frontline Plus for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'john.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'My cat tolerates the application well, no more scratching from fleas.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Frontline Plus for Cats') AND binh_luan = N'Reliable monthly treatment, I''ve used it for months with no issues.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Frontline Plus for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'Reliable monthly treatment, I''ve used it for months with no issues.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Milpro Bird Care Supplement') AND binh_luan = N'My budgies seem more active since I started adding this to their water.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Milpro Bird Care Supplement'), ISNULL((SELECT user_id FROM users WHERE email = N'rose.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'My budgies seem more active since I started adding this to their water.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Milpro Bird Care Supplement') AND binh_luan = N'Easy to mix in, no strong smell that would put the birds off.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Milpro Bird Care Supplement'), ISNULL((SELECT user_id FROM users WHERE email = N'mark.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'Easy to mix in, no strong smell that would put the birds off.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Seresto Collar for Dogs') AND binh_luan = N'Durable collar, my dog has worn it for months with no skin irritation.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Seresto Collar for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'rose.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'Durable collar, my dog has worn it for months with no skin irritation.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Seresto Collar for Dogs') AND binh_luan = N'Does the job, though it took a couple weeks to notice fewer ticks.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Seresto Collar for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'emma.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'Does the job, though it took a couple weeks to notice fewer ticks.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Frontline Flea & Tick Prevention for Dogs') AND binh_luan = N'Simple monthly routine, my dog stopped scratching within days.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Frontline Flea & Tick Prevention for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'mark.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'Simple monthly routine, my dog stopped scratching within days.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Frontline Flea & Tick Prevention for Dogs') AND binh_luan = N'Consistent results every month, will keep repurchasing.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Frontline Flea & Tick Prevention for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'john.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'Consistent results every month, will keep repurchasing.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Capstar Oral Flea Treatment for Dogs') AND binh_luan = N'Works fast when we spot a flea between regular treatments.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Capstar Oral Flea Treatment for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'Works fast when we spot a flea between regular treatments.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Capstar Oral Flea Treatment for Dogs') AND binh_luan = N'Effective but wears off quicker than I expected.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Capstar Oral Flea Treatment for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'anh632thu@gmail.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 3, N'Effective but wears off quicker than I expected.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Revolution Plus for Cats') AND binh_luan = N'Covers everything we need in one application, very convenient for a multi-cat household.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Revolution Plus for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'emma.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'Covers everything we need in one application, very convenient for a multi-cat household.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Revolution Plus for Cats') AND binh_luan = N'My cat handled it well, no more ear mite issues since we started.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Revolution Plus for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'rose.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'My cat handled it well, no more ear mite issues since we started.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Capstar Tablets for Cats') AND binh_luan = N'Quick relief, saw fleas dying off within the hour like advertised.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Capstar Tablets for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'john.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'Quick relief, saw fleas dying off within the hour like advertised.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Capstar Tablets for Cats') AND binh_luan = N'Handy for emergencies between the monthly spot-on treatment.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Capstar Tablets for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'mark.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'Handy for emergencies between the monthly spot-on treatment.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Bravecto Spot-On for Cats') AND binh_luan = N'Great long-lasting protection, one dose covers the whole season.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Bravecto Spot-On for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'Great long-lasting protection, one dose covers the whole season.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Bravecto Spot-On for Cats') AND binh_luan = N'My cat didn''t mind the application at all, coat looks healthy too.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Bravecto Spot-On for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'anh632thu@gmail.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'My cat didn''t mind the application at all, coat looks healthy too.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Aristopet Horse Wormer') AND binh_luan = N'Straightforward dosing and my horse had no reaction to it.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Aristopet Horse Wormer'), ISNULL((SELECT user_id FROM users WHERE email = N'emma.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'Straightforward dosing and my horse had no reaction to it.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Aristopet Horse Wormer') AND binh_luan = N'Did the job for our regular deworming schedule.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Aristopet Horse Wormer'), ISNULL((SELECT user_id FROM users WHERE email = N'john.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'Did the job for our regular deworming schedule.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Bird Mite & Lice Spray') AND binh_luan = N'Easy to spray and my birds didn''t seem bothered by it.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Bird Mite & Lice Spray'), ISNULL((SELECT user_id FROM users WHERE email = N'rose.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'Easy to spray and my birds didn''t seem bothered by it.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Bird Mite & Lice Spray') AND binh_luan = N'Cleared up the mite problem within a couple of applications.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Bird Mite & Lice Spray'), ISNULL((SELECT user_id FROM users WHERE email = N'mark.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'Cleared up the mite problem within a couple of applications.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Heartgard Plus for Dogs') AND binh_luan = N'My dog takes it easily every month, no more worries about heartworm.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Heartgard Plus for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'My dog takes it easily every month, no more worries about heartworm.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Heartgard Plus for Dogs') AND binh_luan = N'Reliable protection, just remember to keep it on a strict monthly schedule.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Heartgard Plus for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'anh632thu@gmail.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'Reliable protection, just remember to keep it on a strict monthly schedule.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Bravecto Plus for Cats') AND binh_luan = N'Covers fleas, ticks, and heartworm in one go - exactly what we needed for our cat.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Bravecto Plus for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'emma.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'Covers fleas, ticks, and heartworm in one go - exactly what we needed for our cat.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Bravecto Plus for Cats') AND binh_luan = N'Good all-in-one option, application was quick and easy.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Bravecto Plus for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'john.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'Good all-in-one option, application was quick and easy.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'AdvantageMulti for Cats') AND binh_luan = N'Broad coverage and my cat tolerates it well every month.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'AdvantageMulti for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'rose.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'Broad coverage and my cat tolerates it well every month.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'AdvantageMulti for Cats') AND binh_luan = N'Works as expected, just a bit strong-smelling right after application.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'AdvantageMulti for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'mark.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'Works as expected, just a bit strong-smelling right after application.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Advantage for Cats') AND binh_luan = N'Basic and effective flea treatment, does what it says.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Advantage for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'Basic and effective flea treatment, does what it says.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Advantage for Cats') AND binh_luan = N'Affordable option that''s kept our cat flea-free for months.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Advantage for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'anh632thu@gmail.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'Affordable option that''s kept our cat flea-free for months.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Profender for Cats') AND binh_luan = N'Cleared up the worm issue our vet flagged during the checkup.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Profender for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'emma.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'Cleared up the worm issue our vet flagged during the checkup.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Profender for Cats') AND binh_luan = N'Simple spot-on application, cat didn''t react badly at all.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Profender for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'john.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'Simple spot-on application, cat didn''t react badly at all.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Dermoscent Essential 6 Skin & Coat Spray for Dogs') AND binh_luan = N'Noticeable improvement in my dog''s coat shine after a few weeks.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Dermoscent Essential 6 Skin & Coat Spray for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'rose.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'Noticeable improvement in my dog''s coat shine after a few weeks.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Dermoscent Essential 6 Skin & Coat Spray for Dogs') AND binh_luan = N'Helped with the dry, flaky skin patches he had before.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Dermoscent Essential 6 Skin & Coat Spray for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'mark.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'Helped with the dry, flaky skin patches he had before.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Adaptil Calming Diffuser for Dogs') AND binh_luan = N'Seemed to calm my dog down during the last thunderstorm season.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Adaptil Calming Diffuser for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'Seemed to calm my dog down during the last thunderstorm season.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Adaptil Calming Diffuser for Dogs') AND binh_luan = N'Noticed less pacing and whining when we''re away from home now.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Adaptil Calming Diffuser for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'anh632thu@gmail.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'Noticed less pacing and whining when we''re away from home now.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Frontline Spray for Cats') AND binh_luan = N'Easy spray bottle, my cat tolerates it better than the spot-on version.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Frontline Spray for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'emma.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'Easy spray bottle, my cat tolerates it better than the spot-on version.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Frontline Spray for Cats') AND binh_luan = N'Works well and the alcohol-free formula didn''t irritate her skin.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Frontline Spray for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'john.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'Works well and the alcohol-free formula didn''t irritate her skin.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Simparica Trio for Dogs') AND binh_luan = N'Decent but my dog seemed a little drowsy the first day after taking it.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Simparica Trio for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 3, N'Decent but my dog seemed a little drowsy the first day after taking it.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Simparica Trio for Dogs') AND binh_luan = N'Works okay, wouldn''t say it''s dramatically better than cheaper options.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Simparica Trio for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'anh632thu@gmail.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 3, N'Works okay, wouldn''t say it''s dramatically better than cheaper options.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Seresto Collar for Dogs') AND binh_luan = N'Fine for the price, though the buckle feels a bit flimsy.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Seresto Collar for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'mark.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 3, N'Fine for the price, though the buckle feels a bit flimsy.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Seresto Collar for Dogs') AND binh_luan = N'Held up well through several baths, still smells fresh.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Seresto Collar for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'emma.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'Held up well through several baths, still smells fresh.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'NexGard Chewables for Dogs') AND binh_luan = N'Best flea chew we''ve tried, our dog stopped scratching almost overnight.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'NexGard Chewables for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'rose.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'Best flea chew we''ve tried, our dog stopped scratching almost overnight.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'NexGard Chewables for Dogs') AND binh_luan = N'Vet recommended this specifically and it''s lived up to the hype.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'NexGard Chewables for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'john.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'Vet recommended this specifically and it''s lived up to the hype.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Frontline Flea & Tick Prevention for Dogs') AND binh_luan = N'Didn''t notice much difference, still found a few fleas after two applications.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Frontline Flea & Tick Prevention for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 2, N'Didn''t notice much difference, still found a few fleas after two applications.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Frontline Flea & Tick Prevention for Dogs') AND binh_luan = N'Works fine once you get the application technique right.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Frontline Flea & Tick Prevention for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'anh632thu@gmail.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'Works fine once you get the application technique right.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Frontline Plus for Cats') AND binh_luan = N'Solid monthly treatment, easy to apply without much fuss.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Frontline Plus for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'rose.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'Solid monthly treatment, easy to apply without much fuss.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Frontline Plus for Cats') AND binh_luan = N'No complaints, does what it''s supposed to.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Frontline Plus for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'mark.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'No complaints, does what it''s supposed to.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Revolution Plus for Cats') AND binh_luan = N'Good coverage but the smell lingers on my cat''s fur for a day or two.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Revolution Plus for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'emma.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 3, N'Good coverage but the smell lingers on my cat''s fur for a day or two.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Revolution Plus for Cats') AND binh_luan = N'Covers everything in one dose, exactly what a multi-cat household needs.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Revolution Plus for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'john.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'Covers everything in one dose, exactly what a multi-cat household needs.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Capstar Tablets for Cats') AND binh_luan = N'Didn''t work as fast as advertised for my cat.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Capstar Tablets for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 2, N'Didn''t work as fast as advertised for my cat.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Capstar Tablets for Cats') AND binh_luan = N'Helped a little but I had to combine it with another treatment.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Capstar Tablets for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'anh632thu@gmail.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 3, N'Helped a little but I had to combine it with another treatment.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Bravecto Spot-On for Cats') AND binh_luan = N'Long-lasting and my cat barely notices the application.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Bravecto Spot-On for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'rose.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'Long-lasting and my cat barely notices the application.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Bravecto Spot-On for Cats') AND binh_luan = N'Reliable protection through the warmer months.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Bravecto Spot-On for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'mark.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'Reliable protection through the warmer months.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Aristopet Horse Wormer') AND binh_luan = N'Did the job but the taste seems off-putting, had to mix it with feed.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Aristopet Horse Wormer'), ISNULL((SELECT user_id FROM users WHERE email = N'emma.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 3, N'Did the job but the taste seems off-putting, had to mix it with feed.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Aristopet Horse Wormer') AND binh_luan = N'Average results, nothing to complain about but nothing amazing either.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Aristopet Horse Wormer'), ISNULL((SELECT user_id FROM users WHERE email = N'john.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 3, N'Average results, nothing to complain about but nothing amazing either.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Milpro Bird Care Supplement') AND binh_luan = N'My birds are noticeably more energetic since we started this.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Milpro Bird Care Supplement'), ISNULL((SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'My birds are noticeably more energetic since we started this.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Milpro Bird Care Supplement') AND binh_luan = N'Great supplement, easy to mix and no bad smell.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Milpro Bird Care Supplement'), ISNULL((SELECT user_id FROM users WHERE email = N'anh632thu@gmail.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'Great supplement, easy to mix and no bad smell.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Bird Mite & Lice Spray') AND binh_luan = N'Cleared up the mites within a week of regular use.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Bird Mite & Lice Spray'), ISNULL((SELECT user_id FROM users WHERE email = N'rose.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'Cleared up the mites within a week of regular use.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Bird Mite & Lice Spray') AND binh_luan = N'Works but you need to be consistent with weekly applications.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Bird Mite & Lice Spray'), ISNULL((SELECT user_id FROM users WHERE email = N'mark.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 3, N'Works but you need to be consistent with weekly applications.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Revolution for Dogs') AND binh_luan = N'Fantastic results, no more fleas and his coat looks shinier too.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Revolution for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'emma.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'Fantastic results, no more fleas and his coat looks shinier too.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Revolution for Dogs') AND binh_luan = N'Highly recommend, easiest monthly treatment we''ve used.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Revolution for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'john.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'Highly recommend, easiest monthly treatment we''ve used.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Heartgard Plus for Dogs') AND binh_luan = N'My dog vomited once after taking it, had to switch products.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Heartgard Plus for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 2, N'My dog vomited once after taking it, had to switch products.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Heartgard Plus for Dogs') AND binh_luan = N'No issues at all, heartworm test came back clean at the vet visit.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Heartgard Plus for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'anh632thu@gmail.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'No issues at all, heartworm test came back clean at the vet visit.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Bravecto Chews for Dogs') AND binh_luan = N'Works but the effect seems to wear off a bit before the 3-month mark.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Bravecto Chews for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'rose.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 3, N'Works but the effect seems to wear off a bit before the 3-month mark.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Bravecto Chews for Dogs') AND binh_luan = N'Convenient dosing schedule, my dog takes it without a fight.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Bravecto Chews for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'mark.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'Convenient dosing schedule, my dog takes it without a fight.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Seresto Collar for Cats') AND binh_luan = N'Good value, my cat''s fur still looks and smells clean.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Seresto Collar for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'emma.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'Good value, my cat''s fur still looks and smells clean.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Seresto Collar for Cats') AND binh_luan = N'Effective and low-maintenance, exactly what I wanted.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Seresto Collar for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'john.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'Effective and low-maintenance, exactly what I wanted.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Bravecto Plus for Cats') AND binh_luan = N'Covers fleas, ticks, and heartworm - simplified our whole routine.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Bravecto Plus for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'Covers fleas, ticks, and heartworm - simplified our whole routine.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Bravecto Plus for Cats') AND binh_luan = N'Works but pricier than I expected for what it offers.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Bravecto Plus for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'anh632thu@gmail.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 3, N'Works but pricier than I expected for what it offers.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'AdvantageMulti for Cats') AND binh_luan = N'Didn''t seem very effective against fleas specifically.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'AdvantageMulti for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'rose.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 2, N'Didn''t seem very effective against fleas specifically.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'AdvantageMulti for Cats') AND binh_luan = N'Good broad-spectrum option, cat tolerated it fine.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'AdvantageMulti for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'mark.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'Good broad-spectrum option, cat tolerated it fine.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Advantage for Cats') AND binh_luan = N'Simple and effective, exactly what our vet suggested.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Advantage for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'emma.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'Simple and effective, exactly what our vet suggested.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Advantage for Cats') AND binh_luan = N'No fleas since we started, very happy with this.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Advantage for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'john.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'No fleas since we started, very happy with this.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Profender for Cats') AND binh_luan = N'Worked eventually but took longer than expected to clear up.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Profender for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 3, N'Worked eventually but took longer than expected to clear up.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Profender for Cats') AND binh_luan = N'Did the job, cat handled the application well.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Profender for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'anh632thu@gmail.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'Did the job, cat handled the application well.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Dermoscent Essential 6 Skin & Coat Spray for Dogs') AND binh_luan = N'Coat is visibly shinier after a month of regular use.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Dermoscent Essential 6 Skin & Coat Spray for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'rose.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'Coat is visibly shinier after a month of regular use.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Dermoscent Essential 6 Skin & Coat Spray for Dogs') AND binh_luan = N'Helped calm down the dry patches on his back.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Dermoscent Essential 6 Skin & Coat Spray for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'mark.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'Helped calm down the dry patches on his back.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Adaptil Calming Diffuser for Dogs') AND binh_luan = N'Some improvement but not a miracle fix for severe anxiety.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Adaptil Calming Diffuser for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'emma.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 3, N'Some improvement but not a miracle fix for severe anxiety.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Adaptil Calming Diffuser for Dogs') AND binh_luan = N'Mild effect, might work better combined with training.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Adaptil Calming Diffuser for Dogs'), ISNULL((SELECT user_id FROM users WHERE email = N'john.demo@pawsome-test.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 3, N'Mild effect, might work better combined with training.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Frontline Spray for Cats') AND binh_luan = N'Cat tolerates it much better than the spot-on version we tried before.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Frontline Spray for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 5, N'Cat tolerates it much better than the spot-on version we tried before.', N'da_duyet');

IF NOT EXISTS (SELECT 1 FROM reviews WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Frontline Spray for Cats') AND binh_luan = N'Good alternative for cats who dislike being touched on the neck.')
    INSERT INTO reviews (product_id, user_id, so_sao, binh_luan, trang_thai) VALUES ((SELECT product_id FROM products WHERE ten = N'Frontline Spray for Cats'), ISNULL((SELECT user_id FROM users WHERE email = N'anh632thu@gmail.com'), (SELECT TOP 1 user_id FROM users ORDER BY user_id)), 4, N'Good alternative for cats who dislike being touched on the neck.', N'da_duyet');


");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Chỉ đồng bộ dữ liệu, không đổi cấu trúc bảng - Down để trống có chủ đích, không có
            // cách nào phân biệt lại đúng dòng nào do migration này thêm vs dòng đã có sẵn từ trước.
        }
    }
}
