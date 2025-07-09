using Microsoft.EntityFrameworkCore;
using SolarX.REPOSITORY.Abstractions;
using SolarX.REPOSITORY.Entity;
using SolarX.SERVICE.Abstractions.IInventoryTransactionServices;
using SolarX.SERVICE.Services.Base;

namespace SolarX.SERVICE.Services.InventoryTransactionServices;

public class InventoryTransactionServices : IInventoryTransactionServices
{
    private readonly IBaseRepository<InventoryTransaction, Guid> _inventoryTransactionRepository;

    public InventoryTransactionServices(IBaseRepository<InventoryTransaction, Guid> inventoryTransactionRepository)
    {
        _inventoryTransactionRepository = inventoryTransactionRepository;
    }

    public async Task<Result> UpdateInventoryTransaction(Guid inventoryTransactionId,
        int quantityChanged)
    {
        var isExist = await _inventoryTransactionRepository.GetById(inventoryTransactionId);
        if (isExist == null || isExist.IsDeleted)
        {
            return Result.CreateResult("Bad request", 400);
        }

        isExist.QuantityChanged = quantityChanged;
        _inventoryTransactionRepository.UpdateEntity(isExist);
        return Result.CreateResult("Update inventory transaction success", 200);
    }

    public async Task<Result<PagedResult<ResponseModel.InventoryTransactionResponseModel>>> GetInventoryTransaction(Guid agencyId,
        Guid inventoryId, Guid productId, DateTimeOffset? dateCharge, string? searchTerm, int pageIndex, int pageSize)
    {
        var query = _inventoryTransactionRepository.GetAllWithQuery(x =>
            x.AgencyId == agencyId && x.InventoryId == inventoryId && x.ProductId == productId && !x.IsDeleted);

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(x => x.Note != null && x.Note.Contains(searchTerm));
        }

        if (dateCharge.HasValue)
        {
            query = query.Where(x => x.CreatedAt == dateCharge);
        }

        query = query.Include(x => x.Agency)
            .Include(x => x.Product);

        var listTransaction = await PagedResult<InventoryTransaction>.CreateAsync(query, pageIndex, pageSize);

        var result = listTransaction.Items.Select(x => new ResponseModel.InventoryTransactionResponseModel(
            x.Id,
            x.QuantityChanged,
            x.Product.Name,
            x.Agency.Name,
            x.Note,
            x.Type.ToString(),
            x.CreatedAt
        )).ToList();

        var response = new PagedResult<ResponseModel.InventoryTransactionResponseModel>(result, listTransaction.PageIndex,
            listTransaction.PageSize,
            listTransaction.TotalCount);
        return Result<PagedResult<ResponseModel.InventoryTransactionResponseModel>>.CreateResult("Get inventory transaction success",
            200, response);
    }
}