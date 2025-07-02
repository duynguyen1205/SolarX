using SolarX.REPOSITORY.Abstractions;
using SolarX.REPOSITORY.Enum;

namespace SolarX.REPOSITORY.Entity;

public class Order : BaseEntity<Guid>, IAuditableEntity
{
    public Guid SellerAgencyId { get; set; }
    public Agency SellerAgency { get; set; }
    
    public Guid? BuyerAgencyId { get; set; }
    public Agency BuyerAgency { get; set; }
    
    public Guid? CustomerId { get; set; }
    public Customer Customer { get; set; }
    
    public decimal TotalAmount { get; set; }
    public bool IsB2C { get; set; }
    public PaymentStatus Status { get; set; }

    public ICollection<OrderItem> Items { get; set; }
    public Payment? Payment { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdateAt { get; set; }
}