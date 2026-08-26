using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncCatalogDataAcrossTeam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Catalog gốc (20 sản phẩm đầu) chưa từng qua migration - được insert thủ công ngoài
            // Git từ trước, nên mỗi máy dev có thể đã tự chỉnh sửa/lệch dữ liệu theo thời gian
            // (tên, ảnh, giá...). Migration này đồng bộ TOÀN BỘ catalog (categories, brands,
            // conditions, products, product_variants, product_images, product_conditions) về đúng
            // 1 phiên bản chuẩn - dùng kiểu UPDATE nếu đã có / INSERT nếu chưa (upsert theo khóa
            // chính) để ép dữ liệu về giống nhau trên mọi máy khi chạy migration này, bất kể máy đó
            // đã có sẵn dữ liệu gì trước đó với cùng ID.
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM categories WHERE category_id = 1)
    UPDATE categories SET ten_danh_muc = N'Chó', danh_muc_cha_id = NULL, mo_ta = N'Sản phẩm chăm sóc sức khỏe cho chó' WHERE category_id = 1;
ELSE
BEGIN
    SET IDENTITY_INSERT categories ON;
    INSERT INTO categories (category_id, ten_danh_muc, danh_muc_cha_id, mo_ta) VALUES (1, N'Chó', NULL, N'Sản phẩm chăm sóc sức khỏe cho chó');
    SET IDENTITY_INSERT categories OFF;
END

IF EXISTS (SELECT 1 FROM categories WHERE category_id = 2)
    UPDATE categories SET ten_danh_muc = N'Mèo', danh_muc_cha_id = NULL, mo_ta = N'Sản phẩm chăm sóc sức khỏe cho mèo' WHERE category_id = 2;
ELSE
BEGIN
    SET IDENTITY_INSERT categories ON;
    INSERT INTO categories (category_id, ten_danh_muc, danh_muc_cha_id, mo_ta) VALUES (2, N'Mèo', NULL, N'Sản phẩm chăm sóc sức khỏe cho mèo');
    SET IDENTITY_INSERT categories OFF;
END

IF EXISTS (SELECT 1 FROM categories WHERE category_id = 3)
    UPDATE categories SET ten_danh_muc = N'Ngựa', danh_muc_cha_id = NULL, mo_ta = N'Sản phẩm chăm sóc sức khỏe cho ngựa' WHERE category_id = 3;
ELSE
BEGIN
    SET IDENTITY_INSERT categories ON;
    INSERT INTO categories (category_id, ten_danh_muc, danh_muc_cha_id, mo_ta) VALUES (3, N'Ngựa', NULL, N'Sản phẩm chăm sóc sức khỏe cho ngựa');
    SET IDENTITY_INSERT categories OFF;
END

IF EXISTS (SELECT 1 FROM categories WHERE category_id = 4)
    UPDATE categories SET ten_danh_muc = N'Chim', danh_muc_cha_id = NULL, mo_ta = N'Sản phẩm chăm sóc sức khỏe cho chim' WHERE category_id = 4;
ELSE
BEGIN
    SET IDENTITY_INSERT categories ON;
    INSERT INTO categories (category_id, ten_danh_muc, danh_muc_cha_id, mo_ta) VALUES (4, N'Chim', NULL, N'Sản phẩm chăm sóc sức khỏe cho chim');
    SET IDENTITY_INSERT categories OFF;
END

IF EXISTS (SELECT 1 FROM brands WHERE brand_id = 1)
    UPDATE brands SET ten_thuong_hieu = N'Bravecto', logo_url = NULL WHERE brand_id = 1;
ELSE
BEGIN
    SET IDENTITY_INSERT brands ON;
    INSERT INTO brands (brand_id, ten_thuong_hieu, logo_url) VALUES (1, N'Bravecto', NULL);
    SET IDENTITY_INSERT brands OFF;
END

IF EXISTS (SELECT 1 FROM brands WHERE brand_id = 3)
    UPDATE brands SET ten_thuong_hieu = N'Frontline Plus', logo_url = NULL WHERE brand_id = 3;
ELSE
BEGIN
    SET IDENTITY_INSERT brands ON;
    INSERT INTO brands (brand_id, ten_thuong_hieu, logo_url) VALUES (3, N'Frontline Plus', NULL);
    SET IDENTITY_INSERT brands OFF;
END

IF EXISTS (SELECT 1 FROM brands WHERE brand_id = 6)
    UPDATE brands SET ten_thuong_hieu = N'Seresto', logo_url = NULL WHERE brand_id = 6;
ELSE
BEGIN
    SET IDENTITY_INSERT brands ON;
    INSERT INTO brands (brand_id, ten_thuong_hieu, logo_url) VALUES (6, N'Seresto', NULL);
    SET IDENTITY_INSERT brands OFF;
END

IF EXISTS (SELECT 1 FROM brands WHERE brand_id = 11)
    UPDATE brands SET ten_thuong_hieu = N'NexGard', logo_url = NULL WHERE brand_id = 11;
ELSE
BEGIN
    SET IDENTITY_INSERT brands ON;
    INSERT INTO brands (brand_id, ten_thuong_hieu, logo_url) VALUES (11, N'NexGard', NULL);
    SET IDENTITY_INSERT brands OFF;
END

IF EXISTS (SELECT 1 FROM conditions WHERE condition_id = 1)
    UPDATE conditions SET ten_tinh_trang = N'Flea, Tick' WHERE condition_id = 1;
ELSE
BEGIN
    SET IDENTITY_INSERT conditions ON;
    INSERT INTO conditions (condition_id, ten_tinh_trang) VALUES (1, N'Flea, Tick');
    SET IDENTITY_INSERT conditions OFF;
END

IF EXISTS (SELECT 1 FROM conditions WHERE condition_id = 2)
    UPDATE conditions SET ten_tinh_trang = N'Wormers' WHERE condition_id = 2;
ELSE
BEGIN
    SET IDENTITY_INSERT conditions ON;
    INSERT INTO conditions (condition_id, ten_tinh_trang) VALUES (2, N'Wormers');
    SET IDENTITY_INSERT conditions OFF;
END

IF EXISTS (SELECT 1 FROM conditions WHERE condition_id = 3)
    UPDATE conditions SET ten_tinh_trang = N'Heartwormers' WHERE condition_id = 3;
ELSE
BEGIN
    SET IDENTITY_INSERT conditions ON;
    INSERT INTO conditions (condition_id, ten_tinh_trang) VALUES (3, N'Heartwormers');
    SET IDENTITY_INSERT conditions OFF;
END

IF EXISTS (SELECT 1 FROM conditions WHERE condition_id = 4)
    UPDATE conditions SET ten_tinh_trang = N'Joint Care' WHERE condition_id = 4;
ELSE
BEGIN
    SET IDENTITY_INSERT conditions ON;
    INSERT INTO conditions (condition_id, ten_tinh_trang) VALUES (4, N'Joint Care');
    SET IDENTITY_INSERT conditions OFF;
END

IF EXISTS (SELECT 1 FROM conditions WHERE condition_id = 5)
    UPDATE conditions SET ten_tinh_trang = N'Skin & Coat' WHERE condition_id = 5;
ELSE
BEGIN
    SET IDENTITY_INSERT conditions ON;
    INSERT INTO conditions (condition_id, ten_tinh_trang) VALUES (5, N'Skin & Coat');
    SET IDENTITY_INSERT conditions OFF;
END

IF EXISTS (SELECT 1 FROM conditions WHERE condition_id = 6)
    UPDATE conditions SET ten_tinh_trang = N'Behavioural' WHERE condition_id = 6;
ELSE
BEGIN
    SET IDENTITY_INSERT conditions ON;
    INSERT INTO conditions (condition_id, ten_tinh_trang) VALUES (6, N'Behavioural');
    SET IDENTITY_INSERT conditions OFF;
END


IF EXISTS (SELECT 1 FROM products WHERE product_id = 1)
    UPDATE products SET ten = N'Simparica Trio for Dogs', mo_ta = N'3-in-1 chewable treatment for dogs: kills fleas and ticks, prevents heartworm, and treats intestinal worms.', category_id = 1, brand_id = 1, lieu_luong = N'1 chewable tablet per month, dosed by body weight', gia_tu = 250000.00, dang_kinh_doanh = 1 WHERE product_id = 1;
