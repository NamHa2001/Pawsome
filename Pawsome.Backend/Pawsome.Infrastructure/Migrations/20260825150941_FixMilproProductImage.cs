using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixMilproProductImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // product_id=12 (Milpro Bird Care Supplement) đang gắn nhầm ảnh dùng chung "/img/1.png"
            // (ảnh của Simparica Trio for Dogs) từ data gốc - đổi đúng sang ảnh Milpro thật có sẵn.
            // Guard theo url cũ, không đụng nếu ai đã tự đổi ảnh khác trước khi migration này chạy.
            migrationBuilder.Sql(@"
UPDATE product_images SET url = N'/img/milpro.png'
WHERE product_id = 12 AND url = N'/img/1.png';
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE product_images SET url = N'/img/1.png'
WHERE product_id = 12 AND url = N'/img/milpro.png';
");
        }
    }
}
