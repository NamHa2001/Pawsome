using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pawsome.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedSampleOrdersForBestSellers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Có review thật (ngụ ý đã mua) nhưng chưa từng có đơn hàng thật nào trong hệ thống, nên
            // khối "Frequently Bought" (dựa vào orders/order_items thật, xem GetBanChayAsync trong
            // ProductService.cs) luôn trống dù review đã tồn tại - không hợp lý. Thêm CHỈ DỮ LIỆU
            // (không đụng code/service của Phần 4) - 1 đơn "đã giao" (da_giao) cho mỗi sản phẩm, số
            // lượng khác nhau để tạo thứ hạng bán chạy rõ ràng trong từng danh mục. Gán hết cho 1 tài
            // khoản đã có sẵn địa chỉ giao hàng (ntathu632@gmail.com) - tránh phải tự thêm địa chỉ
            // mới (bảng addresses thuộc Phần 1, không phải của Phần 2). Nếu máy nào email đó chưa có
            // địa chỉ, hoặc bảng addresses hoàn toàn trống, tự rơi về địa chỉ bất kỳ đã có sẵn/bỏ qua
            // toàn bộ - không tạo địa chỉ giả. Guard theo ma_van_don (mã vận đơn) riêng cho từng dòng
            // để chạy lại nhiều lần không bị trùng đơn.
            migrationBuilder.Sql(@"
DECLARE @addr INT, @uid INT, @vid INT, @gia DECIMAL(12,2), @soluong INT, @orderid INT;

IF EXISTS (SELECT 1 FROM addresses) AND NOT EXISTS (SELECT 1 FROM orders WHERE ma_van_don = N'SEED-ORDER-1')
BEGIN
    SET @addr = ISNULL((SELECT TOP 1 address_id FROM addresses WHERE user_id = (SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com')), (SELECT TOP 1 address_id FROM addresses));
    SET @uid = (SELECT user_id FROM addresses WHERE address_id = @addr);
    SET @vid = (SELECT TOP 1 variant_id FROM product_variants WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Simparica Trio for Dogs') ORDER BY gia ASC);
    SET @gia = (SELECT gia FROM product_variants WHERE variant_id = @vid);
    SET @soluong = 15;

    INSERT INTO orders (user_id, address_id, tien_hang, thanh_tien, trang_thai, ma_van_don)
    VALUES (@uid, @addr, @gia * @soluong, @gia * @soluong, N'da_giao', N'SEED-ORDER-1');
    SET @orderid = SCOPE_IDENTITY();

    INSERT INTO order_items (order_id, variant_id, so_luong, don_gia)
    VALUES (@orderid, @vid, @soluong, @gia);
END

IF EXISTS (SELECT 1 FROM addresses) AND NOT EXISTS (SELECT 1 FROM orders WHERE ma_van_don = N'SEED-ORDER-2')
BEGIN
    SET @addr = ISNULL((SELECT TOP 1 address_id FROM addresses WHERE user_id = (SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com')), (SELECT TOP 1 address_id FROM addresses));
    SET @uid = (SELECT user_id FROM addresses WHERE address_id = @addr);
    SET @vid = (SELECT TOP 1 variant_id FROM product_variants WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Seresto Collar for Dogs') ORDER BY gia ASC);
    SET @gia = (SELECT gia FROM product_variants WHERE variant_id = @vid);
    SET @soluong = 3;

    INSERT INTO orders (user_id, address_id, tien_hang, thanh_tien, trang_thai, ma_van_don)
    VALUES (@uid, @addr, @gia * @soluong, @gia * @soluong, N'da_giao', N'SEED-ORDER-2');
    SET @orderid = SCOPE_IDENTITY();

    INSERT INTO order_items (order_id, variant_id, so_luong, don_gia)
    VALUES (@orderid, @vid, @soluong, @gia);
END

IF EXISTS (SELECT 1 FROM addresses) AND NOT EXISTS (SELECT 1 FROM orders WHERE ma_van_don = N'SEED-ORDER-3')
BEGIN
    SET @addr = ISNULL((SELECT TOP 1 address_id FROM addresses WHERE user_id = (SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com')), (SELECT TOP 1 address_id FROM addresses));
    SET @uid = (SELECT user_id FROM addresses WHERE address_id = @addr);
    SET @vid = (SELECT TOP 1 variant_id FROM product_variants WHERE product_id = (SELECT product_id FROM products WHERE ten = N'NexGard Chewables for Dogs') ORDER BY gia ASC);
    SET @gia = (SELECT gia FROM product_variants WHERE variant_id = @vid);
    SET @soluong = 8;

    INSERT INTO orders (user_id, address_id, tien_hang, thanh_tien, trang_thai, ma_van_don)
    VALUES (@uid, @addr, @gia * @soluong, @gia * @soluong, N'da_giao', N'SEED-ORDER-3');
    SET @orderid = SCOPE_IDENTITY();

    INSERT INTO order_items (order_id, variant_id, so_luong, don_gia)
    VALUES (@orderid, @vid, @soluong, @gia);
END

IF EXISTS (SELECT 1 FROM addresses) AND NOT EXISTS (SELECT 1 FROM orders WHERE ma_van_don = N'SEED-ORDER-4')
BEGIN
    SET @addr = ISNULL((SELECT TOP 1 address_id FROM addresses WHERE user_id = (SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com')), (SELECT TOP 1 address_id FROM addresses));
    SET @uid = (SELECT user_id FROM addresses WHERE address_id = @addr);
    SET @vid = (SELECT TOP 1 variant_id FROM product_variants WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Frontline Flea & Tick Prevention for Dogs') ORDER BY gia ASC);
    SET @gia = (SELECT gia FROM product_variants WHERE variant_id = @vid);
    SET @soluong = 5;

    INSERT INTO orders (user_id, address_id, tien_hang, thanh_tien, trang_thai, ma_van_don)
    VALUES (@uid, @addr, @gia * @soluong, @gia * @soluong, N'da_giao', N'SEED-ORDER-4');
    SET @orderid = SCOPE_IDENTITY();

    INSERT INTO order_items (order_id, variant_id, so_luong, don_gia)
    VALUES (@orderid, @vid, @soluong, @gia);
END

IF EXISTS (SELECT 1 FROM addresses) AND NOT EXISTS (SELECT 1 FROM orders WHERE ma_van_don = N'SEED-ORDER-5')
BEGIN
    SET @addr = ISNULL((SELECT TOP 1 address_id FROM addresses WHERE user_id = (SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com')), (SELECT TOP 1 address_id FROM addresses));
    SET @uid = (SELECT user_id FROM addresses WHERE address_id = @addr);
    SET @vid = (SELECT TOP 1 variant_id FROM product_variants WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Capstar Oral Flea Treatment for Dogs') ORDER BY gia ASC);
    SET @gia = (SELECT gia FROM product_variants WHERE variant_id = @vid);
    SET @soluong = 20;

    INSERT INTO orders (user_id, address_id, tien_hang, thanh_tien, trang_thai, ma_van_don)
    VALUES (@uid, @addr, @gia * @soluong, @gia * @soluong, N'da_giao', N'SEED-ORDER-5');
    SET @orderid = SCOPE_IDENTITY();

    INSERT INTO order_items (order_id, variant_id, so_luong, don_gia)
    VALUES (@orderid, @vid, @soluong, @gia);
END

IF EXISTS (SELECT 1 FROM addresses) AND NOT EXISTS (SELECT 1 FROM orders WHERE ma_van_don = N'SEED-ORDER-6')
BEGIN
    SET @addr = ISNULL((SELECT TOP 1 address_id FROM addresses WHERE user_id = (SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com')), (SELECT TOP 1 address_id FROM addresses));
    SET @uid = (SELECT user_id FROM addresses WHERE address_id = @addr);
    SET @vid = (SELECT TOP 1 variant_id FROM product_variants WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Frontline Plus for Cats') ORDER BY gia ASC);
    SET @gia = (SELECT gia FROM product_variants WHERE variant_id = @vid);
    SET @soluong = 2;

    INSERT INTO orders (user_id, address_id, tien_hang, thanh_tien, trang_thai, ma_van_don)
    VALUES (@uid, @addr, @gia * @soluong, @gia * @soluong, N'da_giao', N'SEED-ORDER-6');
    SET @orderid = SCOPE_IDENTITY();

    INSERT INTO order_items (order_id, variant_id, so_luong, don_gia)
    VALUES (@orderid, @vid, @soluong, @gia);
END

IF EXISTS (SELECT 1 FROM addresses) AND NOT EXISTS (SELECT 1 FROM orders WHERE ma_van_don = N'SEED-ORDER-7')
BEGIN
    SET @addr = ISNULL((SELECT TOP 1 address_id FROM addresses WHERE user_id = (SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com')), (SELECT TOP 1 address_id FROM addresses));
    SET @uid = (SELECT user_id FROM addresses WHERE address_id = @addr);
    SET @vid = (SELECT TOP 1 variant_id FROM product_variants WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Revolution Plus for Cats') ORDER BY gia ASC);
    SET @gia = (SELECT gia FROM product_variants WHERE variant_id = @vid);
    SET @soluong = 10;

    INSERT INTO orders (user_id, address_id, tien_hang, thanh_tien, trang_thai, ma_van_don)
    VALUES (@uid, @addr, @gia * @soluong, @gia * @soluong, N'da_giao', N'SEED-ORDER-7');
    SET @orderid = SCOPE_IDENTITY();

    INSERT INTO order_items (order_id, variant_id, so_luong, don_gia)
    VALUES (@orderid, @vid, @soluong, @gia);
END

IF EXISTS (SELECT 1 FROM addresses) AND NOT EXISTS (SELECT 1 FROM orders WHERE ma_van_don = N'SEED-ORDER-8')
BEGIN
    SET @addr = ISNULL((SELECT TOP 1 address_id FROM addresses WHERE user_id = (SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com')), (SELECT TOP 1 address_id FROM addresses));
    SET @uid = (SELECT user_id FROM addresses WHERE address_id = @addr);
    SET @vid = (SELECT TOP 1 variant_id FROM product_variants WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Capstar Tablets for Cats') ORDER BY gia ASC);
    SET @gia = (SELECT gia FROM product_variants WHERE variant_id = @vid);
    SET @soluong = 6;

    INSERT INTO orders (user_id, address_id, tien_hang, thanh_tien, trang_thai, ma_van_don)
    VALUES (@uid, @addr, @gia * @soluong, @gia * @soluong, N'da_giao', N'SEED-ORDER-8');
    SET @orderid = SCOPE_IDENTITY();

    INSERT INTO order_items (order_id, variant_id, so_luong, don_gia)
    VALUES (@orderid, @vid, @soluong, @gia);
END

IF EXISTS (SELECT 1 FROM addresses) AND NOT EXISTS (SELECT 1 FROM orders WHERE ma_van_don = N'SEED-ORDER-9')
BEGIN
    SET @addr = ISNULL((SELECT TOP 1 address_id FROM addresses WHERE user_id = (SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com')), (SELECT TOP 1 address_id FROM addresses));
    SET @uid = (SELECT user_id FROM addresses WHERE address_id = @addr);
    SET @vid = (SELECT TOP 1 variant_id FROM product_variants WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Bravecto Spot-On for Cats') ORDER BY gia ASC);
    SET @gia = (SELECT gia FROM product_variants WHERE variant_id = @vid);
    SET @soluong = 4;

    INSERT INTO orders (user_id, address_id, tien_hang, thanh_tien, trang_thai, ma_van_don)
    VALUES (@uid, @addr, @gia * @soluong, @gia * @soluong, N'da_giao', N'SEED-ORDER-9');
    SET @orderid = SCOPE_IDENTITY();

    INSERT INTO order_items (order_id, variant_id, so_luong, don_gia)
    VALUES (@orderid, @vid, @soluong, @gia);
END

IF EXISTS (SELECT 1 FROM addresses) AND NOT EXISTS (SELECT 1 FROM orders WHERE ma_van_don = N'SEED-ORDER-10')
BEGIN
    SET @addr = ISNULL((SELECT TOP 1 address_id FROM addresses WHERE user_id = (SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com')), (SELECT TOP 1 address_id FROM addresses));
    SET @uid = (SELECT user_id FROM addresses WHERE address_id = @addr);
    SET @vid = (SELECT TOP 1 variant_id FROM product_variants WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Dorwest Horse Herbal Supplement') ORDER BY gia ASC);
    SET @gia = (SELECT gia FROM product_variants WHERE variant_id = @vid);
    SET @soluong = 12;

    INSERT INTO orders (user_id, address_id, tien_hang, thanh_tien, trang_thai, ma_van_don)
    VALUES (@uid, @addr, @gia * @soluong, @gia * @soluong, N'da_giao', N'SEED-ORDER-10');
    SET @orderid = SCOPE_IDENTITY();

    INSERT INTO order_items (order_id, variant_id, so_luong, don_gia)
    VALUES (@orderid, @vid, @soluong, @gia);
END

IF EXISTS (SELECT 1 FROM addresses) AND NOT EXISTS (SELECT 1 FROM orders WHERE ma_van_don = N'SEED-ORDER-11')
BEGIN
    SET @addr = ISNULL((SELECT TOP 1 address_id FROM addresses WHERE user_id = (SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com')), (SELECT TOP 1 address_id FROM addresses));
    SET @uid = (SELECT user_id FROM addresses WHERE address_id = @addr);
    SET @vid = (SELECT TOP 1 variant_id FROM product_variants WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Aristopet Horse Wormer') ORDER BY gia ASC);
    SET @gia = (SELECT gia FROM product_variants WHERE variant_id = @vid);
    SET @soluong = 7;

    INSERT INTO orders (user_id, address_id, tien_hang, thanh_tien, trang_thai, ma_van_don)
    VALUES (@uid, @addr, @gia * @soluong, @gia * @soluong, N'da_giao', N'SEED-ORDER-11');
    SET @orderid = SCOPE_IDENTITY();

    INSERT INTO order_items (order_id, variant_id, so_luong, don_gia)
    VALUES (@orderid, @vid, @soluong, @gia);
END

IF EXISTS (SELECT 1 FROM addresses) AND NOT EXISTS (SELECT 1 FROM orders WHERE ma_van_don = N'SEED-ORDER-12')
BEGIN
    SET @addr = ISNULL((SELECT TOP 1 address_id FROM addresses WHERE user_id = (SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com')), (SELECT TOP 1 address_id FROM addresses));
    SET @uid = (SELECT user_id FROM addresses WHERE address_id = @addr);
    SET @vid = (SELECT TOP 1 variant_id FROM product_variants WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Milpro Bird Care Supplement') ORDER BY gia ASC);
    SET @gia = (SELECT gia FROM product_variants WHERE variant_id = @vid);
    SET @soluong = 9;

    INSERT INTO orders (user_id, address_id, tien_hang, thanh_tien, trang_thai, ma_van_don)
    VALUES (@uid, @addr, @gia * @soluong, @gia * @soluong, N'da_giao', N'SEED-ORDER-12');
    SET @orderid = SCOPE_IDENTITY();

    INSERT INTO order_items (order_id, variant_id, so_luong, don_gia)
    VALUES (@orderid, @vid, @soluong, @gia);
END

IF EXISTS (SELECT 1 FROM addresses) AND NOT EXISTS (SELECT 1 FROM orders WHERE ma_van_don = N'SEED-ORDER-13')
BEGIN
    SET @addr = ISNULL((SELECT TOP 1 address_id FROM addresses WHERE user_id = (SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com')), (SELECT TOP 1 address_id FROM addresses));
    SET @uid = (SELECT user_id FROM addresses WHERE address_id = @addr);
    SET @vid = (SELECT TOP 1 variant_id FROM product_variants WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Bird Mite & Lice Spray') ORDER BY gia ASC);
    SET @gia = (SELECT gia FROM product_variants WHERE variant_id = @vid);
    SET @soluong = 3;

    INSERT INTO orders (user_id, address_id, tien_hang, thanh_tien, trang_thai, ma_van_don)
    VALUES (@uid, @addr, @gia * @soluong, @gia * @soluong, N'da_giao', N'SEED-ORDER-13');
    SET @orderid = SCOPE_IDENTITY();

    INSERT INTO order_items (order_id, variant_id, so_luong, don_gia)
    VALUES (@orderid, @vid, @soluong, @gia);
END

IF EXISTS (SELECT 1 FROM addresses) AND NOT EXISTS (SELECT 1 FROM orders WHERE ma_van_don = N'SEED-ORDER-14')
BEGIN
    SET @addr = ISNULL((SELECT TOP 1 address_id FROM addresses WHERE user_id = (SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com')), (SELECT TOP 1 address_id FROM addresses));
    SET @uid = (SELECT user_id FROM addresses WHERE address_id = @addr);
    SET @vid = (SELECT TOP 1 variant_id FROM product_variants WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Revolution for Dogs') ORDER BY gia ASC);
    SET @gia = (SELECT gia FROM product_variants WHERE variant_id = @vid);
    SET @soluong = 18;

    INSERT INTO orders (user_id, address_id, tien_hang, thanh_tien, trang_thai, ma_van_don)
    VALUES (@uid, @addr, @gia * @soluong, @gia * @soluong, N'da_giao', N'SEED-ORDER-14');
    SET @orderid = SCOPE_IDENTITY();

    INSERT INTO order_items (order_id, variant_id, so_luong, don_gia)
    VALUES (@orderid, @vid, @soluong, @gia);
END

IF EXISTS (SELECT 1 FROM addresses) AND NOT EXISTS (SELECT 1 FROM orders WHERE ma_van_don = N'SEED-ORDER-15')
BEGIN
    SET @addr = ISNULL((SELECT TOP 1 address_id FROM addresses WHERE user_id = (SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com')), (SELECT TOP 1 address_id FROM addresses));
    SET @uid = (SELECT user_id FROM addresses WHERE address_id = @addr);
    SET @vid = (SELECT TOP 1 variant_id FROM product_variants WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Heartgard Plus for Dogs') ORDER BY gia ASC);
    SET @gia = (SELECT gia FROM product_variants WHERE variant_id = @vid);
    SET @soluong = 6;

    INSERT INTO orders (user_id, address_id, tien_hang, thanh_tien, trang_thai, ma_van_don)
    VALUES (@uid, @addr, @gia * @soluong, @gia * @soluong, N'da_giao', N'SEED-ORDER-15');
    SET @orderid = SCOPE_IDENTITY();

    INSERT INTO order_items (order_id, variant_id, so_luong, don_gia)
    VALUES (@orderid, @vid, @soluong, @gia);
END

IF EXISTS (SELECT 1 FROM addresses) AND NOT EXISTS (SELECT 1 FROM orders WHERE ma_van_don = N'SEED-ORDER-16')
BEGIN
    SET @addr = ISNULL((SELECT TOP 1 address_id FROM addresses WHERE user_id = (SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com')), (SELECT TOP 1 address_id FROM addresses));
    SET @uid = (SELECT user_id FROM addresses WHERE address_id = @addr);
    SET @vid = (SELECT TOP 1 variant_id FROM product_variants WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Bravecto Chews for Dogs') ORDER BY gia ASC);
    SET @gia = (SELECT gia FROM product_variants WHERE variant_id = @vid);
    SET @soluong = 4;

    INSERT INTO orders (user_id, address_id, tien_hang, thanh_tien, trang_thai, ma_van_don)
    VALUES (@uid, @addr, @gia * @soluong, @gia * @soluong, N'da_giao', N'SEED-ORDER-16');
    SET @orderid = SCOPE_IDENTITY();

    INSERT INTO order_items (order_id, variant_id, so_luong, don_gia)
    VALUES (@orderid, @vid, @soluong, @gia);
END

IF EXISTS (SELECT 1 FROM addresses) AND NOT EXISTS (SELECT 1 FROM orders WHERE ma_van_don = N'SEED-ORDER-17')
BEGIN
    SET @addr = ISNULL((SELECT TOP 1 address_id FROM addresses WHERE user_id = (SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com')), (SELECT TOP 1 address_id FROM addresses));
    SET @uid = (SELECT user_id FROM addresses WHERE address_id = @addr);
    SET @vid = (SELECT TOP 1 variant_id FROM product_variants WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Seresto Collar for Cats') ORDER BY gia ASC);
    SET @gia = (SELECT gia FROM product_variants WHERE variant_id = @vid);
    SET @soluong = 8;

    INSERT INTO orders (user_id, address_id, tien_hang, thanh_tien, trang_thai, ma_van_don)
    VALUES (@uid, @addr, @gia * @soluong, @gia * @soluong, N'da_giao', N'SEED-ORDER-17');
    SET @orderid = SCOPE_IDENTITY();

    INSERT INTO order_items (order_id, variant_id, so_luong, don_gia)
    VALUES (@orderid, @vid, @soluong, @gia);
END

IF EXISTS (SELECT 1 FROM addresses) AND NOT EXISTS (SELECT 1 FROM orders WHERE ma_van_don = N'SEED-ORDER-18')
BEGIN
    SET @addr = ISNULL((SELECT TOP 1 address_id FROM addresses WHERE user_id = (SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com')), (SELECT TOP 1 address_id FROM addresses));
    SET @uid = (SELECT user_id FROM addresses WHERE address_id = @addr);
    SET @vid = (SELECT TOP 1 variant_id FROM product_variants WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Bravecto Plus for Cats') ORDER BY gia ASC);
    SET @gia = (SELECT gia FROM product_variants WHERE variant_id = @vid);
    SET @soluong = 22;

    INSERT INTO orders (user_id, address_id, tien_hang, thanh_tien, trang_thai, ma_van_don)
    VALUES (@uid, @addr, @gia * @soluong, @gia * @soluong, N'da_giao', N'SEED-ORDER-18');
    SET @orderid = SCOPE_IDENTITY();

    INSERT INTO order_items (order_id, variant_id, so_luong, don_gia)
    VALUES (@orderid, @vid, @soluong, @gia);
END

IF EXISTS (SELECT 1 FROM addresses) AND NOT EXISTS (SELECT 1 FROM orders WHERE ma_van_don = N'SEED-ORDER-19')
BEGIN
    SET @addr = ISNULL((SELECT TOP 1 address_id FROM addresses WHERE user_id = (SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com')), (SELECT TOP 1 address_id FROM addresses));
    SET @uid = (SELECT user_id FROM addresses WHERE address_id = @addr);
    SET @vid = (SELECT TOP 1 variant_id FROM product_variants WHERE product_id = (SELECT product_id FROM products WHERE ten = N'AdvantageMulti for Cats') ORDER BY gia ASC);
    SET @gia = (SELECT gia FROM product_variants WHERE variant_id = @vid);
    SET @soluong = 5;

    INSERT INTO orders (user_id, address_id, tien_hang, thanh_tien, trang_thai, ma_van_don)
    VALUES (@uid, @addr, @gia * @soluong, @gia * @soluong, N'da_giao', N'SEED-ORDER-19');
    SET @orderid = SCOPE_IDENTITY();

    INSERT INTO order_items (order_id, variant_id, so_luong, don_gia)
    VALUES (@orderid, @vid, @soluong, @gia);
END

IF EXISTS (SELECT 1 FROM addresses) AND NOT EXISTS (SELECT 1 FROM orders WHERE ma_van_don = N'SEED-ORDER-20')
BEGIN
    SET @addr = ISNULL((SELECT TOP 1 address_id FROM addresses WHERE user_id = (SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com')), (SELECT TOP 1 address_id FROM addresses));
    SET @uid = (SELECT user_id FROM addresses WHERE address_id = @addr);
    SET @vid = (SELECT TOP 1 variant_id FROM product_variants WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Advantage for Cats') ORDER BY gia ASC);
    SET @gia = (SELECT gia FROM product_variants WHERE variant_id = @vid);
    SET @soluong = 3;

    INSERT INTO orders (user_id, address_id, tien_hang, thanh_tien, trang_thai, ma_van_don)
    VALUES (@uid, @addr, @gia * @soluong, @gia * @soluong, N'da_giao', N'SEED-ORDER-20');
    SET @orderid = SCOPE_IDENTITY();

    INSERT INTO order_items (order_id, variant_id, so_luong, don_gia)
    VALUES (@orderid, @vid, @soluong, @gia);
END

IF EXISTS (SELECT 1 FROM addresses) AND NOT EXISTS (SELECT 1 FROM orders WHERE ma_van_don = N'SEED-ORDER-21')
BEGIN
    SET @addr = ISNULL((SELECT TOP 1 address_id FROM addresses WHERE user_id = (SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com')), (SELECT TOP 1 address_id FROM addresses));
    SET @uid = (SELECT user_id FROM addresses WHERE address_id = @addr);
    SET @vid = (SELECT TOP 1 variant_id FROM product_variants WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Profender for Cats') ORDER BY gia ASC);
    SET @gia = (SELECT gia FROM product_variants WHERE variant_id = @vid);
    SET @soluong = 9;

    INSERT INTO orders (user_id, address_id, tien_hang, thanh_tien, trang_thai, ma_van_don)
    VALUES (@uid, @addr, @gia * @soluong, @gia * @soluong, N'da_giao', N'SEED-ORDER-21');
    SET @orderid = SCOPE_IDENTITY();

    INSERT INTO order_items (order_id, variant_id, so_luong, don_gia)
    VALUES (@orderid, @vid, @soluong, @gia);
END

IF EXISTS (SELECT 1 FROM addresses) AND NOT EXISTS (SELECT 1 FROM orders WHERE ma_van_don = N'SEED-ORDER-22')
BEGIN
    SET @addr = ISNULL((SELECT TOP 1 address_id FROM addresses WHERE user_id = (SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com')), (SELECT TOP 1 address_id FROM addresses));
    SET @uid = (SELECT user_id FROM addresses WHERE address_id = @addr);
    SET @vid = (SELECT TOP 1 variant_id FROM product_variants WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Dermoscent Essential 6 Skin & Coat Spray for Dogs') ORDER BY gia ASC);
    SET @gia = (SELECT gia FROM product_variants WHERE variant_id = @vid);
    SET @soluong = 6;

    INSERT INTO orders (user_id, address_id, tien_hang, thanh_tien, trang_thai, ma_van_don)
    VALUES (@uid, @addr, @gia * @soluong, @gia * @soluong, N'da_giao', N'SEED-ORDER-22');
    SET @orderid = SCOPE_IDENTITY();

    INSERT INTO order_items (order_id, variant_id, so_luong, don_gia)
    VALUES (@orderid, @vid, @soluong, @gia);
END

IF EXISTS (SELECT 1 FROM addresses) AND NOT EXISTS (SELECT 1 FROM orders WHERE ma_van_don = N'SEED-ORDER-23')
BEGIN
    SET @addr = ISNULL((SELECT TOP 1 address_id FROM addresses WHERE user_id = (SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com')), (SELECT TOP 1 address_id FROM addresses));
    SET @uid = (SELECT user_id FROM addresses WHERE address_id = @addr);
    SET @vid = (SELECT TOP 1 variant_id FROM product_variants WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Adaptil Calming Diffuser for Dogs') ORDER BY gia ASC);
    SET @gia = (SELECT gia FROM product_variants WHERE variant_id = @vid);
    SET @soluong = 14;

    INSERT INTO orders (user_id, address_id, tien_hang, thanh_tien, trang_thai, ma_van_don)
    VALUES (@uid, @addr, @gia * @soluong, @gia * @soluong, N'da_giao', N'SEED-ORDER-23');
    SET @orderid = SCOPE_IDENTITY();

    INSERT INTO order_items (order_id, variant_id, so_luong, don_gia)
    VALUES (@orderid, @vid, @soluong, @gia);
END

IF EXISTS (SELECT 1 FROM addresses) AND NOT EXISTS (SELECT 1 FROM orders WHERE ma_van_don = N'SEED-ORDER-24')
BEGIN
    SET @addr = ISNULL((SELECT TOP 1 address_id FROM addresses WHERE user_id = (SELECT user_id FROM users WHERE email = N'ntathu632@gmail.com')), (SELECT TOP 1 address_id FROM addresses));
    SET @uid = (SELECT user_id FROM addresses WHERE address_id = @addr);
    SET @vid = (SELECT TOP 1 variant_id FROM product_variants WHERE product_id = (SELECT product_id FROM products WHERE ten = N'Frontline Spray for Cats') ORDER BY gia ASC);
    SET @gia = (SELECT gia FROM product_variants WHERE variant_id = @vid);
    SET @soluong = 4;

    INSERT INTO orders (user_id, address_id, tien_hang, thanh_tien, trang_thai, ma_van_don)
    VALUES (@uid, @addr, @gia * @soluong, @gia * @soluong, N'da_giao', N'SEED-ORDER-24');
    SET @orderid = SCOPE_IDENTITY();

    INSERT INTO order_items (order_id, variant_id, so_luong, don_gia)
    VALUES (@orderid, @vid, @soluong, @gia);
END


");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE oi FROM order_items oi JOIN orders o ON o.order_id = oi.order_id WHERE o.ma_van_don LIKE N'SEED-ORDER-%';
DELETE FROM orders WHERE ma_van_don LIKE N'SEED-ORDER-%';
");
        }
    }
}
