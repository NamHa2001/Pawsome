using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedNewProductsAndConsolidateBrands : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Catalog gốc (products/brands/variants/images) chưa từng được seed qua migration -
            // được insert thủ công ngoài migration history từ trước, nên mỗi máy dev có thể đã có
            // sẵn 20 sản phẩm/16 brand gốc với đúng ID như nhau (cùng 1 script gốc). Migration này
            // chỉ áp dụng ĐÚNG phần thay đổi hôm nay (4 sản phẩm mới + gộp brand), có guard
            // IF NOT EXISTS/điều kiện WHERE để chạy an toàn nhiều lần trên nhiều máy khác nhau,
            // kể cả máy đã có sẵn data này rồi (sẽ tự bỏ qua, không lỗi trùng khóa).
            migrationBuilder.Sql(@"
-- Gộp brand: chỉ giữ 4 brand (Bravecto=1, Frontline Plus=3, Seresto=6, NexGard=11), gán lại brand_id
-- cho các sản phẩm đang thuộc brand khác trước khi xóa các brand đó.
UPDATE products SET brand_id = 1 WHERE product_id IN (1, 15, 16, 18, 22) AND (brand_id IS NULL OR brand_id <> 1);
UPDATE products SET brand_id = 11 WHERE product_id IN (3, 5, 8, 19, 20, 23) AND (brand_id IS NULL OR brand_id <> 11);
UPDATE products SET brand_id = 3 WHERE product_id IN (4, 6, 7, 14, 21) AND (brand_id IS NULL OR brand_id <> 3);
UPDATE products SET brand_id = 6 WHERE product_id IN (2, 10, 11, 12, 13, 17) AND (brand_id IS NULL OR brand_id <> 6);

DELETE FROM brands WHERE brand_id IN (2, 4, 5, 7, 8, 9, 10, 12, 13, 14, 15, 16);

-- 4 sản phẩm mới (Profender for Cats, Dermoscent Essential 6 Skin & Coat Spray for Dogs,
-- Adaptil Calming Diffuser for Dogs, Frontline Spray for Cats) - lấp 2 Condition trước đó chưa có
-- sản phẩm nào (Skin & Coat, Behavioural) và làm tròn nhóm Frontline Plus đủ 6 sản phẩm.
IF NOT EXISTS (SELECT 1 FROM products WHERE product_id = 21)
BEGIN
    SET IDENTITY_INSERT products ON;
    INSERT INTO products (product_id, ten, mo_ta, category_id, brand_id, lieu_luong, gia_tu, dang_kinh_doanh)
    VALUES (21, N'Profender for Cats', N'Spot-on all-wormer treatment for cats that treats roundworms and tapeworms.', 2, 3, N'Apply directly to the skin at the back of the neck, repeat every 3 months', 240000, 1);
    SET IDENTITY_INSERT products OFF;

    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES
        (47, 21, N'Small Cats up to 2.5kg', 240000, 20, N'PROF-CAT-S', 1),
        (48, 21, N'Medium Cats 2.5-5kg', 260000, 30, N'PROF-CAT-M', 1);
    SET IDENTITY_INSERT product_variants OFF;

    SET IDENTITY_INSERT product_images ON;
    INSERT INTO product_images (image_id, product_id, url, la_anh_chinh) VALUES (21, 21, N'/img/meo00.png', 1);
    SET IDENTITY_INSERT product_images OFF;

    INSERT INTO product_conditions (product_id, condition_id) VALUES (21, 2);
END

IF NOT EXISTS (SELECT 1 FROM products WHERE product_id = 22)
BEGIN
    SET IDENTITY_INSERT products ON;
    INSERT INTO products (product_id, ten, mo_ta, category_id, brand_id, lieu_luong, gia_tu, dang_kinh_doanh)
    VALUES (22, N'Dermoscent Essential 6 Skin & Coat Spray for Dogs', N'Fatty-acid spot-on spray that nourishes the skin barrier and improves coat shine for dogs.', 1, 1, N'Spray directly onto skin along the back, once weekly', 220000, 1);
    SET IDENTITY_INSERT products OFF;

    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES
        (49, 22, N'Small Dogs 4 Pipettes', 220000, 20, N'DERM-SC-DOG-S', 1),
        (50, 22, N'Large Dogs 4 Pipettes', 280000, 30, N'DERM-SC-DOG-L', 1);
    SET IDENTITY_INSERT product_variants OFF;

    SET IDENTITY_INSERT product_images ON;
    INSERT INTO product_images (image_id, product_id, url, la_anh_chinh) VALUES (22, 22, N'/img/3.png', 1);
    SET IDENTITY_INSERT product_images OFF;

    INSERT INTO product_conditions (product_id, condition_id) VALUES (22, 5);
END

IF NOT EXISTS (SELECT 1 FROM products WHERE product_id = 23)
BEGIN
    SET IDENTITY_INSERT products ON;
    INSERT INTO products (product_id, ten, mo_ta, category_id, brand_id, lieu_luong, gia_tu, dang_kinh_doanh)
    VALUES (23, N'Adaptil Calming Diffuser for Dogs', N'Pheromone diffuser that helps reduce stress and anxiety-related behaviors in dogs.', 1, 11, N'Plug into an electrical outlet in the room your dog uses most, refill lasts about 30 days', 180000, 1);
    SET IDENTITY_INSERT products OFF;

    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES
        (51, 23, N'Starter Kit (Diffuser + Refill)', 350000, 20, N'ADAP-CALM-KIT', 1),
        (52, 23, N'Refill Vial', 180000, 30, N'ADAP-CALM-REFILL', 1);
    SET IDENTITY_INSERT product_variants OFF;

    SET IDENTITY_INSERT product_images ON;
    INSERT INTO product_images (image_id, product_id, url, la_anh_chinh) VALUES (23, 23, N'/img/6.png', 1);
    SET IDENTITY_INSERT product_images OFF;

    INSERT INTO product_conditions (product_id, condition_id) VALUES (23, 6);
END

IF NOT EXISTS (SELECT 1 FROM products WHERE product_id = 24)
BEGIN
    SET IDENTITY_INSERT products ON;
    INSERT INTO products (product_id, ten, mo_ta, category_id, brand_id, lieu_luong, gia_tu, dang_kinh_doanh)
    VALUES (24, N'Frontline Spray for Cats', N'Alcohol-free pump spray that kills fleas and ticks, safe for cats and kittens.', 2, 3, N'Spray onto coat and massage in, avoiding eyes and mouth, may be reapplied weekly', 150000, 1);
    SET IDENTITY_INSERT products OFF;

    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku, dang_kinh_doanh) VALUES
        (53, 24, N'100ml Bottle', 150000, 20, N'FRONT-SPRAY-CAT-S', 1),
        (54, 24, N'250ml Bottle', 190000, 30, N'FRONT-SPRAY-CAT-L', 1);
    SET IDENTITY_INSERT product_variants OFF;

    SET IDENTITY_INSERT product_images ON;
    INSERT INTO product_images (image_id, product_id, url, la_anh_chinh) VALUES (24, 24, N'/img/m2.png', 1);
    SET IDENTITY_INSERT product_images OFF;

    INSERT INTO product_conditions (product_id, condition_id) VALUES (24, 1);
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
-- Xóa 4 sản phẩm mới (variants/images/conditions tự xóa theo do FK Cascade).
DELETE FROM products WHERE product_id IN (21, 22, 23, 24);

