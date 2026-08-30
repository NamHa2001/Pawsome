using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCouponIdToCarts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "coupon_id",
                table: "carts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_carts_coupon",
                table: "carts",
                column: "coupon_id");

            migrationBuilder.AddForeignKey(
                name: "FK_carts_coupons",
                table: "carts",
                column: "coupon_id",
                principalTable: "coupons",
                principalColumn: "coupon_id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_carts_coupons",
                table: "carts");

            migrationBuilder.DropIndex(
                name: "IX_carts_coupon",
                table: "carts");

            migrationBuilder.DropColumn(
                name: "coupon_id",
                table: "carts");
        }
    }
}
