using SolarX.REPOSITORY.Abstractions;
using SolarX.REPOSITORY.Enum;

namespace SolarX.REPOSITORY.Entity;

public class User : BaseEntity<Guid>, IAuditableEntity
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public Role Role { get; set; }

    public Guid AgencyId { get; set; }
    public Agency Agency { get; set; }
    
    
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdateAt { get; set; }
}