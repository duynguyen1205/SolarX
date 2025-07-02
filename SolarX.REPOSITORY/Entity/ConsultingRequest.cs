using SolarX.REPOSITORY.Abstractions;

namespace SolarX.REPOSITORY.Entity;

public class ConsultingRequest : BaseEntity<Guid>, IAuditableEntity
{

    public Guid? AgencyId { get; set; } 
    public Agency Agency { get; set; }

    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string Area { get; set; }
    public DateTimeOffset PreferredTime { get; set; }
    public string Note { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdateAt { get; set; }
}