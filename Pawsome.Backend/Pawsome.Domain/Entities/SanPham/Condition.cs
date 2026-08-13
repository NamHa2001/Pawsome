namespace Pawsome.Domain.Entities.SanPham;

public class Condition
{
    public int ConditionId { get; set; }
    public string TenTinhTrang { get; set; } = null!;

    public ICollection<ProductCondition> ProductConditions { get; set; } = new List<ProductCondition>();
}