ELSE
BEGIN
    SET IDENTITY_INSERT products ON;
    INSERT INTO products (product_id, ten, mo_ta, category_id, brand_id, lieu_luong, gia_tu, dang_kinh_doanh) VALUES (1, N'Simparica Trio for Dogs', N'3-in-1 chewable treatment for dogs: kills fleas and ticks, prevents heartworm, and treats intestinal worms.', 1, 1, N'1 chewable tablet per month, dosed by body weight', 250000.00, 1);
    SET IDENTITY_INSERT products OFF;
END

IF EXISTS (SELECT 1 FROM products WHERE product_id = 2)
    UPDATE products SET ten = N'Seresto Collar for Dogs', mo_ta = N'Flea and tick collar for dogs, odorless and water-resistant, protects for up to 8 months.', category_id = 1, brand_id = 6, lieu_luong = N'Wear continuously, replace after 8 months', gia_tu = 180000.00, dang_kinh_doanh = 1 WHERE product_id = 2;
ELSE
BEGIN
    SET IDENTITY_INSERT products ON;
    INSERT INTO products (product_id, ten, mo_ta, category_id, brand_id, lieu_luong, gia_tu, dang_kinh_doanh) VALUES (2, N'Seresto Collar for Dogs', N'Flea and tick collar for dogs, odorless and water-resistant, protects for up to 8 months.', 1, 6, N'Wear continuously, replace after 8 months', 180000.00, 1);
    SET IDENTITY_INSERT products OFF;
END

IF EXISTS (SELECT 1 FROM products WHERE product_id = 3)
    UPDATE products SET ten = N'NexGard Chewables for Dogs', mo_ta = N'Chewable treatment for dogs that kills fleas and ticks.', category_id = 1, brand_id = 11, lieu_luong = N'1 chewable tablet per month', gia_tu = 520000.00, dang_kinh_doanh = 1 WHERE product_id = 3;
ELSE
BEGIN
    SET IDENTITY_INSERT products ON;
    INSERT INTO products (product_id, ten, mo_ta, category_id, brand_id, lieu_luong, gia_tu, dang_kinh_doanh) VALUES (3, N'NexGard Chewables for Dogs', N'Chewable treatment for dogs that kills fleas and ticks.', 1, 11, N'1 chewable tablet per month', 520000.00, 1);
    SET IDENTITY_INSERT products OFF;
END

IF EXISTS (SELECT 1 FROM products WHERE product_id = 4)
    UPDATE products SET ten = N'Frontline Flea & Tick Prevention for Dogs', mo_ta = N'Monthly spot-on treatment for dogs that kills fleas and ticks.', category_id = 1, brand_id = 3, lieu_luong = N'Apply directly to the skin at the back of the neck, once a month', gia_tu = 350000.00, dang_kinh_doanh = 1 WHERE product_id = 4;
ELSE
BEGIN
    SET IDENTITY_INSERT products ON;
    INSERT INTO products (product_id, ten, mo_ta, category_id, brand_id, lieu_luong, gia_tu, dang_kinh_doanh) VALUES (4, N'Frontline Flea & Tick Prevention for Dogs', N'Monthly spot-on treatment for dogs that kills fleas and ticks.', 1, 3, N'Apply directly to the skin at the back of the neck, once a month', 350000.00, 1);
    SET IDENTITY_INSERT products OFF;
END

IF EXISTS (SELECT 1 FROM products WHERE product_id = 5)
    UPDATE products SET ten = N'Capstar Oral Flea Treatment for Dogs', mo_ta = N'Fast-acting oral flea treatment for dogs.', category_id = 1, brand_id = 11, lieu_luong = N'1 tablet per dose, may be repeated daily if needed', gia_tu = 95000.00, dang_kinh_doanh = 1 WHERE product_id = 5;
ELSE
BEGIN
    SET IDENTITY_INSERT products ON;
    INSERT INTO products (product_id, ten, mo_ta, category_id, brand_id, lieu_luong, gia_tu, dang_kinh_doanh) VALUES (5, N'Capstar Oral Flea Treatment for Dogs', N'Fast-acting oral flea treatment for dogs.', 1, 11, N'1 tablet per dose, may be repeated daily if needed', 95000.00, 1);
    SET IDENTITY_INSERT products OFF;
END

IF EXISTS (SELECT 1 FROM products WHERE product_id = 6)
    UPDATE products SET ten = N'Frontline Plus for Cats', mo_ta = N'Spot-on treatment for cats that kills fleas, ticks, and flea eggs.', category_id = 2, brand_id = 3, lieu_luong = N'Apply directly to the skin at the back of the neck, once a month', gia_tu = 160000.00, dang_kinh_doanh = 1 WHERE product_id = 6;
ELSE
BEGIN
    SET IDENTITY_INSERT products ON;
    INSERT INTO products (product_id, ten, mo_ta, category_id, brand_id, lieu_luong, gia_tu, dang_kinh_doanh) VALUES (6, N'Frontline Plus for Cats', N'Spot-on treatment for cats that kills fleas, ticks, and flea eggs.', 2, 3, N'Apply directly to the skin at the back of the neck, once a month', 160000.00, 1);
    SET IDENTITY_INSERT products OFF;
END

IF EXISTS (SELECT 1 FROM products WHERE product_id = 7)
    UPDATE products SET ten = N'Revolution Plus for Cats', mo_ta = N'Spot-on treatment for cats that controls fleas, ear mites, heartworm, and intestinal worms.', category_id = 2, brand_id = 3, lieu_luong = N'Apply directly to the skin at the back of the neck, once a month', gia_tu = 230000.00, dang_kinh_doanh = 1 WHERE product_id = 7;
ELSE
BEGIN
    SET IDENTITY_INSERT products ON;
    INSERT INTO products (product_id, ten, mo_ta, category_id, brand_id, lieu_luong, gia_tu, dang_kinh_doanh) VALUES (7, N'Revolution Plus for Cats', N'Spot-on treatment for cats that controls fleas, ear mites, heartworm, and intestinal worms.', 2, 3, N'Apply directly to the skin at the back of the neck, once a month', 230000.00, 1);
    SET IDENTITY_INSERT products OFF;
END

IF EXISTS (SELECT 1 FROM products WHERE product_id = 8)
    UPDATE products SET ten = N'Capstar Tablets for Cats', mo_ta = N'Oral tablet that kills fleas on cats within 30 minutes.', category_id = 2, brand_id = 11, lieu_luong = N'1 tablet per dose, may be repeated daily if needed', gia_tu = 90000.00, dang_kinh_doanh = 1 WHERE product_id = 8;
ELSE
BEGIN
    SET IDENTITY_INSERT products ON;
    INSERT INTO products (product_id, ten, mo_ta, category_id, brand_id, lieu_luong, gia_tu, dang_kinh_doanh) VALUES (8, N'Capstar Tablets for Cats', N'Oral tablet that kills fleas on cats within 30 minutes.', 2, 11, N'1 tablet per dose, may be repeated daily if needed', 90000.00, 1);
    SET IDENTITY_INSERT products OFF;
END

IF EXISTS (SELECT 1 FROM products WHERE product_id = 9)
    UPDATE products SET ten = N'Bravecto Spot-On for Cats', mo_ta = N'Spot-on treatment for cats that kills fleas and ticks, protects for up to 12 weeks.', category_id = 2, brand_id = 1, lieu_luong = N'Apply directly to the skin at the back of the neck, once every 3 months', gia_tu = 210000.00, dang_kinh_doanh = 1 WHERE product_id = 9;
ELSE
BEGIN
    SET IDENTITY_INSERT products ON;
    INSERT INTO products (product_id, ten, mo_ta, category_id, brand_id, lieu_luong, gia_tu, dang_kinh_doanh) VALUES (9, N'Bravecto Spot-On for Cats', N'Spot-on treatment for cats that kills fleas and ticks, protects for up to 12 weeks.', 2, 1, N'Apply directly to the skin at the back of the neck, once every 3 months', 210000.00, 1);
    SET IDENTITY_INSERT products OFF;
END

