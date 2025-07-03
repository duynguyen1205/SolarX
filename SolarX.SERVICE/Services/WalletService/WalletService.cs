using Microsoft.EntityFrameworkCore;
using SolarX.REPOSITORY.Abstractions;
using SolarX.REPOSITORY.Entity;
using SolarX.SERVICE.Abstractions.IWalletService;
using SolarX.SERVICE.Services.Base;

namespace SolarX.SERVICE.Services.WalletService;

public class WalletService : IWalletService
{
    private readonly IBaseRepository<AgencyWallet, Guid> _agencyWalletRepository;

    public WalletService(IBaseRepository<AgencyWallet, Guid> agencyWalletRepository)
    {
        _agencyWalletRepository = agencyWalletRepository;
    }

    public Task DebitWithCreditAsync(Guid agencyId, decimal amount, string reason)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<ResponseModel.WalletResponseModel?>> GetBalanceAsync(Guid agencyId)
    {
        var agencyExisting = await _agencyWalletRepository.GetAllWithQuery(x => x.AgencyId == agencyId && !x.IsDeleted)
            .FirstOrDefaultAsync();

        if (agencyExisting == null)
        {
            return Result<ResponseModel.WalletResponseModel?>.CreateResult("Bad request", 400, null);
        }

        var available = agencyExisting.CreditLimit - agencyExisting.CurrentDebt;
        var response = new ResponseModel.WalletResponseModel(
            agencyExisting.Balance,
            agencyExisting.CreditLimit,
            agencyExisting.CurrentDebt,
            available
        );

        return Result<ResponseModel.WalletResponseModel?>.CreateResult("Get balance success", 200, response);
    }
}