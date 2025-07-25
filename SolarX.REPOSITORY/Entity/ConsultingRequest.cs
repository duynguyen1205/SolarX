using SolarX.REPOSITORY.Abstractions;
using SolarX.REPOSITORY.Enum;

namespace SolarX.REPOSITORY.Entity;

public class ConsultingRequest : BaseEntity<Guid>, IAuditableEntity
{

    public Guid? AgencyId { get; set; }
    public Agency Agency { get; set; }

    public decimal Length { get; set; }
    public decimal Width { get; set; }
    public decimal Slope { get; set; }
    public double AverageUsageLast3Months { get; set; }
    public string MainPurpose { get; set; }
    public string UsageTime { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string Area { get; set; }
    public string Email { get; set; }
    public string? Message { get; set; }
    public string ImgUrl { get; set; }
    public ConsultingRequestStatus Status { get; set; } = ConsultingRequestStatus.Pending;
    public ConsultingRequestType RequestType { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdateAt { get; set; }
}