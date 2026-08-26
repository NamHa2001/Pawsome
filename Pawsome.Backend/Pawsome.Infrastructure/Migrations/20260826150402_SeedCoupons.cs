using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedCoupons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Bảng coupons chưa có migration seed nào (Phần 3 chưa làm) - tạm thêm 5 coupon chung
            // để trang chủ (banner Flash Sale, welcome-bonus subscribe) có dữ liệu thật để chạy,
            // không phải chờ. Guard theo ma_code, không đụng nếu ai đã tự tạo trùng mã.
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM coupons WHERE ma_code = N'SAVE10')
INSERT INTO coupons (ma_code, loai_giam, gia_tri, ngay_bat_dau, ngay_ket_thuc, so_luong)
VALUES (N'SAVE10', N'percent', 10, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM coupons WHERE ma_code = N'SAVE20')
INSERT INTO coupons (ma_code, loai_giam, gia_tri, ngay_bat_dau, ngay_ket_thuc, so_luong)
VALUES (N'SAVE20', N'percent', 20, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM coupons WHERE ma_code = N'FREESHIP30')
INSERT INTO coupons (ma_code, loai_giam, gia_tri, ngay_bat_dau, ngay_ket_thuc, so_luong)
VALUES (N'FREESHIP30', N'fixed', 30000, NULL, NULL, NULL);

IF NOT EXISTS (SELECT 1 FROM coupons WHERE ma_code = N'FLASH50')
INSERT INTO coupons (ma_code, loai_giam, gia_tri, ngay_bat_dau, ngay_ket_thuc, so_luong)
VALUES (N'FLASH50', N'percent', 50, NULL, NULL, 20);

IF NOT EXISTS (SELECT 1 FROM coupons WHERE ma_code = N'NEWMEMBER15')
INSERT INTO coupons (ma_code, loai_giam, gia_tri, ngay_bat_dau, ngay_ket_thuc, so_luong)
VALUES (N'NEWMEMBER15', N'percent', 15, NULL, NULL, NULL);
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM coupons WHERE ma_code IN (N'SAVE10', N'SAVE20', N'FREESHIP30', N'FLASH50', N'NEWMEMBER15');
");
        }
    }
}
