using SolarX.REPOSITORY.Abstractions;

namespace SolarX.REPOSITORY.Entity;

public class ProductSpecification : BaseEntity<Guid>, IAuditableEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; }

    public string Key { get; set; } // "Hiệu suất"
    public string Value { get; set; } // "20.95%"
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdateAt { get; set; }
}