IF EXISTS (SELECT 1 FROM products WHERE product_id = 10)
    UPDATE products SET ten = N'Dorwest Horse Herbal Supplement', mo_ta = N'Herbal supplement that supports joint health and digestion in horses.', category_id = 3, brand_id = 6, lieu_luong = N'Mix into the daily feed ration as directed on the packaging', gia_tu = 450000.00, dang_kinh_doanh = 1 WHERE product_id = 10;
ELSE
BEGIN
    SET IDENTITY_INSERT products ON;
    INSERT INTO products (product_id, ten, mo_ta, category_id, brand_id, lieu_luong, gia_tu, dang_kinh_doanh) VALUES (10, N'Dorwest Horse Herbal Supplement', N'Herbal supplement that supports joint health and digestion in horses.', 3, 6, N'Mix into the daily feed ration as directed on the packaging', 450000.00, 1);
    SET IDENTITY_INSERT products OFF;
END

IF EXISTS (SELECT 1 FROM products WHERE product_id = 11)
    UPDATE products SET ten = N'Aristopet Horse Wormer', mo_ta = N'Oral dewormer solution for horses.', category_id = 3, brand_id = 6, lieu_luong = N'Administer orally dosed by body weight, repeat every 3 months', gia_tu = 380000.00, dang_kinh_doanh = 1 WHERE product_id = 11;
ELSE
BEGIN
    SET IDENTITY_INSERT products ON;
    INSERT INTO products (product_id, ten, mo_ta, category_id, brand_id, lieu_luong, gia_tu, dang_kinh_doanh) VALUES (11, N'Aristopet Horse Wormer', N'Oral dewormer solution for horses.', 3, 6, N'Administer orally dosed by body weight, repeat every 3 months', 380000.00, 1);
    SET IDENTITY_INSERT products OFF;
END

IF EXISTS (SELECT 1 FROM products WHERE product_id = 12)
    UPDATE products SET ten = N'Milpro Bird Care Supplement', mo_ta = N'Vitamin and mineral supplement powder that boosts immunity in pet birds.', category_id = 4, brand_id = 6, lieu_luong = N'Sprinkle over food or mix into drinking water daily', gia_tu = 95000.00, dang_kinh_doanh = 1 WHERE product_id = 12;
ELSE
BEGIN
    SET IDENTITY_INSERT products ON;
    INSERT INTO products (product_id, ten, mo_ta, category_id, brand_id, lieu_luong, gia_tu, dang_kinh_doanh) VALUES (12, N'Milpro Bird Care Supplement', N'Vitamin and mineral supplement powder that boosts immunity in pet birds.', 4, 6, N'Sprinkle over food or mix into drinking water daily', 95000.00, 1);
    SET IDENTITY_INSERT products OFF;
END

IF EXISTS (SELECT 1 FROM products WHERE product_id = 13)
    UPDATE products SET ten = N'Bird Mite & Lice Spray', mo_ta = N'Spray treatment for mites and lice on pet birds, safe for direct application to feathers.', category_id = 4, brand_id = 6, lieu_luong = N'Spray from 20cm away from the feathers, once a week', gia_tu = 175000.00, dang_kinh_doanh = 1 WHERE product_id = 13;
ELSE
BEGIN
    SET IDENTITY_INSERT products ON;
    INSERT INTO products (product_id, ten, mo_ta, category_id, brand_id, lieu_luong, gia_tu, dang_kinh_doanh) VALUES (13, N'Bird Mite & Lice Spray', N'Spray treatment for mites and lice on pet birds, safe for direct application to feathers.', 4, 6, N'Spray from 20cm away from the feathers, once a week', 175000.00, 1);
    SET IDENTITY_INSERT products OFF;
END

IF EXISTS (SELECT 1 FROM products WHERE product_id = 14)
    UPDATE products SET ten = N'Revolution for Dogs', mo_ta = N'Spot-on treatment for dogs that controls fleas, ear mites, mange, and heartworm.', category_id = 1, brand_id = 3, lieu_luong = N'Apply directly to the skin at the back of the neck, once a month', gia_tu = 340000.00, dang_kinh_doanh = 1 WHERE product_id = 14;
ELSE
BEGIN
    SET IDENTITY_INSERT products ON;
    INSERT INTO products (product_id, ten, mo_ta, category_id, brand_id, lieu_luong, gia_tu, dang_kinh_doanh) VALUES (14, N'Revolution for Dogs', N'Spot-on treatment for dogs that controls fleas, ear mites, mange, and heartworm.', 1, 3, N'Apply directly to the skin at the back of the neck, once a month', 340000.00, 1);
    SET IDENTITY_INSERT products OFF;
END

IF EXISTS (SELECT 1 FROM products WHERE product_id = 15)
    UPDATE products SET ten = N'Heartgard Plus for Dogs', mo_ta = N'Chewable treatment for dogs that prevents heartworm and treats intestinal worms.', category_id = 1, brand_id = 1, lieu_luong = N'1 chewable tablet per month, dosed by body weight', gia_tu = 290000.00, dang_kinh_doanh = 1 WHERE product_id = 15;
ELSE
BEGIN
    SET IDENTITY_INSERT products ON;
    INSERT INTO products (product_id, ten, mo_ta, category_id, brand_id, lieu_luong, gia_tu, dang_kinh_doanh) VALUES (15, N'Heartgard Plus for Dogs', N'Chewable treatment for dogs that prevents heartworm and treats intestinal worms.', 1, 1, N'1 chewable tablet per month, dosed by body weight', 290000.00, 1);
    SET IDENTITY_INSERT products OFF;
END

IF EXISTS (SELECT 1 FROM products WHERE product_id = 16)
    UPDATE products SET ten = N'Bravecto Chews for Dogs', mo_ta = N'Chewable treatment for dogs that kills fleas and ticks, protects for up to 12 weeks.', category_id = 1, brand_id = 1, lieu_luong = N'1 chewable tablet every 3 months', gia_tu = 280000.00, dang_kinh_doanh = 1 WHERE product_id = 16;
ELSE
BEGIN
    SET IDENTITY_INSERT products ON;
    INSERT INTO products (product_id, ten, mo_ta, category_id, brand_id, lieu_luong, gia_tu, dang_kinh_doanh) VALUES (16, N'Bravecto Chews for Dogs', N'Chewable treatment for dogs that kills fleas and ticks, protects for up to 12 weeks.', 1, 1, N'1 chewable tablet every 3 months', 280000.00, 1);
    SET IDENTITY_INSERT products OFF;
END

IF EXISTS (SELECT 1 FROM products WHERE product_id = 17)
    UPDATE products SET ten = N'Seresto Collar for Cats', mo_ta = N'Flea and tick collar for cats, odorless and water-resistant, protects for up to 8 months.', category_id = 2, brand_id = 6, lieu_luong = N'Wear continuously, replace after 8 months', gia_tu = 260000.00, dang_kinh_doanh = 1 WHERE product_id = 17;
ELSE
BEGIN
    SET IDENTITY_INSERT products ON;
    INSERT INTO products (product_id, ten, mo_ta, category_id, brand_id, lieu_luong, gia_tu, dang_kinh_doanh) VALUES (17, N'Seresto Collar for Cats', N'Flea and tick collar for cats, odorless and water-resistant, protects for up to 8 months.', 2, 6, N'Wear continuously, replace after 8 months', 260000.00, 1);
    SET IDENTITY_INSERT products OFF;
END

IF EXISTS (SELECT 1 FROM products WHERE product_id = 18)
    UPDATE products SET ten = N'Bravecto Plus for Cats', mo_ta = N'Spot-on treatment for cats that kills fleas and ticks and prevents heartworm.', category_id = 2, brand_id = 1, lieu_luong = N'Apply directly to the skin at the back of the neck, once every 3 months', gia_tu = 420000.00, dang_kinh_doanh = 1 WHERE product_id = 18;
ELSE
BEGIN
    SET IDENTITY_INSERT products ON;
    INSERT INTO products (product_id, ten, mo_ta, category_id, brand_id, lieu_luong, gia_tu, dang_kinh_doanh) VALUES (18, N'Bravecto Plus for Cats', N'Spot-on treatment for cats that kills fleas and ticks and prevents heartworm.', 2, 1, N'Apply directly to the skin at the back of the neck, once every 3 months', 420000.00, 1);
    SET IDENTITY_INSERT products OFF;
