using Pawsome.API.DTOs.SanPham;

namespace Pawsome.API.Services.SanPham;

// Danh sách tình trạng sức khỏe (YC-2.3) - chỉ đọc, dữ liệu cố định theo menu
// "Shop by Condition" đã có sẵn trong header (xem ConditionConfiguration.HasData).
public interface IConditionService
{
    Task<List<ConditionDto>> GetAllAsync();
}
