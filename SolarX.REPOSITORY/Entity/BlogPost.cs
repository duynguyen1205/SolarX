using SolarX.REPOSITORY.Abstractions;

namespace SolarX.REPOSITORY.Entity;

public class BlogPost : BaseEntity<Guid>, IAuditableEntity
{
    public Guid AgencyId { get; set; }
    public Agency Agency { get; set; }

    public string Tittle { get; set; }
    public string Content { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdateAt { get; set; }
}