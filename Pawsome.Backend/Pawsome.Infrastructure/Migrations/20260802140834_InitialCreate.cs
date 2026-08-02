using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pawsome.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "brands",
                columns: table => new
                {
                    brand_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ten_thuong_hieu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    logo_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_brands", x => x.brand_id);
                });

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    category_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ten_danh_muc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    danh_muc_cha_id = table.Column<int>(type: "int", nullable: true),
                    mo_ta = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.category_id);
                    table.ForeignKey(
                        name: "FK_categories_parent",
                        column: x => x.danh_muc_cha_id,
                        principalTable: "categories",
                        principalColumn: "category_id");
                });

            migrationBuilder.CreateTable(
                name: "coupons",
                columns: table => new
                {
                    coupon_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ma_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    loai_giam = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    gia_tri = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    ngay_bat_dau = table.Column<DateOnly>(type: "date", nullable: true),
                    ngay_ket_thuc = table.Column<DateOnly>(type: "date", nullable: true),
                    so_luong = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_coupons", x => x.coupon_id);
                    table.CheckConstraint("CK_coupons_gia_tri", "gia_tri > 0");
                    table.CheckConstraint("CK_coupons_loai_giam", "loai_giam IN (N'percent', N'fixed')");
                    table.CheckConstraint("CK_coupons_ngay", "ngay_bat_dau IS NULL OR ngay_ket_thuc IS NULL OR ngay_ket_thuc >= ngay_bat_dau");
                    table.CheckConstraint("CK_coupons_so_luong", "so_luong IS NULL OR so_luong >= 0");
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ten_vai_tro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    mo_ta = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    mo_ta = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    category_id = table.Column<int>(type: "int", nullable: false),
                    brand_id = table.Column<int>(type: "int", nullable: true),
                    lieu_luong = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    gia_tu = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    diem_danh_gia_tb = table.Column<decimal>(type: "decimal(2,1)", precision: 2, scale: 1, nullable: false, defaultValue: 0m),
                    dang_kinh_doanh = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ngay_tao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    ngay_cap_nhat = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.product_id);
                    table.CheckConstraint("CK_products_diem_danh_gia", "diem_danh_gia_tb BETWEEN 0 AND 5");
                    table.CheckConstraint("CK_products_gia_tu", "gia_tu IS NULL OR gia_tu >= 0");
                    table.ForeignKey(
                        name: "FK_products_brands",
                        column: x => x.brand_id,
                        principalTable: "brands",
                        principalColumn: "brand_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_products_categories",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "category_id");
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    role_id = table.Column<int>(type: "int", nullable: false),
                    email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ho_ten = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    so_dien_thoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    diem_pawpoints = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    trang_thai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "active"),
                    ngay_tao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    ngay_cap_nhat = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.user_id);
                    table.CheckConstraint("CK_users_diem_pawpoints", "diem_pawpoints >= 0");
                    table.CheckConstraint("CK_users_trangthai", "trang_thai IN (N'active', N'locked')");
                    table.ForeignKey(
                        name: "FK_users_roles",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "role_id");
                });

            migrationBuilder.CreateTable(
                name: "product_images",
                columns: table => new
                {
                    image_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    la_anh_chinh = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_images", x => x.image_id);
                    table.ForeignKey(
                        name: "FK_images_products",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_variants",
                columns: table => new
                {
                    variant_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    ten_bien_the = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    gia = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    so_luong_ton = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    sku = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    dang_kinh_doanh = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_variants", x => x.variant_id);
                    table.CheckConstraint("CK_variants_gia", "gia > 0");
                    table.CheckConstraint("CK_variants_soluongton", "so_luong_ton >= 0");
                    table.ForeignKey(
                        name: "FK_variants_products",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "addresses",
                columns: table => new
                {
                    address_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    nguoi_nhan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    so_dien_thoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    dia_chi_chi_tiet = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    phuong_xa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    quan_huyen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    tinh_thanh = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    la_mac_dinh = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_addresses", x => x.address_id);
                    table.ForeignKey(
                        name: "FK_addresses_users",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "audit_logs",
                columns: table => new
                {
                    log_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    hanh_dong = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    doi_tuong = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    doi_tuong_id = table.Column<int>(type: "int", nullable: true),
                    chi_tiet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    dia_chi_ip = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    ngay_tao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_logs", x => x.log_id);
                    table.ForeignKey(
                        name: "FK_audit_logs_users",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "blog_posts",
                columns: table => new
                {
                    post_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tieu_de = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    noi_dung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    chu_de = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    tac_gia_id = table.Column<int>(type: "int", nullable: false),
                    anh_dai_dien = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ngay_dang = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blog_posts", x => x.post_id);
                    table.ForeignKey(
                        name: "FK_blog_posts_users",
                        column: x => x.tac_gia_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "carts",
                columns: table => new
                {
                    cart_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    ngay_cap_nhat = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_carts", x => x.cart_id);
                    table.ForeignKey(
                        name: "FK_carts_users",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "reviews",
                columns: table => new
                {
                    review_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    so_sao = table.Column<byte>(type: "tinyint", nullable: false),
                    binh_luan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    trang_thai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "cho_duyet"),
                    ngay_tao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reviews", x => x.review_id);
                    table.CheckConstraint("CK_reviews_sosao", "so_sao BETWEEN 1 AND 5");
                    table.CheckConstraint("CK_reviews_trangthai", "trang_thai IN (N'cho_duyet', N'da_duyet', N'tu_choi')");
                    table.ForeignKey(
                        name: "FK_reviews_products",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "product_id");
                    table.ForeignKey(
                        name: "FK_reviews_users",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "wishlists",
                columns: table => new
                {
                    wishlist_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    ngay_them = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wishlists", x => x.wishlist_id);
                    table.ForeignKey(
                        name: "FK_wishlists_products",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_wishlists_users",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "auto_orders",
                columns: table => new
                {
                    auto_order_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    variant_id = table.Column<int>(type: "int", nullable: false),
                    so_luong = table.Column<int>(type: "int", nullable: false),
                    tan_suat = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ngay_ke_tiep = table.Column<DateOnly>(type: "date", nullable: false),
                    trang_thai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "active")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auto_orders", x => x.auto_order_id);
                    table.CheckConstraint("CK_auto_orders_soluong", "so_luong > 0");
                    table.CheckConstraint("CK_auto_orders_trangthai", "trang_thai IN (N'active', N'paused', N'cancelled')");
                    table.ForeignKey(
                        name: "FK_auto_orders_users",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK_auto_orders_variants",
                        column: x => x.variant_id,
                        principalTable: "product_variants",
                        principalColumn: "variant_id");
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    order_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    address_id = table.Column<int>(type: "int", nullable: false),
                    coupon_id = table.Column<int>(type: "int", nullable: true),
                    ngay_dat = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    tien_hang = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    phi_van_chuyen = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false, defaultValue: 0m),
                    giam_gia = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false, defaultValue: 0m),
                    thanh_tien = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    trang_thai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    don_vi_van_chuyen = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ma_van_don = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ngay_cap_nhat = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.order_id);
                    table.CheckConstraint("CK_orders_giamgia", "giam_gia >= 0");
                    table.CheckConstraint("CK_orders_phivanchuyen", "phi_van_chuyen >= 0");
                    table.CheckConstraint("CK_orders_thanhtien", "thanh_tien >= 0");
                    table.CheckConstraint("CK_orders_tienhang", "tien_hang >= 0");
                    table.ForeignKey(
                        name: "FK_orders_addresses",
                        column: x => x.address_id,
                        principalTable: "addresses",
                        principalColumn: "address_id");
                    table.ForeignKey(
                        name: "FK_orders_coupons",
                        column: x => x.coupon_id,
                        principalTable: "coupons",
                        principalColumn: "coupon_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_orders_users",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "cart_items",
                columns: table => new
                {
                    cart_item_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cart_id = table.Column<int>(type: "int", nullable: false),
                    variant_id = table.Column<int>(type: "int", nullable: false),
                    so_luong = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cart_items", x => x.cart_item_id);
                    table.CheckConstraint("CK_cart_items_soluong", "so_luong > 0");
                    table.ForeignKey(
                        name: "FK_cart_items_carts",
                        column: x => x.cart_id,
                        principalTable: "carts",
                        principalColumn: "cart_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cart_items_variants",
                        column: x => x.variant_id,
                        principalTable: "product_variants",
                        principalColumn: "variant_id");
                });

            migrationBuilder.CreateTable(
                name: "order_items",
                columns: table => new
                {
                    order_item_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    order_id = table.Column<int>(type: "int", nullable: false),
                    variant_id = table.Column<int>(type: "int", nullable: false),
                    so_luong = table.Column<int>(type: "int", nullable: false),
                    don_gia = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_items", x => x.order_item_id);
                    table.CheckConstraint("CK_order_items_dongia", "don_gia >= 0");
                    table.CheckConstraint("CK_order_items_soluong", "so_luong > 0");
                    table.ForeignKey(
                        name: "FK_order_items_orders",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "order_id");
                    table.ForeignKey(
                        name: "FK_order_items_variants",
                        column: x => x.variant_id,
                        principalTable: "product_variants",
                        principalColumn: "variant_id");
                });

            migrationBuilder.CreateTable(
                name: "pawpoints_transactions",
                columns: table => new
                {
                    transaction_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    order_id = table.Column<int>(type: "int", nullable: true),
                    so_diem = table.Column<int>(type: "int", nullable: false),
                    loai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ngay_giao_dich = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pawpoints_transactions", x => x.transaction_id);
                    table.CheckConstraint("CK_pawpoints_loai", "loai IN (N'earn', N'redeem', N'bonus')");
                    table.CheckConstraint("CK_pawpoints_sodiem", "so_diem <> 0");
                    table.ForeignKey(
                        name: "FK_pawpoints_orders",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_pawpoints_users",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    payment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    order_id = table.Column<int>(type: "int", nullable: false),
                    phuong_thuc = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    so_tien = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    trang_thai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ma_giao_dich = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ngay_thanh_toan = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payments", x => x.payment_id);
                    table.CheckConstraint("CK_payments_sotien", "so_tien > 0");
                    table.ForeignKey(
                        name: "FK_payments_orders",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "order_id");
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "role_id", "mo_ta", "ten_vai_tro" },
                values: new object[,]
                {
                    { 1, "Khách hàng - người mua sản phẩm", "Customer" },
                    { 2, "Quản trị viên - quản lý toàn hệ thống", "Admin" },
                    { 3, "Người kiểm duyệt - duyệt đánh giá, nội dung", "Moderator" },
                    { 4, "Nhân viên hỗ trợ - chăm sóc khách hàng", "Support" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_addresses_user",
                table: "addresses",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_ngaytao",
                table: "audit_logs",
                column: "ngay_tao");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_user",
                table: "audit_logs",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_auto_orders_user",
                table: "auto_orders",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_auto_orders_variant",
                table: "auto_orders",
                column: "variant_id");

            migrationBuilder.CreateIndex(
                name: "IX_blog_posts_author",
                table: "blog_posts",
                column: "tac_gia_id");

            migrationBuilder.CreateIndex(
                name: "UQ_brands_ten",
                table: "brands",
                column: "ten_thuong_hieu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cart_items_cart",
                table: "cart_items",
                column: "cart_id");

            migrationBuilder.CreateIndex(
                name: "IX_cart_items_variant",
                table: "cart_items",
                column: "variant_id");

            migrationBuilder.CreateIndex(
                name: "UQ_cart_items_cart_variant",
                table: "cart_items",
                columns: new[] { "cart_id", "variant_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_carts_user",
                table: "carts",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_categories_parent",
                table: "categories",
                column: "danh_muc_cha_id");

            migrationBuilder.CreateIndex(
                name: "UQ_coupons_code",
                table: "coupons",
                column: "ma_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_order_items_order",
                table: "order_items",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_items_variant",
                table: "order_items",
                column: "variant_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_address",
                table: "orders",
                column: "address_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_coupon",
                table: "orders",
                column: "coupon_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_ngaydat",
                table: "orders",
                column: "ngay_dat");

            migrationBuilder.CreateIndex(
                name: "IX_orders_trangthai",
                table: "orders",
                column: "trang_thai");

            migrationBuilder.CreateIndex(
                name: "IX_orders_user",
                table: "orders",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_pawpoints_order",
                table: "pawpoints_transactions",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_pawpoints_user",
                table: "pawpoints_transactions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_payments_order",
                table: "payments",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_images_product",
                table: "product_images",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_variants_product",
                table: "product_variants",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "UQ_product_variants_sku",
                table: "product_variants",
                column: "sku",
                unique: true,
                filter: "[sku] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_products_brand",
                table: "products",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_category",
                table: "products",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_reviews_product",
                table: "reviews",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_reviews_trangthai",
                table: "reviews",
                column: "trang_thai");

            migrationBuilder.CreateIndex(
                name: "IX_reviews_user",
                table: "reviews",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "UQ_roles_ten",
                table: "roles",
                column: "ten_vai_tro",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_role",
                table: "users",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "UQ_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_wishlists_product",
                table: "wishlists",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_wishlists_user",
                table: "wishlists",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "UQ_wishlists_user_product",
                table: "wishlists",
                columns: new[] { "user_id", "product_id" },
                unique: true);

            // ── Trigger + RECURSIVE_TRIGGERS OFF ────────────────────────────────
            // EF Core Migrations không tự sinh trigger được, chèn tay bằng raw SQL
            // để khớp đúng Pawsome_Database.sql (mỗi CREATE TRIGGER phải là câu lệnh
            // đầu tiên trong batch của nó, nên mỗi trigger để trong 1 lời gọi Sql() riêng).

            // ALTER DATABASE không được phép chạy trong transaction -> suppressTransaction: true
            migrationBuilder.Sql(
                "ALTER DATABASE CURRENT SET RECURSIVE_TRIGGERS OFF;",
                suppressTransaction: true);

            migrationBuilder.Sql("""
                CREATE TRIGGER trg_users_set_ngaycapnhat ON users
                AFTER UPDATE AS
                BEGIN
                    SET NOCOUNT ON;
                    UPDATE u SET ngay_cap_nhat = GETDATE()
                    FROM users u INNER JOIN inserted i ON u.user_id = i.user_id;
                END
                """);

            migrationBuilder.Sql("""
                CREATE TRIGGER trg_products_set_ngaycapnhat ON products
                AFTER UPDATE AS
                BEGIN
                    SET NOCOUNT ON;
                    UPDATE p SET ngay_cap_nhat = GETDATE()
                    FROM products p INNER JOIN inserted i ON p.product_id = i.product_id;
                END
                """);

            migrationBuilder.Sql("""
                CREATE TRIGGER trg_orders_set_ngaycapnhat ON orders
                AFTER UPDATE AS
                BEGIN
                    SET NOCOUNT ON;
                    UPDATE o SET ngay_cap_nhat = GETDATE()
                    FROM orders o INNER JOIN inserted i ON o.order_id = i.order_id;
                END
                """);

            migrationBuilder.Sql("""
                CREATE TRIGGER trg_cart_items_touch_cart ON cart_items
                AFTER INSERT, UPDATE, DELETE AS
                BEGIN
                    SET NOCOUNT ON;
                    UPDATE c SET ngay_cap_nhat = GETDATE()
                    FROM carts c
                    WHERE c.cart_id IN (SELECT cart_id FROM inserted
                                         UNION
                                         SELECT cart_id FROM deleted);
                END
                """);

            migrationBuilder.Sql("""
                CREATE TRIGGER trg_pawpoints_sync_balance ON pawpoints_transactions
                AFTER INSERT, UPDATE, DELETE AS
                BEGIN
                    SET NOCOUNT ON;
                    UPDATE u
                    SET diem_pawpoints = ISNULL((
                            SELECT SUM(pt.so_diem)
                            FROM pawpoints_transactions pt
                            WHERE pt.user_id = u.user_id
                        ), 0)
                    FROM users u
                    WHERE u.user_id IN (SELECT user_id FROM inserted
                                         UNION
                                         SELECT user_id FROM deleted);
                END
                """);

            migrationBuilder.Sql("""
                CREATE TRIGGER trg_reviews_sync_avg ON reviews
                AFTER INSERT, UPDATE, DELETE AS
                BEGIN
                    SET NOCOUNT ON;
                    UPDATE p
                    SET diem_danh_gia_tb = ISNULL((
                            SELECT AVG(CAST(r.so_sao AS DECIMAL(4,2)))
                            FROM reviews r
                            WHERE r.product_id = p.product_id AND r.trang_thai = N'da_duyet'
                        ), 0)
                    FROM products p
                    WHERE p.product_id IN (SELECT product_id FROM inserted
                                            UNION
                                            SELECT product_id FROM deleted);
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_logs");

            migrationBuilder.DropTable(
                name: "auto_orders");

            migrationBuilder.DropTable(
                name: "blog_posts");

            migrationBuilder.DropTable(
                name: "cart_items");

            migrationBuilder.DropTable(
                name: "order_items");

            migrationBuilder.DropTable(
                name: "pawpoints_transactions");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropTable(
                name: "product_images");

            migrationBuilder.DropTable(
                name: "reviews");

            migrationBuilder.DropTable(
                name: "wishlists");

            migrationBuilder.DropTable(
                name: "carts");

            migrationBuilder.DropTable(
                name: "product_variants");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "addresses");

            migrationBuilder.DropTable(
                name: "coupons");

            migrationBuilder.DropTable(
                name: "brands");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
