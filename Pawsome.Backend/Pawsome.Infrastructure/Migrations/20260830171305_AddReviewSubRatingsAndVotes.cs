using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewSubRatingsAndVotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "diem_chat_luong",
                table: "reviews",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "diem_gia_tri",
                table: "reviews",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "diem_hai_long_thu_cung",
                table: "reviews",
                type: "tinyint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "review_votes",
                columns: table => new
                {
                    review_vote_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    review_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    huu_ich = table.Column<bool>(type: "bit", nullable: false),
                    ngay_tao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_review_votes", x => x.review_vote_id);
                    table.ForeignKey(
                        name: "FK_review_votes_reviews",
                        column: x => x.review_id,
                        principalTable: "reviews",
                        principalColumn: "review_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_review_votes_users",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_reviews_diemchatluong",
                table: "reviews",
                sql: "diem_chat_luong IS NULL OR diem_chat_luong BETWEEN 1 AND 5");

            migrationBuilder.AddCheckConstraint(
                name: "CK_reviews_diemgiatri",
                table: "reviews",
                sql: "diem_gia_tri IS NULL OR diem_gia_tri BETWEEN 1 AND 5");

            migrationBuilder.AddCheckConstraint(
                name: "CK_reviews_diemhailong",
                table: "reviews",
                sql: "diem_hai_long_thu_cung IS NULL OR diem_hai_long_thu_cung BETWEEN 1 AND 5");

            migrationBuilder.CreateIndex(
                name: "IX_review_votes_user_id",
                table: "review_votes",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "UQ_review_votes_review_user",
                table: "review_votes",
                columns: new[] { "review_id", "user_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "review_votes");

            migrationBuilder.DropCheckConstraint(
                name: "CK_reviews_diemchatluong",
                table: "reviews");

            migrationBuilder.DropCheckConstraint(
                name: "CK_reviews_diemgiatri",
                table: "reviews");

            migrationBuilder.DropCheckConstraint(
                name: "CK_reviews_diemhailong",
                table: "reviews");

            migrationBuilder.DropColumn(
                name: "diem_chat_luong",
                table: "reviews");

            migrationBuilder.DropColumn(
                name: "diem_gia_tri",
                table: "reviews");

            migrationBuilder.DropColumn(
                name: "diem_hai_long_thu_cung",
                table: "reviews");
        }
    }
}
