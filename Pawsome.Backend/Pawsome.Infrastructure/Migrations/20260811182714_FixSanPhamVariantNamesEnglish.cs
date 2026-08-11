using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixSanPhamVariantNamesEnglish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Bản vá cho migration TranslateSanPhamDataToEnglish: SKU thực tế trên DB đã lệch so
            // với nội dung ghi trong file migration SeedSanPhamData gốc (khả năng do có script
            // sqlcmd sửa tay trực tiếp DB trước đó mà không đồng bộ lại vào migration), nên điều
            // kiện khớp theo sku ở migration trước bị trật với hầu hết các dòng. Đổi sang khớp
            // theo product_id + variant_id (đã xác nhận đúng thực tế qua sqlcmd, ổn định hơn sku).
            migrationBuilder.Sql(@"
UPDATE product_variants SET ten_bien_the = N'Dogs 4.5-10kg' WHERE variant_id = 1 AND product_id = 1;
UPDATE product_variants SET ten_bien_the = N'Dogs 10-22kg' WHERE variant_id = 2 AND product_id = 1;
UPDATE product_variants SET ten_bien_the = N'Small Dogs (under 10kg)' WHERE variant_id = 3 AND product_id = 2;
UPDATE product_variants SET ten_bien_the = N'Large Dogs (>18kg)' WHERE variant_id = 4 AND product_id = 3;
UPDATE product_variants SET ten_bien_the = N'Small Dogs (<18kg)' WHERE variant_id = 5 AND product_id = 3;
UPDATE product_variants SET ten_bien_the = N'Dogs 5-10kg' WHERE variant_id = 6 AND product_id = 4;
UPDATE product_variants SET ten_bien_the = N'6-Tablet Pack' WHERE variant_id = 7 AND product_id = 5;
UPDATE product_variants SET ten_bien_the = N'Adult Cats' WHERE variant_id = 8 AND product_id = 6;
UPDATE product_variants SET ten_bien_the = N'Cats 1.2-2.8kg' WHERE variant_id = 9 AND product_id = 7;
UPDATE product_variants SET ten_bien_the = N'6-Tablet Pack' WHERE variant_id = 10 AND product_id = 8;
UPDATE product_variants SET ten_bien_the = N'Cats 2.5-5kg' WHERE variant_id = 11 AND product_id = 9;
UPDATE product_variants SET ten_bien_the = N'500g Box' WHERE variant_id = 12 AND product_id = 10;
UPDATE product_variants SET ten_bien_the = N'1L Bottle' WHERE variant_id = 13 AND product_id = 11;
UPDATE product_variants SET ten_bien_the = N'100g Pack' WHERE variant_id = 14 AND product_id = 12;
UPDATE product_variants SET ten_bien_the = N'150ml Spray Bottle' WHERE variant_id = 15 AND product_id = 13;
UPDATE product_variants SET ten_bien_the = N'Dogs 10-20kg' WHERE variant_id = 16 AND product_id = 14;
UPDATE product_variants SET ten_bien_the = N'Dogs 4-10kg' WHERE variant_id = 17 AND product_id = 15;
UPDATE product_variants SET ten_bien_the = N'Dogs 10.1-25kg' WHERE variant_id = 18 AND product_id = 16;
UPDATE product_variants SET ten_bien_the = N'Cat Collar' WHERE variant_id = 19 AND product_id = 17;
UPDATE product_variants SET ten_bien_the = N'Cats over 2.8kg' WHERE variant_id = 20 AND product_id = 18;
UPDATE product_variants SET ten_bien_the = N'Cats 2.5-5kg' WHERE variant_id = 21 AND product_id = 19;
UPDATE product_variants SET ten_bien_the = N'Cats under 2.5kg' WHERE variant_id = 22 AND product_id = 20;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE product_variants SET ten_bien_the = N'Chó 4.5-10kg' WHERE variant_id = 1 AND product_id = 1;
UPDATE product_variants SET ten_bien_the = N'Chó 10-22kg' WHERE variant_id = 2 AND product_id = 1;
UPDATE product_variants SET ten_bien_the = N'Chó nhỏ dưới 10kg' WHERE variant_id = 3 AND product_id = 2;
UPDATE product_variants SET ten_bien_the = N'Chó lớn (>18kg)' WHERE variant_id = 4 AND product_id = 3;
UPDATE product_variants SET ten_bien_the = N'Chó nhỏ (<18kg)' WHERE variant_id = 5 AND product_id = 3;
UPDATE product_variants SET ten_bien_the = N'Chó 5-10kg' WHERE variant_id = 6 AND product_id = 4;
UPDATE product_variants SET ten_bien_the = N'Vỉ 6 viên' WHERE variant_id = 7 AND product_id = 5;
UPDATE product_variants SET ten_bien_the = N'Mèo trưởng thành' WHERE variant_id = 8 AND product_id = 6;
UPDATE product_variants SET ten_bien_the = N'Mèo 1.2-2.8kg' WHERE variant_id = 9 AND product_id = 7;
UPDATE product_variants SET ten_bien_the = N'Vỉ 6 viên' WHERE variant_id = 10 AND product_id = 8;
UPDATE product_variants SET ten_bien_the = N'Mèo 2.5-5kg' WHERE variant_id = 11 AND product_id = 9;
UPDATE product_variants SET ten_bien_the = N'Hộp 500g' WHERE variant_id = 12 AND product_id = 10;
UPDATE product_variants SET ten_bien_the = N'Chai 1L' WHERE variant_id = 13 AND product_id = 11;
UPDATE product_variants SET ten_bien_the = N'Gói 100g' WHERE variant_id = 14 AND product_id = 12;
UPDATE product_variants SET ten_bien_the = N'Chai xịt 150ml' WHERE variant_id = 15 AND product_id = 13;
UPDATE product_variants SET ten_bien_the = N'Chó 10-20kg' WHERE variant_id = 16 AND product_id = 14;
UPDATE product_variants SET ten_bien_the = N'Chó 4-10kg' WHERE variant_id = 17 AND product_id = 15;
UPDATE product_variants SET ten_bien_the = N'Chó 10.1-25kg' WHERE variant_id = 18 AND product_id = 16;
UPDATE product_variants SET ten_bien_the = N'Vòng cổ mèo' WHERE variant_id = 19 AND product_id = 17;
UPDATE product_variants SET ten_bien_the = N'Mèo trên 2.8kg' WHERE variant_id = 20 AND product_id = 18;
UPDATE product_variants SET ten_bien_the = N'Mèo 2.5-5kg' WHERE variant_id = 21 AND product_id = 19;
UPDATE product_variants SET ten_bien_the = N'Mèo dưới 2.5kg' WHERE variant_id = 22 AND product_id = 20;
");
        }
    }
}
