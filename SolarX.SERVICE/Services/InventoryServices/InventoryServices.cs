using Microsoft.EntityFrameworkCore;
using SolarX.REPOSITORY.Abstractions;
using SolarX.REPOSITORY.Entity;
using SolarX.REPOSITORY.Enum;
using SolarX.SERVICE.Abstractions.IInventoryServices;
using SolarX.SERVICE.Services.Base;

namespace SolarX.SERVICE.Services.InventoryServices;

public class InventoryServices : IInventoryServices
{
    private readonly IBaseRepository<Inventory, Guid> _inventoryRepository;
    private readonly IBaseRepository<Agency, Guid> _agencyRepository;
    private readonly IBaseRepository<Product, Guid> _productRepository;
    private readonly IBaseRepository<InventoryTransaction, Guid> _inventoryTransactionRepository;

    public InventoryServices(IBaseRepository<Inventory, Guid> inventoryRepository, IBaseRepository<Agency, Guid> agencyRepository,
        IBaseRepository<Product, Guid> productRepository, IBaseRepository<InventoryTransaction, Guid> inventoryTransactionRepository)
    {
        _inventoryRepository = inventoryRepository;
        _agencyRepository = agencyRepository;
        _productRepository = productRepository;
        _inventoryTransactionRepository = inventoryTransactionRepository;
    }


    public async Task<Result<PagedResult<ResponseModel.InventoryResponseModel>>> GetInventory(Guid agencyId, string? searchTerm,
        int pageIndex, int pageSize)
    {
        var query = _inventoryRepository.GetAllWithQuery(x => x.AgencyId == agencyId && !x.IsDeleted);
        query = query.Include(x => x.Product)
            .ThenInclude(x => x.Category);
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(x => x.Product.Name.Contains(searchTerm) || x.Product.Category.Name.Contains(searchTerm));
        }

        var listInventory = await PagedResult<Inventory>.CreateAsync(query, pageIndex, pageSize);
        var response = listInventory.Items.Select(x => new ResponseModel.InventoryResponseModel(
            x.Id,
            x.ProductId,
            x.Product.Name,
            x.Product.BasePrice,
            x.Quantity
        )).ToList();
        var result = new PagedResult<ResponseModel.InventoryResponseModel>(response, listInventory.PageIndex, listInventory.PageSize,
            listInventory.TotalCount);
        return Result<PagedResult<ResponseModel.InventoryResponseModel>>.CreateResult("Get inventory success", 200, result);
    }

    public async Task<Result> CreateInventory(Guid agencyId, List<RequestModel.InventoryRequestModel> request)
    {
        if (request.Count == 0)
        {
            return Result.CreateResult("Inventory request cannot be empty", 400, "Bad Request");
        }

        var agencyExists = await _agencyRepository.GetById(agencyId);
        if (agencyExists == null || agencyExists.IsDeleted)
        {
            return Result.CreateResult("Agency not found", 404, "Not Found");
        }

        var productIds = request.Select(x => x.ProductId).Distinct().ToList();

        var existingProducts = await _productRepository
            .GetAllWithQuery(p => productIds.Contains(p.Id) && p.IsActive && !p.IsDeleted)
            .Select(p => p.Id)
            .ToListAsync();

        var missingProductIds = productIds.Except(existingProducts).ToList();
        if (missingProductIds.Count != 0)
        {
            return Result.CreateResult(
                $"Products not found: {string.Join(", ", missingProductIds)}",
                404,
                "Not Found"
            );
        }

        var existingInventories = await _inventoryRepository
            .GetAllWithQuery(i => i.AgencyId == agencyId && productIds.Contains(i.ProductId))
            .ToDictionaryAsync(i => i.ProductId, i => i);

        var inventoriesToAdd = new List<Inventory>();
        var inventoriesToUpdate = new List<Inventory>();
        var transactionsToAdd = new List<InventoryTransaction>();

        foreach (var item in request)
        {
            if (item.Quantity <= 0)
            {
                return Result.CreateResult($"Quantity must be greater than 0 for product {item.ProductId}", 400, "Bad Request");
            }

            if (existingInventories.TryGetValue(item.ProductId, out var existingInventory))
            {
                existingInventory.Quantity += item.Quantity;
                inventoriesToUpdate.Add(existingInventory);

                var existingTransaction = new InventoryTransaction
                {
                    Id = Guid.NewGuid(),
                    AgencyId = agencyId,
                    ProductId = item.ProductId,
                    InventoryId = existingInventory.Id,
                    Type = InventoryTransactionType.ImportForInventory,
                    QuantityChanged = +item.Quantity,
                    OrderId = null,
                    Note = $"Stock replenishment by admin - Added {item.Quantity} units"
                };
                transactionsToAdd.Add(existingTransaction);
            }
            else
            {
                var newInventory = new Inventory
                {
                    Id = Guid.NewGuid(),
                    AgencyId = agencyId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };
                inventoriesToAdd.Add(newInventory);

                var newTransaction = new InventoryTransaction
                {
                    Id = Guid.NewGuid(),
                    AgencyId = agencyId,
                    ProductId = item.ProductId,
                    InventoryId = newInventory.Id,
                    Type = InventoryTransactionType.ImportForInventory,
                    QuantityChanged = +item.Quantity,
                    Note = $"Initial stock creation by admin - {item.Quantity} units"
                };
                transactionsToAdd.Add(newTransaction);
            }
        }

        if (inventoriesToAdd.Count != 0)
        {
            _inventoryRepository.AddBulkAsync(inventoriesToAdd);
        }

        if (inventoriesToUpdate.Count != 0)
        {
            _inventoryRepository.UpdateBulk(inventoriesToUpdate);
        }

        if (transactionsToAdd.Count != 0)
        {
            _inventoryTransactionRepository.AddBulkAsync(transactionsToAdd);
        }

        return Result.CreateResult(
            $"Successfully created/updated {request.Count} inventory items for agency",
            201,
            "Created"
        );
    }

    public async Task<Result> UpdateAdminInventory(Guid inventoryId, RequestModel.InventoryRequestModel request)
    {
        var inventoryExist = await _inventoryRepository.GetAllWithQuery(x => x.Id == inventoryId && x.ProductId == request.ProductId)
            .FirstOrDefaultAsync();
        if (inventoryExist == null || inventoryExist.IsDeleted)
        {
            return Result.CreateResult("Inventory not found", 404);
        }

        inventoryExist.Quantity += request.Quantity;
        var existingTransaction = new InventoryTransaction
        {
            Id = Guid.NewGuid(),
            AgencyId = inventoryExist.AgencyId,
            ProductId = request.ProductId,
            InventoryId = inventoryId,
            Type = InventoryTransactionType.ImportForInventory,
            QuantityChanged = +request.Quantity,
            Note = $"Stock replenishment by admin - Added {request.Quantity} units"
        };
        _inventoryRepository.UpdateEntity(inventoryExist);
        _inventoryTransactionRepository.AddEntity(existingTransaction);
        return Result.CreateResult("Update inventory success", 202);
    }
}