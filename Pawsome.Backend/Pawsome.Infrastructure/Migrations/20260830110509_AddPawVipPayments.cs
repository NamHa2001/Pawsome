using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPawVipPayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "pawvip_payments",
                columns: table => new
                {
                    pawvip_payment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    tier = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    so_tien = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    phuong_thuc = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    trang_thai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "cho_thanh_toan"),
                    ma_giao_dich = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ngay_tao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    ngay_thanh_toan = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pawvip_payments", x => x.pawvip_payment_id);
                    table.CheckConstraint("CK_pawvip_payments_tier", "tier IN (N'thuong', N'nang-cao', N'vip')");
                    table.ForeignKey(
                        name: "FK_pawvip_payments_users",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_pawvip_payments_user",
                table: "pawvip_payments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "UQ_pawvip_payments_magiaodich",
                table: "pawvip_payments",
                column: "ma_giao_dich",
                unique: true,
                filter: "[ma_giao_dich] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pawvip_payments");
        }
    }
}