END

IF EXISTS (SELECT 1 FROM products WHERE product_id = 19)
    UPDATE products SET ten = N'AdvantageMulti for Cats', mo_ta = N'Spot-on treatment for cats that prevents heartworm and treats a broad range of parasites.', category_id = 2, brand_id = 11, lieu_luong = N'Apply directly to the skin at the back of the neck, once a month', gia_tu = 240000.00, dang_kinh_doanh = 1 WHERE product_id = 19;
ELSE
BEGIN
    SET IDENTITY_INSERT products ON;
    INSERT INTO products (product_id, ten, mo_ta, category_id, brand_id, lieu_luong, gia_tu, dang_kinh_doanh) VALUES (19, N'AdvantageMulti for Cats', N'Spot-on treatment for cats that prevents heartworm and treats a broad range of parasites.', 2, 11, N'Apply directly to the skin at the back of the neck, once a month', 240000.00, 1);
    SET IDENTITY_INSERT products OFF;
END

IF EXISTS (SELECT 1 FROM products WHERE product_id = 20)
    UPDATE products SET ten = N'Advantage for Cats', mo_ta = N'Spot-on flea treatment for cats.', category_id = 2, brand_id = 11, lieu_luong = N'Apply directly to the skin at the back of the neck, once a month', gia_tu = 220000.00, dang_kinh_doanh = 1 WHERE product_id = 20;
ELSE
BEGIN
    SET IDENTITY_INSERT products ON;
    INSERT INTO products (product_id, ten, mo_ta, category_id, brand_id, lieu_luong, gia_tu, dang_kinh_doanh) VALUES (20, N'Advantage for Cats', N'Spot-on flea treatment for cats.', 2, 11, N'Apply directly to the skin at the back of the neck, once a month', 220000.00, 1);
    SET IDENTITY_INSERT products OFF;
END

IF EXISTS (SELECT 1 FROM products WHERE product_id = 21)
    UPDATE products SET ten = N'Profender for Cats', mo_ta = N'Spot-on all-wormer treatment for cats that treats roundworms and tapeworms.', category_id = 2, brand_id = 3, lieu_luong = N'Apply directly to the skin at the back of the neck, repeat every 3 months', gia_tu = 240000.00, dang_kinh_doanh = 1 WHERE product_id = 21;
ELSE
BEGIN
    SET IDENTITY_INSERT products ON;
    INSERT INTO products (product_id, ten, mo_ta, category_id, brand_id, lieu_luong, gia_tu, dang_kinh_doanh) VALUES (21, N'Profender for Cats', N'Spot-on all-wormer treatment for cats that treats roundworms and tapeworms.', 2, 3, N'Apply directly to the skin at the back of the neck, repeat every 3 months', 240000.00, 1);
    SET IDENTITY_INSERT products OFF;
END

IF EXISTS (SELECT 1 FROM products WHERE product_id = 22)
    UPDATE products SET ten = N'Dermoscent Essential 6 Skin & Coat Spray for Dogs', mo_ta = N'Fatty-acid spot-on spray that nourishes the skin barrier and improves coat shine for dogs.', category_id = 1, brand_id = 1, lieu_luong = N'Spray directly onto skin along the back, once weekly', gia_tu = 220000.00, dang_kinh_doanh = 1 WHERE product_id = 22;
ELSE
BEGIN
    SET IDENTITY_INSERT products ON;
    INSERT INTO products (product_id, ten, mo_ta, category_id, brand_id, lieu_luong, gia_tu, dang_kinh_doanh) VALUES (22, N'Dermoscent Essential 6 Skin & Coat Spray for Dogs', N'Fatty-acid spot-on spray that nourishes the skin barrier and improves coat shine for dogs.', 1, 1, N'Spray directly onto skin along the back, once weekly', 220000.00, 1);
    SET IDENTITY_INSERT products OFF;
END

IF EXISTS (SELECT 1 FROM products WHERE product_id = 23)
    UPDATE products SET ten = N'Adaptil Calming Diffuser for Dogs', mo_ta = N'Pheromone diffuser that helps reduce stress and anxiety-related behaviors in dogs.', category_id = 1, brand_id = 11, lieu_luong = N'Plug into an electrical outlet in the room your dog uses most, refill lasts about 30 days', gia_tu = 180000.00, dang_kinh_doanh = 1 WHERE product_id = 23;
ELSE
BEGIN
    SET IDENTITY_INSERT products ON;
    INSERT INTO products (product_id, ten, mo_ta, category_id, brand_id, lieu_luong, gia_tu, dang_kinh_doanh) VALUES (23, N'Adaptil Calming Diffuser for Dogs', N'Pheromone diffuser that helps reduce stress and anxiety-related behaviors in dogs.', 1, 11, N'Plug into an electrical outlet in the room your dog uses most, refill lasts about 30 days', 180000.00, 1);
    SET IDENTITY_INSERT products OFF;
END

IF EXISTS (SELECT 1 FROM products WHERE product_id = 24)
    UPDATE products SET ten = N'Frontline Spray for Cats', mo_ta = N'Alcohol-free pump spray that kills fleas and ticks, safe for cats and kittens.', category_id = 2, brand_id = 3, lieu_luong = N'Spray onto coat and massage in, avoiding eyes and mouth, may be reapplied weekly', gia_tu = 150000.00, dang_kinh_doanh = 1 WHERE product_id = 24;
ELSE
BEGIN
    SET IDENTITY_INSERT products ON;
    INSERT INTO products (product_id, ten, mo_ta, category_id, brand_id, lieu_luong, gia_tu, dang_kinh_doanh) VALUES (24, N'Frontline Spray for Cats', N'Alcohol-free pump spray that kills fleas and ticks, safe for cats and kittens.', 2, 3, N'Spray onto coat and massage in, avoiding eyes and mouth, may be reapplied weekly', 150000.00, 1);
    SET IDENTITY_INSERT products OFF;
END


IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 1)
    UPDATE product_variants SET product_id = 1, ten_bien_the = N'Dogs 4.5-10kg', gia = 250000.00, so_luong_ton = 50, sku = N'BRAV-DOG-S', dang_kinh_doanh = 1 WHERE variant_id = 1;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (1, 1, N'Dogs 4.5-10kg', 250000.00, 50, N'BRAV-DOG-S', 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 2)
    UPDATE product_variants SET product_id = 1, ten_bien_the = N'Dogs 10-22kg', gia = 320000.00, so_luong_ton = 35, sku = N'BRAV-DOG-M', dang_kinh_doanh = 1 WHERE variant_id = 2;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (2, 1, N'Dogs 10-22kg', 320000.00, 35, N'BRAV-DOG-M', 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 3)
    UPDATE product_variants SET product_id = 2, ten_bien_the = N'Small Dogs (under 10kg)', gia = 180000.00, so_luong_ton = 35, sku = N'FTL-DOG-S', dang_kinh_doanh = 1 WHERE variant_id = 3;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (3, 2, N'Small Dogs (under 10kg)', 180000.00, 35, N'FTL-DOG-S', 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 4)
    UPDATE product_variants SET product_id = 3, ten_bien_the = N'Large Dogs (>18kg)', gia = 620000.00, so_luong_ton = 19, sku = N'SER-DOG-L', dang_kinh_doanh = 1 WHERE variant_id = 4;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (4, 3, N'Large Dogs (>18kg)', 620000.00, 19, N'SER-DOG-L', 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 5)
    UPDATE product_variants SET product_id = 3, ten_bien_the = N'Small Dogs (<18kg)', gia = 520000.00, so_luong_ton = 25, sku = N'SER-DOG-S', dang_kinh_doanh = 1 WHERE variant_id = 5;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (5, 3, N'Small Dogs (<18kg)', 520000.00, 25, N'SER-DOG-S', 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 6)
    UPDATE product_variants SET product_id = 4, ten_bien_the = N'Dogs 5-10kg', gia = 350000.00, so_luong_ton = 24, sku = N'SIM-DOG-S', dang_kinh_doanh = 1 WHERE variant_id = 6;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (6, 4, N'Dogs 5-10kg', 350000.00, 24, N'SIM-DOG-S', 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 7)
    UPDATE product_variants SET product_id = 5, ten_bien_the = N'6-Tablet Pack', gia = 95000.00, so_luong_ton = 45, sku = N'CAP-DOG-6', dang_kinh_doanh = 1 WHERE variant_id = 7;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (7, 5, N'6-Tablet Pack', 95000.00, 45, N'CAP-DOG-6', 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 8)
    UPDATE product_variants SET product_id = 6, ten_bien_the = N'Adult Cats', gia = 160000.00, so_luong_ton = 35, sku = N'FTL-CAT-A', dang_kinh_doanh = 1 WHERE variant_id = 8;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (8, 6, N'Adult Cats', 160000.00, 35, N'FTL-CAT-A', 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 9)
    UPDATE product_variants SET product_id = 7, ten_bien_the = N'Cats 1.2-2.8kg', gia = 230000.00, so_luong_ton = 25, sku = N'BRAV-CAT-S', dang_kinh_doanh = 1 WHERE variant_id = 9;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (9, 7, N'Cats 1.2-2.8kg', 230000.00, 25, N'BRAV-CAT-S', 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 10)
    UPDATE product_variants SET product_id = 8, ten_bien_the = N'6-Tablet Pack', gia = 90000.00, so_luong_ton = 60, sku = N'CAP-CAT-6', dang_kinh_doanh = 1 WHERE variant_id = 10;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (10, 8, N'6-Tablet Pack', 90000.00, 60, N'CAP-CAT-6', 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 11)
    UPDATE product_variants SET product_id = 9, ten_bien_the = N'Cats 2.5-5kg', gia = 210000.00, so_luong_ton = 20, sku = N'PRO-CAT-M', dang_kinh_doanh = 1 WHERE variant_id = 11;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (11, 9, N'Cats 2.5-5kg', 210000.00, 20, N'PRO-CAT-M', 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 12)
    UPDATE product_variants SET product_id = 10, ten_bien_the = N'500g Box', gia = 450000.00, so_luong_ton = 15, sku = N'DOR-HOR-500', dang_kinh_doanh = 1 WHERE variant_id = 12;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (12, 10, N'500g Box', 450000.00, 15, N'DOR-HOR-500', 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 13)
    UPDATE product_variants SET product_id = 11, ten_bien_the = N'1L Bottle', gia = 380000.00, so_luong_ton = 10, sku = N'ARI-HOR-1L', dang_kinh_doanh = 1 WHERE variant_id = 13;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (13, 11, N'1L Bottle', 380000.00, 10, N'ARI-HOR-1L', 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 14)
    UPDATE product_variants SET product_id = 12, ten_bien_the = N'100g Pack', gia = 95000.00, so_luong_ton = 40, sku = N'MIL-BIRD-100', dang_kinh_doanh = 1 WHERE variant_id = 14;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (14, 12, N'100g Pack', 95000.00, 40, N'MIL-BIRD-100', 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 15)
    UPDATE product_variants SET product_id = 13, ten_bien_the = N'150ml Spray Bottle', gia = 175000.00, so_luong_ton = 18, sku = N'BIRD-SPRAY-150', dang_kinh_doanh = 1 WHERE variant_id = 15;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (15, 13, N'150ml Spray Bottle', 175000.00, 18, N'BIRD-SPRAY-150', 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 16)
    UPDATE product_variants SET product_id = 14, ten_bien_the = N'Dogs 10-20kg', gia = 340000.00, so_luong_ton = 29, sku = N'BRAV-SPOT-DOG-M', dang_kinh_doanh = 1 WHERE variant_id = 16;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (16, 14, N'Dogs 10-20kg', 340000.00, 29, N'BRAV-SPOT-DOG-M', 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 17)
    UPDATE product_variants SET product_id = 15, ten_bien_the = N'Dogs 4-10kg', gia = 290000.00, so_luong_ton = 25, sku = N'ADV-DOG-S', dang_kinh_doanh = 1 WHERE variant_id = 17;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (17, 15, N'Dogs 4-10kg', 290000.00, 25, N'ADV-DOG-S', 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 18)
    UPDATE product_variants SET product_id = 16, ten_bien_the = N'Dogs 10.1-25kg', gia = 310000.00, so_luong_ton = 28, sku = N'NEX-DOG-M', dang_kinh_doanh = 1 WHERE variant_id = 18;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (18, 16, N'Dogs 10.1-25kg', 310000.00, 28, N'NEX-DOG-M', 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 19)
    UPDATE product_variants SET product_id = 17, ten_bien_the = N'Cat Collar', gia = 260000.00, so_luong_ton = 20, sku = N'BRAV-PLUS-CAT-L', dang_kinh_doanh = 1 WHERE variant_id = 19;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (19, 17, N'Cat Collar', 260000.00, 20, N'BRAV-PLUS-CAT-L', 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 20)
    UPDATE product_variants SET product_id = 18, ten_bien_the = N'Cats over 2.8kg', gia = 480000.00, so_luong_ton = 22, sku = N'SER-CAT', dang_kinh_doanh = 1 WHERE variant_id = 20;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (20, 18, N'Cats over 2.8kg', 480000.00, 22, N'SER-CAT', 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 21)
    UPDATE product_variants SET product_id = 19, ten_bien_the = N'Cats 2.5-5kg', gia = 240000.00, so_luong_ton = 18, sku = N'REV-CAT-M', dang_kinh_doanh = 1 WHERE variant_id = 21;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (21, 19, N'Cats 2.5-5kg', 240000.00, 18, N'REV-CAT-M', 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 22)
    UPDATE product_variants SET product_id = 20, ten_bien_the = N'Cats under 2.5kg', gia = 220000.00, so_luong_ton = 24, sku = N'NEX-CAT-S', dang_kinh_doanh = 1 WHERE variant_id = 22;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (22, 20, N'Cats under 2.5kg', 220000.00, 24, N'NEX-CAT-S', 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 27)
    UPDATE product_variants SET product_id = 2, ten_bien_the = N'Large Dogs (over 10kg)', gia = 220000.00, so_luong_ton = 30, sku = NULL, dang_kinh_doanh = 1 WHERE variant_id = 27;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (27, 2, N'Large Dogs (over 10kg)', 220000.00, 30, NULL, 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 28)
    UPDATE product_variants SET product_id = 4, ten_bien_the = N'Dogs 10-20kg', gia = 420000.00, so_luong_ton = 30, sku = NULL, dang_kinh_doanh = 1 WHERE variant_id = 28;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (28, 4, N'Dogs 10-20kg', 420000.00, 30, NULL, 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 29)
    UPDATE product_variants SET product_id = 5, ten_bien_the = N'12-Tablet Pack', gia = 170000.00, so_luong_ton = 30, sku = NULL, dang_kinh_doanh = 1 WHERE variant_id = 29;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (29, 5, N'12-Tablet Pack', 170000.00, 30, NULL, 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 30)
    UPDATE product_variants SET product_id = 6, ten_bien_the = N'Large Cats (over 6kg)', gia = 190000.00, so_luong_ton = 30, sku = NULL, dang_kinh_doanh = 1 WHERE variant_id = 30;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (30, 6, N'Large Cats (over 6kg)', 190000.00, 30, NULL, 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 31)
    UPDATE product_variants SET product_id = 7, ten_bien_the = N'Cats 2.8-6.5kg', gia = 260000.00, so_luong_ton = 30, sku = NULL, dang_kinh_doanh = 1 WHERE variant_id = 31;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (31, 7, N'Cats 2.8-6.5kg', 260000.00, 30, NULL, 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 32)
    UPDATE product_variants SET product_id = 8, ten_bien_the = N'12-Tablet Pack', gia = 160000.00, so_luong_ton = 30, sku = NULL, dang_kinh_doanh = 1 WHERE variant_id = 32;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (32, 8, N'12-Tablet Pack', 160000.00, 30, NULL, 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 33)
    UPDATE product_variants SET product_id = 9, ten_bien_the = N'Cats over 5kg', gia = 260000.00, so_luong_ton = 30, sku = NULL, dang_kinh_doanh = 1 WHERE variant_id = 33;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (33, 9, N'Cats over 5kg', 260000.00, 30, NULL, 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 34)
    UPDATE product_variants SET product_id = 10, ten_bien_the = N'1kg Box', gia = 800000.00, so_luong_ton = 20, sku = NULL, dang_kinh_doanh = 1 WHERE variant_id = 34;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (34, 10, N'1kg Box', 800000.00, 20, NULL, 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 35)
    UPDATE product_variants SET product_id = 11, ten_bien_the = N'2L Bottle', gia = 700000.00, so_luong_ton = 20, sku = NULL, dang_kinh_doanh = 1 WHERE variant_id = 35;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (35, 11, N'2L Bottle', 700000.00, 20, NULL, 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 36)
    UPDATE product_variants SET product_id = 12, ten_bien_the = N'300g Pack', gia = 220000.00, so_luong_ton = 20, sku = NULL, dang_kinh_doanh = 1 WHERE variant_id = 36;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (36, 12, N'300g Pack', 220000.00, 20, NULL, 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 37)
    UPDATE product_variants SET product_id = 13, ten_bien_the = N'300ml Spray Bottle', gia = 320000.00, so_luong_ton = 20, sku = NULL, dang_kinh_doanh = 1 WHERE variant_id = 37;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (37, 13, N'300ml Spray Bottle', 320000.00, 20, NULL, 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 38)
    UPDATE product_variants SET product_id = 14, ten_bien_the = N'Dogs 20-40kg', gia = 420000.00, so_luong_ton = 30, sku = NULL, dang_kinh_doanh = 1 WHERE variant_id = 38;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (38, 14, N'Dogs 20-40kg', 420000.00, 30, NULL, 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 39)
    UPDATE product_variants SET product_id = 15, ten_bien_the = N'Dogs 10-25kg', gia = 380000.00, so_luong_ton = 30, sku = NULL, dang_kinh_doanh = 1 WHERE variant_id = 39;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (39, 15, N'Dogs 10-25kg', 380000.00, 30, NULL, 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 40)
    UPDATE product_variants SET product_id = 16, ten_bien_the = N'Toy Dogs 2-4.5kg', gia = 280000.00, so_luong_ton = 30, sku = NULL, dang_kinh_doanh = 1 WHERE variant_id = 40;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (40, 16, N'Toy Dogs 2-4.5kg', 280000.00, 30, NULL, 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 41)
    UPDATE product_variants SET product_id = 16, ten_bien_the = N'Dogs 25-40kg', gia = 350000.00, so_luong_ton = 30, sku = NULL, dang_kinh_doanh = 1 WHERE variant_id = 41;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (41, 16, N'Dogs 25-40kg', 350000.00, 30, NULL, 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 42)
    UPDATE product_variants SET product_id = 16, ten_bien_the = N'Dogs 40-56kg', gia = 390000.00, so_luong_ton = 30, sku = NULL, dang_kinh_doanh = 1 WHERE variant_id = 42;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (42, 16, N'Dogs 40-56kg', 390000.00, 30, NULL, 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 43)
    UPDATE product_variants SET product_id = 17, ten_bien_the = N'Large Cat Collar', gia = 280000.00, so_luong_ton = 30, sku = NULL, dang_kinh_doanh = 1 WHERE variant_id = 43;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (43, 17, N'Large Cat Collar', 280000.00, 30, NULL, 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 44)
    UPDATE product_variants SET product_id = 18, ten_bien_the = N'Cats 1.2-2.8kg', gia = 420000.00, so_luong_ton = 30, sku = NULL, dang_kinh_doanh = 1 WHERE variant_id = 44;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (44, 18, N'Cats 1.2-2.8kg', 420000.00, 30, NULL, 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 45)
    UPDATE product_variants SET product_id = 19, ten_bien_the = N'Cats over 5kg', gia = 280000.00, so_luong_ton = 30, sku = NULL, dang_kinh_doanh = 1 WHERE variant_id = 45;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (45, 19, N'Cats over 5kg', 280000.00, 30, NULL, 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 46)
    UPDATE product_variants SET product_id = 20, ten_bien_the = N'Cats 2.5-7.5kg', gia = 260000.00, so_luong_ton = 30, sku = NULL, dang_kinh_doanh = 1 WHERE variant_id = 46;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (46, 20, N'Cats 2.5-7.5kg', 260000.00, 30, NULL, 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 47)
    UPDATE product_variants SET product_id = 21, ten_bien_the = N'Small Cats up to 2.5kg', gia = 240000.00, so_luong_ton = 20, sku = N'PROF-CAT-S', dang_kinh_doanh = 1 WHERE variant_id = 47;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (47, 21, N'Small Cats up to 2.5kg', 240000.00, 20, N'PROF-CAT-S', 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 48)
    UPDATE product_variants SET product_id = 21, ten_bien_the = N'Medium Cats 2.5-5kg', gia = 260000.00, so_luong_ton = 30, sku = N'PROF-CAT-M', dang_kinh_doanh = 1 WHERE variant_id = 48;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (48, 21, N'Medium Cats 2.5-5kg', 260000.00, 30, N'PROF-CAT-M', 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 49)
    UPDATE product_variants SET product_id = 22, ten_bien_the = N'Small Dogs 4 Pipettes', gia = 220000.00, so_luong_ton = 20, sku = N'DERM-SC-DOG-S', dang_kinh_doanh = 1 WHERE variant_id = 49;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (49, 22, N'Small Dogs 4 Pipettes', 220000.00, 20, N'DERM-SC-DOG-S', 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 50)
    UPDATE product_variants SET product_id = 22, ten_bien_the = N'Large Dogs 4 Pipettes', gia = 280000.00, so_luong_ton = 30, sku = N'DERM-SC-DOG-L', dang_kinh_doanh = 1 WHERE variant_id = 50;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (50, 22, N'Large Dogs 4 Pipettes', 280000.00, 30, N'DERM-SC-DOG-L', 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 51)
    UPDATE product_variants SET product_id = 23, ten_bien_the = N'Starter Kit (Diffuser + Refill)', gia = 350000.00, so_luong_ton = 20, sku = N'ADAP-CALM-KIT', dang_kinh_doanh = 1 WHERE variant_id = 51;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (51, 23, N'Starter Kit (Diffuser + Refill)', 350000.00, 20, N'ADAP-CALM-KIT', 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 52)
    UPDATE product_variants SET product_id = 23, ten_bien_the = N'Refill Vial', gia = 180000.00, so_luong_ton = 30, sku = N'ADAP-CALM-REFILL', dang_kinh_doanh = 1 WHERE variant_id = 52;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (52, 23, N'Refill Vial', 180000.00, 30, N'ADAP-CALM-REFILL', 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 53)
    UPDATE product_variants SET product_id = 24, ten_bien_the = N'100ml Bottle', gia = 150000.00, so_luong_ton = 20, sku = N'FRONT-SPRAY-CAT-S', dang_kinh_doanh = 1 WHERE variant_id = 53;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (53, 24, N'100ml Bottle', 150000.00, 20, N'FRONT-SPRAY-CAT-S', 1);
    SET IDENTITY_INSERT product_variants OFF;
