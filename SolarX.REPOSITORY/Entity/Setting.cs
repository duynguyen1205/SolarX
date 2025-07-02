using SolarX.REPOSITORY.Abstractions;

namespace SolarX.REPOSITORY.Entity;

public class Setting : BaseEntity<Guid>, IAuditableEntity
{
    public Guid? AgencyId { get; set; }
    public Agency Agency { get; set; }
    
    public string Key { get; set; }
    public string Value { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdateAt { get; set; }
}