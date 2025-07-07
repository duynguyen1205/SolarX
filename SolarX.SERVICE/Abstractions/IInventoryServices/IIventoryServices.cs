using SolarX.REPOSITORY.Abstractions;
using SolarX.SERVICE.Services.Base;
using SolarX.SERVICE.Services.InventoryServices;

namespace SolarX.SERVICE.Abstractions.IInventoryServices;

public interface IInventoryServices
{
    Task<Result<PagedResult<ResponseModel.InventoryResponseModel>>> GetInventory(Guid agencyId, string? searchTerm, int pageIndex,
        int pageSize);

    Task<Result> CreateInventory(Guid agencyId, List<RequestModel.InventoryRequestModel> request);
    Task<Result> UpdateAdminInventory(Guid inventoryId, RequestModel.InventoryRequestModel request);
}