-- Khôi phục 12 brand đã xóa.
SET IDENTITY_INSERT brands ON;
IF NOT EXISTS (SELECT 1 FROM brands WHERE brand_id = 2)
INSERT INTO brands (brand_id, ten_thuong_hieu) VALUES (2, N'Capstar');
IF NOT EXISTS (SELECT 1 FROM brands WHERE brand_id = 4)
INSERT INTO brands (brand_id, ten_thuong_hieu) VALUES (4, N'Profender');
IF NOT EXISTS (SELECT 1 FROM brands WHERE brand_id = 5)
INSERT INTO brands (brand_id, ten_thuong_hieu) VALUES (5, N'Simparica Trio');
IF NOT EXISTS (SELECT 1 FROM brands WHERE brand_id = 7)
INSERT INTO brands (brand_id, ten_thuong_hieu) VALUES (7, N'Dorwest');
IF NOT EXISTS (SELECT 1 FROM brands WHERE brand_id = 8)
INSERT INTO brands (brand_id, ten_thuong_hieu) VALUES (8, N'Aristopet');
IF NOT EXISTS (SELECT 1 FROM brands WHERE brand_id = 9)
INSERT INTO brands (brand_id, ten_thuong_hieu) VALUES (9, N'Milpro');
IF NOT EXISTS (SELECT 1 FROM brands WHERE brand_id = 10)
INSERT INTO brands (brand_id, ten_thuong_hieu) VALUES (10, N'Advantage Multi');
IF NOT EXISTS (SELECT 1 FROM brands WHERE brand_id = 12)
INSERT INTO brands (brand_id, ten_thuong_hieu) VALUES (12, N'Revolution Plus');
IF NOT EXISTS (SELECT 1 FROM brands WHERE brand_id = 13)
INSERT INTO brands (brand_id, ten_thuong_hieu) VALUES (13, N'Heartgard');
IF NOT EXISTS (SELECT 1 FROM brands WHERE brand_id = 14)
INSERT INTO brands (brand_id, ten_thuong_hieu) VALUES (14, N'Advantage');
IF NOT EXISTS (SELECT 1 FROM brands WHERE brand_id = 15)
INSERT INTO brands (brand_id, ten_thuong_hieu) VALUES (15, N'Dermoscent');
IF NOT EXISTS (SELECT 1 FROM brands WHERE brand_id = 16)
INSERT INTO brands (brand_id, ten_thuong_hieu) VALUES (16, N'Adaptil');
SET IDENTITY_INSERT brands OFF;

-- Trả brand_id của từng sản phẩm về đúng giá trị gốc trước khi gộp.
UPDATE products SET brand_id = 5 WHERE product_id = 1;
UPDATE products SET brand_id = 6 WHERE product_id = 2;
UPDATE products SET brand_id = 11 WHERE product_id = 3;
UPDATE products SET brand_id = 3 WHERE product_id = 4;
UPDATE products SET brand_id = 2 WHERE product_id = 5;
UPDATE products SET brand_id = 3 WHERE product_id = 6;
UPDATE products SET brand_id = 12 WHERE product_id = 7;
UPDATE products SET brand_id = 2 WHERE product_id = 8;
UPDATE products SET brand_id = 1 WHERE product_id = 9;
UPDATE products SET brand_id = 7 WHERE product_id = 10;
UPDATE products SET brand_id = 8 WHERE product_id = 11;
UPDATE products SET brand_id = 9 WHERE product_id = 12;
UPDATE products SET brand_id = NULL WHERE product_id = 13;
UPDATE products SET brand_id = 12 WHERE product_id = 14;
UPDATE products SET brand_id = 13 WHERE product_id = 15;
UPDATE products SET brand_id = 1 WHERE product_id = 16;
UPDATE products SET brand_id = 6 WHERE product_id = 17;
UPDATE products SET brand_id = 1 WHERE product_id = 18;
UPDATE products SET brand_id = 10 WHERE product_id = 19;
UPDATE products SET brand_id = 14 WHERE product_id = 20;
");
        }
    }
}
