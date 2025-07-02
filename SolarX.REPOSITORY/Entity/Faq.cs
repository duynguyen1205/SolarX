using SolarX.REPOSITORY.Abstractions;

namespace SolarX.REPOSITORY.Entity;

public class Faq : BaseEntity<Guid>, IAuditableEntity
{
    public string Question { get; set; }
    public string Answear { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdateAt { get; set; }
}