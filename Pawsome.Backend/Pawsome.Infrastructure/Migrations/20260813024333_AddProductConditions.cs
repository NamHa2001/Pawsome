using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pawsome.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductConditions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "conditions",
                columns: table => new
                {
                    condition_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ten_tinh_trang = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conditions", x => x.condition_id);
                });

            migrationBuilder.CreateTable(
                name: "product_conditions",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "int", nullable: false),
                    condition_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_conditions", x => new { x.product_id, x.condition_id });
                    table.ForeignKey(
                        name: "FK_product_conditions_conditions",
                        column: x => x.condition_id,
                        principalTable: "conditions",
                        principalColumn: "condition_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_product_conditions_products",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "conditions",
                columns: new[] { "condition_id", "ten_tinh_trang" },
                values: new object[,]
                {
                    { 1, "Flea, Tick" },
                    { 2, "Wormers" },
                    { 3, "Heartwormers" },
                    { 4, "Joint Care" },
                    { 5, "Skin & Coat" },
                    { 6, "Behavioural" }
                });

            migrationBuilder.CreateIndex(
                name: "UQ_conditions_ten",
                table: "conditions",
                column: "ten_tinh_trang",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_product_conditions_condition",
                table: "product_conditions",
                column: "condition_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "product_conditions");

            migrationBuilder.DropTable(
                name: "conditions");
        }
    }
}
