using SolarX.REPOSITORY.Abstractions;

namespace SolarX.REPOSITORY.Entity;

public class ProductImage : BaseEntity<Guid>, IAuditableEntity
{
    public int ImageIndex { get; set; }
    public string ImageUrl { get; set; }

    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdateAt { get; set; }
}