using SolarX.REPOSITORY.Abstractions;

namespace SolarX.REPOSITORY.Entity;

public class AgencyWallet : BaseEntity<Guid>, IAuditableEntity
{
    public decimal Balance { get; set; }
    public decimal CreditLimit { get; set; }
    public decimal CurrentDebt { get; set; }

    public Guid AgencyId { get; set; }
    public Agency Agency { get; set; }

    public ICollection<WalletTransaction> Transactions { get; set; } = new List<WalletTransaction>();
   
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdateAt { get; set; }
}