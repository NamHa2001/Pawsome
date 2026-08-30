using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceLaPawvipWithPawVipTier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "la_pawvip",
                table: "users");

            migrationBuilder.AddColumn<string>(
                name: "pawvip_tier",
                table: "users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "user_id",
                keyValue: 1,
                column: "pawvip_tier",
                value: null);

            migrationBuilder.AddCheckConstraint(
                name: "CK_users_pawvip_tier",
                table: "users",
                sql: "pawvip_tier IS NULL OR pawvip_tier IN (N'thuong', N'nang-cao', N'vip')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_users_pawvip_tier",
                table: "users");

            migrationBuilder.DropColumn(
                name: "pawvip_tier",
                table: "users");

            migrationBuilder.AddColumn<bool>(
                name: "la_pawvip",
                table: "users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "user_id",
                keyValue: 1,
                columns: new string[0],
                values: new object[0]);
        }
    }
}
