using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedSanPhamData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Bọc IF NOT EXISTS (SELECT 1 FROM categories) - chỉ chèn dữ liệu mẫu khi bảng
            // categories đang trống. Nếu máy nào đã có category/product riêng (tự tạo qua
            // Admin API) thì bỏ qua toàn bộ, không đụng gì, không có rủi ro trùng khóa chính
            // như trường hợp SeedAdminUserAndBlogPosts hardcode user_id=1 từng gặp phải.
            // Chó/Mèo có đủ 8 sản phẩm mỗi danh mục để khớp đúng bản thiết kế gốc (2 hàng x 4).
            // Tên sản phẩm + ảnh đã đối chiếu trực tiếp với chữ in trên hộp trong từng ảnh
            // img/ (không suy đoán theo tên file) để đảm bảo khớp đúng, không còn ông nói gà
            // bà nói vịt (VD trước đó lỡ gán "Frontline Plus" cho ảnh hộp "Heartgard Plus").
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM categories)
BEGIN
    SET IDENTITY_INSERT categories ON;
    INSERT INTO categories (category_id, ten_danh_muc, danh_muc_cha_id, mo_ta) VALUES
        (1, N'Chó', NULL, N'Sản phẩm chăm sóc sức khỏe cho chó'),
        (2, N'Mèo', NULL, N'Sản phẩm chăm sóc sức khỏe cho mèo'),
        (3, N'Ngựa', NULL, N'Sản phẩm chăm sóc sức khỏe cho ngựa'),
        (4, N'Chim', NULL, N'Sản phẩm chăm sóc sức khỏe cho chim');
    SET IDENTITY_INSERT categories OFF;

    SET IDENTITY_INSERT brands ON;
    INSERT INTO brands (brand_id, ten_thuong_hieu) VALUES
        (1, N'Bravecto'),
        (2, N'Capstar'),
        (3, N'Frontline Plus'),
        (4, N'Profender'),
        (5, N'Simparica Trio'),
        (6, N'Seresto'),
        (7, N'Dorwest'),
        (8, N'Aristopet'),
        (9, N'Milpro'),
        (10, N'Advantage Multi'),
        (11, N'NexGard'),
        (12, N'Revolution Plus'),
        (13, N'Heartgard'),
        (14, N'Advantage');
    SET IDENTITY_INSERT brands OFF;

    SET IDENTITY_INSERT products ON;
    INSERT INTO products (product_id, ten, mo_ta, category_id, brand_id, lieu_luong) VALUES
        (1, N'Simparica Trio for Dogs', N'3-in-1 chewable treatment for dogs: kills fleas and ticks, prevents heartworm, and treats intestinal worms.', 1, 5, N'1 chewable tablet per month, dosed by body weight'),
        (2, N'Seresto Collar for Dogs', N'Flea and tick collar for dogs, odorless and water-resistant, protects for up to 8 months.', 1, 6, N'Wear continuously, replace after 8 months'),
        (3, N'NexGard Chewables for Dogs', N'Chewable treatment for dogs that kills fleas and ticks.', 1, 11, N'1 chewable tablet per month'),
        (4, N'Frontline Flea & Tick Prevention for Dogs', N'Monthly spot-on treatment for dogs that kills fleas and ticks.', 1, 3, N'Apply directly to the skin at the back of the neck, once a month'),
        (5, N'Capstar Oral Flea Treatment for Dogs', N'Fast-acting oral flea treatment for dogs.', 1, 2, N'1 tablet per dose, may be repeated daily if needed'),
        (6, N'Frontline Plus for Cats', N'Spot-on treatment for cats that kills fleas, ticks, and flea eggs.', 2, 3, N'Apply directly to the skin at the back of the neck, once a month'),
        (7, N'Revolution Plus for Cats', N'Spot-on treatment for cats that controls fleas, ear mites, heartworm, and intestinal worms.', 2, 12, N'Apply directly to the skin at the back of the neck, once a month'),
        (8, N'Capstar Tablets for Cats', N'Oral tablet that kills fleas on cats within 30 minutes.', 2, 2, N'1 tablet per dose, may be repeated daily if needed'),
        (9, N'Bravecto Spot-On for Cats', N'Spot-on treatment for cats that kills fleas and ticks, protects for up to 12 weeks.', 2, 1, N'Apply directly to the skin at the back of the neck, once every 3 months'),
        (10, N'Dorwest Horse Herbal Supplement', N'Herbal supplement that supports joint health and digestion in horses.', 3, 7, N'Mix into the daily feed ration as directed on the packaging'),
        (11, N'Aristopet Horse Wormer', N'Oral dewormer solution for horses.', 3, 8, N'Administer orally dosed by body weight, repeat every 3 months'),
        (12, N'Milpro Bird Care Supplement', N'Vitamin and mineral supplement powder that boosts immunity in pet birds.', 4, 9, N'Sprinkle over food or mix into drinking water daily'),
        (13, N'Bird Mite & Lice Spray', N'Spray treatment for mites and lice on pet birds, safe for direct application to feathers.', 4, NULL, N'Spray from 20cm away from the feathers, once a week'),
        (14, N'Revolution for Dogs', N'Spot-on treatment for dogs that controls fleas, ear mites, mange, and heartworm.', 1, 12, N'Apply directly to the skin at the back of the neck, once a month'),
        (15, N'Heartgard Plus for Dogs', N'Chewable treatment for dogs that prevents heartworm and treats intestinal worms.', 1, 13, N'1 chewable tablet per month, dosed by body weight'),
        (16, N'Bravecto Chews for Dogs', N'Chewable treatment for dogs that kills fleas and ticks, protects for up to 12 weeks.', 1, 1, N'1 chewable tablet every 3 months'),
        (17, N'Seresto Collar for Cats', N'Flea and tick collar for cats, odorless and water-resistant, protects for up to 8 months.', 2, 6, N'Wear continuously, replace after 8 months'),
        (18, N'Bravecto Plus for Cats', N'Spot-on treatment for cats that kills fleas and ticks and prevents heartworm.', 2, 1, N'Apply directly to the skin at the back of the neck, once every 3 months'),
        (19, N'AdvantageMulti for Cats', N'Spot-on treatment for cats that prevents heartworm and treats a broad range of parasites.', 2, 10, N'Apply directly to the skin at the back of the neck, once a month'),
        (20, N'Advantage for Cats', N'Spot-on flea treatment for cats.', 2, 14, N'Apply directly to the skin at the back of the neck, once a month');
    SET IDENTITY_INSERT products OFF;

    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku) VALUES
        (1, 1, N'Dogs 4.5-10kg', 250000, 50, N'SIM-DOG-S'),
        (2, 1, N'Dogs 10-22kg', 320000, 35, N'SIM-DOG-M'),
        (3, 2, N'Small Dogs (under 10kg)', 180000, 40, N'SER-DOG-S'),
        (4, 3, N'Large Dogs (>18kg)', 620000, 20, N'NEX-DOG-L'),
        (5, 3, N'Small Dogs (<18kg)', 520000, 25, N'NEX-DOG-S'),
        (6, 4, N'Dogs 5-10kg', 350000, 30, N'FTL-DOG-S'),
        (7, 5, N'6-Tablet Pack', 95000, 45, N'CAP-DOG-6'),
        (8, 6, N'Adult Cats', 160000, 35, N'FTL-CAT-A'),
        (9, 7, N'Cats 1.2-2.8kg', 230000, 25, N'REV-CAT-S'),
        (10, 8, N'6-Tablet Pack', 90000, 60, N'CAP-CAT-6'),
        (11, 9, N'Cats 2.5-5kg', 210000, 20, N'BRAV-SPOT-CAT-M'),
        (12, 10, N'500g Box', 450000, 15, N'DOR-HOR-500'),
        (13, 11, N'1L Bottle', 380000, 10, N'ARI-HOR-1L'),
        (14, 12, N'100g Pack', 95000, 40, N'MIL-BIRD-100'),
        (15, 13, N'150ml Spray Bottle', 175000, 18, N'BIRD-SPRAY-150'),
        (16, 14, N'Dogs 10-20kg', 340000, 30, N'REV-DOG-M'),
        (17, 15, N'Dogs 4-10kg', 290000, 25, N'HGD-DOG-S'),
        (18, 16, N'Dogs 10.1-25kg', 310000, 28, N'BRAV-DOG-M'),
        (19, 17, N'Cat Collar', 480000, 22, N'SER-CAT'),
        (20, 18, N'Cats over 2.8kg', 260000, 20, N'BRAV-PLUS-CAT-L'),
        (21, 19, N'Cats 2.5-5kg', 240000, 18, N'ADVM-CAT-M'),
        (22, 20, N'Cats under 2.5kg', 220000, 24, N'ADV-CAT-S');
    SET IDENTITY_INSERT product_variants OFF;

    SET IDENTITY_INSERT product_images ON;
    INSERT INTO product_images (image_id, product_id, url, la_anh_chinh) VALUES
        (1, 1, N'/img/1.png', 1),
        (2, 2, N'/img/2.png', 1),
        (3, 3, N'/img/3.png', 1),
        (4, 4, N'/img/4.png', 1),
        (5, 5, N'/img/5.png', 1),
        (6, 6, N'/img/m1.png', 1),
        (7, 7, N'/img/m2.png', 1),
        (8, 8, N'/img/m3.png', 1),
        (9, 9, N'/img/m4.png', 1),
        (10, 10, N'/img/Dorwest.png', 1),
        (11, 11, N'/img/aristopet.png', 1),
        (12, 12, N'/img/1.png', 1),
        (13, 13, N'/img/2.png', 1),
        (14, 14, N'/img/7.png', 1),
        (15, 15, N'/img/cho0.png', 1),
        (16, 16, N'/img/cho00.png', 1),
        (17, 17, N'/img/m5.png', 1),
        (18, 18, N'/img/m6.png', 1),
        (19, 19, N'/img/m7.png', 1),
        (20, 20, N'/img/m8.png', 1);
    SET IDENTITY_INSERT product_images OFF;

    -- products.gia_tu là cột cache (giá thấp nhất trong các biến thể) - bình thường được
    -- ProductService tự tính khi tạo/sửa qua API (xem CapNhatGiaTu trong ProductService.cs),
    -- nhưng insert thẳng bằng SQL thì không tự chạy nên phải tính tay ở đây.
    UPDATE p
    SET gia_tu = (SELECT MIN(v.gia) FROM product_variants v WHERE v.product_id = p.product_id)
    FROM products p
    WHERE p.product_id BETWEEN 1 AND 20;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM product_images WHERE image_id BETWEEN 1 AND 20;
DELETE FROM product_variants WHERE variant_id BETWEEN 1 AND 22;
DELETE FROM products WHERE product_id BETWEEN 1 AND 20;
DELETE FROM brands WHERE brand_id BETWEEN 1 AND 14;
DELETE FROM categories WHERE category_id BETWEEN 1 AND 4;
");
        }
    }
}
