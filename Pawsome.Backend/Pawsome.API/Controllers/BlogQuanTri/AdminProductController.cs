using Microsoft.AspNetCore.Mvc;

namespace Pawsome.API.Controllers.BlogQuanTri;

// Thêm/sửa/xóa sản phẩm, tồn kho, danh mục, thương hiệu. Phải gọi IProductService/
// ICategoryService/IBrandService của Phần 2 - không tự viết data access riêng
// (xem Pawsome_KhungDuAn.md mục 2, ghi chú (*)).
[ApiController]
[Route("api/admin/[controller]")]
public class AdminProductController : ControllerBase
{
}
