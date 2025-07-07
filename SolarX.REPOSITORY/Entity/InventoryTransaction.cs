using SolarX.REPOSITORY.Abstractions;
using SolarX.REPOSITORY.Enum;

namespace SolarX.REPOSITORY.Entity;

public class InventoryTransaction : BaseEntity<Guid>, IAuditableEntity
{
    public Guid AgencyId { get; set; } // Kho nào (tổng hoặc chi nhánh)
    public Agency Agency { get; set; }

    public Guid ProductId { get; set; } // Sản phẩm nào
    public Product Product { get; set; }

    public Guid InventoryId { get; set; }
    public Inventory Inventory { get; set; }

    public Guid? OrderId { get; set; }
    public Order? Order { get; set; }
    public int QuantityChanged { get; set; } // +10 nếu nhập, -5 nếu xuất
    public InventoryTransactionType Type { get; set; } // Nhập, Xuất, Điều chỉnh, Trả hàng
    public string? Note { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdateAt { get; set; }
}