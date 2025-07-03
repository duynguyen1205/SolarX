using SolarX.SERVICE.Services.Base;
using SolarX.SERVICE.Services.WalletService;

namespace SolarX.SERVICE.Abstractions.IWalletService;

public interface IWalletService
{
    Task DebitWithCreditAsync(Guid agencyId, decimal amount, string reason);
    Task<Result<ResponseModel.WalletResponseModel?>> GetBalanceAsync(Guid agencyId);
}