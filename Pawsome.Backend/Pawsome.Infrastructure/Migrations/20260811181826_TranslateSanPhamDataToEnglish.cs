using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TranslateSanPhamDataToEnglish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Sửa lại dữ liệu mẫu do migration SeedSanPhamData chèn trước đó (mo_ta/lieu_luong
            // đang là tiếng Việt trong khi UI đã chốt dùng tiếng Anh - xem CLAUDE.md quy tắc 5).
            // Mỗi UPDATE khớp cả 2 điều kiện (id + cột ổn định không đổi: products.ten hoặc
            // product_variants.sku) để chỉ chạm đúng dòng dữ liệu mẫu của Phần 2, tránh sửa
            // nhầm dữ liệu tự tạo của người khác nếu họ trùng id nhưng khác sản phẩm - an toàn
            // chạy Update-Database ở bất kỳ trạng thái DB nào, không lỗi, không đụng dữ liệu lạ.
            migrationBuilder.Sql(@"
UPDATE products SET mo_ta = N'3-in-1 chewable treatment for dogs: kills fleas and ticks, prevents heartworm, and treats intestinal worms.', lieu_luong = N'1 chewable tablet per month, dosed by body weight' WHERE product_id = 1 AND ten = N'Simparica Trio for Dogs';
UPDATE products SET mo_ta = N'Flea and tick collar for dogs, odorless and water-resistant, protects for up to 8 months.', lieu_luong = N'Wear continuously, replace after 8 months' WHERE product_id = 2 AND ten = N'Seresto Collar for Dogs';
UPDATE products SET mo_ta = N'Chewable treatment for dogs that kills fleas and ticks.', lieu_luong = N'1 chewable tablet per month' WHERE product_id = 3 AND ten = N'NexGard Chewables for Dogs';
UPDATE products SET mo_ta = N'Monthly spot-on treatment for dogs that kills fleas and ticks.', lieu_luong = N'Apply directly to the skin at the back of the neck, once a month' WHERE product_id = 4 AND ten = N'Frontline Flea & Tick Prevention for Dogs';
UPDATE products SET mo_ta = N'Fast-acting oral flea treatment for dogs.', lieu_luong = N'1 tablet per dose, may be repeated daily if needed' WHERE product_id = 5 AND ten = N'Capstar Oral Flea Treatment for Dogs';
UPDATE products SET mo_ta = N'Spot-on treatment for cats that kills fleas, ticks, and flea eggs.', lieu_luong = N'Apply directly to the skin at the back of the neck, once a month' WHERE product_id = 6 AND ten = N'Frontline Plus for Cats';
UPDATE products SET mo_ta = N'Spot-on treatment for cats that controls fleas, ear mites, heartworm, and intestinal worms.', lieu_luong = N'Apply directly to the skin at the back of the neck, once a month' WHERE product_id = 7 AND ten = N'Revolution Plus for Cats';
UPDATE products SET mo_ta = N'Oral tablet that kills fleas on cats within 30 minutes.', lieu_luong = N'1 tablet per dose, may be repeated daily if needed' WHERE product_id = 8 AND ten = N'Capstar Tablets for Cats';
UPDATE products SET mo_ta = N'Spot-on treatment for cats that kills fleas and ticks, protects for up to 12 weeks.', lieu_luong = N'Apply directly to the skin at the back of the neck, once every 3 months' WHERE product_id = 9 AND ten = N'Bravecto Spot-On for Cats';
UPDATE products SET mo_ta = N'Herbal supplement that supports joint health and digestion in horses.', lieu_luong = N'Mix into the daily feed ration as directed on the packaging' WHERE product_id = 10 AND ten = N'Dorwest Horse Herbal Supplement';
UPDATE products SET mo_ta = N'Oral dewormer solution for horses.', lieu_luong = N'Administer orally dosed by body weight, repeat every 3 months' WHERE product_id = 11 AND ten = N'Aristopet Horse Wormer';
UPDATE products SET mo_ta = N'Vitamin and mineral supplement powder that boosts immunity in pet birds.', lieu_luong = N'Sprinkle over food or mix into drinking water daily' WHERE product_id = 12 AND ten = N'Milpro Bird Care Supplement';
UPDATE products SET mo_ta = N'Spray treatment for mites and lice on pet birds, safe for direct application to feathers.', lieu_luong = N'Spray from 20cm away from the feathers, once a week' WHERE product_id = 13 AND ten = N'Bird Mite & Lice Spray';
UPDATE products SET mo_ta = N'Spot-on treatment for dogs that controls fleas, ear mites, mange, and heartworm.', lieu_luong = N'Apply directly to the skin at the back of the neck, once a month' WHERE product_id = 14 AND ten = N'Revolution for Dogs';
UPDATE products SET mo_ta = N'Chewable treatment for dogs that prevents heartworm and treats intestinal worms.', lieu_luong = N'1 chewable tablet per month, dosed by body weight' WHERE product_id = 15 AND ten = N'Heartgard Plus for Dogs';
UPDATE products SET mo_ta = N'Chewable treatment for dogs that kills fleas and ticks, protects for up to 12 weeks.', lieu_luong = N'1 chewable tablet every 3 months' WHERE product_id = 16 AND ten = N'Bravecto Chews for Dogs';
UPDATE products SET mo_ta = N'Flea and tick collar for cats, odorless and water-resistant, protects for up to 8 months.', lieu_luong = N'Wear continuously, replace after 8 months' WHERE product_id = 17 AND ten = N'Seresto Collar for Cats';
UPDATE products SET mo_ta = N'Spot-on treatment for cats that kills fleas and ticks and prevents heartworm.', lieu_luong = N'Apply directly to the skin at the back of the neck, once every 3 months' WHERE product_id = 18 AND ten = N'Bravecto Plus for Cats';
UPDATE products SET mo_ta = N'Spot-on treatment for cats that prevents heartworm and treats a broad range of parasites.', lieu_luong = N'Apply directly to the skin at the back of the neck, once a month' WHERE product_id = 19 AND ten = N'AdvantageMulti for Cats';
UPDATE products SET mo_ta = N'Spot-on flea treatment for cats.', lieu_luong = N'Apply directly to the skin at the back of the neck, once a month' WHERE product_id = 20 AND ten = N'Advantage for Cats';

UPDATE product_variants SET ten_bien_the = N'Dogs 4.5-10kg' WHERE variant_id = 1 AND sku = N'SIM-DOG-S';
UPDATE product_variants SET ten_bien_the = N'Dogs 10-22kg' WHERE variant_id = 2 AND sku = N'SIM-DOG-M';
UPDATE product_variants SET ten_bien_the = N'Small Dogs (under 10kg)' WHERE variant_id = 3 AND sku = N'SER-DOG-S';
UPDATE product_variants SET ten_bien_the = N'Large Dogs (>18kg)' WHERE variant_id = 4 AND sku = N'NEX-DOG-L';
UPDATE product_variants SET ten_bien_the = N'Small Dogs (<18kg)' WHERE variant_id = 5 AND sku = N'NEX-DOG-S';
UPDATE product_variants SET ten_bien_the = N'Dogs 5-10kg' WHERE variant_id = 6 AND sku = N'FTL-DOG-S';
UPDATE product_variants SET ten_bien_the = N'6-Tablet Pack' WHERE variant_id = 7 AND sku = N'CAP-DOG-6';
UPDATE product_variants SET ten_bien_the = N'Adult Cats' WHERE variant_id = 8 AND sku = N'FTL-CAT-A';
UPDATE product_variants SET ten_bien_the = N'Cats 1.2-2.8kg' WHERE variant_id = 9 AND sku = N'REV-CAT-S';
UPDATE product_variants SET ten_bien_the = N'6-Tablet Pack' WHERE variant_id = 10 AND sku = N'CAP-CAT-6';
UPDATE product_variants SET ten_bien_the = N'Cats 2.5-5kg' WHERE variant_id = 11 AND sku = N'BRAV-SPOT-CAT-M';
UPDATE product_variants SET ten_bien_the = N'500g Box' WHERE variant_id = 12 AND sku = N'DOR-HOR-500';
UPDATE product_variants SET ten_bien_the = N'1L Bottle' WHERE variant_id = 13 AND sku = N'ARI-HOR-1L';
UPDATE product_variants SET ten_bien_the = N'100g Pack' WHERE variant_id = 14 AND sku = N'MIL-BIRD-100';
UPDATE product_variants SET ten_bien_the = N'150ml Spray Bottle' WHERE variant_id = 15 AND sku = N'BIRD-SPRAY-150';
UPDATE product_variants SET ten_bien_the = N'Dogs 10-20kg' WHERE variant_id = 16 AND sku = N'REV-DOG-M';
UPDATE product_variants SET ten_bien_the = N'Dogs 4-10kg' WHERE variant_id = 17 AND sku = N'HGD-DOG-S';
UPDATE product_variants SET ten_bien_the = N'Dogs 10.1-25kg' WHERE variant_id = 18 AND sku = N'BRAV-DOG-M';
UPDATE product_variants SET ten_bien_the = N'Cat Collar' WHERE variant_id = 19 AND sku = N'SER-CAT';
UPDATE product_variants SET ten_bien_the = N'Cats over 2.8kg' WHERE variant_id = 20 AND sku = N'BRAV-PLUS-CAT-L';
UPDATE product_variants SET ten_bien_the = N'Cats 2.5-5kg' WHERE variant_id = 21 AND sku = N'ADVM-CAT-M';
UPDATE product_variants SET ten_bien_the = N'Cats under 2.5kg' WHERE variant_id = 22 AND sku = N'ADV-CAT-S';
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE products SET mo_ta = N'Thuốc nhai 3 tác dụng: trị ve rận, phòng giun tim, trị giun sán cho chó.', lieu_luong = N'1 viên nhai/tháng theo cân nặng' WHERE product_id = 1 AND ten = N'Simparica Trio for Dogs';
UPDATE products SET mo_ta = N'Vòng cổ chống ve rận cho chó, không mùi, chống nước, bảo vệ tới 8 tháng.', lieu_luong = N'Đeo liên tục, thay mới sau 8 tháng' WHERE product_id = 2 AND ten = N'Seresto Collar for Dogs';
UPDATE products SET mo_ta = N'Thuốc nhai trị bọ chét và ve cho chó.', lieu_luong = N'1 viên nhai/tháng' WHERE product_id = 3 AND ten = N'NexGard Chewables for Dogs';
UPDATE products SET mo_ta = N'Thuốc nhỏ gáy trị bọ chét và ve cho chó, dùng hàng tháng.', lieu_luong = N'Nhỏ trực tiếp lên da gáy, 1 tháng/lần' WHERE product_id = 4 AND ten = N'Frontline Flea & Tick Prevention for Dogs';
UPDATE products SET mo_ta = N'Viên uống diệt bọ chét nhanh cho chó.', lieu_luong = N'1 viên/lần, có thể lặp lại mỗi ngày nếu cần' WHERE product_id = 5 AND ten = N'Capstar Oral Flea Treatment for Dogs';
UPDATE products SET mo_ta = N'Thuốc nhỏ gáy trị bọ chét, ve và trứng ve cho mèo.', lieu_luong = N'Nhỏ trực tiếp lên da gáy, 1 tháng/lần' WHERE product_id = 6 AND ten = N'Frontline Plus for Cats';
UPDATE products SET mo_ta = N'Thuốc nhỏ gáy trị bọ chét, ve tai, giun tim và giun sán cho mèo.', lieu_luong = N'Nhỏ trực tiếp lên da gáy, 1 tháng/lần' WHERE product_id = 7 AND ten = N'Revolution Plus for Cats';
UPDATE products SET mo_ta = N'Viên uống diệt bọ chét nhanh trong 30 phút cho mèo.', lieu_luong = N'1 viên/lần, có thể lặp lại mỗi ngày nếu cần' WHERE product_id = 8 AND ten = N'Capstar Tablets for Cats';
UPDATE products SET mo_ta = N'Thuốc nhỏ gáy trị bọ chét và ve cho mèo, bảo vệ tới 12 tuần.', lieu_luong = N'Nhỏ trực tiếp lên da gáy, 3 tháng/lần' WHERE product_id = 9 AND ten = N'Bravecto Spot-On for Cats';
UPDATE products SET mo_ta = N'Thực phẩm bổ sung thảo dược hỗ trợ khớp và tiêu hóa cho ngựa.', lieu_luong = N'Trộn vào khẩu phần ăn hàng ngày theo hướng dẫn trên bao bì' WHERE product_id = 10 AND ten = N'Dorwest Horse Herbal Supplement';
UPDATE products SET mo_ta = N'Dung dịch tẩy giun sán đường uống cho ngựa.', lieu_luong = N'Uống trực tiếp theo cân nặng, lặp lại mỗi 3 tháng' WHERE product_id = 11 AND ten = N'Aristopet Horse Wormer';
UPDATE products SET mo_ta = N'Bột bổ sung vitamin và khoáng chất tăng đề kháng cho chim cảnh.', lieu_luong = N'Rắc lên thức ăn hoặc pha vào nước uống hàng ngày' WHERE product_id = 12 AND ten = N'Milpro Bird Care Supplement';
UPDATE products SET mo_ta = N'Xịt trị rận mạt lông cho chim cảnh, an toàn khi phun trực tiếp lên lông.', lieu_luong = N'Xịt cách lông 20cm, 1 lần/tuần' WHERE product_id = 13 AND ten = N'Bird Mite & Lice Spray';
UPDATE products SET mo_ta = N'Thuốc nhỏ gáy trị bọ chét, ve tai, ghẻ và giun tim cho chó.', lieu_luong = N'Nhỏ trực tiếp lên da gáy, 1 tháng/lần' WHERE product_id = 14 AND ten = N'Revolution for Dogs';
UPDATE products SET mo_ta = N'Thuốc nhai phòng giun tim và trị giun sán đường ruột cho chó.', lieu_luong = N'1 viên nhai/tháng theo cân nặng' WHERE product_id = 15 AND ten = N'Heartgard Plus for Dogs';
UPDATE products SET mo_ta = N'Thuốc nhai trị ve rận cho chó, bảo vệ tới 12 tuần.', lieu_luong = N'1 viên nhai/3 tháng' WHERE product_id = 16 AND ten = N'Bravecto Chews for Dogs';
UPDATE products SET mo_ta = N'Vòng cổ chống ve rận cho mèo, không mùi, chống nước, bảo vệ tới 8 tháng.', lieu_luong = N'Đeo liên tục, thay mới sau 8 tháng' WHERE product_id = 17 AND ten = N'Seresto Collar for Cats';
UPDATE products SET mo_ta = N'Thuốc nhỏ gáy trị ve rận và phòng giun tim cho mèo.', lieu_luong = N'Nhỏ trực tiếp lên da gáy, 3 tháng/lần' WHERE product_id = 18 AND ten = N'Bravecto Plus for Cats';
UPDATE products SET mo_ta = N'Thuốc nhỏ gáy phòng giun tim và trị ký sinh trùng phổ rộng cho mèo.', lieu_luong = N'Nhỏ trực tiếp lên da gáy, 1 tháng/lần' WHERE product_id = 19 AND ten = N'AdvantageMulti for Cats';
UPDATE products SET mo_ta = N'Thuốc nhỏ gáy trị bọ chét cho mèo.', lieu_luong = N'Nhỏ trực tiếp lên da gáy, 1 tháng/lần' WHERE product_id = 20 AND ten = N'Advantage for Cats';

UPDATE product_variants SET ten_bien_the = N'Chó 4.5-10kg' WHERE variant_id = 1 AND sku = N'SIM-DOG-S';
UPDATE product_variants SET ten_bien_the = N'Chó 10-22kg' WHERE variant_id = 2 AND sku = N'SIM-DOG-M';
UPDATE product_variants SET ten_bien_the = N'Chó nhỏ dưới 10kg' WHERE variant_id = 3 AND sku = N'SER-DOG-S';
UPDATE product_variants SET ten_bien_the = N'Chó lớn (>18kg)' WHERE variant_id = 4 AND sku = N'NEX-DOG-L';
UPDATE product_variants SET ten_bien_the = N'Chó nhỏ (<18kg)' WHERE variant_id = 5 AND sku = N'NEX-DOG-S';
UPDATE product_variants SET ten_bien_the = N'Chó 5-10kg' WHERE variant_id = 6 AND sku = N'FTL-DOG-S';
UPDATE product_variants SET ten_bien_the = N'Vỉ 6 viên' WHERE variant_id = 7 AND sku = N'CAP-DOG-6';
UPDATE product_variants SET ten_bien_the = N'Mèo trưởng thành' WHERE variant_id = 8 AND sku = N'FTL-CAT-A';
UPDATE product_variants SET ten_bien_the = N'Mèo 1.2-2.8kg' WHERE variant_id = 9 AND sku = N'REV-CAT-S';
UPDATE product_variants SET ten_bien_the = N'Vỉ 6 viên' WHERE variant_id = 10 AND sku = N'CAP-CAT-6';
UPDATE product_variants SET ten_bien_the = N'Mèo 2.5-5kg' WHERE variant_id = 11 AND sku = N'BRAV-SPOT-CAT-M';
UPDATE product_variants SET ten_bien_the = N'Hộp 500g' WHERE variant_id = 12 AND sku = N'DOR-HOR-500';
UPDATE product_variants SET ten_bien_the = N'Chai 1L' WHERE variant_id = 13 AND sku = N'ARI-HOR-1L';
UPDATE product_variants SET ten_bien_the = N'Gói 100g' WHERE variant_id = 14 AND sku = N'MIL-BIRD-100';
UPDATE product_variants SET ten_bien_the = N'Chai xịt 150ml' WHERE variant_id = 15 AND sku = N'BIRD-SPRAY-150';
UPDATE product_variants SET ten_bien_the = N'Chó 10-20kg' WHERE variant_id = 16 AND sku = N'REV-DOG-M';
UPDATE product_variants SET ten_bien_the = N'Chó 4-10kg' WHERE variant_id = 17 AND sku = N'HGD-DOG-S';
UPDATE product_variants SET ten_bien_the = N'Chó 10.1-25kg' WHERE variant_id = 18 AND sku = N'BRAV-DOG-M';
UPDATE product_variants SET ten_bien_the = N'Vòng cổ mèo' WHERE variant_id = 19 AND sku = N'SER-CAT';
UPDATE product_variants SET ten_bien_the = N'Mèo trên 2.8kg' WHERE variant_id = 20 AND sku = N'BRAV-PLUS-CAT-L';
UPDATE product_variants SET ten_bien_the = N'Mèo 2.5-5kg' WHERE variant_id = 21 AND sku = N'ADVM-CAT-M';
UPDATE product_variants SET ten_bien_the = N'Mèo dưới 2.5kg' WHERE variant_id = 22 AND sku = N'ADV-CAT-S';
");
        }
    }
}
