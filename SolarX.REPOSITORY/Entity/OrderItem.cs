using SolarX.REPOSITORY.Abstractions;

namespace SolarX.REPOSITORY.Entity;

public class OrderItem: BaseEntity<Guid>, IAuditableEntity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; }
    
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdateAt { get; set; }
}