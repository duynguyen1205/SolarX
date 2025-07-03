using SolarX.REPOSITORY.Abstractions;
using SolarX.REPOSITORY.Enum;

namespace SolarX.REPOSITORY.Entity;

public class Payment : BaseEntity<Guid>, IAuditableEntity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; }

    public decimal Amount { get; set; }
    public DateTimeOffset PaidAt { get; set; }

    public PaymentMethod Method { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdateAt { get; set; }
}