END

IF EXISTS (SELECT 1 FROM product_variants WHERE variant_id = 54)
    UPDATE product_variants SET product_id = 24, ten_bien_the = N'250ml Bottle', gia = 190000.00, so_luong_ton = 30, sku = N'FRONT-SPRAY-CAT-L', dang_kinh_doanh = 1 WHERE variant_id = 54;
ELSE
BEGIN
    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES (54, 24, N'250ml Bottle', 190000.00, 30, N'FRONT-SPRAY-CAT-L', 1);
    SET IDENTITY_INSERT product_variants OFF;
END


IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 1 AND la_anh_chinh = 1)
    UPDATE product_images SET url = N'/img/1.png' WHERE product_id = 1 AND la_anh_chinh = 1;
ELSE IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 1)
    UPDATE TOP (1) product_images SET url = N'/img/1.png', la_anh_chinh = 1 WHERE product_id = 1;
ELSE
    INSERT INTO product_images (product_id, url, la_anh_chinh) VALUES (1, N'/img/1.png', 1);

IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 2 AND la_anh_chinh = 1)
    UPDATE product_images SET url = N'/img/2.png' WHERE product_id = 2 AND la_anh_chinh = 1;
ELSE IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 2)
    UPDATE TOP (1) product_images SET url = N'/img/2.png', la_anh_chinh = 1 WHERE product_id = 2;
