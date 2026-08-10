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
        (1, N'Simparica Trio for Dogs', N'Thuốc nhai 3 tác dụng: trị ve rận, phòng giun tim, trị giun sán cho chó.', 1, 5, N'1 viên nhai/tháng theo cân nặng'),
        (2, N'Seresto Collar for Dogs', N'Vòng cổ chống ve rận cho chó, không mùi, chống nước, bảo vệ tới 8 tháng.', 1, 6, N'Đeo liên tục, thay mới sau 8 tháng'),
        (3, N'NexGard Chewables for Dogs', N'Thuốc nhai trị bọ chét và ve cho chó.', 1, 11, N'1 viên nhai/tháng'),
        (4, N'Frontline Flea & Tick Prevention for Dogs', N'Thuốc nhỏ gáy trị bọ chét và ve cho chó, dùng hàng tháng.', 1, 3, N'Nhỏ trực tiếp lên da gáy, 1 tháng/lần'),
        (5, N'Capstar Oral Flea Treatment for Dogs', N'Viên uống diệt bọ chét nhanh cho chó.', 1, 2, N'1 viên/lần, có thể lặp lại mỗi ngày nếu cần'),
        (6, N'Frontline Plus for Cats', N'Thuốc nhỏ gáy trị bọ chét, ve và trứng ve cho mèo.', 2, 3, N'Nhỏ trực tiếp lên da gáy, 1 tháng/lần'),
        (7, N'Revolution Plus for Cats', N'Thuốc nhỏ gáy trị bọ chét, ve tai, giun tim và giun sán cho mèo.', 2, 12, N'Nhỏ trực tiếp lên da gáy, 1 tháng/lần'),
        (8, N'Capstar Tablets for Cats', N'Viên uống diệt bọ chét nhanh trong 30 phút cho mèo.', 2, 2, N'1 viên/lần, có thể lặp lại mỗi ngày nếu cần'),
        (9, N'Bravecto Spot-On for Cats', N'Thuốc nhỏ gáy trị bọ chét và ve cho mèo, bảo vệ tới 12 tuần.', 2, 1, N'Nhỏ trực tiếp lên da gáy, 3 tháng/lần'),
        (10, N'Dorwest Horse Herbal Supplement', N'Thực phẩm bổ sung thảo dược hỗ trợ khớp và tiêu hóa cho ngựa.', 3, 7, N'Trộn vào khẩu phần ăn hàng ngày theo hướng dẫn trên bao bì'),
        (11, N'Aristopet Horse Wormer', N'Dung dịch tẩy giun sán đường uống cho ngựa.', 3, 8, N'Uống trực tiếp theo cân nặng, lặp lại mỗi 3 tháng'),
        (12, N'Milpro Bird Care Supplement', N'Bột bổ sung vitamin và khoáng chất tăng đề kháng cho chim cảnh.', 4, 9, N'Rắc lên thức ăn hoặc pha vào nước uống hàng ngày'),
        (13, N'Bird Mite & Lice Spray', N'Xịt trị rận mạt lông cho chim cảnh, an toàn khi phun trực tiếp lên lông.', 4, NULL, N'Xịt cách lông 20cm, 1 lần/tuần'),
        (14, N'Revolution for Dogs', N'Thuốc nhỏ gáy trị bọ chét, ve tai, ghẻ và giun tim cho chó.', 1, 12, N'Nhỏ trực tiếp lên da gáy, 1 tháng/lần'),
        (15, N'Heartgard Plus for Dogs', N'Thuốc nhai phòng giun tim và trị giun sán đường ruột cho chó.', 1, 13, N'1 viên nhai/tháng theo cân nặng'),
        (16, N'Bravecto Chews for Dogs', N'Thuốc nhai trị ve rận cho chó, bảo vệ tới 12 tuần.', 1, 1, N'1 viên nhai/3 tháng'),
        (17, N'Seresto Collar for Cats', N'Vòng cổ chống ve rận cho mèo, không mùi, chống nước, bảo vệ tới 8 tháng.', 2, 6, N'Đeo liên tục, thay mới sau 8 tháng'),
        (18, N'Bravecto Plus for Cats', N'Thuốc nhỏ gáy trị ve rận và phòng giun tim cho mèo.', 2, 1, N'Nhỏ trực tiếp lên da gáy, 3 tháng/lần'),
        (19, N'AdvantageMulti for Cats', N'Thuốc nhỏ gáy phòng giun tim và trị ký sinh trùng phổ rộng cho mèo.', 2, 10, N'Nhỏ trực tiếp lên da gáy, 1 tháng/lần'),
        (20, N'Advantage for Cats', N'Thuốc nhỏ gáy trị bọ chét cho mèo.', 2, 14, N'Nhỏ trực tiếp lên da gáy, 1 tháng/lần');
    SET IDENTITY_INSERT products OFF;

    SET IDENTITY_INSERT product_variants ON;
    INSERT INTO product_variants (variant_id, product_id, ten_bien_the, gia, so_luong_ton, sku) VALUES
        (1, 1, N'Chó 4.5-10kg', 250000, 50, N'SIM-DOG-S'),
        (2, 1, N'Chó 10-22kg', 320000, 35, N'SIM-DOG-M'),
        (3, 2, N'Chó nhỏ dưới 10kg', 180000, 40, N'SER-DOG-S'),
        (4, 3, N'Chó lớn (>18kg)', 620000, 20, N'NEX-DOG-L'),
        (5, 3, N'Chó nhỏ (<18kg)', 520000, 25, N'NEX-DOG-S'),
        (6, 4, N'Chó 5-10kg', 350000, 30, N'FTL-DOG-S'),
        (7, 5, N'Vỉ 6 viên', 95000, 45, N'CAP-DOG-6'),
        (8, 6, N'Mèo trưởng thành', 160000, 35, N'FTL-CAT-A'),
        (9, 7, N'Mèo 1.2-2.8kg', 230000, 25, N'REV-CAT-S'),
        (10, 8, N'Vỉ 6 viên', 90000, 60, N'CAP-CAT-6'),
        (11, 9, N'Mèo 2.5-5kg', 210000, 20, N'BRAV-SPOT-CAT-M'),
        (12, 10, N'Hộp 500g', 450000, 15, N'DOR-HOR-500'),
        (13, 11, N'Chai 1L', 380000, 10, N'ARI-HOR-1L'),
        (14, 12, N'Gói 100g', 95000, 40, N'MIL-BIRD-100'),
        (15, 13, N'Chai xịt 150ml', 175000, 18, N'BIRD-SPRAY-150'),
        (16, 14, N'Chó 10-20kg', 340000, 30, N'REV-DOG-M'),
        (17, 15, N'Chó 4-10kg', 290000, 25, N'HGD-DOG-S'),
        (18, 16, N'Chó 10.1-25kg', 310000, 28, N'BRAV-DOG-M'),
        (19, 17, N'Vòng cổ mèo', 480000, 22, N'SER-CAT'),
        (20, 18, N'Mèo trên 2.8kg', 260000, 20, N'BRAV-PLUS-CAT-L'),
        (21, 19, N'Mèo 2.5-5kg', 240000, 18, N'ADVM-CAT-M'),
        (22, 20, N'Mèo dưới 2.5kg', 220000, 24, N'ADV-CAT-S');
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
