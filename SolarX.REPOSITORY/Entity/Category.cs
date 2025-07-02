using SolarX.REPOSITORY.Abstractions;

namespace SolarX.REPOSITORY.Entity;

public class Category: BaseEntity<Guid>, IAuditableEntity
{
    public string Name { get; set; }
    public ICollection<Product> Products { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdateAt { get; set; }
}