using SolarX.REPOSITORY.Abstractions;

namespace SolarX.REPOSITORY.Entity;

public class WalletTransaction : BaseEntity<Guid>, IAuditableEntity
{
    public Guid AgencyWalletId { get; set; }
    public AgencyWallet AgencyWallet { get; set; }

    public decimal Amount { get; set; }
    public string Reason { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdateAt { get; set; }
}