ELSE
    INSERT INTO product_images (product_id, url, la_anh_chinh) VALUES (2, N'/img/2.png', 1);

IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 3 AND la_anh_chinh = 1)
    UPDATE product_images SET url = N'/img/3.png' WHERE product_id = 3 AND la_anh_chinh = 1;
ELSE IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 3)
    UPDATE TOP (1) product_images SET url = N'/img/3.png', la_anh_chinh = 1 WHERE product_id = 3;
ELSE
    INSERT INTO product_images (product_id, url, la_anh_chinh) VALUES (3, N'/img/3.png', 1);

IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 4 AND la_anh_chinh = 1)
    UPDATE product_images SET url = N'/img/4.png' WHERE product_id = 4 AND la_anh_chinh = 1;
ELSE IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 4)
    UPDATE TOP (1) product_images SET url = N'/img/4.png', la_anh_chinh = 1 WHERE product_id = 4;
ELSE
    INSERT INTO product_images (product_id, url, la_anh_chinh) VALUES (4, N'/img/4.png', 1);

IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 5 AND la_anh_chinh = 1)
    UPDATE product_images SET url = N'/img/5.png' WHERE product_id = 5 AND la_anh_chinh = 1;
ELSE IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 5)
    UPDATE TOP (1) product_images SET url = N'/img/5.png', la_anh_chinh = 1 WHERE product_id = 5;
ELSE
    INSERT INTO product_images (product_id, url, la_anh_chinh) VALUES (5, N'/img/5.png', 1);

IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 6 AND la_anh_chinh = 1)
    UPDATE product_images SET url = N'/img/m1.png' WHERE product_id = 6 AND la_anh_chinh = 1;
ELSE IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 6)
    UPDATE TOP (1) product_images SET url = N'/img/m1.png', la_anh_chinh = 1 WHERE product_id = 6;
ELSE
    INSERT INTO product_images (product_id, url, la_anh_chinh) VALUES (6, N'/img/m1.png', 1);

IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 7 AND la_anh_chinh = 1)
    UPDATE product_images SET url = N'/img/m2.png' WHERE product_id = 7 AND la_anh_chinh = 1;
ELSE IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 7)
    UPDATE TOP (1) product_images SET url = N'/img/m2.png', la_anh_chinh = 1 WHERE product_id = 7;
ELSE
    INSERT INTO product_images (product_id, url, la_anh_chinh) VALUES (7, N'/img/m2.png', 1);

IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 8 AND la_anh_chinh = 1)
    UPDATE product_images SET url = N'/img/m3.png' WHERE product_id = 8 AND la_anh_chinh = 1;
ELSE IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 8)
    UPDATE TOP (1) product_images SET url = N'/img/m3.png', la_anh_chinh = 1 WHERE product_id = 8;
ELSE
    INSERT INTO product_images (product_id, url, la_anh_chinh) VALUES (8, N'/img/m3.png', 1);

IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 9 AND la_anh_chinh = 1)
    UPDATE product_images SET url = N'/img/m4.png' WHERE product_id = 9 AND la_anh_chinh = 1;
ELSE IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 9)
    UPDATE TOP (1) product_images SET url = N'/img/m4.png', la_anh_chinh = 1 WHERE product_id = 9;
ELSE
    INSERT INTO product_images (product_id, url, la_anh_chinh) VALUES (9, N'/img/m4.png', 1);

IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 10 AND la_anh_chinh = 1)
    UPDATE product_images SET url = N'/img/Dorwest.png' WHERE product_id = 10 AND la_anh_chinh = 1;
ELSE IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 10)
    UPDATE TOP (1) product_images SET url = N'/img/Dorwest.png', la_anh_chinh = 1 WHERE product_id = 10;
ELSE
    INSERT INTO product_images (product_id, url, la_anh_chinh) VALUES (10, N'/img/Dorwest.png', 1);

IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 11 AND la_anh_chinh = 1)
    UPDATE product_images SET url = N'/img/aristopet.png' WHERE product_id = 11 AND la_anh_chinh = 1;
ELSE IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 11)
    UPDATE TOP (1) product_images SET url = N'/img/aristopet.png', la_anh_chinh = 1 WHERE product_id = 11;
ELSE
    INSERT INTO product_images (product_id, url, la_anh_chinh) VALUES (11, N'/img/aristopet.png', 1);

IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 12 AND la_anh_chinh = 1)
    UPDATE product_images SET url = N'/img/milpro.png' WHERE product_id = 12 AND la_anh_chinh = 1;
ELSE IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 12)
    UPDATE TOP (1) product_images SET url = N'/img/milpro.png', la_anh_chinh = 1 WHERE product_id = 12;
ELSE
    INSERT INTO product_images (product_id, url, la_anh_chinh) VALUES (12, N'/img/milpro.png', 1);

IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 13 AND la_anh_chinh = 1)
    UPDATE product_images SET url = N'/img/2.png' WHERE product_id = 13 AND la_anh_chinh = 1;
ELSE IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 13)
    UPDATE TOP (1) product_images SET url = N'/img/2.png', la_anh_chinh = 1 WHERE product_id = 13;
ELSE
    INSERT INTO product_images (product_id, url, la_anh_chinh) VALUES (13, N'/img/2.png', 1);

IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 14 AND la_anh_chinh = 1)
    UPDATE product_images SET url = N'/img/7.png' WHERE product_id = 14 AND la_anh_chinh = 1;
ELSE IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 14)
    UPDATE TOP (1) product_images SET url = N'/img/7.png', la_anh_chinh = 1 WHERE product_id = 14;
ELSE
    INSERT INTO product_images (product_id, url, la_anh_chinh) VALUES (14, N'/img/7.png', 1);

IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 15 AND la_anh_chinh = 1)
    UPDATE product_images SET url = N'/img/cho0.png' WHERE product_id = 15 AND la_anh_chinh = 1;
ELSE IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 15)
    UPDATE TOP (1) product_images SET url = N'/img/cho0.png', la_anh_chinh = 1 WHERE product_id = 15;
ELSE
    INSERT INTO product_images (product_id, url, la_anh_chinh) VALUES (15, N'/img/cho0.png', 1);

IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 16 AND la_anh_chinh = 1)
    UPDATE product_images SET url = N'/img/cho00.png' WHERE product_id = 16 AND la_anh_chinh = 1;
ELSE IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 16)
    UPDATE TOP (1) product_images SET url = N'/img/cho00.png', la_anh_chinh = 1 WHERE product_id = 16;
ELSE
    INSERT INTO product_images (product_id, url, la_anh_chinh) VALUES (16, N'/img/cho00.png', 1);

IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 17 AND la_anh_chinh = 1)
    UPDATE product_images SET url = N'/img/m5.png' WHERE product_id = 17 AND la_anh_chinh = 1;
ELSE IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 17)
    UPDATE TOP (1) product_images SET url = N'/img/m5.png', la_anh_chinh = 1 WHERE product_id = 17;
ELSE
    INSERT INTO product_images (product_id, url, la_anh_chinh) VALUES (17, N'/img/m5.png', 1);

IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 18 AND la_anh_chinh = 1)
    UPDATE product_images SET url = N'/img/m6.png' WHERE product_id = 18 AND la_anh_chinh = 1;
ELSE IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 18)
    UPDATE TOP (1) product_images SET url = N'/img/m6.png', la_anh_chinh = 1 WHERE product_id = 18;
ELSE
    INSERT INTO product_images (product_id, url, la_anh_chinh) VALUES (18, N'/img/m6.png', 1);

IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 19 AND la_anh_chinh = 1)
    UPDATE product_images SET url = N'/img/m7.png' WHERE product_id = 19 AND la_anh_chinh = 1;
ELSE IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 19)
    UPDATE TOP (1) product_images SET url = N'/img/m7.png', la_anh_chinh = 1 WHERE product_id = 19;
ELSE
    INSERT INTO product_images (product_id, url, la_anh_chinh) VALUES (19, N'/img/m7.png', 1);

IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 20 AND la_anh_chinh = 1)
    UPDATE product_images SET url = N'/img/m8.png' WHERE product_id = 20 AND la_anh_chinh = 1;
ELSE IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 20)
    UPDATE TOP (1) product_images SET url = N'/img/m8.png', la_anh_chinh = 1 WHERE product_id = 20;
