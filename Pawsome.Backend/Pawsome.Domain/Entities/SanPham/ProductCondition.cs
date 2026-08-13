namespace Pawsome.Domain.Entities.SanPham;

public class ProductCondition
{
    public int ProductId { get; set; }
    public int ConditionId { get; set; }

    public Product Product { get; set; } = null!;
    public Condition Condition { get; set; } = null!;
}
