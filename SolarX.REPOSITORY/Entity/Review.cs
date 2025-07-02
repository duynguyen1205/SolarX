using SolarX.REPOSITORY.Abstractions;

namespace SolarX.REPOSITORY.Entity;

public class Review : BaseEntity<Guid>, IAuditableEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; }

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; }
    
    public int Rating { get; set; }
    public string Comment { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdateAt { get; set; }
}