ELSE
    INSERT INTO product_images (product_id, url, la_anh_chinh) VALUES (20, N'/img/m8.png', 1);

IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 21 AND la_anh_chinh = 1)
    UPDATE product_images SET url = N'/img/meo00.png' WHERE product_id = 21 AND la_anh_chinh = 1;
ELSE IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 21)
    UPDATE TOP (1) product_images SET url = N'/img/meo00.png', la_anh_chinh = 1 WHERE product_id = 21;
ELSE
    INSERT INTO product_images (product_id, url, la_anh_chinh) VALUES (21, N'/img/meo00.png', 1);

IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 22 AND la_anh_chinh = 1)
    UPDATE product_images SET url = N'/img/3.png' WHERE product_id = 22 AND la_anh_chinh = 1;
ELSE IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 22)
    UPDATE TOP (1) product_images SET url = N'/img/3.png', la_anh_chinh = 1 WHERE product_id = 22;
ELSE
    INSERT INTO product_images (product_id, url, la_anh_chinh) VALUES (22, N'/img/3.png', 1);

IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 23 AND la_anh_chinh = 1)
    UPDATE product_images SET url = N'/img/6.png' WHERE product_id = 23 AND la_anh_chinh = 1;
ELSE IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 23)
    UPDATE TOP (1) product_images SET url = N'/img/6.png', la_anh_chinh = 1 WHERE product_id = 23;
ELSE
    INSERT INTO product_images (product_id, url, la_anh_chinh) VALUES (23, N'/img/6.png', 1);

IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 24 AND la_anh_chinh = 1)
    UPDATE product_images SET url = N'/img/m2.png' WHERE product_id = 24 AND la_anh_chinh = 1;
ELSE IF EXISTS (SELECT 1 FROM product_images WHERE product_id = 24)
    UPDATE TOP (1) product_images SET url = N'/img/m2.png', la_anh_chinh = 1 WHERE product_id = 24;
ELSE
    INSERT INTO product_images (product_id, url, la_anh_chinh) VALUES (24, N'/img/m2.png', 1);

IF NOT EXISTS (SELECT 1 FROM product_conditions WHERE product_id = 1 AND condition_id = 1)
    INSERT INTO product_conditions (product_id, condition_id) VALUES (1, 1);
IF NOT EXISTS (SELECT 1 FROM product_conditions WHERE product_id = 1 AND condition_id = 2)
    INSERT INTO product_conditions (product_id, condition_id) VALUES (1, 2);
IF NOT EXISTS (SELECT 1 FROM product_conditions WHERE product_id = 1 AND condition_id = 3)
    INSERT INTO product_conditions (product_id, condition_id) VALUES (1, 3);
IF NOT EXISTS (SELECT 1 FROM product_conditions WHERE product_id = 2 AND condition_id = 1)
    INSERT INTO product_conditions (product_id, condition_id) VALUES (2, 1);
IF NOT EXISTS (SELECT 1 FROM product_conditions WHERE product_id = 3 AND condition_id = 1)
    INSERT INTO product_conditions (product_id, condition_id) VALUES (3, 1);
IF NOT EXISTS (SELECT 1 FROM product_conditions WHERE product_id = 4 AND condition_id = 1)
    INSERT INTO product_conditions (product_id, condition_id) VALUES (4, 1);
IF NOT EXISTS (SELECT 1 FROM product_conditions WHERE product_id = 5 AND condition_id = 1)
    INSERT INTO product_conditions (product_id, condition_id) VALUES (5, 1);
IF NOT EXISTS (SELECT 1 FROM product_conditions WHERE product_id = 6 AND condition_id = 1)
    INSERT INTO product_conditions (product_id, condition_id) VALUES (6, 1);
IF NOT EXISTS (SELECT 1 FROM product_conditions WHERE product_id = 7 AND condition_id = 1)
    INSERT INTO product_conditions (product_id, condition_id) VALUES (7, 1);
IF NOT EXISTS (SELECT 1 FROM product_conditions WHERE product_id = 7 AND condition_id = 2)
    INSERT INTO product_conditions (product_id, condition_id) VALUES (7, 2);
IF NOT EXISTS (SELECT 1 FROM product_conditions WHERE product_id = 7 AND condition_id = 3)
    INSERT INTO product_conditions (product_id, condition_id) VALUES (7, 3);
IF NOT EXISTS (SELECT 1 FROM product_conditions WHERE product_id = 8 AND condition_id = 1)
    INSERT INTO product_conditions (product_id, condition_id) VALUES (8, 1);
IF NOT EXISTS (SELECT 1 FROM product_conditions WHERE product_id = 9 AND condition_id = 1)
    INSERT INTO product_conditions (product_id, condition_id) VALUES (9, 1);
IF NOT EXISTS (SELECT 1 FROM product_conditions WHERE product_id = 10 AND condition_id = 4)
    INSERT INTO product_conditions (product_id, condition_id) VALUES (10, 4);
IF NOT EXISTS (SELECT 1 FROM product_conditions WHERE product_id = 11 AND condition_id = 2)
    INSERT INTO product_conditions (product_id, condition_id) VALUES (11, 2);
IF NOT EXISTS (SELECT 1 FROM product_conditions WHERE product_id = 14 AND condition_id = 1)
    INSERT INTO product_conditions (product_id, condition_id) VALUES (14, 1);
IF NOT EXISTS (SELECT 1 FROM product_conditions WHERE product_id = 14 AND condition_id = 3)
    INSERT INTO product_conditions (product_id, condition_id) VALUES (14, 3);
IF NOT EXISTS (SELECT 1 FROM product_conditions WHERE product_id = 15 AND condition_id = 2)
    INSERT INTO product_conditions (product_id, condition_id) VALUES (15, 2);
IF NOT EXISTS (SELECT 1 FROM product_conditions WHERE product_id = 15 AND condition_id = 3)
    INSERT INTO product_conditions (product_id, condition_id) VALUES (15, 3);
IF NOT EXISTS (SELECT 1 FROM product_conditions WHERE product_id = 16 AND condition_id = 1)
    INSERT INTO product_conditions (product_id, condition_id) VALUES (16, 1);
IF NOT EXISTS (SELECT 1 FROM product_conditions WHERE product_id = 17 AND condition_id = 1)
    INSERT INTO product_conditions (product_id, condition_id) VALUES (17, 1);
IF NOT EXISTS (SELECT 1 FROM product_conditions WHERE product_id = 18 AND condition_id = 1)
    INSERT INTO product_conditions (product_id, condition_id) VALUES (18, 1);
IF NOT EXISTS (SELECT 1 FROM product_conditions WHERE product_id = 18 AND condition_id = 3)
    INSERT INTO product_conditions (product_id, condition_id) VALUES (18, 3);
IF NOT EXISTS (SELECT 1 FROM product_conditions WHERE product_id = 19 AND condition_id = 3)
    INSERT INTO product_conditions (product_id, condition_id) VALUES (19, 3);
IF NOT EXISTS (SELECT 1 FROM product_conditions WHERE product_id = 20 AND condition_id = 1)
    INSERT INTO product_conditions (product_id, condition_id) VALUES (20, 1);
IF NOT EXISTS (SELECT 1 FROM product_conditions WHERE product_id = 21 AND condition_id = 2)
    INSERT INTO product_conditions (product_id, condition_id) VALUES (21, 2);
IF NOT EXISTS (SELECT 1 FROM product_conditions WHERE product_id = 22 AND condition_id = 5)
    INSERT INTO product_conditions (product_id, condition_id) VALUES (22, 5);
IF NOT EXISTS (SELECT 1 FROM product_conditions WHERE product_id = 23 AND condition_id = 6)
    INSERT INTO product_conditions (product_id, condition_id) VALUES (23, 6);
IF NOT EXISTS (SELECT 1 FROM product_conditions WHERE product_id = 24 AND condition_id = 1)
    INSERT INTO product_conditions (product_id, condition_id) VALUES (24, 1);

");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Migration này chỉ đồng bộ dữ liệu (không đổi cấu trúc bảng) nên không có cách nào
            // khôi phục lại đúng "trạng thái lệch" trước đó của từng máy - Down để trống có chủ đích.
        }
    }
}
