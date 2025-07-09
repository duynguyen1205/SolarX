using SolarX.REPOSITORY.Abstractions;
using SolarX.SERVICE.Services.Base;
using SolarX.SERVICE.Services.InventoryTransactionServices;

namespace SolarX.SERVICE.Abstractions.IInventoryTransactionServices;

public interface IInventoryTransactionServices
{
    public Task<Result> UpdateInventoryTransaction(Guid inventoryTransactionId, int quantityChanged);

    public Task<Result<PagedResult<ResponseModel.InventoryTransactionResponseModel>>> GetInventoryTransaction(Guid agencyId,
        Guid inventoryId, Guid productId, DateTimeOffset? dateCharge, string? searchTerm, int pageIndex, int pageSize);
}